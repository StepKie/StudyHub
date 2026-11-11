namespace StudyHub.Core.Flashcards;

public static class StudyQueue
{
	public const int DefaultNewCardsPerDay = 20;

	/// <summary>Due reviews (oldest first) followed by as many new cards as today's limit still allows.</summary>
	public static List<Card> Build(Deck deck, DateOnly today, int newCardsPerDay = DefaultNewCardsPerDay)
	{
		var reviews = deck.Cards
			.Where(c => !c.State.IsNew && c.State.Due <= today)
			.OrderBy(c => c.State.Due)
			.ToList();

		var introducedToday = deck.Cards.Count(c => c.State.FirstReviewed == today);
		var newAllowed = Math.Max(0, newCardsPerDay - introducedToday);
		var fresh = deck.Cards
			.Where(c => c.State.IsNew)
			.Take(newAllowed);

		return [.. reviews, .. fresh];
	}
}
