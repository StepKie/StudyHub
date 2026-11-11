using AwesomeAssertions;
using StudyHub.Core.Flashcards;

namespace StudyHub.Tests.Flashcards;

public class CardTextImporterTests
{
	[Fact]
	public void Anki_plain_text_export_uses_directives_for_separator_and_tags()
	{
		const string text = "#separator:tab\n#html:true\n#tags column:3\nCor\tHeart<br>organ\tanatomy\nRen\tKidney\t\n";

		var result = CardTextImporter.Parse(text);

		result.Cards.Select(c => (c.Front, c.Back, c.Tag)).Should().Equal(
			("Cor", "Heart\norgan", "anatomy"),
			("Ren", "Kidney", null));
	}

	[Theory]
	[InlineData("Cor,Heart\nRen,Kidney")]
	[InlineData("Cor;Heart\nRen;Kidney")]
	[InlineData("Cor\tHeart\nRen\tKidney")]
	[InlineData("﻿Cor,\"Heart\"\r\nRen,\"Kidney\"\r\n")]
	public void Delimiter_is_detected_when_no_directive_is_present(string text)
	{
		var result = CardTextImporter.Parse(text);

		result.Cards.Select(c => (c.Front, c.Back)).Should().Equal(("Cor", "Heart"), ("Ren", "Kidney"));
		result.SkippedRows.Should().Be(0);
	}

	[Fact]
	public void Quoted_fields_may_contain_the_delimiter_and_html_entities_are_decoded()
	{
		var result = CardTextImporter.Parse("\"Nephro, nephr/o\",\"kidney &amp; related\"");

		result.Cards.Should().ContainSingle()
			.Which.Should().BeEquivalentTo(new { Front = "Nephro, nephr/o", Back = "kidney & related" }, o => o.ExcludingMissingMembers());
	}

	[Fact]
	public void Rows_without_a_front_or_back_are_skipped_and_counted()
	{
		var result = CardTextImporter.Parse("Cor,Heart\nLonely\n,NoFront\nRen,Kidney");

		result.Cards.Should().HaveCount(2);
		result.SkippedRows.Should().Be(2);
	}
}
