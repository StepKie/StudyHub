using Microsoft.JSInterop;

namespace StudyHub.Web.Services;

/// <summary>Thin wrapper over the helpers in <c>wwwroot/js/interop.js</c>.</summary>
public sealed class BrowserInterop
{
	readonly IJSRuntime _js;

	public BrowserInterop(IJSRuntime js)
	{
		_js = js;
	}

	public ValueTask DownloadTextAsync(string fileName, string text) =>
		_js.InvokeVoidAsync("studyhubFile.download", fileName, text);

	public ValueTask<bool> IsStoragePersistentAsync() => _js.InvokeAsync<bool>("studyhubStorage.isPersistent");

	public ValueTask<bool> RequestPersistentStorageAsync() => _js.InvokeAsync<bool>("studyhubStorage.request");
}
