namespace StudyHub.Core.Flashcards;

/// <summary>Review counts of one deck section on one calendar day; reviews recorded before sections were tracked have no <see cref="Tag"/>.</summary>
public sealed class DayStats
{
	public DateOnly Date { get; set; }

	public string? Tag { get; set; }

	public int Reviews { get; set; }

	public int Again { get; set; }
}
