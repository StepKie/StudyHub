namespace StudyHub.Core.Flashcards;

public static class DeckStatistics
{
	public static int ReviewsOn(Deck deck, DateOnly day, string? tag = null) => deck.History
		.Where(d => d.Date == day && (tag is null || d.Tag == tag))
		.Sum(d => d.Reviews);

	public static int AgainOn(Deck deck, DateOnly day) => deck.History
		.Where(d => d.Date == day)
		.Sum(d => d.Again);

	/// <summary>Reviews of one day per section; reviews recorded without a section are grouped under a null tag.</summary>
	public static List<(string? Tag, int Reviews)> BySection(Deck deck, DateOnly day) => deck.History
		.Where(d => d.Date == day && d.Reviews > 0)
		.GroupBy(d => d.Tag)
		.Select(g => (g.Key, g.Sum(d => d.Reviews)))
		.OrderBy(s => s.Key)
		.ToList();

	/// <summary>Earliest due date among already studied cards, or null if none has been studied.</summary>
	public static DateOnly? NextDue(Deck deck, string? tag = null) => deck.Cards
		.Where(c => (tag is null || c.Tag == tag) && !c.State.IsNew)
		.Min(c => c.State.Due);

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
