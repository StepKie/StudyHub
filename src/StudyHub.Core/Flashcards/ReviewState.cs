namespace StudyHub.Core.Flashcards;

/// <summary>Spaced-repetition progress of a single card. A card with no <see cref="Due"/> date has never been reviewed.</summary>
public sealed class ReviewState
{
	public int Repetitions { get; set; }

	public double Ease { get; set; } = Sm2Scheduler.StartEase;

	public int IntervalDays { get; set; }

	public DateOnly? Due { get; set; }

	public DateOnly? FirstReviewed { get; set; }

	public bool IsNew => Due is null;

	public ReviewState Clone() => (ReviewState)MemberwiseClone();
}
