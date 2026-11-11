using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace StudyHub.Web.Services;

/// <summary>Chooses the UI language, remembers it in localStorage and applies it by reloading the app.</summary>
public sealed class LanguageService
{
	public const string DefaultLanguage = "de";

	const string StorageKey = "studyhub.culture";

	public static readonly IReadOnlyList<string> Supported = ["de", "en"];

	readonly IJSRuntime _js;
	readonly NavigationManager _nav;

	public LanguageService(IJSRuntime js, NavigationManager nav)
	{
		_js = js;
		_nav = nav;
	}

	public string Current => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

	public string Other => Supported.First(l => l != Current);

	/// <summary>Applies the stored language (German when none is stored) to the current thread; call once before the app starts.</summary>
	public static void ApplyStored(IJSInProcessRuntime js)
	{
		var stored = js.Invoke<string?>("studyhubSettings.get", StorageKey);
		var language = stored is not null && Supported.Contains(stored) ? stored : DefaultLanguage;
		var culture = CultureInfo.GetCultureInfo(language);
		CultureInfo.DefaultThreadCurrentCulture = culture;
		CultureInfo.DefaultThreadCurrentUICulture = culture;
		CultureInfo.CurrentCulture = culture;
		CultureInfo.CurrentUICulture = culture;
		js.InvokeVoid("studyhubSettings.setLanguage", language);
	}

	public async Task SwitchAsync(string language)
	{
		await _js.InvokeVoidAsync("studyhubSettings.set", StorageKey, language);
		_nav.NavigateTo(_nav.Uri, forceLoad: true);
	}
}
