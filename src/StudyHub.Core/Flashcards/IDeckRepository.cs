namespace StudyHub.Core.Flashcards;

public interface IDeckRepository
{
	Task<IReadOnlyList<Deck>> GetAllAsync();

	Task<Deck?> GetAsync(string id);

	Task SaveAsync(Deck deck);

	Task DeleteAsync(string id);
}
