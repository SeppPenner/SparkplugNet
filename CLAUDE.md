# Project rules for Claude

## What this is

SparkplugNet is a library that implements the Sparkplug IIoT standard on top of
[MQTTnet](https://github.com/dotnet/MQTTnet). It is published as the NuGet package
[SparkplugNet](https://www.nuget.org/packages/SparkplugNet/), so the library project sets
`GeneratePackageOnBuild` and the repository ships `BuildAndPushPackage.bat` for the upload. Both
Sparkplug payload versions are supported, version A (the old Kura format, namespace `spAv1.0`) and
version B (`spBv1.0`), and for version B both specification versions 2.2 and 3.0.

One solution `src/SparkplugNet.sln` with exactly three projects:

- `src/SparkplugNet/SparkplugNet.csproj`, the library, multi targeting `net8.0;net9.0`.
- `src/SparkplugNet.Tests/SparkplugNet.Tests.csproj`, MSTest, `net8.0`, 66 test methods in seven
  test classes.
- `src/SparkplugNet.Examples/SparkplugNet.Examples.csproj`, `Exe`, `net8.0`, a console demo that
  runs an application and a node for both payload versions against `localhost:1883`.

Layout inside `src/SparkplugNet`:

- `Core/SparkplugBase.cs` and its partial files (`.Events.cs`, `.EventArgs.cs`,
  `.KnownMetricStorage.cs`): the generic base of everything, `SparkplugBase<T> where T : IMetric`.
  It owns the `IMqttClient`, the sequence and session numbers and the nested class
  `KnownMetricStorage`.
- `Core/Application/SparkplugApplicationBase.cs` plus its partial files: the SCADA host side. It
  subscribes to the whole namespace, tracks `NodeStates` and `DeviceStates` and publishes STATE.
- `Core/Node/SparkplugNodeBase.cs` plus `.Device.cs` and the partial files: the edge node side,
  including the devices hanging off a node.
- `Core/Messages/SparkplugMessageGenerator.cs`: the largest file of the repository. It builds every
  MQTT message (STATE, NBIRTH, DBIRTH, NDEATH, DDEATH, NDATA, DDATA, NCMD, DCMD), each of them once
  for version A and once for version B. New message types follow that same pairing.
- `Core/Messages/SparkplugTopicGenerator.cs`: builds and `Core/Topics/` parses the topic strings.
- `Core/PayloadHelper.cs`: protobuf-net serialization plus the byte conversion helpers. The file
  also carries the assembly level `InternalsVisibleTo("SparkplugNet.Tests")`, which is what makes
  the internal message generator testable.
- `VersionA/` and `VersionB/`: `Data/` holds the public metric types, `ProtoBuf/` the generated
  wire format types, `PayloadConverter.cs` converts between the two, and `SparkplugApplication.cs`
  and `SparkplugNode.cs` are the concrete classes a consumer instantiates.
- `GlobalUsings.cs`: all usings of the project, including the aliases `VersionAData`,
  `VersionBData`, `VersionAProtoBuf`, `VersionBProtoBuf`, `VersionADataTypeEnum`,
  `VersionBDataTypeEnum` and `SystemCancellationToken`.

Repository root: `README.md` (badges, supported frameworks, structure), `HowToUse.md` (the usage
samples for both versions), `Changelog.md`, `Updating.md` (the five step release recipe),
`Version3Compatibility.md` (the TCK rule table), `License.txt` (MIT), `Icon.png` and `Icon.svg`,
`.all-contributorsrc`, `BuildAndPushPackage.bat`, `Delete-BIN-OBJ-Folders.bat`, `.editorconfig`
(under `src`) and `.gitattributes`. `doc/` holds the specification PDFs and the two `.proto` files
the ProtoBuf classes were generated from.

## Build

```powershell
dotnet build src/SparkplugNet.sln -c Release
```

```powershell
dotnet test src/SparkplugNet.sln -c Release
```

- The library multi targets `net8.0;net9.0`, test and example project are single target `net8.0`.
  A release build therefore reports four projects, the library counts twice.
- `src/Directory.Build.props` contains nothing but `GenerateDocumentationFile`. Every other build
  property lives in the three `.csproj` files and is duplicated there.
- `TreatWarningsAsErrors` is enabled in all three projects, so every warning breaks the build,
  NuGet warnings (`NU****`) from restore included. A clean build reports zero warnings, keep it
  that way.
- The `NoWarn` list is `NU1803,CS0618,CS0809,NU1901,NU1902` in all three projects. `CS0618` and
  `CS0809` are there because the library keeps obsolete members alive, `NU1901` and `NU1902` hide
  low and moderate vulnerability audit findings. Fix warnings instead of extending that list.
  `NuGetAuditMode=all` is on, so a vulnerable transitive package fails the build too.
- Versions come from GitVersion.MsBuild out of the git tags, for example `1.3.11-5` for the fifth
  commit after tag `1.3.10`. There is no `GitVersion.yml`, the defaults apply. Never edit a version
  property or an assembly version by hand.
- Restore needs nuget.org. If a private feed is configured globally on the machine and answers 404
  for public packages, restore fails with `NU1301`. Then build with an explicit source:
  `dotnet build src/SparkplugNet.sln --source https://api.nuget.org/v3/index.json`.
- Tests are MSTest and need no broker and no network. They cover the message generator for
  specification versions 2.2 and 3.0, the payload converters for both versions, the known metric
  storage including its filtering, and the metric timestamp handling. `EqualityHelper.cs` is not a
  test class, it is the deep comparison the payload tests run on. Never claim a test run happened
  without running it.
- Beyond the tests, behaviour against a real broker is only verified by running
  `SparkplugNet.Examples` against an MQTT broker on `localhost:1883`. There is no test that opens a
  connection.

## Code conventions

Follow the surrounding code, it is consistent throughout every file:

- File header comment block with `<copyright file="..." company="Hämmer Electronics">` and a
  `<summary>`, then the file-scoped namespace.
- XML doc comments on every type and every member, private members included, no exceptions.
  Implementations of an interface member additionally carry `<inheritdoc cref="..."/>` and
  `<seealso cref="..."/>` pointing at that interface. `GenerateDocumentationFile` is on, so a
  missing comment is a warning and therefore an error.
- `Nullable`, `ImplicitUsings` and `LangVersion latest` are enabled.
- New `using` directives go into the `GlobalUsings.cs` of the respective project, inside the
  existing `#pragma warning disable IDE0065` block, never at the top of a file. The editorconfig
  requires usings inside the namespace (`csharp_using_directive_placement=inside_namespace:warning`),
  which global usings cannot satisfy, that is what the pragma is for. Do not add other pragmas. The
  comment text in that block is German because Visual Studio generated it, leave it alone.
- Fields, properties, methods and events are always accessed with `this.` qualification
  (`dotnet_style_qualification_for_*` at severity `warning`).
- `src/.editorconfig` also enforces braces everywhere, no multiple blank lines, four spaces, CRLF,
  UTF-8, file scoped namespaces, `System` usings sorted first and `IDE0005` as warning. Analyzer
  warnings are fixed, not silenced.
- The `.csproj` files use four spaces of indentation inside `<Project>`, unlike the Visual Studio
  default.

## Known quirks

Do not silently "clean up" these, they are existing behaviour:

- **Version A is the Kura format, not "Sparkplug A".** `VersionA/Data/KuraMetric.cs` carries a flat
  set of typed value properties, version B uses `Metric` with a `DataType` and a single boxed
  value. The two are not convertible into each other, which is why every message generator method
  and every event exists twice.
- **The ProtoBuf classes are generated, the data classes are not.** `VersionA/ProtoBuf/` and
  `VersionB/ProtoBuf/` mirror `doc/SparkplugA.proto` and `doc/SparkplugB.proto`. The classes under
  `Data/` are handwritten and richer. `PayloadConverter` is the only bridge, and a change on one
  side without the other silently breaks the wire format.
- **STATE messages differ per specification version.** Version 2.2 publishes the plain strings
  `ONLINE` and `OFFLINE` on `STATE/<host>`, version 3.0 publishes the JSON of `StateMessage` on
  `spBv1.0/STATE/<host>`. Both are retained and sent with QoS 1, the rest of the library uses QoS 0.
  The application deliberately skips incoming STATE messages in `OnApplicationMessageReceived`
  because they are UTF-8 and not protobuf.
- **`SparkplugMessageTopic.TryParse` decides what is handled.** Anything it cannot parse is
  dropped without an error, the STATE branch is the only explicit exception. A topic typo therefore
  looks like silence, not like a failure.
- **`InternalsVisibleTo` sits in `PayloadHelper.cs`.** Not in an `AssemblyInfo.cs`, not in the
  `.csproj`. The tests reach `SparkplugMessageGenerator` and `PayloadHelper` through it.
- **`KnownMetricStorage` keys by name and by alias in two dictionaries.** `Metrics` concatenates
  both, so a metric that has a name and an alias would show up twice. Version A metrics only ever
  go into the name dictionary.
- **The session number starts at -1.** `LastSessionNumber` is incremented before the connect, so
  the first bdSeq that reaches the broker is 0, which is what the specification demands.
  `LastSequenceNumber` starts at 0 and is incremented after publishing.
- **`eclipse_tahu` is a gitlink without a `.gitmodules` entry.** `git ls-files -s` shows mode
  `160000` for it, but there is no submodule configuration, so a fresh clone gets an empty
  directory and `git submodule update` does nothing. It points at the Eclipse Tahu repository and
  is used as a reference for the specification only.
- **AppVeyor badge without CI in the repository.** `README.md` links an AppVeyor build that is
  configured outside of this repository. There is no `.github` folder and no pipeline file here.
- **`src/SparkplugNet.sln.DotSettings`** is tracked and holds nothing but a ReSharper user
  dictionary (the Sparkplug message type abbreviations, `Kura`, `scada`, `H_00E4mmer`). Leave it
  alone.
- **`.gitattributes` sets `* text=auto`** and every rule of the Visual Studio template below it is
  commented out. The `.pdf` files under `doc/` are only safe because git detects them as binary,
  any further binary file should get its own rule.
- **`Updating.md` describes the old, manual release.** The authoritative order is the one under
  "Releasing" below. `Updating.md` says "build with Visual Studio", which nobody does.

## Releasing

1. Make the change.
2. Add an entry at the top of `Changelog.md` in the existing format:
   `* **Version 1.3.11.0 (2026-08-18)**: Short description.`
3. Update `<PackageReleaseNotes>` in `src/SparkplugNet/SparkplugNet.csproj` to the same text. It is
   the one version string that is maintained by hand, everything else comes from GitVersion.
4. If the change is user visible, update `README.md` (the "Available for" list, the "Special notes"
   section for breaking changes) and `HowToUse.md`.
5. Commit that.
6. Tag the commit with the plain version number, no `v` prefix (`1.3.10`, `1.3.9`, ...). The
   existing tags are lightweight tags, create new ones the same way.
7. Push the commits and the tag.
8. Publish the package with `BuildAndPushPackage.bat`. It deletes every `bin` and `obj` below
   `src`, builds Release and pushes `*.nupkg` and `*.snupkg` to nuget.org and to the GitHub package
   feed. It needs `NUGET_API_KEY` and `GITHUB_API_KEY` in the environment and it must run after the
   tag, otherwise GitVersion burns a prerelease version into the package.

The version in the `Changelog.md` has four parts (`1.3.11.0`), the tag has three (`1.3.11`).
GitVersion turns the tag into the assembly version, so an untagged commit produces something like
`1.3.11-5+Branch.master.Sha...`.

## Git

- **Never amend a commit.** No `git commit --amend`, not for a typo in the message, not to add a
  forgotten file, not even when the commit is still local. Write a follow-up commit instead. The
  release versions come from tags on exact commits, an amended commit leaves its tag pointing at a
  commit that no longer exists in the branch.

## Writing style

- Commit messages are written **in English only**: short, precise subject line, explanatory body
  when needed.
- Code comments and comments in project files such as `.csproj` are **always English**, regardless
  of the language used in the conversation.
- **No em dashes or en dashes** (`—`, `–`), neither in prose, commit messages, code comments nor
  documentation. Use a regular hyphen, comma, colon, parentheses or a separate sentence.
- German texts (documentation, chat replies) always use real umlauts and ß, never ASCII
  transliterations such as `ae`, `oe`, `ue` or `ss`. Identifiers, file names and configuration keys
  stay unchanged where umlauts are technically undesirable.
