# StudyHub

Study tools for university students that run in the browser and install to the home screen of an iPhone, Android phone or desktop.

## Flashcards

- Spaced-repetition trainer with Anki's four answer buttons (Again, Hard, Good, Easy).
- Import Anki "Notes in Plain Text" exports or any CSV / TSV file (column 1 front, column 2 back, optional column 3 tag).
- Works offline once loaded.
- Progress is stored on the device only. Use the Backup page to save and restore it.

## Development

```bash
dotnet test StudyHub.slnx
dotnet watch --project src/StudyHub.Web run --launch-profile http
```

Requires the .NET 10 SDK.
