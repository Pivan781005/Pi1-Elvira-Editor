# π1 Elvira Editor — Development Notes (v1.0 release + post-v1.0 roadmap)

## v1.0 Release (R11A)

- Public product: **π1 Elvira Editor v1.0** (`v1.0.0`, assembly `1.0.0.0`).
- Technical project/namespace remains `Pi1ElviraEditor`; repository remote is
  `Pi1-Elvira-Editor`. The repository/project rename is **COMPLETE** and is
  not future work.
- v1.0 architecture: SUPPORTED INSTALLATION → IMMUTABLE ORIGINAL GAMEROOT →
  EDITABLE EDITION PROJECT STATE → SAVE TO PROJECT → COMPOSITE BUILD /
  BUILD VARIANT → `VARIANTS\<runtime>\<edition>\` → MODS & LAUNCHER →
  DOS RUNTIME HOST (DOSBox Classic / DOSBox Staging / DOSBox-X) → RUN.
- Spice86, custom π1 DOSBox-X builds, palette conversion, 0x81 remap, Locale
  Translation Workbench and UI-mode redesigns are post-v1.0 research only.

## Post-v1.0 Roadmap (NOT implemented in v1.0)

### Version 1.1 / Next UX

- Basic / Advanced / Expert UI modes.
- Basic mode as simplified normal-user workflow.
- Smart Guidance / state-based next-action highlighting.
- First-run guidance: Find games → choose installation → choose
  edition/runtime as appropriate → choose Graphics/Text/Font editing.
- Recurring guidance: dirty → Save; saved/stale → Build; LaunchReady → Run.
- Guidance is presentation-only and must reuse authoritative application state.
- Expert mode must expose all current v1.0 functionality.
- No duplicate business logic per UI mode.

### Graphics

- Palette substitution/conversion preview.
- Optionally convert incompatible imported images to the active Elvira palette.
- Preview before/after.
- Preserve valid 16-color target constraints.
- User confirmation before applying conversion.

### Font / Character Set Research

- Investigate whether reserved HUD erase glyph 0x81 can be safely moved,
  remapped, or decoupled.
- Objective: potentially free the entire desired character set.
- Do NOT promise feasibility until runtime/code paths are proven.

### Runtime UI / Localization

- RUNVGA Runtime UI capacity/mapping research.
- Unresolved RUNIT Runtime UI routes (LOAD_FAILURE, FILE_NOT_FOUND,
  TRY_ANOTHER_DISK).
- Canonical/default Runtime UI decoding improvements.
- Locale Translation Workbench.

### Runtime / Emulation Research

- Evaluate Spice86 as an analysis/emulation/runtime option.
- Investigate integration opportunities.
- Evaluate/use our own custom DOSBox-X build where technically beneficial.
- Do NOT replace the three supported v1.0 host families (DOSBox Classic,
  DOSBox Staging, DOSBox-X) without separate design and regression work.

### Longer Term

- Whole-codebase benefit/effort/risk audit.
- Possible .NET 10 WPF/MVVM redesign / future 2.0 branch.

## Historical v1.1b snapshot (preserved, pre-release)

### Included in this historical snapshot (pre-release, preserved)

- Game profile selector: Auto-detect / Elvira I / Elvira II.
- GAMEPC Text column fills the available grid width.
- GAMEPC cell tooltip uses the actual hovered text instead of stale shared state.
- Elvira I interactive NPC dialogue validator for the observed 96-byte DOS rule, including current/limit/overrun and predicted truncated suffix.
- Elvira II does not inherit the Elvira I dialogue limit automatically.

## RUNVGA Font Editor

The integrated font editor now works with actual RUNVGA font bytes when a supported layout is detected.

### Side-by-side editing

- ORIGINAL: read-only glyph loaded from the selected RUNVGA.EXE.
- EDITED: independent working copy.
- Reset restores the selected glyph from ORIGINAL.
- Modified glyphs are marked in the slot list.

### Character coverage

The list always contains all 256 CP852 byte values, including letters, digits, punctuation and symbols. This includes characters such as `!`, `?`, `"`, `'`, `,`, `.`, `:`, `;`, brackets and other CP852 slots.

### Recovered geometry

- one glyph = 8 bytes = 8 bitmap rows,
- renderer-visible width = 6 pixels (bits 7..2),
- the two rightmost columns (bits 1..0) are renderer-inactive,
- those two columns are displayed gray,
- normal editing locks them; Advanced mode can unlock them for forensic work.

### Supported RUNVGA layouts

1. **Original unpacked ASCII layout**
   - 98 glyphs, byte values 32..129,
   - recovered load-module font offset: `0x17E16`,
   - validated using the blank space glyph, recovered lowercase `a` bitmap and nearby `Press any key to continue` string,
   - relocated/unpacked fallback can find the table by glyph signature.

2. **V5 extended CP852 layout**
   - 256 glyphs x 8 bytes = 2048 bytes,
   - renderer patch is identified by the recovered V5 patch signature,
   - the extended font is read from the final 2048 bytes of the EXE,
   - known production build physical font offset: `0x3AE60`.

If a RUNVGA is packed/compressed or belongs to an unknown release and neither layout can be verified, the editor reports `Unknown / unsupported` and disables writing instead of guessing an offset.

### Safe output

- `Save copy as...` creates a modified EXE copy.
- `Apply to EXE` first creates a timestamped backup `RUNVGA.EXE.bak_font_YYYYMMDD_HHMMSS` and only then writes edited glyph bytes.
- Unknown layouts are never modified.

### Automatic load

Opening Font Editor from the main application passes the current game directory. If `RUNVGA.EXE` exists there, the font editor attempts to load it automatically. Manual `Open RUNVGA.EXE` remains available.

## Intentionally not included yet

- RUNEGA.EXE support. The source model already reserves `FontSourceType.RunEga`, but no EGA offsets or renderer assumptions are invented.
- Automatic conversion of an original 98-glyph RUNVGA into the V5 architecture. That operation patches executable code and should be implemented/tested separately from simple glyph editing.
- Generic unpacking of compressed/packed RUNVGA variants.
- Elvira II-specific RUNVGA layout assumptions until separately verified.
- Compile verification in this environment (no .NET SDK/MSBuild is installed here); Visual Studio is the first build check.

## Font Editor performance/display fix

- Replaced the 128 child controls used by the two 8x8 matrices with two double-buffered custom-drawn matrix controls.
- Matrix selection/pixel toggling no longer destroys and recreates 128 WinForms controls on every click or glyph change.
- Replaced the textual glyph ListBox with owner-drawn rows. Only visible rows are painted.
- Every slot 0x00-0xFF is now visible by byte value, including control/non-printable slots.
- Each list row includes a thumbnail rendered from the actual loaded Elvira glyph bitmap, so display does not depend on Windows/Consolas supporting the corresponding CP852 Unicode character.
- Control bytes are labelled as [0xNN], and SPACE is explicitly labelled [SPACE].

## 2026-08-22 - RUNVGA active-font detection + font import/export

- Fixed V5 detection using the runtime-verified renderer bytes at physical `0xF356`: `8C D8 05 4F 23 8E D8`.
- V5 executables now load the active 256 x 8 CP852 font from fixed physical offset `0x3AE60`.
- Removed the obsolete 23-byte V5 signature.
- Removed `data.Length - 2048` font-location logic; appended hooks after the font no longer break detection.
- Removed fallback scanning for the historical lowercase `a` glyph; this previously selected the orphaned 98-glyph table inside patched V5 builds.
- Original unpacked RUNVGA is recognized only when the original renderer signature `BE A6 28` is present at physical `0xF355` and the 98-glyph table validates.
- Unknown layouts are read-only / unsupported instead of being guessed.
- Added `Import font...` for V5 layouts. It accepts either:
  - a raw 2048-byte (`256 x 8`) `.bin` / `.fnt`, or
  - another detected V5 RUNVGA `.EXE`.
- Import changes only the EDITED side. ORIGINAL always remains the font loaded from the target EXE.
- Added `Export font...` for V5 layouts. It writes the current EDITED font as an exact 2048-byte binary asset and reports SHA-256.
- Existing `Apply to EXE` and `Save copy as...` remain available; Apply creates a timestamped backup first.
- Original 98-glyph RUNVGA files remain editable in-place for their native 0x20-0x81 range, but standalone 256-glyph import/export is disabled because those EXEs do not contain a complete CP852 font asset.
- RUNEGA remains architecture-reserved but is not implemented in this build.
- Automatic upgrade of an original 98-glyph RUNVGA to the V5 extended renderer is NOT implemented yet.

## Font UI fix 2026-08-22
- REAL BITMAP PREVIEW moved into a dedicated docked right panel so it cannot be clipped off-screen.
- Added quick jumps for 0-9, A-Z, a-z and CP852 slots.
- Original 98-glyph RUNVGA detection now has a strongly-validated table fallback at 0x1A216 for unpacked variants where renderer bytes differ.
- Unknown small/packed RUNVGA files now explain that the native 98-glyph font is compressed in the packed load module and appears normally only after DOS self-unpacking at runtime.
- Native original range 0x20-0x81 is explicitly shown in the source status; it includes punctuation, digits and normal Latin letters.


## Font Editor working-copy state
- EDITED now starts empty after opening an EXE.
- Added `Copy Original → Edited` to explicitly create a working copy.
- Import initializes EDITED from the selected 256x8 font.
- Apply/Save/Export remain disabled until EDITED has usable data.
- List marker: `*` = source only, `E` = edited equals original, `M` = modified.

### Packed original RUNVGA font visibility fix
- Added detection for the known packed Elvira I RUNVGA printable font run at physical 0x15A69.
- The packed run covers byte codes 0x2F-0x7E (80 glyphs), therefore digits 0-9, A-Z and a-z are now visible even before full DOS self-unpacking.
- Code 0x5E is stored as a 7-scanline glyph in this packed layout; subsequent glyph offsets are shifted by one byte and the loader compensates for it.
- Detection validates known original A/a bitmaps plus additional non-empty glyphs before accepting the packed layout.
- Packed source is treated as a read/edit working source only: direct Apply/Save-to-packed-EXE remains disabled until packed-runtime writeback is separately proven.
- Copy Original -> Edited is available for the directly recovered glyphs.

## Vector font import (TTF / OTF / SFD)
- `Import font...` now accepts `.ttf`, `.otf`, and `.sfd` in addition to raw `.bin`, `.fnt`, `.f08`, and V5 RUNVGA `.exe` sources.
- Vector glyphs are mapped Unicode -> CP852 and rasterized to the Elvira renderer's 6x8 active bitmap area (bits 7..2), producing a 256x8 / 2048-byte working font in EDITED.
- Import dialog lets you choose the font family, pixel size, X offset, and Y offset, with a live 2x CP852 preview before committing the rasterized result to EDITED.
- Rasterization uses monochrome `SingleBitPerPixelGridFit`; the two renderer-inactive rightmost bits stay clear.
- `.sfd` import is supported when FontForge is installed. The editor locates `fontforge.exe` in PATH/common Windows install folders, converts the SFD to a temporary TTF, rasterizes it, then removes the temporary file. If FontForge is not installed, the editor gives a clear error and leaves EDITED untouched.

## 2026-08-22 - composed CP852 diacritics for vector imports
- Added optional (default ON) `Compose CP852 diacritics from base letters` mode for TTF/OTF/SFD imports.
- The rasterizer first creates normal ASCII glyphs, then constructs common Slovak/Czech CP852 glyphs from those same base letters plus tiny 6x8 pixel accents.
- Slovak `ď/Ď`, `ľ/Ľ`, and `ť/Ť` are handled specially with a right-side apostrophe-like caron rather than the normal caron-above template.
- Covered accents include acute, caron, circumflex, diaeresis and Czech ring; direct precomposed vector rasterization remains available by unchecking the option.
- Goal: avoid the severe base-letter shrinking/distortion observed when Windows rasterizes precomposed Unicode diacritics directly into only 6x8 pixels.

## Optical CP852 diacritic composer
- Composed vector import no longer places accents at fixed columns.
- Accent X position is derived from the rasterized base glyph's real 6x8 bounding box.
- Top accents reserve 1-2 scanlines and vertically resample the base glyph when needed instead of blindly shifting/cropping its bottom.
- Narrow glyphs such as i/l/r and wide glyphs such as A/M/W therefore receive different accent positions automatically.
- Slovak ď/ľ/ť and Ď/Ľ/Ť keep dedicated right-side apostrophe/caron rules; ľ/Ľ uses a tighter vertical mark.
- Manual global X/Y offsets remain available as a final override before optical composition.

## 2026-08-22 - baseline-aware vector font compositor
- Added a font-wide baseline normalization pass before composed CP852 diacritics are generated.
- Normal uppercase/lowercase Latin letters and digits target bitmap row 6; lowercase descenders `g/j/p/q/y` may extend to row 7.
- Accented variants preserve the same class baseline as their unaccented ASCII base glyph.
- Top-accent room is now created with baseline-preserving vertical resampling instead of per-glyph optical repositioning.
- Right-side caron forms (`d/l/t` families) keep the normalized base letter baseline and only adjust horizontally when necessary.
- Status text now reports `composed/baseline-aware` for this mode.

## Full CP852 character-set viewer
- Added `Full character set...` button next to REAL BITMAP PREVIEW.
- Opens a separate 16x16 CP852 table for all byte slots `0x00-0xFF`.
- Zoom slider supports integer nearest-neighbour zoom from 1x through 8x.
- Viewer can switch between EDITED and ORIGINAL without closing.
- Each cell shows the bitmap glyph, the CP852 slot label when compact enough, and the byte code.
- Table is scrollable at larger zoom factors.
- Clicking any cell selects the same byte slot in the main font editor.

## Full character set button visibility fix
- Replaced fixed `X=250` positioning in the REAL BITMAP PREVIEW header with a two-column `TableLayoutPanel`.
- `Full character set...` now occupies an AutoSize right column, so it remains visible when the preview pane is narrow or Windows DPI scaling changes.
- The viewer itself remains a 16x16 CP852 table with Edited/Original switching and integer zoom 1x-8x.

## 2026-08-22 — full 8×8 vector import + wide character-set viewer
- Vector TTF/OTF/SFD import now rasterizes the complete stored 8×8 glyph instead of discarding bits 1..0.
- Default vector preset is 8.0 px, X offset +1, Y offset 0, composed/baseline-aware diacritics enabled.
- X/Y manual offsets are intentionally limited to -2..+2 for small 8×8 alignment adjustments.
- The two rightmost columns are preserved in EDITED/import/export and appear in gray in the glyph editor and full-set viewer.
- Warning: the recovered Elvira DOS renderer still displays only the left six columns; pixels stored in gray columns may therefore be invisible in-game.
- Full Character Set viewer defaults to 32×8 wide layout and can switch between 16×16, 32×8 and 64×4.
- Full Character Set viewer renders all eight stored columns and marks columns 7–8 in gray.


## Unified editor navigation

- Added permanent top navigation: **Sprite Editor | Text Editor | Font Editor | About**.
- The active editor button is shown in inverse/highlight mode.
- Text Editor no longer needs a `Back to VGA` button.
- Font Editor is now hosted inside the main application window instead of opening as a separate modal window.
- Switching editor modes preserves the main application window and toolbar.
- The embedded Font Editor can reload `RUNVGA.EXE` when the game directory changes.

## Unified navigation follow-up: maximized startup + embedded font toolbar visibility

- Main application now starts maximized (`FormWindowState.Maximized`).
- Embedded Font Editor is hosted below the permanent application header.
- This restores the Font Editor command row inside unified mode: Open RUNVGA.EXE, Apply to EXE, Save copy as, Copy Original -> Edited, Import font, Export font.
- Cause fixed: the unified application header overlaid the first ~78 px of the hidden tab page, so the Font Editor's own top toolbar was present but hidden underneath it.

## v1.1b – localization + application icon fix
- Unified navigation labels are now fully localized: Editor spritov / Sprite Editor / Editor spritů, Text editor and Font editor.
- Sprite grid headers `Flags` and `Edit` now follow SK/EN/CZ localization.
- Text editor localizes Search, Encoding, Text context, context choices, File offset and GAMEPC string count.
- Embedded Font Editor now follows the selected UI language, including toolbar, headings, bitmap preview, glyph categories, source/layout labels and full character set viewer.
- Vector TTF/OTF/SFD import dialog now follows SK/EN/CZ localization.
- Version raised to `v1.1b` (`AssemblyVersion/FileVersion 1.1.0.1`).
- Restored application/taskbar icon by embedding `app.ico` in the executable and assigning the associated executable icon to the main window.

## v1.1b text diagnostics / UI polish

- GAMEPC text editor now shows current encoded byte count and a DOS-limit diagnostics column.
- For **Elvira I / Interactive NPC dialogue**, the observed original-DOS 96-byte CP852 word-aware limit is simulated. Over-limit rows show `+N` bytes and the selected-row warning also reports the approximate number of characters that would be omitted at the word boundary.
- Elvira II does not inherit the Elvira I limit until it is experimentally proven.
- Detected/selected game profile in the shared header is bold in every mode/language.
- Font import status bolds the imported font filename.
- Font-in-context bitmap samples render at 2x their previous size.
- Sprite `Zones found` information moved from the shared header to the Sprite Editor bottom status bar, matching the GAMEPC status-bar pattern.

## v1.1b – non-destructive glyph positioning
- Added arrow buttons next to the EDITED glyph and keyboard arrow support when the EDITED matrix has focus.
- Arrow movement is non-destructive: glyph pixels are stored in logical coordinates and only ShiftX/ShiftY changes while positioning.
- Pixels outside the final 8x8 area are preserved in memory while editing and can be brought back with the opposite arrow.
- Apply to EXE, Save copy and Export font are the clipping boundaries. If any pixels are outside 8x8, the editor shows a localized SK/EN/CZ warning with glyph/pixel counts before continuing.
- Reset glyph resets both pixels and shift back to the original glyph.

## v1.1b hotfix - Text editor DataGridView reentrancy
- Fixed intermittent `System.InvalidOperationException` / `SetCurrentCellAddressCore` reentrant call while leaving an edited text cell or moving through the GAMEPC grid.
- Added `_refreshingTextGrid` guard so programmatic row rebuild/selection changes do not re-enter selection/current-cell handlers.
- `CellEndEdit` no longer rebuilds the grid synchronously; the rebuild is queued with `BeginInvoke` until the DataGridView finishes its current-cell transition.
- Refresh preserves the selected GAMEPC entry and attempts to preserve scroll position.
- CurrentCell/Selection validation handlers are suppressed during programmatic refresh.

## v1.2 – text diagnostics / validation / font workflow

- Automatic Elvira I diagnostics: 417/424 confirmed affected, 298/425/627 confirmed safe, other >96 B as possible risk.
- Risk-only filter, visible/clipped simulation and per-string ignore sidecar `.pi1-text-diagnostics.json`.
- Strict encoding/NUL validation and post-write string-count round-trip check.
- Vector import presets: Default, Lexis, Pixel Operator, HP-style.
- Full Character Set status includes ID/HEX/table offset; 32×8 remains default with zoom 1×–8×.
- Font footer status summarizes glyph/edited/shifted counts.
- E2 remains a separate profile; no inherited 96 B rule.
