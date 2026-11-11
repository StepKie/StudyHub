namespace StudyHub.Core.Flashcards;

public sealed class Card
{
	public required string Id { get; init; }

	public required string Front { get; set; }

	public required string Back { get; set; }

	public string? Tag { get; set; }

	public ReviewState State { get; set; } = new();

	public static Card Create(string front, string back, string? tag = null) => new()
	{
		Id = Guid.NewGuid().ToString("N"),
		Front = front,
		Back = back,
		Tag = tag,
	};
}
