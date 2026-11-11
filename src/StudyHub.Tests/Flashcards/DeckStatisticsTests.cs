using AwesomeAssertions;
using StudyHub.Core.Flashcards;

namespace StudyHub.Tests.Flashcards;

public class DeckStatisticsTests
{
	static readonly DateOnly _today = new(2026, 11, 11);

	[Fact]
	public void RecordReview_accumulates_per_day_and_counts_Again_separately()
	{
		var deck = Deck.Create("d");

		deck.RecordReview(_today, Rating.Good);
		deck.RecordReview(_today, Rating.Again);
		deck.RecordReview(_today.AddDays(1), Rating.Easy);

		deck.History.Should().HaveCount(2);
		DeckStatistics.ReviewsOn(deck, _today).Should().Be(2);
		deck.History.Single(d => d.Date == _today).Again.Should().Be(1);
	}

	[Theory]
	[InlineData(new[] { 0 }, 1)]
	[InlineData(new[] { 0, -1, -2 }, 3)]
	[InlineData(new[] { -1, -2 }, 2)]
	[InlineData(new[] { 0, -2 }, 1)]
	[InlineData(new[] { -2 }, 0)]
	public void Streak_counts_consecutive_days_ending_today_or_yesterday(int[] dayOffsets, int expected)
	{
		var deck = Deck.Create("d");
		foreach (var offset in dayOffsets)
		{
			deck.RecordReview(_today.AddDays(offset), Rating.Good);
		}

		DeckStatistics.Streak(deck, _today).Should().Be(expected);
	}

	[Fact]
	public void GetTags_returns_distinct_tags_in_first_seen_order_and_skips_untagged()
	{
		var deck = Deck.Create("d", [Card.Create("a", "x", "B"), Card.Create("b", "x"), Card.Create("c", "x", "A"), Card.Create("d", "x", "B")]);

		deck.GetTags().Should().Equal("B", "A");
	}

	[Fact]
	public void Reviews_are_tracked_per_section_and_summed_per_day()
	{
		var deck = Deck.Create("d");

		deck.RecordReview(_today, Rating.Good, "A");
		deck.RecordReview(_today, Rating.Again, "A");
		deck.RecordReview(_today, Rating.Good, "B");
		deck.RecordReview(_today, Rating.Good);

		DeckStatistics.ReviewsOn(deck, _today).Should().Be(4);
		DeckStatistics.ReviewsOn(deck, _today, "A").Should().Be(2);
		DeckStatistics.AgainOn(deck, _today).Should().Be(1);
		DeckStatistics.BySection(deck, _today).Should().Equal((null, 1), ("A", 2), ("B", 1));
	}

	[Fact]
	public void NextDue_is_the_earliest_studied_card_in_scope_and_null_when_nothing_is_studied()
	{
		var soon = Card.Create("a", "x", "A");
		soon.State.Due = _today.AddDays(1);
		var later = Card.Create("b", "x", "B");
		later.State.Due = _today.AddDays(6);
		var deck = Deck.Create("d", [later, soon, Card.Create("c", "x", "C")]);

		DeckStatistics.NextDue(deck).Should().Be(_today.AddDays(1));
		DeckStatistics.NextDue(deck, "B").Should().Be(_today.AddDays(6));
		DeckStatistics.NextDue(deck, "C").Should().BeNull();
	}

	[Fact]
	public void Backup_without_history_still_loads()
	{
		const string oldBackup = """{ "Version": 1, "Decks": [ { "Id": "x", "Name": "Old", "Cards": [] } ] }""";

		var decks = DeckBackup.Deserialize(oldBackup);

		decks.Single().History.Should().BeEmpty();
	}
}
