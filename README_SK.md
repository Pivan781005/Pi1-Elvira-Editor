# π1 Elvira Editor v1.0 (SK)

Aktuálna verejná identita: **π1 Elvira Editor v1.0** (`1.0.0`).

Aktuálny pracovný postup v1.0: **EDIT → SAVE TO PROJECT → BUILD VARIANT → RUN**.
Originálne súbory v GameRoot (`RUNVGA.EXE`, `RUNEGA.EXE`, `RUNIT.EXE`, `GAMEPC`,
VGA zdroje) sú nemenné; výstupy patria do vlastnených
`VARIANTS\<runtime>\<edition>\`; priamy zápis do GameRoot bol odstránený.
Priame `APLIKOVAŤ DO HRY` / `.bak_original` postupy popísané nižšie sú
predbežné (pre-release) a vo v1.0 neplatia. Podrobný aktuálny popis je
v `README.md` a v lokalizovanej Pomoci (`Help/help.sk.md`), UI jazyky: EN/SK/CS.

---

Nižšie je zachovaný historický predbežný obsah (neaktuálny workflow, verzie
1.1b–1.3) pre referenciu; nie je to popis v1.0.

# π1 Elvira Editor — GUI + automatický deploy

## Bezpečnosť CP852 fontu pre Elvira I a II

Verzia 1.3 podporuje vlastné CP852 fonty pre Elvira I aj Elvira II. Glyph
`0x81` je v oboch hrách rezervovaný na mazanie dynamických HUD hodnôt pred
prekreslením, preto ho editor automaticky zachová. Ostatné podporované glyphy
je možné normálne upravovať alebo importovať.

Toto je GUI verzia decoder/encoder workflowu pre **Elvira 1 DOS xNN2.VGA**.

## Čo pribudlo

- Windows Forms GUI.
- Výber herného adresára.
- Automatické nájdenie všetkých `012.VGA`, `022.VGA`, ...
- Zoznam image entries s ID, offsetom, RAW/RLE, rozmermi a flags.
- Náhľad bitmapy priamo z VGA.
- `Nahradiť PNG...` pre vybraný image.
- `Export PNG...`.
- Viac editov naraz.
- Automatický rebuild VGA.
- Automatické premenovanie/produkčný deploy do herného adresára.
- `Obnoviť originál`.

## Automatický backup/deploy

Pri prvom deployi pre napr. `012.VGA` vznikne:

- `012.VGA.bak_original` — čistý originál. **Nikdy sa automaticky neprepisuje.**
- `012.VGA.bak_previous` — verzia tesne pred posledným deployom.
- nový patched súbor sa automaticky presunie na `012.VGA`.

Takže po stlačení `APLIKOVAŤ DO HRY` už nemusíš ručne nič kopírovať ani premenovávať.

Tlačidlo `Obnoviť originál` vráti `.bak_original` späť na pôvodný názov.

## Ako použiť

1. Otvor `Pi1ElviraEditor.sln` vo Visual Studio 2022.
2. F5.
3. Herný adresár je predvyplnený:
   `C:\Games\GOG\Elvira`
4. Vyber napr. `012.VGA`.
5. Klikni na image v zozname — zobrazí sa náhľad.
6. `Nahradiť PNG...`
7. Po príprave editov klikni `APLIKOVAŤ DO HRY`.
8. Spusti Elviru.

## Dôležité k PNG

Zatiaľ je to stále diagnostická 16-farebná paleta.

Replacement PNG musí:
- mať úplne rovnaké rozmery,
- používať iba existujúce diagnostické farby,
- nemať antialiasing,
- transparentná = index 0.

Ďalší krok môže byť doplnenie reálnej Elvira palette/mask semantiky, aby náhľady
zodpovedali farbám priamo v hre.


## Scroll fix

Zoznam image entries má zapnutý natívny vertikálny scrollbar. Funguje aj koliesko myši, Home a End.

## GRIDFIX

Zoznam image entries bol zmenený z ListView na DataGridView.

Dôvod: ListView sa v niektorých DPI/layout kombináciách vedel dostať do stavu,
kde natívny scrollbar neumožnil návrat na úplný začiatok zoznamu.

DataGridView má:
- natívny vertikálny aj horizontálny scrollbar,
- explicitné FirstDisplayedScrollingRowIndex = 0 po načítaní,
- Home = prvý image,
- End = posledný image,
- zobrazenie aj image ID 0000, ak je jeho entry platný.


## TOPPAD workaround

Pred tabuľku je vložených 5 prázdnych riadkov ako workaround pre WinForms/DPI scroll bug. Reálne image entries začínajú až pod nimi.


## PALETTE update

GUI teraz pri `xNN2.VGA` automaticky hľadá zodpovedajúci `xNN1.VGA`:

- `012.VGA` -> `011.VGA`
- `092.VGA` -> `091.VGA`

Z `xNN1.VGA` načíta palette table podľa Elvira 1 DOS AGOS formátu.
V hornej časti GUI pribudol prepínač `Paleta`.

Prvá reálna palette bank sa vyberie automaticky. Ak má zóna viac palette bankov,
môžeš medzi nimi prepínať a okamžite vidíš výsledok v preview.

Farba index 0 sa v preview zobrazuje transparentne, rovnako ako pri bežnom
transparentnom sprite draw v engine.

Nad tabuľkou je pevná legenda:

`Poradie/ID | Offset v xNN2.VGA | Typ (RAW/RLE) | Rozlisenie | Flags | Edit`


## SPACERFIX

Odstránených 5 prázdnych dátových riadkov. Namiesto nich je nad tabuľkou pevný 75 px horný panel; jeho spodných 24 px tvorí legenda. Reálne image entries začínajú od prvého riadku gridu.


## SPACERFIX72

Horný odstup nad tabuľkou bol zväčšený z 64 px na 72 px, aby sa hlavička stĺpcov zobrazila celá aj pri tvojom DPI/layout nastavení.


## ZOOM75 update

- horný odstup nad tabuľkou: 75 px,
- väčší pravý panel pre náhľad spriteov,
- väčšie predvolené okno aplikácie,
- nový zoom náhľadu: Fit / 100 / 200 / 300 / 400 / 600 / 800 %,
- pri väčšom zoome je preview v scrollovateľnom paneli.


## HALFSPLIT + CENTERED75 update

- pri štarte sa hlavné okno rozdelí 50/50 medzi tabuľku a renderer,
- používateľ môže splitter potom normálne ručne posúvať,
- renderer má rovnaký horný offset 75 px ako tabuľka,
- 100/200/300/... % zoom je centrovaný, pokiaľ sa sprite zmestí,
- ak sprite prerastie viewport, zobrazia sa scrollbary,
- ak prerastie iba jeden rozmer, druhý rozmer zostáva centrovaný,
- pri zmene veľkosti okna sa náhľad automaticky precentruje.


## HALFSPLIT FIX2

Opravený pád:
`SplitterDistance must be between Panel1MinSize and Width - Panel2MinSize`.

Zmena:
- Panel1MinSize/Panel2MinSize sa počas BuildUi nastavujú na 0.
- Až po dokončení layoutu sa vypočíta bezpečné minimum podľa reálnej šírky okna.
- 50/50 SplitterDistance sa clampne do platného rozsahu.
- Ak je WinForms práve v prechodnom layout stave, vykoná sa jeden oneskorený retry.
- Po štarte ostáva 50/50 split; používateľ ho potom môže ručne posúvať.
- 75 px offset a centered zoom zostávajú zachované.


## π1 Elvira I&II VGA Editor

Nové:
- názov aplikácie: π1 Elvira I&II VGA Editor,
- UI jazyky: Slovenčina / English / Čeština,
- nová karta Editor textov,
- načítanie GAMEPC string table od offsetu 0x12,
- index + file offset + dĺžka + editovateľný text,
- filtrovanie/hľadanie,
- selectable encoding: CP852 / Windows-1250 / Latin1/Raw,
- bezpečné in-place uloženie iba rovnakej alebo kratšej dĺžky,
- kratšie reťazce sa doplnia nulami,
- dlhšie reťazce sú odmietnuté,
- GAMEPC.bak_original a GAMEPC.bak_previous.


## Rebranding

Aplikácia bola premenovaná na `π1 Elvira I&II VGA Editor`.


## Localization fix
- default UI language is now English,
- removed redundant manual column legend above the grid,
- DataGridView column headers are localized directly,
- dynamic zone-count status is localized too.


## Version 1.0 / About
- titulok aplikácie: `π1 Elvira I&II VGA Editor v1.0`
- pridané tlačidlo `About / O programe / O programu`
- About zobrazuje názov, verziu, stručný popis a GPL-3.0 licenciu
- assembly/file version nastavená na 1.0.0


## Compile fix
- odstránené neplatné odkazy na neexistujúce `btnCancelEdit` a `btnApply`
  v `ApplyLanguage()`.
- lokalizácia používa iba reálne existujúce button fields.


## Startup localization fix
- English is now the actual internal default language before the initial folder scan.
- Initial zone-count status uses localized `ZonesFound`, so startup shows `Zones found: N`.
- Language ComboBox widened to 150 px so `Slovenčina`, `English`, and `Čeština` fit.
- Pixel zoom / About / status controls shifted right accordingly.


## LANGFIX2
Opravené priamo v zdroji:
- `ScanGameFolder()` už neprepisuje English status hard-coded slovenským
  `Nájdených zón`, ale používa `UiText["ZonesFound"]`.
- `Palette:` a `Language:` captions sú fieldy a menia sa za behu.
- názvy palette bankov sa lokalizujú (`Palette 000` / `Paleta 000`).
- diagnostická paleta sa lokalizuje.
- default zostáva English.


## UIFIX3
- Pixel zoom lokalizovaný do SK/EN/CZ.
- Language ComboBox používa owner-draw s paddingom, aby bol text čitateľný.
- Pridané viditeľné tlačidlo Text editor / Textový editor.
- Tlačidlo otvorí existujúcu kartu GAMEPC text editora.
- About text používa lokalizované texty a skutočné nové riadky.


## Language ComboBox centering
- language ComboBox uses OwnerDrawFixed
- `Slovenčina`, `English`, `Čeština` are horizontally and vertically centered
- selected item and dropdown entries use the same centered renderer


## Language dropdown spacing fix
- language ComboBox moved right to X=615
- width reduced to 145 px
- it no longer overlaps the `Language:` label
- centered item rendering remains enabled


## Restore warning + preview reload
- Restore original no longer throws when `.bak_original` is missing.
- Instead it shows a localized warning and returns safely.
- Added a Reload button next to preview Zoom.
- Reload re-decodes the currently selected image and reapplies the current zoom.


## GAMEPC parser fix
- first GAMEPC text slot starts at 0x12, but if byte 0x12 is '=' it is
  treated as a control/metadata marker rather than visible text;
- first editable string therefore starts at 0x13 and the '=' marker is
  preserved during save;
- all following strings continue to be parsed as NUL-terminated strings,
  so their physical offsets remain unchanged;
- row 666 at 0x3BDA is not synthetically extended: the editor shows the
  bytes actually present in the Slovak GAMEPC.


## GAMEPC parser fix 2
- prvý editovateľný text začína na 0x14; bajty 0x12-0x13 sú zachované ako prefix/metadata,
- hranica 0x3C00 už nereže posledný text,
- nový string sa nesmie začať na/za 0x3C00, ale posledný string začatý pred 0x3C00 sa dočíta až po NUL,
- tým sa opravuje index 666, ktorý začína na 0x3BDA a predtým mal presne 38 bajtov len preto,
  že 0x3C00 - 0x3BDA = 38.

## v1.1b – rozpracované zmeny

- Game profile: Auto-detect / Elvira I / Elvira II, s možnosťou manuálneho override.
- Textový stĺpec v GAMEPC editore vypĺňa dostupnú šírku okna.
- Tooltip textovej bunky je vždy generovaný z aktuálneho riadku (oprava stale/leak tooltipu).
- Voliteľný kontext `Interactive NPC dialogue` a validátor pre pozorovaný Elvira I DOS limit 96 bajtov v CP852. Limit nie je aplikovaný globálne na GAMEPC a Elvira II ho nepreberá bez dôkazu.
- Integrovaný Font Editor prototype: inventár všetkých 256 CP852 slotov vrátane písmen, číslic, interpunkcie a symbolov; známe slovenské glyphy sú editovateľné.
- Font editor zobrazuje 8x8 storage a vizuálne sivou označuje posledné dva stĺpce ako predpokladaný spacing/reserved pri 6x8 aktívnom glyphe. Toto rozloženie je zatiaľ označené ako provisional, kým sa z RUNVGA.EXE znovu neodvodí font table a patch mechanizmus.

### Font Editor - aktívny RUNVGA font a import/export (v1.1b)

Font Editor teraz rozlišuje aktívnu architektúru podľa DOS renderera, nie podľa náhodného výskytu bitmapy znaku v EXE. Pri V5 variante načíta plnú 256-znakovú CP852 tabuľku z `0x3AE60`; historická 98-znaková tabuľka, ktorá môže v patchnutom EXE stále fyzicky zostať, sa nepovažuje za aktívny font.

Pri V5 fontoch sú k dispozícii `Import font...` a `Export font...`. Import podporuje raw 2048-bajtový font (256 × 8) alebo font z iného detegovaného V5 RUNVGA.EXE. Import vždy mení iba pravú stranu EDITED; ľavá ORIGINAL zostáva referenciou fontu načítaného z cieľového EXE. Export uloží presne 2048 bajtov a vypíše SHA-256.

Originálny 98-glyph RUNVGA naďalej zobrazuje a umožňuje upravovať iba nativny rozsah 0x20-0x81. Sloty mimo neho sa jasne zobrazujú ako nenachádzajúce sa v danom EXE. Automatický upgrade originálneho RUNVGA na V5 CP852 renderer zatiaľ nie je súčasťou tejto verzie.

### Poznámka k originálnemu RUNVGA fontu
Natívny unpacked RUNVGA obsahuje 98 glyphov pre kódy 0x20-0x81. Sú v ňom bežné znaky, interpunkcia, čísla 0-9, A-Z a a-z. Ak je originálny RUNVGA stále zabalený/komprimovaný, táto tabuľka nie je vo fyzickom EXE dostupná na známom unpacked ofsete; vznikne až po self-unpackingu DOS programu. Editor taký súbor označí ako unsupported/packed namiesto zobrazovania falošných prázdnych glyphov.


## Font Editor – pracovná kópia
Po otvorení EXE zostáva EDITED zámerne prázdny. Tlačidlo `Copy Original → Edited` vytvorí explicitnú pracovnú kópiu reálne načítaných glyphov. Import 256×8 fontu naplní EDITED samostatne. ORIGINAL sa tým nemení.

## Packed original RUNVGA - pismena a cisla
Pri znamom zabalenom originalnom Elvira I RUNVGA vie Font Editor nacitat priamo z EXE realny 80-glyph usek 0x2F-0x7E na fyzickom ofsete 0x15A69. Tym su viditelne cislice 0-9, A-Z a a-z bez potreby spustat DOS unpacker. Znak 0x5E ma v packed variante iba 7 ulozenych scanline bajtov, preto loader kompenzuje jednobajtovy posun dalsich glyphov. Pri packed variante je priame prepisovanie EXE zatial vypnute; data sa daju pouzit ako ORIGINAL referencia a skopirovat cez Copy Original -> Edited.

### Import TTF / OTF / SFD fontov
Font editor dokaze okrem `.F08`/`.BIN`/`.FNT` nacitat aj vektorove `.TTF` a `.OTF` fonty a previezt ich do 256-slotovej CP852 tabulky Elviry. Pri importe sa Unicode znaky mapuju na CP852 a rasterizuju do realnej 6x8 aktivnej oblasti DOS renderera. Mozno nastavit velkost v pixeloch a X/Y posun a pred importom sa zobrazi bitmapovy nahlad. `.SFD` sa automaticky skonvertuje cez FontForge, ak je FontForge nainstalovany.

### TTF/OTF/SFD: skladaná diakritika pre 6×8
Pri importe vektorového fontu je predvolene zapnutá voľba **Compose CP852 diacritics from base letters**. Editor najprv rasterizuje čisté základné písmeno (`a`, `c`, `d`, `l`, `t`...) a potom pridá malý pixelový akcent vhodný pre 6×8. Tým sa základné písmeno nedeformuje tak ako pri priamom zmenšení hotového Unicode znaku. Slovenské `ď`, `ľ`, `ť` (aj veľké varianty) majú vlastný pravostranný mäkčeň/apostrofový tvar. Voľbu možno vypnúť a porovnať s priamym rasterizovaním TTF/OTF glyphu.

### Inteligentnejšia diakritika pri TTF/OTF importe
Režim `Compose + optically align CP852 diacritics` skladá slovenské/české znaky zo základného rasterizovaného písmena a pixelovej diakritiky. Poloha akútu, mäkčeňa, vokáňa, bodiek a krúžku sa už neurčuje pevnou X pozíciou, ale podľa skutočného bounding boxu každého 6x8 glyphu. Úzke `i/l/r` a široké `A/M/W` preto dostávajú odlišné optické centrovanie. `ď/ľ/ť` a veľké varianty používajú vlastné pravostranné pravidlá. Globálne X/Y offsety zostávajú ako manuálny override.

### Baseline-aware skladanie diakritiky
Pri skladanom TTF/OTF/SFD importe editor po novom najprv znormalizuje latinske pismena a cislice na spolocnu zapisovu liniu. Bezna velka/mala pismena a cislice koncia na riadku 6, skutocne spodne presahy (`g`, `j`, `p`, `q`, `y`) mozu pouzit riadok 7. Diakritika sa potom pridava bez nahodneho posuvania zakladneho pismena hore/dole. Ak je pre akcent hore malo miesta, vertikalne sa prisposobi iba telo glyphu a jeho baseline ostane zachovana. Specialne `d/l/t` varianty s pravostrannym makcenom zostavaju samostatnou triedou. Cielom je, aby `A/Á`, `a/á/ä`, `E/É`, `u/ú` atd. sedeli na rovnakej spodnej linii a aby font pri texte vizualne "neskakal".

### Celá znaková sada / lupa 1x-8x
V paneli `REAL BITMAP PREVIEW` je tlačidlo `Full character set...`. Otvorí samostatnú 16x16 tabuľku všetkých slotov `0x00-0xFF`, podobnú klasickému DOS character-map pohľadu. Zoom sa dá meniť plynulo po celých krokoch od `1x` do `8x`; raster zostáva nearest-neighbour bez antialiasingu. Okno vie prepínať `EDITED`/`ORIGINAL`, pri väčšom zoome sa posúva scrollbarmi a kliknutie na glyph vyberie rovnaký slot aj v hlavnom editore.

### Plný 8×8 import vektorového fontu
Import TTF/OTF/SFD teraz zachováva **všetkých 8 stĺpcov každého glyphu**. Predvolené nastavenie je 8.0 px, X offset +1, Y offset 0 a zapnutá skladaná/baseline-aware CP852 diakritika. Posuny X/Y sú určené len na jemné doladenie v malej 8×8 bunke.

Dôležité: podľa doteraz zrekonštruovaného DOS renderera Elviry sú v hre aktívne iba ľavé 6 stĺpcov. Stĺpce 7–8 sa preto v editore zobrazujú sivou. Importované pixely v nich sa **nezahodia** — zostávajú uložené v 256×8 tabuľke a pri exporte/uložení sa zachovajú — ale v pôvodnom renderer-i nemusia byť viditeľné v hre.

### Full Character Set
Okno `Full character set...` má predvolený široký layout **32×8**. Dá sa prepínať medzi `16×16`, `32×8` a `64×4` a zoomovať od 1× do 8×. Viewer zobrazuje všetkých 8 uložených stĺpcov; posledné dva sú sivé ako renderer-inactive.

---

## v1.2 – diagnostika textov a dokončenie pracovného UI

Verzia 1.2 stabilizuje Elvira I workflow a pripravuje architektúru na samostatný profil Elvira II. Podpora Elvira II textových poolov `TEXT01–TEXT09` zatiaľ **nie je implementovaná**; profil E2 zámerne nepoužíva 96-bajtové pravidlo z Elviry I.

### Elvira I – DOS text diagnostics

Text Editor v automatickom režime rozlišuje:

- **červené / potvrdené riziko** – runtime potvrdené stringy 417 a 424; pri prekročení 96 B sa simuluje word-aware orezanie,
- **oranžové / možné riziko** – ostatné E1 stringy nad 96 B, ktorých konkrétna runtime cesta nie je potvrdená,
- **zelené / potvrdene bezpečné** – referenčné stringy 298, 425 a 627, ktoré používajú inú textovú cestu,
- **sivé / ignorované** – warning manuálne potlačený používateľom.

K dispozícii je filter **Zobraziť iba rizikové texty** a checkbox **Ignorovať upozornenie pre tento string**. Ignore stav sa neukladá do `GAMEPC`; zapisuje sa do sidecar súboru `.pi1-text-diagnostics.json` v adresári hry.

Editor počíta dĺžku v skutočných bajtoch zvoleného encodingu a pred uložením kontroluje:

- vložený NUL znak,
- znaky, ktoré nie je možné bezstratovo zakódovať,
- zachovanie počtu stringov po zápise.

### Font Editor

- nedeštruktívny posun glyphu šípkami a klávesovými šípkami,
- pixely mimo 8×8 sa nestrácajú počas presúvania; orežú sa až pri Apply / Save copy / Export po potvrdení,
- Full Character Set: 32×8 default, 16×16 a 64×4 alternatíva, zoom 1×–8×, Original/Edited, synchronizovaný výber,
- stavový riadok Full Character Set zobrazuje byte ID, HEX glyphu a offset v 256×8 tabuľke,
- vektorový import má presety **Default / Lexis / Pixel Operator / HP-style**; parametre po zvolení zostávajú ručne nastaviteľné.

### Vektorové fonty – praktické obmedzenia

TTF/OTF import je vhodný najmä ako základ. Pri 8×8/6×8 rastri môže byť natívna vektorová diakritika nevhodne zmenšená alebo posunutá. Režim **Compose + optically align CP852 diacritics** preto skladá slovenské znaky zo základného písmena a vlastnej diakritiky; `ď/ľ/ť` a veľké varianty používajú osobitné pravidlá.

Referenčné/testované zdroje fontov:

- Lexis (CC0): https://github.com/damianvila/font-lexis
- Pixel Operator: https://www.dafont.com/pixel-operator.font
- Oldschool PC Font Resource / PxPlus: https://int10h.org/oldschool-pc-fonts/download/
- Perfect DOS VGA 437: https://www.dafont.com/perfect-dos-vga-437.font

Pri distribúcii fontu spolu s projektom vždy skontrolujte licenciu konkrétneho fontu. Odkaz v README neznamená, že je font súčasťou projektu alebo že sa jeho licencia zhoduje s licenciou editora.

### Elvira II – pripravený samostatný profil

Z doterajšej analýzy vieme, že Elvira II má podobný `GAMEPC` textový pool, ale odlišný TABLES bytecode a ďalší textový pool `TEXT01–TEXT09`. Verzia 1.2 drží E2 diagnostiku oddelene: byte counter funguje, ale E1 96-byte warning sa na E2 automaticky neaplikuje. Implementácia `TEXT01–TEXT09` a E2 TABLES metadata je plánovaná ako samostatný krok.

### Herné dáta

Projekt neobsahuje originálne herné EXE/VGA/GAMEPC/TABLES ani iné komerčné assety. Používateľ pracuje so svojou vlastnou inštaláciou hry. Elvira a súvisiace herné názvy, grafika a ochranné známky patria príslušným držiteľom práv; screenshoty a názvy slúžia iba na dokumentáciu kompatibility.

### Nemenný GameRoot a varianty (v1.0)

Originálne herné súbory v GameRoot (`RUNVGA.EXE`, `RUNEGA.EXE`, `RUNIT.EXE`, `GAMEPC`, VGA zdroje) sú nemenné: žiadna bežná akcia editora ich nenahrádza. Úpravy písma sa ukladajú cez Uložiť do projektu a materializujú sa zostavením variantu do vlastnených výstupov `VARIANTS\<runtime>\<edition>\`. Priame zapisovanie do GameRoot bolo odstránené; vo v1.0 neexistuje O-file architektúra ani migrácia z predbežných zostavení. Výnimkou je generovanie/obnova launcher BAT s existujúcou zálohou.
