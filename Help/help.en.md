# π1 Elvira Editor Help

## [getting-started] Getting Started
Use the **Installation** selector to choose a supported VGA game. The editor remembers valid installations in your user application settings, never in the game folder. **Find games...** performs a fast, bounded check of common GOG locations and remembered paths; it does not scan entire disks automatically at startup. **Browse folder...** always lets you validate and add a game folder manually. Multiple Elvira I/II installations remain separate entries, and the selected installation identifies and activates the supported game automatically. Select English, Slovak, or Czech in **Interface language**. Use Graphics, Text, Font, and Mods & Launcher; Help is read-only and About is a short product summary.

Supported games: **Elvira: Mistress of the Dark** and **Elvira II: The Jaws of Cerberus**.

## [graphics] Graphics Editor
Choose a VGA file, palette, sprite entry, and pixel zoom. The list identifies RAW/RLE entries where relevant and the preview shows the selected image. Use Replace PNG or Import PNG to make an edit, Export PNG to save a copy, Cancel edit/Discard to abandon it, **Save To Project** to persist it to the project, and Restore original when an original backup is available. Only current supported VGA workflows are offered.

## [text] Text Editor
Open a compatible GAMEPC-style data file, reload it, search it, choose encoding/context, then Save, Save As, or Create Variant. Compatible names include GAMEPC, GAMEPCSK, GAMEPCCZ, GAMEPCDE, and MYMOD: compatibility is determined by the supported data format and context, not the literal name. GAMEPC automatically rebuilds its complete text pool when translation length changes, preserving logical string indices and the file's non-text data. Original B, Translation B, and Δ B are informational encoded-byte counts; no fixed Elvira I or Elvira II runtime text-length limit is currently shown.

The **Original EN** edition is the read-only baseline. Editable editions (e.g., SK, S1) own isolated project state. **Save** writes project state only. **Build Variant** materializes GAMEPCxx into editor-owned VARIANTS.

## [game-data] Game Data Files
The current data file is the file currently open in Text Editor. A variant has a display name, data file, enabled state, order, and availability. **ELVIRA_MODS.INI** stores that catalog. Opening Mods & Launcher alone never creates or rewrites it. A missing file remains in the catalog as Missing/unavailable and is never silently deleted.

## [variants] Variants and Mods
Use Add, Edit, Remove, Move Up, Move Down, Enable/Disable, and Open Data File in Text Editor to manage catalog metadata. Removing a variant removes only catalog metadata, never its physical data file. The launcher shows enabled, available variants in catalog order.

## [mods-launcher] Mods & Launcher
Set Launcher file, Modder name, and Default variant. The table shows #, Name, Data file, Status, and Enabled. Preview BAT makes no writes; Generate / Update Launcher writes the selected BAT; Restore Original Launcher copies its immutable same-basename .BAK back to the active BAT.

The runtime menu uses ENTER for DefaultVariant, 1..9/A..Z for explicit variants, 0 for Sound Setup, ESC to return to DOS, and ignores invalid/unassigned keys. It supports 35 explicit variants and packs entries horizontally in the BAT menu where they fit.

## [sound] Sound Setup
Generated launchers are game-specific.

### Elvira I
`RUNVGA <DataFile> <sound option>`: /p PC Internal Speaker; /t IBM/Tandy 3 voice; /a AdLib sound card; /c CMS Gameblaster; /s Soundblaster music card; /m0 Generic Midi; /m1 Roland MT-32 / LAPC-1; /m2 Casio CT460.

### Elvira II
`RUNIT <DataFile> <sound option>`: /p PC Internal speaker; /t IBM/Tandy 3 voice; /a AdLib sound card; /s Soundblaster music card; /m Roland MT-32 / LAPC-1.

Runtime Sound Setup creates/updates `SET PI1SND=/s` in PI1SND.BAT, not ELVIRA_MODS.INI. Missing/invalid PI1SND.BAT forces setup: Back/ESC/ENTER are ignored until a valid choice. Normal setup permits 0 = Back.

## [font] Font Editor
Preview the original font, work in the editable copy, use **Import font...**, Export font..., Apply Font, Save Patched Copy, filters, glyph editor, Original/Edited views, shifts, and 1x/2x/4x/8x previews. "Import font..." intentionally supports more than TTF.

All recovered renderers use 8 rows and show 6 columns from source bits 7..2; the two rightmost bitmap columns are renderer-inactive. Original glyph 0x81 is `00 FC FC FC FC FC FC 00`, a protected HUD erase/blanking glyph. Patched V5/RUNIT-V2 extended fonts use `FC FC FC FC FC FC FC FC` for an intentional 6x8 full-cell HUD erase. Editing, copy/reset, import, and every V5/V2 output path enforce that value; generated RUNEGA intentionally retains the original `00 FC FC FC FC FC FC 00` mask; opening an older malformed patched EXE remains read-only until you explicitly create an output.

### Elvira I / Elvira II
Original RUNVGA table: 0x1A216..0x1A525, 784 bytes, 98 glyphs, 0x20..0x81. V5: 0x3AE60..0x3B65F, 2048 bytes, 256x8 CP852-compatible table. Original RUNIT: 0x168CA..0x16BD9, same 784-byte/98-glyph range. RUNIT V2 is split: LOW 0x20..0x81 original; HIGH 0x82..0xFF at 0x28480..0x2886F (126x8=1008), helper at 0x28870. These are validated supported executable layouts, not universal offsets.

## [backups] Backups and Restore
Opening/previewing does not create a backup. The standalone Font Editor **Apply changes to EXE** creates an immutable O-file backup (RUNVGAO.EXE/RUNITO.EXE) before writing. Launcher generation creates an immutable .BAK once. Normal project Text/Graphics edits do not write O-files to GameRoot; they save to project state and materialize through CompositeBuild into owned VARIANTS. Restore operations are explicit and scoped to editor-owned mutable backups.

## [technical] Technical Details
Elvira I V5 provides the 256x8 extended font table; Elvira II RUNIT V2 uses split low/high font storage. PI1MENU.COM is a project-owned DOS helper for portable BIOS keyboard input, valid-key filtering, ENTER/Setup/Variant/ESC mapping, and the fixed 17x8 Pi1 Sound Setup logo. It does not launch games, generate BAT, manage variants, or store sound preferences: BAT is launcher logic and COM is UI/input helper. The logo uses CP437 block 0xDB, Pi attribute 07, digit 1 attribute 0F. Target environments include classic DOSBox, DOSBox-X, DOSBox Staging, and compatible DOS environments.

## [troubleshooting] Troubleshooting
If a game is not detected, choose the correct VGA installation; unsupported EXEs cannot be patched. Check missing data files/variants and choose an available default. Preview makes no writes. If PI1SND is missing/invalid, complete forced Sound Setup. Font import or Apply Font is disabled until a supported source/working copy is loaded; 0x81 is intentionally not editable. Do not overwrite an existing BAT backup or accept replacing a manually modified launcher without review.

## [supported] Supported / Unsupported
### Supported in v1.0
- **Elvira: Mistress of the Dark — VGA**
- **Elvira: Mistress of the Dark — EGA**
- **Elvira II: The Jaws of Cerberus — VGA**

### Currently unsupported
`RUNEGA` font/CP852 mutation is outside the supported scope. EGA/RUNEGA is a supported runtime target for CompositeBuild; only RUNEGA executable/font patching is unsupported.

## [about] About / Credits
**π1 Elvira Editor v1.0** supports VGA/EGA graphics editing, GAMEPC text/data variants, extended CP852-compatible fonts, Mods & Launcher, and a portable DOS launcher helper. Glyph `0x81` is protected as the HUD erase glyph. RUNEGA font/CP852 mutation is unsupported; EGA/RUNEGA remains a supported runtime target.

Project: `https://github.com/Pivan781005/Pi1-Elvira-Editor`

Remember these crews? :) Phrozen Crew, UCF, CORE, PARADOX, Razor 1911, Fairlight, TRSi, INC, Hybrid, Prestige, CLASS, MYTH, Drink Or Die, DREAD, ORiON, ECLiPSE. A little tribute to the scene that made DOS history; no affiliation or endorsement implied.
