using AwesomeAssertions;
using StudyHub.Core.Flashcards;

namespace StudyHub.Tests.Flashcards;

public class StudyQueueTests
{
	static readonly DateOnly _today = new(2026, 11, 11);

	[Fact]
	public void Due_reviews_come_first_oldest_first_then_new_cards()
	{
		var newCard = Card.Create("new", "n");
		var dueYesterday = Card.Create("a", "a");
		dueYesterday.State.Due = _today.AddDays(-1);
		var dueLongAgo = Card.Create("b", "b");
		dueLongAgo.State.Due = _today.AddDays(-9);
		var notDue = Card.Create("c", "c");
		notDue.State.Due = _today.AddDays(3);
		var deck = Deck.Create("d", [newCard, notDue, dueYesterday, dueLongAgo]);

		var queue = StudyQueue.Build(deck, _today);

		queue.Should().Equal(dueLongAgo, dueYesterday, newCard);
	}

	[Fact]
	public void New_card_limit_accounts_for_cards_already_introduced_today()
	{
		var introduced = Card.Create("seen", "s");
		introduced.State.Due = _today.AddDays(1);
		introduced.State.FirstReviewed = _today;
		var fresh = Enumerable.Range(0, 5).Select(i => Card.Create($"f{i}", "x"));
		var deck = Deck.Create("d", fresh.Prepend(introduced));

		var queue = StudyQueue.Build(deck, _today, newCardsPerDay: 3);

		queue.Should().HaveCount(2);
	}
}
