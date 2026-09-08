# π1 Elvira Editor v1.0

A Windows editor for the original DOS releases of **Elvira: Mistress of the Dark** and **Elvira II: The Jaws of Cerberus**. It edits VGA resources, GAMEPC text, and the games' extended CP852-compatible bitmap fonts.

## Architecture Overview

### Installation → Project → Edition → Runtime Target

The editor uses a clean four-layer architecture:

**Installation**
- A detected or manually opened game installation directory
- Determines the game profile (Elvira I VGA, Elvira I EGA, Elvira II VGA)
- Acts as the pristine source baseline — never modified by normal workflows

**Project**
- Editor-owned state associated with an installation
- Isolates editor data from the pristine game files
- Stored under `<GameRoot>\ElviraEditor\Project\`

**Edition**
- Original EN (English) baseline — read-only, pristine
- Project-owned translated editions (e.g., SK, CZ, or custom two-character codes)
- Global Edition is authoritative for the active variant
- Edition codes are exactly 2 ASCII alphanumeric characters (DOS 8.3 constraint)

**Runtime Target**
- Elvira I VGA → `RUNVGA.EXE`
- Elvira I EGA → `RUNEGA.EXE`
- Elvira II VGA → `RUNIT.EXE`

**Generated Runnable Identity**
A runnable variant is the combination: **Installation + Project + Edition + Runtime Target**

Output artifacts are materialized into **owned, disposable VARIANTS** directories under `<GameRoot>\VARIANTS\<KEY>\<EDITION>\` (e.g. `VARIANTS\E1VGA\SK`), never written directly into the pristine GameRoot. Editions coexist: each Full build starts from a pristine-only copy inside its own edition directory.

### GameRoot Policy — Important Nuance

**NORMAL PROJECT WORKFLOW:**
- Editor project state lives outside pristine game files (`ElviraEditor\Project\`)
- Generated text/graphics/runtime artifacts go through CompositeBuild
- Runnable outputs belong in `VARIANTS\`
- Legacy direct GAMEPCxx/RUNxxx project-build workflows into GameRoot were removed in R10

**CONTROLLED EXCEPTIONS (explicit, user-confirmed):**
- Standalone Font Editor "Apply changes to EXE" — gated by R9D executable trust validation, preserves O-backup semantics
- Launcher companion generation/restoration where currently supported
- Recovery/Safety restoration operations

The Font Apply path is explicit, protected by R9D trust, and is NOT the normal Project/Edition/Runtime build path.

## Supported Games & Runtimes

| Game | Runtime | Executable |
|------|---------|------------|
| Elvira I | VGA | `RUNVGA.EXE` |
| Elvira I | EGA | `RUNEGA.EXE` |
| Elvira II | VGA | `RUNIT.EXE` |

Interface localization: English, Slovak, Czech.

**Note:** Czech UI localization exists, but Czech game binary/data localization is NOT supported.

## Text Architecture

- Pristine `GAMEPC` is the baseline input
- Original EN is read-only baseline edition
- Translation editions live in project-owned state (`Project\Projects\<CODE>\text-translations.json`)
- Global Edition is authoritative
- Obsolete independent Text edition selector removed
- Generated `GAMEPCxx` files materialized into owned build output (VARIANTS)
- Normal Text workflow does NOT create `GAMEPCO`/`GAMEPCxx` in pristine GameRoot
- Text save = save project state
- Save As/Export = editor-owned/user-selected export behavior
- Runtime UI text is separate from GAMEPC translation text

**DOS Naming:**
- DOS 8.3 restrictions apply
- E1 translation suffix = exactly 2 ASCII alphanumeric characters (e.g., `SK`, `S1`)
- E2 translation suffix = exactly 2 ASCII alphanumeric characters

**False-dirty-state fix:** Loading an unchanged edition no longer falsely marks it dirty.

## Graphics Architecture

- Graphics edits are project-owned (`Project\Projects\<CODE>\graphics-edits.json`)
- Editions are isolated
- Runtime-specific scope preserved (`Shared` or `RuntimeSpecific`)
- Graphics state materialized through CompositeBuild into owned VARIANTS
- Legacy direct Graphics deploy action removed in R10

**PNG Replacement Validation:**
- Exact dimensions required
- Exact active 16-color palette RGB required
- Alpha < 128 → palette index 0 (transparent)
- Otherwise RGB must match palette exactly (indices 1–15)
- Duplicate RGB handling is deterministic
- Invalid colors reported before committing invalid state

**Real Elvira I palette resolution** via `Elvira1PaletteResolver` (per VGA1/VGA2 pair audit).
**Deterministic/idempotent Fit preview centering** via `MainForm.ComputeFitPlacement`.

**NOT implemented:** Palette substitution/conversion (post-v1.0 work).

## Font Architecture

### Supported Executable Forms & Trust Model (R9D)

R9D introduced a strict three-way distinction:

**FORMAT recognition ≠ trusted provenance ≠ permission to mutate**

### Executable Classification Model

| Classification | Meaning |
|----------------|---------|
| `Missing` | File not found |
| `SupportedPacked` | Original packed executable, frozen hash verified |
| `SupportedCanonical` | Canonical unpacked baseline, frozen hash verified |
| `GeneratedExtended` | Recognized generated format (V5/V2), structure validated |
| `Unsupported` | Any other executable — fail closed |

### Per-Runtime Details

**Elvira I RUNVGA:**
- Packed original: `0x18C39` bytes, SHA-256 `70878FA8C1F2CEB7B2C624AD4DE36C8CBDA10AEDA65A7DA4991C2334B282FFD1`
- Canonical unpacked: `0x2CE30` bytes, SHA-256 `C6CEC09B41D42D92DDD0588B54E5729720CDC91ABEF8083F9205F237A9686756`
- Generated V5 extended CP852: `0x3B660` bytes, font at `0x3AE60` (2048 bytes)
- Mutable region: only font table at `0x3AE60..0x3B65F`
- Protected HUD glyph `0x81` = `00 FC FC FC FC FC FC 00` (original) → `FC FC FC FC FC FC FC FC` (patched)

**Elvira II RUNIT:**
- Packed original: `0x13F20` bytes, SHA-256 `8E9E9C327E02B464E0A956F1544017C45C94C32E1E86732EB17FDED8BBC32FD8`
- Canonical unpacked: `0x27A20` bytes, SHA-256 `7FDE00D641D3BDE82B1732DAD59D21CE19F48573CBEFAD6C64C11754660EB415`
- Generated V2 extended CP852 (split font): `0x288C1` bytes
  - LOW font `0x20..0x81` at canonical offset
  - HIGH font `0x82..0xFF` at `0x28480` (1008 bytes)
  - Helper at `0x28870`
- **Derived Trust Anchor** (V2 frozen with LOW zeroed): SHA-256 `D82200F218F0B181356E602AB8AA8253DBC17B72D0A2B6865B6423B3034CF4DB`
- Protected HUD glyph `0x81` preserved in both LOW and HIGH

**RUNEGA:**
- Supported packed/canonical executable identity (frozen hashes)
- Font mutation NOT supported — no CompositeBuild font replacement exists
- `GeneratedExtended` structural validation only, no trusted mutation path

### R9D Trusted-Patch Rules

- **RUNVGA V5**: Structure validated, protected/frozen invariants validated, only documented deterministic modifications in mutable font region tolerated, immutable content must resolve back to trusted canonical evidence
- **RUNIT V2**: Structure/header/renderer invariants validated, documented mutable font region excluded from immutable trust anchor, remaining immutable content must match deterministic trusted evidence (`TrustedMaskedSha256`)
- **Protected HUD glyph 0x81** is a critical invariant — never remapped
- **0x81 remapping research is NOT implemented**

## Authoritative Binary Identities (Full SHA-256)

| Executable | Classification | Size | SHA-256 |
|------------|---------------|------|---------|
| RUNVGA | PackedOriginal | 0x18C39 | 70878FA8C1F2CEB7B2C624AD4DE36C8CBDA10AEDA65A7DA4991C2334B282FFD1 |
| RUNVGA | CanonicalUnpacked | 0x2CE30 | C6CEC09B41D42D92DDD0588B54E5729720CDC91ABEF8083F9205F237A9686756 |
| RUNVGA | GeneratedExtended (V5) | 0x3B660 | (structure only — font varies) |
| RUNEGA | PackedOriginal | 0x198EF | FE596D7DB1CEFB643F2C2BEC1EC5342CC1F7DBA2F47AFE3512B6C6FF8DE05EA1 |
| RUNEGA | CanonicalUnpacked | 0x25CD0 | A15243583A1774BAF8F77DF599063675ED9EF36A573A35CF6F2504705B3BAB56 |
| RUNEGA | GeneratedExtended | 0x33C00 | (structural only) |
| RUNIT | PackedOriginal | 0x13F20 | 8E9E9C327E02B464E0A956F1544017C45C94C32E1E86732EB17FDED8BBC32FD8 |
| RUNIT | CanonicalUnpacked | 0x27A20 | 7FDE00D641D3BDE82B1732DAD59D21CE19F48573CBEFAD6C64C11754660EB415 |
| RUNIT | GeneratedExtended (V2) | 0x288C1 | **Derived Trust Anchor**: BAB9D7DDE86ABD42C3A728952E736A4C7541CF86F34C3301D2FFCECC905B46BD |
| RUNIT | DerivedTrustAnchor (LOW zeroed) | — | D82200F218F0B181356E602AB8AA8253DBC17B72D0A2B6865B6423B3034CF4DB |

**GeneratedExtended hashes are DERIVED TRUST ANCHORS** — not discovered original game fingerprints.

## R9D Unsupported Binary Rejection

- Filename alone is insufficient
- DOS MZ validity alone is insufficient
- File size alone is insufficient
- Successful unpacking alone is insufficient
- Unsupported/modified executable revisions **fail closed**
- Bootstrap/build/font operations use appropriate identity/trust gates
- Generated executable structure alone does NOT authorize mutation

## Run / Debug Readiness

`LaunchReady` requires more than file existence. Current checks:

- Valid editor-owned variant directory
- Expected runtime artifacts present
- Supported executable identity/format (R9D gate)
- Valid variant manifest (`variant-manifest.json`)
- Recorded runtime artifact integrity/provenance (SHA-256 match)
- Configured runtime/build readiness

**Post-build modification** of executable or data artifact → **fail-closed** not-ready/build-incomplete behavior.

**Diagnostics differentiated:**
1. Executable missing/unsupported/modified
2. Generated runtime artifact no longer matches authorized build output

Run/Debug does NOT automatically rebuild unless explicitly invoked.

## Composite Build

`CompositeBuildService` is the normal production build authority.

### Pipeline Stages (in order)

1. `ValidateContext` — project/variant context valid
2. `ValidateBaseline` — pristine manifest matches baseline
3. `ValidateProjectInputs` — project state valid
4. `ValidateVariant` — variant directory owned/valid
5. `PrepareDisposableVariant` — pristine-only copy to disposable area
6. `ApplyDataTransformations` — text translations materialized
7. `ApplyGraphicsTransformations` — graphics edits materialized (VGA rebuild)
8. `ApplyFontTransformations` — stage exists; validates/preflights project font state via `NoProjectChangesBuildStep`; normal saved font override materialization is unavailable in v1.0 (preflight fails if saved overrides exist)
9. `ApplyRuntimeUiTransformations` — stage exists; validates/preflights Runtime UI project state via `RuntimeUiProjectBuildStep`; validated VGA structured overrides (Pause.menu/Confirm.generic/Save.overwrite) and audited EGA overrides materialize into the owned VARIANT output by `TranslationAwareExecutableBuildStep`; all other Runtime UI overrides still fail the build
10. `ApplyExecutableTransformation` — executable built (RUNVGA/RUNEGA/RUNIT)
11. `ValidateOutput` — owned variant directory structurally valid + manifest written

### v1.0 Materialization Matrix

| Domain | Project State | CompositeBuild Materialization |
|--------|---------------|-------------------------------|
| **Text** | YES (isolated per edition) | YES — `SelectedTranslationProjectBuildStep` |
| **Graphics** | YES (isolated, runtime-scoped) | YES — `GraphicsProjectMaterializationBuildStep` |
| **Font** | YES (isolated per edition) | NO — `NoProjectChangesBuildStep` fails preflight if saved font overrides exist |
| **Runtime UI** | YES (isolated, evidence-validated) | VGA Pause.menu/Confirm.generic/Save.overwrite + audited EGA records (see below); all other overrides fail preflight |
| **Executable** | N/A (derived) | YES — `TranslationAwareExecutableBuildStep` (RUNVGA/RUNEGA/RUNIT) |

**Controlled Exception:** Standalone Font Editor "Apply changes to EXE" writes directly to GameRoot (R9D-gated, O-backup preserved). This is NOT the normal Project/Edition/Runtime build path.

### Key Concepts

- `DisposableVariantBuildService` — pristine-only copy, manifest-driven
- `VariantDirectoryService` — `<VARIANTS>\<KEY>\<EDITION>` + `.pi1-variant-owner.json` ownership marker (runtime+edition scoped; schema 2 binds the edition)
- `ActiveProjectCompositeBuildSteps` — 5 steps for active project (Translation, Graphics, Font, Runtime UI, Executable)
- `VariantManifestService` — `variant-manifest.json` with provenance
- `ProjectVariantOwnership` — `EN` = immutable baseline; owned state at `Project\Projects\<CODE>\`
- Deterministic/reproducible rebuild behavior (R9C)

## Edition Isolation

**Original EN:**
- Pristine/read-only baseline
- No writable project-state directory

**Other Editions (e.g., SK, CZ, custom):**
- Project-owned
- Text isolated (`text-translations.json`)
- Graphics isolated (`graphics-edits.json`)
- Font isolated (`font-edits.json`)
- Runtime UI isolated (`runtime-ui.json`)
- The Original (`EN`) edition is read-only: explicit edit attempts explain this instead of silently doing nothing.

State sharing between runtime targets inside the SAME edition is proven only where explicitly implemented. No sharing across projects or editions.

## Runtime UI — Evidence-Based Support

Evidence terminology used by the project:

- `PROVEN` / `PROVEN LIVE` — live runtime verification
- `PROVEN BY BINARY` — static binary evidence
- `STRONG EVIDENCE` — strong indirect evidence
- `INFERRED` — inferred from patterns
- `UNKNOWN` — not determined

### Elvira I RUNEGA
- **Proven** Runtime UI bank: module `0x31000..0x317FF` / physical `0x33400..0x33BFF` / segment `0x3100`
- **Rejected**: `0x30800..0x30FFF` (startup-written, not persistent runtime UI)
- 8 records (`F000`–`F007`): Pause.menu (`ProvenLive`), Confirm.generic (`ProvenByBinary`), Save.prompt (`ProvenLive`), Save.failed (`ProvenByBinary`), Restore.loadFailed (`ProvenByBinary`), Restore.fileNotFound (`ProvenByBinary`), Disk.retry (`ProvenByBinary`), Save.overwrite (`ProvenByBinary`)
- R9F audit: all eight records have audited edit contracts and are editable/materializable. Simple messages (`Save.failed`, `Restore.loadFailed`, `Restore.fileNotFound`, `Disk.retry`) accept a 1..max message after their frozen indent prefix; structured records (`Pause.menu`, `Confirm.generic`, `Save.prompt`, `Save.overwrite`) accept one semantic field with frozen separators, fixed columns and button lines (`Continue`/`Quit`, `Yes`/`No`) preserved byte-exact. Button/input-field geometry is frozen (read-only subfields). Originals decode authoritatively from the canonical RUNEGA source spans; the output bank lives at the same `0x33400` offsets in the generated executable. Capacity fit alone never implies editability: unstructured values fail validation with hotspot errors instead of a misleading Valid.
- R9F V3 storage limits: record storage stays the fixed English envelope (e.g. `Save.failed` 18 bytes incl. NUL) because variable-length bank relocation is NOT proven safe: the 9-byte bridge holds no selector/offset table, record spans are non-uniform (no algorithmic stride), and no debugger/memory evidence shows the free bank areas are never runtime scratch. A longer translation is rejected with its composed-vs-envelope byte counts, never truncated. Storage capacity (bank bytes) and screen geometry (line width, fixed columns, hotspots) are validated and reported as distinct conditions.
- R9F V3 semantic editor: double-click (or Edit) opens the structured record (Title/Prompt/Message plus read-only buttons/questions/geometry with reasons). The grid keeps Record/Original/Project/Validation/Details with field-only presentation; raw CR separators, padding and NUL are never shown to the translator.
- R9F V4 hotspot labels (RUNEGA in-place materializer only): `Continue`/`Quit`/`Yes`/`No` are translatable fixed-width slots — TEXT MAY CHANGE, HOTSPOT GEOMETRY MUST NOT CHANGE. Shorter labels are space-padded so following columns never move; overlong labels are rejected with their slot width, never truncated (e.g. `Continue`→`Pokračuj` and `Yes`→`Áno` fit exactly; a 6-character label does not fit a 4-wide EGA `Quit` slot and is rejected). EGA record byte length is invariant under button translation.
- R9F V3 runtime-scoped state: `runtime-ui.json` schema v2 keys every override by `(Runtime, LogicalRecordId)` — `(Elvira1Ega, PauseMenu)` and `(Elvira1Vga, PauseMenu)` are independent. An EGA override never appears in a VGA view, grid, validation, build or materialization. Legacy schema-v1 files migrate deterministically (unambiguous records assigned to their only qualifying runtime; ambiguous residue kept unassigned, surfaced, never built, never silently duplicated; v1 bytes backed up to `runtime-ui.v1.backup.json` before the first validated v2 write).

### Elvira I RUNVGA
- 8 logical records with frozen binary routes (original blocks, runtime sources, renderer/dispatcher call sites)
- **Pause.menu / Confirm.generic / Save.overwrite**: human-proven-live variable-width structured records (DOSBox RVGBTN POC). Translated labels render and operate with frozen start anchors and lengths differing from English; the clickable hotspot is separate fixed geometry — translated labels may have variable visual lengths, fixed anchor/hotspot geometry is preserved, text and mouse hotspot are separate concepts. Hotspot width does NOT follow translated text, and not every visible character is clickable.
- Live observations (evidence, not a per-character guarantee): `Koniec` rendered 6 characters with the fifth clickable and the final sixth outside the effective hotspot; `Nie` rendered 3 characters with the final character still clickable; `Pokračuj` resumed gameplay; `Áno` quit/overwrote correctly; no crash/corruption.
- Production model: semantic fields (Title/Continue/Quit, Prompt/Yes/No, Message/Question/Yes/No); first labels keep ≥1 separating space before the frozen second anchor (else visual collision); second labels and message lines bounded by proven one-line visual widths (Pause option row 21 cols, Confirm/Overwrite button row 18 cols, message lines 21 cols — widest live-rendered content in this dialog family). Fitting records materialize in place; over-envelope records materialize via one deterministic append (per-record thunk + relocated record, fixed slots, retained call relocations + bridge relocations, memory-margin fix, MZ update, explicit whitelist).
- Other 5 records (`Save.prompt`, `Save.failed`, `Restore.loadFailed`, `Restore.fileNotFound`, `Disk.retry`): genuinely read-only (route-proven yet layout-incomplete where applicable): locked cells, programmatic edits rejected without touching project state, no Reset, builds fail closed. No EGA spans are borrowed for them. RUNIT is untouched.

### Variant Status and Runnable Identity
- Runnable identity is Installation + Project + Edition + Runtime. Owned outputs live in `VARIANTS\<RuntimeKey>\<EditionCode>` (e.g. `VARIANTS\E1VGA\SK`): SK and S1 builds for the same runtime coexist independently — building one edition never deletes or replaces another, and building EGA never touches VGA outputs. The variant manifest records both runtime and edition codes.
- Legacy flat outputs (pre-R9F files directly in `VARIANTS\E1VGA`) are detected, reported, and left untouched: they are never launched, never rebuilt in place, and never silently deleted. Recovery offers an explicit, hash-verified **Adopt legacy output** action (moves only fully manifest-proven payloads into `VARIANTS\<KEY>\<EDITION>`; anything unproven aborts untouched) as well as explicit removal. A disabled Rebuild button is explained in the recovery status line.
- Variant Manager readiness is selection-independent: each row evaluates its own runtime+edition artifacts plus the manifest, so selecting another runtime never flips a built row. Translated Mods entries resolve per edition inside the active runtime (pristine `GAMEPC` still resolves to GameRoot); never-built editions report Missing/Not-built, never corrupt; corrupt/foreign outputs report Invalid distinctly.

### Elvira II RUNIT
- **SAVE_FAILURE** (`F100`) — `SupportedAndMapped` / `ProvenByBinary` (mapped)
- **LOAD_FAILURE** (`F101`) — `KnownButMappingIncomplete` / `Unknown` (route/mapping incomplete, unresolved)
- **FILE_NOT_FOUND** (`F102`) — `KnownButMappingIncomplete` / `Unknown` (route/mapping incomplete, unresolved)
- **TRY_ANOTHER_DISK** (`F103`) — `KnownButMappingIncomplete` / `Unknown` (route/mapping incomplete, unresolved)
- **Rejected**: `0x27B90..0x2838F` (startup/runtime-written, not accepted as persistent Runtime UI storage)

**Historical investigation preserved separately from production guarantees.**

## Storage / Safety / Recovery

### Exact Paths (from `EditorStorageLayout`)

```
<GameRoot>\ElviraEditor\Baseline
<GameRoot>\ElviraEditor\Baseline\Mutable
<GameRoot>\ElviraEditor\Project
<GameRoot>\ElviraEditor\Logs
<GameRoot>\ElviraEditor\Metadata
<GameRoot>\VARIANTS
```

### Concepts

- **Pristine manifest** (`Baseline/pristine-manifest.json`) — schema v1, GameId, Distribution=Unknown
- **Immutable baseline** — GAMEPC, EXE, VGA files classified `Immutable`
- **MutableBackedUp semantics** — ELVIRA.BAT/CERBERUS.BAT backed up once to `.BAK` (immutable)
- **Owned generated files** — in `VARIANTS\<KEY>\<EDITION>` with ownership marker
- **Variant ownership marker** — `.pi1-variant-owner.json` (Schema=2 for runtime+edition outputs: Product, GameId, VariantId, DirectoryKey, ProjectCode, BaselineFingerprint; Schema=1 legacy runtime roots)
- **Unexpected external files** — reported, not silently deleted
- **Recovery** — explicit, scoped (`RecoverySafetyService`)
- **Restoration** — scoped to mutable backed-up files
- **Unknown binaries** — not silently adopted
- **No full duplicate-original vault** — architecture uses immutable baseline + mutable backups + owned VARIANTS

## PRE-R9D UI Stabilization (Final Outcomes)

- Repeated edition/runtime switching stabilized
- Resource/layout accumulation fixed
- Deterministic structural MainForm layout (5-col × 3-row header grid, 108px, 36px rows)
- Graphics Fit placement stabilized (`ComputeFitPlacement`)
- Action button geometry normalized
- Navigation bottom-border clipping fixed
- Active Variant / Edition ComboBox clipping fixed
- Obsolete Game selector removed
- Header reorganized around: **Installation | Active Variant | Edition | Detected**
- Selectors use consistent width (128px for Build Variant / Find Games / Browse)
- Final layout accepted in real Windows QA

## Localization

- EN / SK / CS interface localization
- Dynamic localization service (`UiLocalizationService`) + catalog (`JsonUiLocaleProvider`)
- Fallback to English for missing keys
- Localized Help (Help/*.md per locale)
- Terminology consistency enforced

**R10 Slovak audit:** Pre-existing Slovak locale text corruption in some sk.json strings was analyzed. **Result: 0 actual corruptions found** — the apparent issues were UTF-8 display artifacts in tooling, not data defects. All 413 diacritic-bearing values decode correctly. No locale data changes required.

## Help Documentation

Help/*.md audited against final post-R10 UI/workflow:

- Removed stale references to: manual Game selector, direct Graphics Deploy button, TranslationVariantService, direct GAMEPCxx/RUNxxx project workflow into GameRoot, obsolete independent Text translation selector, old Apply-to-game workflow, old header geometry/controls, obsolete build terminology
- **Preserved:** Font Editor standalone "Apply changes to EXE" (still supported, R9D-gated)
- EN/SK/CS Help describes consistent workflow: **EDIT → SAVE TO PROJECT → BUILD VARIANT → RUN**

## Release Notes v1.0 Summary

**Actual v1.0 Functionality:**
- Elvira I VGA / EGA
- Elvira II VGA
- Project/Edition/Runtime model
- Text translation editions (project-owned, isolated)
- Graphics editing (project-owned, runtime-scoped)
- Font/CP852 functionality (V5 RUNVGA, V2 RUNIT, RUNEGA identity only)
- Evidence-based Runtime UI editing
- Disposable/owned variant builds (VARIANTS)
- Build reproducibility (R9C deterministic)
- Run/Debug readiness with provenance validation
- EN/SK/CS interface localization
- Recovery/Safety (explicit, scoped)
- Unsupported binary rejection (R9D fail-closed)
- PRE-R9D UI stabilization
- R10 removal of obsolete legacy write workflows

**NOT advertised as complete:**
- Incomplete RUNVGA Runtime UI capacity/mapping
- Unresolved RUNIT Runtime UI routes (LOAD_FAILURE, FILE_NOT_FOUND, TRY_ANOTHER_DISK)
- Czech game binary/data localization
- Not every GameRoot write eliminated (Font Apply + recovery/launcher exceptions remain)

## Known Limitations (v1.0)

1. **Incomplete RUNVGA Runtime UI** — capacity/mapping knowledge incomplete
2. **Unresolved RUNIT Runtime UI** — LOAD_FAILURE, FILE_NOT_FOUND, TRY_ANOTHER_DISK routes not fully mapped
3. **Protected HUD glyph 0x81** — cannot be remapped (engine invariant)
4. **Standalone Font Apply architectural exception** — writes to GameRoot, not VARIANTS
5. **Supported executable identities required** — unsupported binaries fail closed
6. **Font materialization** — no CompositeBuild font replacement for RUNEGA or normal workflow yet
7. **Runtime UI materialization** — Font/Runtime UI build steps currently fail preflight if overrides exist (no materializer implemented)
8. **0x81 remapping research** — not implemented

## Post-v1.0 / Future Research (NOT IMPLEMENTED IN v1.0)

1. RUNVGA Runtime UI capacity/mapping research
2. RUNIT unresolved Runtime UI routes
3. Canonical default Runtime UI decoding
4. CP/font 0x81 remap research
5. Locale Translation Workbench
6. Whole-codebase benefit/effort/risk audit
7. Possible .NET 10 WPF/MVVM redesign / 2.0 branch
8. Palette substitution/conversion preview
9. Repository/folder release rename to `Pi1-Elvira-I-II-Editor` (R11 work)

## R10 Pre-existing 22 Failures — R11 Release-Readiness Classification

The R10 regression comparison established identical failing sets on clean main and R10 (85 PASS / 22 FAIL over its 107-test battery).

R10 clean main: 85 PASS / 22 FAIL.

R10 result: 85 PASS / 22 FAIL — identical failing set.

R9E/V5 additionally ran a broader current-dispatch diagnostic battery (110 currently dispatched smoke flags, 113 invocations incl. per-game pristine capture/validate/recapture: 93 PASS / 20 FAIL), but it is not definition-identical to the historical 107-test R10 battery and therefore is not used as a direct regression-count comparison. No direct definition-identical R10 comparison was performed for R9E/V5.

| Class | Description | Examples |
|-------|-------------|----------|
| **A — Missing curated/external fixtures** | Tests require fixtures not in repo | GAMEPCSK fixtures (parser, repack, terminal-delimiter, fixed-hotspot, trace-correlation), canonical RUNEGA fixture, V5 fixture/preview, golden/patched binaries, trace files, canonical EXE audits |
| **B/C — Stale assertions (fixed in R9E/V5)** | Hardcoded expectations updated to production contract | `json-locales` cs-bootstrap contract, `ui-localization` + `installation-context-activation` neutral-state separator, `button-terminology` sk fallback expectation — all PASS now |
| **B/C — Stale assertions (remaining)** | Help hierarchy expectation vs current Help model | `help-smoke` + `version-smoke` (`HelpDocumentService.ValidateAll` group hierarchy) |
| **E — Intentional R9D fail-closed behavior** | Correctly blocked untrusted/patched binaries | Patched-HUD / untrusted binary case |
| **G — Fixture/environment preconditions** | Tests need specific runtime state | run-debug fixture-manifest readiness, variant-build-status fixture-manifest, variant-directory-real VARIANTS payload |
| **R11 triage** | Pre-existing failure needing R11 verdict (not introduced by R9E: font-deploy/gate paths unmodified) | `partial-backup-smoke` corrupt_exe_o not blocked — possible fail-open vs stale expectation |

**Locale corruption (R10 observation):** R9E directly parsed current `sk.json` as strict UTF-8 — **0 U+FFFD, 0 mojibake patterns, 413 valid diacritic values**. Classified as **"not reproduced in current source; no active locale defect proven"**.

**Items where exact smoke identity cannot be reconstructed from available evidence:** Marked **"R11 re-run required"** — not called a product defect.

## Roadmap State

| Milestone | Status |
|-----------|--------|
| 1/50–48/50 | COMPLETE |
| 48/50: R10 — Legacy Cleanup | COMPLETE |
| **49/50: R9E — Technical Documentation Update** | **CURRENT** |
| 50/50: R11 — Release | PENDING |

PRE-R9D remains an unnumbered stabilization phase between R9C and R9D. Earlier milestones not renumbered.

---

**Product Identity:** π1 Elvira Editor v1.0
**Repository:** Pi1ElviraEditor (local directory; remote: Pi1-Elvira-I-II-Editor)
**License:** GPL-3.0
