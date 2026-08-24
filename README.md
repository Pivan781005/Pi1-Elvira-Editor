# π1 Elvira I&II VGA Editor v1.3

A Windows editor for the original DOS releases of **Elvira: Mistress of the
Dark** and **Elvira II: The Jaws of Cerberus**. It edits VGA resources, GAMEPC
text, and the games' extended CP852-compatible bitmap fonts.

## Opening a game

Use **Open Game EXE...** in the Font Editor. The editor identifies supported
Elvira I and Elvira II executable formats from their binary contents, rather
than relying on the filename. Opening or previewing files is read-only.

## Fonts

- Elvira I uses the verified V5 extended-font format.
- Elvira II uses the verified V2 split-font format.
- Both expose font slots `0x20..0xFF`: **224 slots**, **223 editable glyphs**.
- Editable slots are `0x20..0x80` and `0x82..0xFF`.
- Slot `0x81` is reserved in both games. It is an internal HUD erase glyph and
  is automatically preserved during editing, TTF import, save, and apply.

Import a tested `.ttf` font through **Import font...**. The editor rasterizes it
into the DOS bitmap font and maps supported characters to CP852. Normal font
editing, import, export, save-copy, and apply operations keep the reserved
`0x81` glyph intact.

## Text and VGA resources

The Text Editor supports GAMEPC text with CP852, Windows-1250, and raw Latin-1
encoding choices. The VGA Editor previews and replaces supported VGA images.

## Backups and safety

The first persistent executable activation preserves the original executable
and GAMEPC data as immutable O-files:

- Elvira I: `RUNVGA.EXE` → `RUNVGAO.EXE`, `GAMEPC` → `GAMEPCO`
- Elvira II: `RUNIT.EXE` → `RUNITO.EXE`, `GAMEPC` → `GAMEPCO`

Only a VGA file that is actually changed receives an immutable first-write
backup, for example `012.VGA` → `012O.VGA`. Previewing, opening, and scanning
do not create backups. Existing O-files are never overwritten; repeated apply
operations regenerate only the active files from the preserved originals.

## Limitations

Use the exact supported English DOS executable variants. Keep the generated
O-files. Font import validation is based on the tested `.ttf` workflow; always
review the bitmap preview before applying changes to a game installation.
