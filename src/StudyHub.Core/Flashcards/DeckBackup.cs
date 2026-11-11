using System.Text.Json;

namespace StudyHub.Core.Flashcards;

/// <summary>Versioned JSON snapshot of all decks, used to move progress between devices and to guard against browser data loss.</summary>
public static class DeckBackup
{
	public const int CurrentVersion = 1;

	static readonly JsonSerializerOptions _options = new() { WriteIndented = true };

	public static string Serialize(IEnumerable<Deck> decks) =>
		JsonSerializer.Serialize(new BackupFile(CurrentVersion, decks.ToList()), _options);

	public static List<Deck> Deserialize(string json)
	{
		var file = JsonSerializer.Deserialize<BackupFile>(json, _options)
			?? throw new InvalidDataException("Backup file is empty.");

		return file.Version == CurrentVersion
			? file.Decks
			: throw new InvalidDataException($"Unsupported backup version {file.Version}.");
	}

	sealed record BackupFile(int Version, List<Deck> Decks);
}
