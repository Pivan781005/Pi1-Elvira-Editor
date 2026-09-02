# Pomocník π1 Elvira I & II Editor

## [getting-started] Začíname
V selektore **Inštalácia** vyberte podporovanú VGA hru. Editor si platné inštalácie pamätá v používateľských nastaveniach aplikácie, nikdy nie v priečinku hry. **Nájsť hry...** vykoná rýchlu ohraničenú kontrolu bežných GOG umiestnení a zapamätaných ciest; pri štarte automaticky neprehľadáva celé disky. Pomocou **Vybrať adresár...** môžete priečinok hry vždy manuálne overiť a pridať. Viaceré inštalácie Elvira I/II ostávajú samostatnými položkami a **Hra: Automatická detekcia** ihneď zobrazí overenú vybranú hru. Jazyk zvoľte v **Jazyk rozhrania**. Používajte Grafiku, Text, Font a Mody a spúšťač; Pomoc je iba na čítanie a O programe je stručné zhrnutie.

Podporované hry: **Elvira: Mistress of the Dark** a **Elvira II: The Jaws of Cerberus**.

## [graphics] Editor grafiky
Vyberte VGA súbor, paletu, položku sprite a pixelové zväčšenie. Zoznam označuje RAW/RLE položky tam, kde to platí, a náhľad ukazuje vybraný obraz. Nahradiť PNG alebo Import PNG vytvorí editáciu, Export PNG uloží kópiu, Zrušiť edit ju zahodí, Aplikovať do hry ju uloží a Obnoviť originál použije dostupnú zálohu. K dispozícii sú iba aktuálne podporované VGA postupy.

## [text] Editor textu
Otvorte kompatibilný dátový súbor typu GAMEPC, načítajte ho znova, hľadajte, zvoľte kódovanie/kontext a potom Uložte, Uložte ako alebo Vytvorte variant. Kompatibilné názvy sú GAMEPC, GAMEPCSK, GAMEPCCZ, GAMEPCDE a MYMOD: rozhoduje podporovaný formát a kontext, nie doslovný názov. GAMEPC pri zmene dĺžky prekladu automaticky znovu vytvorí celý textový pool a zachová logické indexy aj netextové dáta súboru. Pôvodné B, Preklad B a Δ B sú informatívne počty kódovaných bajtov; pevný runtime limit dĺžky textu Elviry I ani Elviry II sa momentálne nezobrazuje.

## [game-data] Herné dátové súbory
Aktuálny dátový súbor je súbor práve otvorený v Editore textu. Variant má zobrazovaný názov, dátový súbor, povolenie, poradie a dostupnosť. Katalóg ukladá **ELVIRA_MODS.INI**. Samotné otvorenie Modov a spúšťača ho nikdy nevytvorí ani neprepíše. Chýbajúci súbor zostane v katalógu ako Chýba/nedostupný a nikdy sa potichu neodstráni.

## [variants] Varianty a mody
Pridať, Upraviť, Odstrániť, Posunúť vyššie, Posunúť nižšie, Povoliť/Zakázať a Otvoriť dátový súbor v Text Editore spravujú metadata katalógu. Odstránenie variantu odstráni iba metadata, nie fyzický dátový súbor. Spúšťač zobrazuje povolené dostupné varianty v poradí katalógu.

## [mods-launcher] Mody a spúšťač
Nastavte súbor spúšťača, meno moddera a predvolený variant. Tabuľka zobrazuje #, Názov, Dátový súbor, Stav a Povolený. Náhľad BAT nič nezapisuje; Vytvoriť / aktualizovať spúšťač zapíše BAT; Obnoviť pôvodný spúšťač skopíruje nemenný .BAK rovnakého mena späť do aktívneho BAT.

Runtime menu: ENTER spustí DefaultVariant, 1..9/A..Z vyberá variant, 0 je Sound Setup, ESC návrat do DOSu a neplatné/nepriradené klávesy sa ignorujú. Podporuje 35 explicitných variantov a položky BAT ukladá vodorovne, kde sa zmestia.

## [sound] Nastavenie zvuku
Generované spúšťače sú špecifické pre hru.

### Elvira I
`RUNVGA <DataFile> <sound option>`: /p PC Internal Speaker; /t IBM/Tandy 3 voice; /a AdLib sound card; /c CMS Gameblaster; /s Soundblaster music card; /m0 Generic Midi; /m1 Roland MT-32 / LAPC-1; /m2 Casio CT460.

### Elvira II
`RUNIT <DataFile> <sound option>`: /p PC Internal speaker; /t IBM/Tandy 3 voice; /a AdLib sound card; /s Soundblaster music card; /m Roland MT-32 / LAPC-1.

Sound Setup vytvára/aktualizuje `SET PI1SND=/s` v PI1SND.BAT, nie v ELVIRA_MODS.INI. Chýbajúci/neplatný PI1SND.BAT vynúti nastavenie: Back/ESC/ENTER sa ignorujú do platnej voľby. Bežné nastavenie umožňuje 0 = Back.

## [font] Editor fontu
Prehliadajte originálny font, pracujte v editovateľnej kópii a používajte **Import font...**, Export font..., Apply Font, Save Patched Copy, filtre, editor glyphov, Original/Edited, posuny a náhľady 1x/2x/4x/8x. „Import font...” zámerne podporuje viac formátov než TTF.

Všetky obnovené renderery používajú 8 riadkov a zobrazujú 6 stĺpcov z bitov 7..2; dve pravé bitmapové kolóny sú neaktívne. Originálny glyph 0x81 je `00 FC FC FC FC FC FC 00`, chránený HUD erase/blanking glyph. Patched font používa `FC FC FC FC FC FC FC FC` ako úmyselný plný 6x8 HUD erase. Úprava, kopírovanie/reset, import aj každý výstup V5/V2 túto hodnotu vynútia; otvorenie staršieho chybného patched EXE zostáva iba na čítanie, kým výslovne nevytvoríte výstup.

### Elvira I / Elvira II
Originál RUNVGA: 0x1A216..0x1A525, 784 bajtov, 98 glyphov, 0x20..0x81. V5: 0x3AE60..0x3B65F, 2048 bajtov, 256x8 CP852-compatible. Originál RUNIT: 0x168CA..0x16BD9, rovnaký rozsah. RUNIT V2 je split: LOW 0x20..0x81 originál; HIGH 0x82..0xFF na 0x28480..0x2886F (126x8=1008), helper 0x28870. Ide o overené podporované layouty, nie univerzálne offsety.

## [backups] Zálohy a obnovenie
Otvorenie/náhľad nevytvorí zálohu. Prvá trvalá úprava fontu/dát/grafiky chráni nemenné O-súbory, napr. RUNVGAO.EXE/RUNITO.EXE, GAMEPCO alebo 012O.VGA. Generovanie launcheru používa `ELVIRA.BAT` -> `ELVIRA.BAK`: BAK je nemenný, nikdy sa neprepíše/nevymaže a Obnoviť ho skopíruje späť do aktívneho BAT.

## [technical] Technické podrobnosti
Elvira I V5 poskytuje tabuľku 256x8; Elvira II RUNIT V2 používa split low/high font. PI1MENU.COM je DOS helper projektu pre BIOS vstup, filtrovanie platných kláves, ENTER/Setup/Variant/ESC a pevné 17x8 Pi1 logo. Nespúšťa hry, negeneruje BAT, nespravuje varianty ani neukladá zvuk: BAT je logika, COM je UI/vstup. Logo používa CP437 0xDB, Pi atribút 07 a číslica 1 atribút 0F. Cieľ: classic DOSBox, DOSBox-X, DOSBox Staging a kompatibilné DOS prostredia.

## [troubleshooting] Riešenie problémov
Ak hra nie je rozpoznaná, vyberte správnu VGA inštaláciu; nepodporované EXE nemožno patchovať. Skontrolujte chýbajúci dátový súbor/variant a dostupný predvolený variant. Náhľad nič nezapisuje. Pri chýbajúcom/neplatnom PI1SND dokončite vynútený Sound Setup. Import/Apply Font je dostupný až po načítaní podporovaného zdroja/pracovnej kópie; 0x81 sa zámerne nedá upraviť. Neopisujte BAT zálohu bez kontroly ručne upraveného launcheru.

## [supported] Podporované / nepodporované
### Podporované vo v1.0
- **Elvira: Mistress of the Dark — VGA**
- **Elvira II: The Jaws of Cerberus — VGA**

### Aktuálne nepodporované
`RUNEGA` a EGA patchovanie EXE/fontov sú mimo podporovaného rozsahu. **Editor RUNEGA nikdy nemení a V5/V2 patche sa naň nevzťahujú.**

## [about] O programe / Credits
**π1 Elvira I & II Editor v1.0** podporuje VGA grafiku, textové/dátové varianty kompatibilné s GAMEPC, rozšírené fonty kompatibilné s CP852, Mody a spúšťač a prenosný DOS helper. Glyph `0x81` je chránený glyph na mazanie HUD; RUNEGA/EGA nie je podporovaný a nikdy sa nemení.

Projekt: `https://github.com/Pivan781005/Pi1-Elvira-I-II-Editor`

Remember these crews? :) Phrozen Crew, UCF, CORE, PARADOX, Razor 1911, Fairlight, TRSi, INC, Hybrid, Prestige, CLASS, MYTH, Drink Or Die, DREAD, ORiON, ECLiPSE. Malá pocta scéne, ktorá tvorila DOS históriu; bez afiliácie alebo podpory.
