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
	public void Backup_without_history_still_loads()
	{
		const string oldBackup = """{ "Version": 1, "Decks": [ { "Id": "x", "Name": "Old", "Cards": [] } ] }""";

		var decks = DeckBackup.Deserialize(oldBackup);

		decks.Single().History.Should().BeEmpty();
	}
}
