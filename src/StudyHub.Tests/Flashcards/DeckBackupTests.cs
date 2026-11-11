using AwesomeAssertions;
using StudyHub.Core.Flashcards;

namespace StudyHub.Tests.Flashcards;

public class DeckBackupTests
{
	[Fact]
	public void Round_trip_preserves_cards_and_review_state()
	{
		var card = Card.Create("Cor", "Heart", "anatomy");
		Sm2Scheduler.Review(card.State, Rating.Good, new DateOnly(2026, 11, 11));
		var deck = Deck.Create("Terminology", [card]);

		var restored = DeckBackup.Deserialize(DeckBackup.Serialize([deck]));

		restored.Should().BeEquivalentTo([deck]);
	}

	[Fact]
	public void Unknown_version_is_rejected()
	{
		var act = () => DeckBackup.Deserialize("""{ "Version": 99, "Decks": [] }""");

		act.Should().Throw<InvalidDataException>();
	}
}
