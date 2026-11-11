using Microsoft.Extensions.Localization;

namespace StudyHub.Web.Services;

public static class IntervalFormat
{
	public static string Short(int days, IStringLocalizer l) => days switch
	{
		0 => l["Interval_LessThanDay"],
		< 30 => l["Interval_Days", days],
		< 365 => l["Interval_Months", Math.Round(days / 30.0, 1)],
		_ => l["Interval_Years", Math.Round(days / 365.0, 1)],
	};
}
