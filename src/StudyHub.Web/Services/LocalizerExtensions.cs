using Microsoft.Extensions.Localization;

namespace StudyHub.Web.Services;

public static class LocalizerExtensions
{
	/// <summary>Looks up <paramref name="key"/>, or <c>key_One</c> when <paramref name="count"/> is 1; the count is argument {0}, <paramref name="more"/> follow.</summary>
	public static LocalizedString Plural(this IStringLocalizer l, string key, int count, params object[] more) =>
		l[count == 1 ? key + "_One" : key, [count, .. more]];
}
