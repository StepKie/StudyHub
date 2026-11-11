namespace StudyHub.Core.Flashcards;

/// <summary>
/// SM-2 style scheduler with Anki's four answer buttons. Pure functions of the card state and today's date,
/// so callers own the clock.
/// </summary>
public static class Sm2Scheduler
{
	public const double StartEase = 2.5;

	public const double MinEase = 1.3;

	const double HardMultiplier = 1.2;

	const double EasyBonus = 1.3;

	/// <summary>Applies <paramref name="rating"/> to <paramref name="state"/> in place and sets the next due date.</summary>
	public static void Review(ReviewState state, Rating rating, DateOnly today)
	{
		state.FirstReviewed ??= today;
		var previous = state.IntervalDays;

		switch (rating)
		{
			case Rating.Again:
				state.Repetitions = 0;
				state.IntervalDays = 0;
				state.Ease = Math.Max(MinEase, state.Ease - 0.2);
				break;
			case Rating.Hard:
				state.IntervalDays = Math.Max(1, (int)Math.Round(previous * HardMultiplier));
				state.Ease = Math.Max(MinEase, state.Ease - 0.15);
				state.Repetitions++;
				break;
			case Rating.Good:
				state.IntervalDays = state.Repetitions switch
				{
					0 => 1,
					1 => 6,
					_ => Math.Max(previous + 1, (int)Math.Round(previous * state.Ease)),
				};
				state.Repetitions++;
				break;
			case Rating.Easy:
				state.IntervalDays = state.Repetitions switch
				{
					0 => 4,
					1 => 10,
					_ => Math.Max(previous + 1, (int)Math.Round(previous * state.Ease * EasyBonus)),
				};
				state.Ease += 0.15;
				state.Repetitions++;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(rating), rating, null);
		}

		state.Due = today.AddDays(state.IntervalDays);
	}

	/// <summary>Interval in days that <paramref name="rating"/> would produce, without changing <paramref name="state"/>.</summary>
	public static int PreviewInterval(ReviewState state, Rating rating, DateOnly today)
	{
		var copy = state.Clone();
		Review(copy, rating, today);

		return copy.IntervalDays;
	}
}
