namespace StudyHub.Core.Flashcards;

public static class StudyQueue
{
	public const int DefaultNewCardsPerDay = 20;

	/// <summary>
	/// Due reviews (oldest first) followed by as many new cards as the daily limit still allows.
	/// A <paramref name="tag"/> restricts both the cards and the daily count to one section.
	/// </summary>
	public static List<Card> Build(Deck deck, DateOnly today, int newCardsPerDay = DefaultNewCardsPerDay, string? tag = null)
	{
		var scope = tag is null ? deck.Cards : deck.Cards.Where(c => c.Tag == tag).ToList();

		var reviews = scope
			.Where(c => !c.State.IsNew && c.State.Due <= today)
			.OrderBy(c => c.State.Due)
			.ToList();

		var introducedToday = scope.Count(c => c.State.FirstReviewed == today);
		var newAllowed = Math.Max(0, newCardsPerDay - introducedToday);
		var fresh = scope
			.Where(c => c.State.IsNew)
			.Take(newAllowed);

		return [.. reviews, .. fresh];
	}

	/// <summary>True when anything is due or still new, ignoring the daily new-card limit.</summary>
	public static bool HasWork(Deck deck, DateOnly today, string? tag = null) =>
		deck.Cards.Any(c => (tag is null || c.Tag == tag) && (c.State.IsNew || c.State.Due <= today));
}
