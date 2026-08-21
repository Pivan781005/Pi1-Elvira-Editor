# Elvira VGA Editor — GUI + automatický deploy

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

1. Otvor `ElviraVgaEditor.sln` vo Visual Studio 2022.
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
