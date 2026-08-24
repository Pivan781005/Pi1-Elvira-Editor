# Changelog

## 1.3

- Added verified extended CP852 font support for Elvira I and Elvira II.
- Added automatic binary-content detection for supported game executables.
- Added immutable executable, GAMEPC, and first-write VGA backup workflows.
- Added tested TTF import for CP852 bitmap-font generation.
- Protected engine-reserved glyph `0x81` in both games; it is used internally
  to erase dynamic HUD values before redraw.
- Added Elvira I V5 and Elvira II V2 golden, import, save/reopen, and backup
  workflow regression coverage.

## 1.2

- Elvira I automatic text diagnostics with confirmed/possible/safe/ignored states.
- Word-aware 96-byte simulation for known Elvira I dialogue-risk cases.
- Risk-only text filter and persistent per-string ignore sidecar.
- Strict text save validation for NUL, encoding round-trip and string count.
- Preserved TextGrid reentrancy hotfix.
- Non-destructive glyph shifting with clipping only at output time.
- Vector font import presets: Default, Lexis, Pixel Operator and HP-style.
- Full Character Set status information (ID, HEX, table offset), 32×8 default and 1×–8× zoom.
- Unified localized status summaries and SK/EN/CZ strings for new UI.
- Elvira II remains a separate profile and does not inherit the Elvira I 96-byte rule.

## 1.1b

- Unified Sprite/Text/Font navigation.
- Font import/export and CP852 256-glyph workflow.
- Baseline-aware composed Slovak diacritics.
- Full character-set viewer.
- Application icon/localization fixes.
- TextGrid reentrancy hotfix.

## v1.2 localization refresh fix
- Localized the Game profile Auto-detect value: SK `Automatická detekcia`, EN `Auto-detect`, CZ `Automatická detekce`.
- Language changes now refresh the detected/selected game caption immediately; no extra Refresh action is required.
- Explicitly center the `Copy Original -> Edited` button text vertically and horizontally.
- Localized recent Font Editor dialogs/messages and file-dialog captions/filters in Slovak, English, and Czech.
- Removed several remaining hard-coded Slovak/English UI strings from Sprite/Font editor initialization paths.
