namespace StudyHub.Core.Flashcards;

public static class DeckStatistics
{
	public static int ReviewsOn(Deck deck, DateOnly day) =>
		deck.History.FirstOrDefault(d => d.Date == day)?.Reviews ?? 0;

	/// <summary>Consecutive days with at least one review, ending today or, if nothing is reviewed yet today, yesterday.</summary>
	public static int Streak(Deck deck, DateOnly today)
	{
		var studied = deck.History
			.Where(d => d.Reviews > 0)
			.Select(d => d.Date)
			.ToHashSet();

		var cursor = studied.Contains(today) ? today : today.AddDays(-1);
		var streak = 0;
		while (studied.Contains(cursor))
		{
			streak++;
			cursor = cursor.AddDays(-1);
		}

		return streak;
	}
}
