using System.Text.Json;
using Microsoft.JSInterop;
using StudyHub.Core.Flashcards;

namespace StudyHub.Web.Services;

/// <summary>Stores one JSON document per deck in IndexedDB through the <c>window.studyhubIdb</c> helper.</summary>
public sealed class IndexedDbDeckRepository : IDeckRepository
{
	readonly IJSRuntime _js;

	public IndexedDbDeckRepository(IJSRuntime js)
	{
		_js = js;
	}

	public async Task<IReadOnlyList<Deck>> GetAllAsync()
	{
		var payloads = await _js.InvokeAsync<string[]>("studyhubIdb.getAll");

		return payloads
			.Select(Deserialize)
			.OrderBy(d => d.Name, StringComparer.CurrentCultureIgnoreCase)
			.ToList();
	}

	public async Task<Deck?> GetAsync(string id)
	{
		var json = await _js.InvokeAsync<string?>("studyhubIdb.get", id);

		return string.IsNullOrEmpty(json) ? null : Deserialize(json);
	}

	public async Task SaveAsync(Deck deck) =>
		await _js.InvokeVoidAsync("studyhubIdb.put", deck.Id, JsonSerializer.Serialize(deck));

	public async Task DeleteAsync(string id) =>
		await _js.InvokeVoidAsync("studyhubIdb.remove", id);

	static Deck Deserialize(string json) => JsonSerializer.Deserialize<Deck>(json)!;
}
