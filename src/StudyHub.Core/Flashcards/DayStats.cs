namespace StudyHub.Core.Flashcards;

/// <summary>Review counts of one deck on one calendar day.</summary>
public sealed class DayStats
{
	public DateOnly Date { get; set; }

	public int Reviews { get; set; }

	public int Again { get; set; }
}
