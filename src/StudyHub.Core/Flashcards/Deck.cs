namespace StudyHub.Core.Flashcards;

public sealed class Deck
{
	public required string Id { get; init; }

	public required string Name { get; set; }

	public List<Card> Cards { get; set; } = [];

	public static Deck Create(string name, IEnumerable<Card>? cards = null) => new()
	{
		Id = Guid.NewGuid().ToString("N"),
		Name = name,
		Cards = cards?.ToList() ?? [],
	};
}
