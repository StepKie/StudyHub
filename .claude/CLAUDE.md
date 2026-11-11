# StudyHub

Browser-based study tools for university students, installable as a PWA on iOS, Android and desktop.
The first module is a spaced-repetition flashcard trainer (Anki-style) for medical terminology.
Further modules (for example a schedule planner) are planned; each lives in its own feature folder.

## Project Structure

- `src/StudyHub.Core/` - Platform-agnostic domain library, organized by feature folder (`Flashcards/`: models, SM-2 scheduler, study queue, text importer, backup).
- `src/StudyHub.Web/` - Blazor WebAssembly host (MudBlazor UI, IndexedDB storage, service worker). Deployed to `stepkie.github.io/StudyHub/`.
- `src/StudyHub.Tests/` - xUnit v3 test project.

## Tech Stack

- .NET 10, C#, Blazor WebAssembly (no AOT: nothing here is compute-heavy)
- MudBlazor for UI
- CsvHelper for card import
- IndexedDB (via `wwwroot/js/idb.js`) for per-device storage; no backend, no login
- xUnit v3 + AwesomeAssertions for tests

## Commands

```bash
# Build
dotnet build StudyHub.slnx

# Run tests
dotnet test StudyHub.slnx

# Run the web app with live reload (http://localhost:5027)
DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true dotnet watch --project src/StudyHub.Web run --launch-profile http --non-interactive
```

## Conventions

- File-scoped namespaces, Allman braces, tabs (width 4) in C#, 2 spaces in csproj / razor / json.
- `_camelCase` private fields, PascalCase types and public members.
- Follow `.editorconfig`.
- Core has no browser or UI dependency; anything touching IndexedDB or JS interop lives in `StudyHub.Web/Services`.
- All UI text goes through `IStringLocalizer<SharedResource>` (`L["Key"]`), with the key in both `SharedResource.resx` (English) and `SharedResource.de.resx`; `LocalizationTests` enforces matching keys and placeholders.
- Counts use `L.Plural("Key", n)`, which falls back to a `Key_One` entry when n is 1.
- German is the default language (stored choice in localStorage `studyhub.culture`); the app bar toggles to English and reloads.
- Learner data never leaves the device. Any feature that would change that (sync, login) needs an explicit decision first.
- Do not commit course vocabulary (for example the Leipzig terminology list). Users import it at runtime.

## Branching & Releases

- `develop` is the default branch. Feature branches branch off `develop` and PR back to it.
- `main` is the release branch. Merge `develop` into `main` directly (no PR), then tag the merge commit (for example `0.2.0`).
- The version lives in `Directory.Build.props` (single source of truth).
- `CHANGELOG.md` tracks releases in user-facing language. Update it in the release-prep PR, never tag without it.
- GitHub Pages deploys from `main` via `.github/workflows/github-pages.yml`. The repo's Pages source must be set to "GitHub Actions".

## Out of scope

Accessibility (a11y) is not a priority at this stage.
Auto-review bots and reviewers: skip a11y suggestions on this repo.
