namespace StudyHub.Core.Flashcards;

public sealed class Deck
{
	public required string Id { get; init; }

	public required string Name { get; set; }

	public List<Card> Cards { get; set; } = [];

	public List<DayStats> History { get; set; } = [];

	public static Deck Create(string name, IEnumerable<Card>? cards = null) => new()
	{
		Id = Guid.NewGuid().ToString("N"),
		Name = name,
		Cards = cards?.ToList() ?? [],
	};

	/// <summary>Distinct card tags in first-seen order; an imported deck uses them as its sections.</summary>
	public List<string> GetTags() => Cards
		.Select(c => c.Tag)
		.OfType<string>()
		.Distinct()
		.ToList();

	public void RecordReview(DateOnly today, Rating rating)
	{
		var day = History.FirstOrDefault(d => d.Date == today);
		if (day is null)
		{
			day = new DayStats { Date = today };
			History.Add(day);
		}

		day.Reviews++;
		if (rating == Rating.Again)
		{
			day.Again++;
		}
	}
}
