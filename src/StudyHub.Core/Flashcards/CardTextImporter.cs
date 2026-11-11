using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;

namespace StudyHub.Core.Flashcards;

public sealed record ImportResult(List<Card> Cards, int SkippedRows);

/// <summary>
/// Imports cards from delimited text: Anki's "Notes in Plain Text" export (with its <c>#separator:</c> / <c>#tags column:</c> directives)
/// or a plain CSV. Column 1 is the front, column 2 the back, and an optional third column the tag.
/// </summary>
public static partial class CardTextImporter
{
	static readonly char[] _candidateDelimiters = ['\t', ';', ',', '|'];

	public static ImportResult Parse(string text)
	{
		var lines = text.TrimStart('﻿').Split('\n').Select(l => l.TrimEnd('\r')).ToList();
		var directives = lines.TakeWhile(l => l.StartsWith('#')).ToList();
		var body = string.Join('\n', lines.Skip(directives.Count));

		var delimiter = DelimiterFromDirectives(directives) ?? DetectDelimiter(body);
		var tagColumn = TagColumnFromDirectives(directives) ?? 2;

		var config = new CsvConfiguration(CultureInfo.InvariantCulture)
		{
			Delimiter = delimiter.ToString(),
			HasHeaderRecord = false,
			BadDataFound = null,
			MissingFieldFound = null,
			IgnoreBlankLines = true,
			TrimOptions = TrimOptions.Trim,
		};

		var cards = new List<Card>();
		var skipped = 0;
		using var csv = new CsvReader(new StringReader(body), config);
		while (csv.Read())
		{
			var front = ToPlainText(csv.GetField(0));
			var back = ToPlainText(csv.GetField(1));
			if (string.IsNullOrWhiteSpace(front) || string.IsNullOrWhiteSpace(back))
			{
				skipped++;
				continue;
			}

			var tag = ToPlainText(csv.GetField(tagColumn));
			cards.Add(Card.Create(front, back, string.IsNullOrWhiteSpace(tag) ? null : tag));
		}

		return new ImportResult(cards, skipped);
	}

	static char? DelimiterFromDirectives(List<string> directives)
	{
		var value = directives
			.Select(d => d.TrimStart('#').Split(':', 2))
			.Where(p => p.Length == 2 && p[0].Trim().Equals("separator", StringComparison.OrdinalIgnoreCase))
			.Select(p => p[1].Trim().ToUpperInvariant())
			.FirstOrDefault();

		return value switch
		{
			"TAB" => '\t',
			"SEMICOLON" => ';',
			"COMMA" => ',',
			"PIPE" => '|',
			"SPACE" => ' ',
			_ => null,
		};
	}

	static int? TagColumnFromDirectives(List<string> directives)
	{
		var value = directives
			.Select(d => d.TrimStart('#').Split(':', 2))
			.Where(p => p.Length == 2 && p[0].Trim().Equals("tags column", StringComparison.OrdinalIgnoreCase))
			.Select(p => p[1].Trim())
			.FirstOrDefault();

		return int.TryParse(value, out var oneBased) && oneBased > 0 ? oneBased - 1 : null;
	}

	static char DetectDelimiter(string body)
	{
		var firstLine = body.Split('\n').FirstOrDefault(l => !string.IsNullOrWhiteSpace(l)) ?? string.Empty;

		return _candidateDelimiters
			.OrderByDescending(d => firstLine.Count(c => c == d))
			.First();
	}

	static string ToPlainText(string? field)
	{
		if (string.IsNullOrEmpty(field) || field.AsSpan().IndexOfAny('<', '&') < 0)
		{
			return field ?? string.Empty;
		}

		var withBreaks = LineBreakRegex().Replace(field, "\n");

		return WebUtility.HtmlDecode(TagRegex().Replace(withBreaks, string.Empty)).Trim();
	}

	[GeneratedRegex(@"<\s*br\s*/?\s*>", RegexOptions.IgnoreCase)]
	private static partial Regex LineBreakRegex();

	[GeneratedRegex("<[^>]+>")]
	private static partial Regex TagRegex();
}
