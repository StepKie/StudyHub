namespace StudyHub.Core.Flashcards;

public static class StudyQueue
{
	public const int NewCardsPerSession = 20;

	/// <summary>
	/// Due reviews (oldest first) followed by the next new cards, at most <paramref name="newCards"/> of them.
	/// A <paramref name="tag"/> restricts the session to one section. There is no daily limit: another session simply yields the next batch.
	/// </summary>
	public static List<Card> Build(Deck deck, DateOnly today, int newCards = NewCardsPerSession, string? tag = null)
	{
		var scope = tag is null ? deck.Cards : deck.Cards.Where(c => c.Tag == tag).ToList();

		var reviews = scope
			.Where(c => !c.State.IsNew && c.State.Due <= today)
			.OrderBy(c => c.State.Due)
			.ToList();

		var fresh = scope
			.Where(c => c.State.IsNew)
			.Take(newCards);

		return [.. reviews, .. fresh];
	}

	/// <summary>True when anything is due or still new.</summary>
	public static bool HasWork(Deck deck, DateOnly today, string? tag = null) =>
		deck.Cards.Any(c => (tag is null || c.Tag == tag) && (c.State.IsNew || c.State.Due <= today));
}
