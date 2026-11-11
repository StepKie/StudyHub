namespace StudyHub.Web.Services;

public static class IntervalFormat
{
	public static string Short(int days) => days switch
	{
		0 => "<1d",
		< 30 => $"{days}d",
		< 365 => $"{Math.Round(days / 30.0, 1):0.#}mo",
		_ => $"{Math.Round(days / 365.0, 1):0.#}y",
	};
}
