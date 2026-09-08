# π1 Elvira Editor v1.0

Initial public release of **π1 Elvira Editor**.

A Windows editor for working with graphics and text resources from **Elvira: Mistress of the Dark** and **Elvira II: The Jaws of Cerberus**.

## Features

- Support for Elvira I (VGA & EGA) and Elvira II (VGA)
- Project/Edition/Runtime architecture isolating editor state from pristine game files
- VGA resource detection, parsing, and sprite/image preview
- Multiple VGA palettes with real Elvira I palette resolution
- Pixel-perfect preview with adjustable zoom (Fit / 100%–800%)
- PNG export and validated PNG replacement (exact dimensions, exact palette RGB, alpha<128→index 0)
- GAMEPC text editor with CP852/Windows-1250/Latin-1 encoding, repack on length change
- Translation editions: project-owned, isolated per edition (Original EN read-only baseline)
- Extended CP852-compatible bitmap font support:
  - Elvira I RUNVGA: V5 extended format (256 glyphs × 8 bytes)
  - Elvira II RUNIT: V2 split-font format (LOW 0x20–0x81 + HIGH 0x82–0xFF)
  - 224 total slots; 223 editable glyphs, with 0x81 reserved for HUD erase behavior (editable 0x20–0x80, 0x82–0xFF)
  - TTF/OTF/SFD import with CP852 mapping, composed diacritics, baseline-aware rasterization
  - R9D trusted-patch gates: structure + frozen invariant validation before mutation
- Evidence-based Runtime UI editing:
  - RUNEGA: 8 records mapped (ProvenLive/ProvenByBinary)
  - RUNVGA: routes frozen per record (Pause.menu PROVEN LIVE, others PROVEN BY BINARY); layout/capacity mapping incomplete, patching blocked
  - RUNIT: SAVE_FAILURE mapped; LOAD_FAILURE/FILE_NOT_FOUND/TRY_ANOTHER_DISK unresolved
- Disposable/owned variant builds via CompositeBuild → `VARIANTS\<KEY>\`
- Build reproducibility (deterministic stages, provenance manifests)
- Run/Debug readiness with artifact integrity/provenance validation
- EN/SK/CS interface localization (dynamic, with English fallback)
- Localized Help (EN/SK/CS)
- Recovery/Safety: explicit scoped restoration, immutable O-files, mutable backups
- Unsupported binary rejection (R9D): filename/size/unpack insufficient; fail-closed
- PRE-R9D UI stabilization: deterministic layout, no clipping, consistent selectors
- R10 legacy cleanup: removed TranslationVariantService, GAMEPCO migration, direct Graphics deploy, SafeDeployer VGA path, manual game selector, obsolete text-risk controls

## Installation

1. Download the R11 release package (final package identity is populated during R11 release packaging).
2. Extract the archive.
3. Run `Pi1ElviraEditor.exe`.
4. Select your Elvira I or Elvira II game directory via **Find games...** or **Browse folder...**.

No separate .NET installation required (self-contained x64 build).

## Important

This project does **not** contain or distribute original Elvira game data, graphics, text resources or other copyrighted game assets.

You must provide your own legally obtained copy of the game.

Always keep a backup of your original game files before modifying them.

## Recommended User Flow

For normal project workflows:

**EDIT → SAVE TO PROJECT → BUILD VARIANT → RUN**

- Graphics/Text/Font edits are saved to the project (`Save To Project`)
- `Build Variant` runs CompositeBuild, materializing Text, Graphics and supported Font/Runtime UI state into `VARIANTS\`
- Elvira I VGA (V5) and Elvira II VGA (V2) font edits materialize into owned variant executables; EGA font edits fail the build closed (no proven writer)
- `Run` / `Debug` launches the authorized variant (readiness validated)

Original (EN) game assets in GameRoot are immutable: no normal editor action replaces them. Legacy O-files remain recovery-only.

### v1.0 CompositeBuild Materialization Matrix

| Domain | Project State | CompositeBuild Materialization |
|--------|---------------|-------------------------------|
| Text | YES (isolated per edition) | YES |
| Graphics | YES (isolated, runtime-scoped) | YES |
| Font | YES (isolated per edition, runtime-scoped) | YES for Elvira I VGA (V5) and Elvira II VGA (V2); EGA edits fail preflight closed |
| Runtime UI | YES (isolated, evidence-validated) | VGA Pause.menu/Confirm.generic/Save.overwrite + audited EGA records; all other overrides fail preflight |
| Executable | N/A (derived) | YES (RUNVGA/RUNEGA/RUNIT) |

## Download Verification

Final package name and SHA-256 will be populated during R11 release packaging.

## License

The editor source code is released under the **GNU General Public License v3.0 (GPL-3.0)**.
