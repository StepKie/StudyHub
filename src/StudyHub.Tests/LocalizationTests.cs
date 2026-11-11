using System.Text.RegularExpressions;
using System.Xml.Linq;
using AwesomeAssertions;

namespace StudyHub.Tests;

public partial class LocalizationTests
{
	static Dictionary<string, string> Load(string fileName) => XDocument
		.Load(Path.Combine(AppContext.BaseDirectory, "Localization", fileName))
		.Root!
		.Elements("data")
		.ToDictionary(e => (string)e.Attribute("name")!, e => (string)e.Element("value")!);

	static List<string> Placeholders(string text) => PlaceholderRegex()
		.Matches(text)
		.Select(m => m.Value)
		.Distinct()
		.Order()
		.ToList();

	[Fact]
	public void German_has_exactly_the_keys_of_the_English_resource()
	{
		var english = Load("SharedResource.resx");
		var german = Load("SharedResource.de.resx");

		german.Keys.Should().BeEquivalentTo(english.Keys);
	}

	[Fact]
	public void Every_translation_uses_the_same_placeholders_as_its_English_text()
	{
		var english = Load("SharedResource.resx");
		var german = Load("SharedResource.de.resx");

		foreach (var (key, text) in english)
		{
			Placeholders(german[key]).Should().Equal(Placeholders(text), $"placeholders of '{key}' must match");
		}
	}

	[Fact]
	public void Every_singular_key_has_a_plural_counterpart()
	{
		var english = Load("SharedResource.resx");

		foreach (var key in english.Keys.Where(k => k.EndsWith("_One", StringComparison.Ordinal)))
		{
			english.Should().ContainKey(key[..^"_One".Length]);
		}
	}

	[GeneratedRegex(@"\{\d+\}")]
	private static partial Regex PlaceholderRegex();
}
