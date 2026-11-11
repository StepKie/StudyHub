using AwesomeAssertions;
using StudyHub.Core.Flashcards;

namespace StudyHub.Tests.Flashcards;

public class Sm2SchedulerTests
{
	static readonly DateOnly _today = new(2026, 11, 11);

	[Fact]
	public void Good_on_new_card_is_due_tomorrow()
	{
		var state = new ReviewState();

		Sm2Scheduler.Review(state, Rating.Good, _today);

		state.IntervalDays.Should().Be(1);
		state.Due.Should().Be(_today.AddDays(1));
		state.FirstReviewed.Should().Be(_today);
	}

	[Fact]
	public void Consecutive_Good_grows_interval_one_six_then_by_ease()
	{
		var state = new ReviewState();
		var intervals = new List<int>();

		for (var i = 0; i < 4; i++)
		{
			Sm2Scheduler.Review(state, Rating.Good, _today);
			intervals.Add(state.IntervalDays);
		}

		intervals.Should().Equal(1, 6, 15, 38);
	}

	[Fact]
	public void Again_resets_progress_and_lowers_ease_but_never_below_minimum()
	{
		var state = new ReviewState { Repetitions = 5, IntervalDays = 40, Ease = 1.35 };

		Sm2Scheduler.Review(state, Rating.Again, _today);

		state.Repetitions.Should().Be(0);
		state.IntervalDays.Should().Be(0);
		state.Ease.Should().Be(Sm2Scheduler.MinEase);
		state.Due.Should().Be(_today);
	}

	[Fact]
	public void Easy_beats_Good_beats_Hard_for_a_mature_card()
	{
		var mature = new ReviewState { Repetitions = 4, IntervalDays = 20, Ease = 2.5, Due = _today };

		var hard = Sm2Scheduler.PreviewInterval(mature, Rating.Hard, _today);
		var good = Sm2Scheduler.PreviewInterval(mature, Rating.Good, _today);
		var easy = Sm2Scheduler.PreviewInterval(mature, Rating.Easy, _today);

		hard.Should().BeLessThan(good);
		good.Should().BeLessThan(easy);
	}

	[Fact]
	public void PreviewInterval_does_not_change_the_state()
	{
		var state = new ReviewState { Repetitions = 2, IntervalDays = 6, Due = _today };

		Sm2Scheduler.PreviewInterval(state, Rating.Easy, _today);

		state.Repetitions.Should().Be(2);
		state.IntervalDays.Should().Be(6);
		state.FirstReviewed.Should().BeNull();
	}
}
