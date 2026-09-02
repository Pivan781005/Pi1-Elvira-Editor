# Nápověda π1 Elvira I & II Editor

## [getting-started] Začínáme
V selektoru **Instalace** vyberte podporovanou VGA hru. Editor si platné instalace pamatuje v uživatelském nastavení aplikace, nikdy ve složce hry. **Najít hry...** provede rychlou omezenou kontrolu běžných umístění GOG a zapamatovaných cest; při spuštění automaticky neprohledává celé disky. Pomocí **Vybrat adresář...** můžete složku hry vždy ručně ověřit a přidat. Více instalací Elvira I/II zůstává samostatnými položkami a **Hra: Automatická detekce** ihned zobrazí ověřenou vybranou hru. Jazyk vyberte v **Jazyk rozhraní**. Používejte Grafiku, Text, Font a Mody a spouštěč; Nápověda je jen pro čtení a O programu je stručné shrnutí.

Podporované hry: **Elvira: Mistress of the Dark** a **Elvira II: The Jaws of Cerberus**.

## [graphics] Editor grafiky
Vyberte VGA soubor, paletu, položku sprite a pixelové zvětšení. Seznam označuje RAW/RLE položky, kde to platí, a náhled ukazuje vybraný obraz. Nahradit PNG nebo Import PNG vytvoří úpravu, Export PNG uloží kopii, Zrušit edit ji zahodí, Aplikovat do hry ji uloží a Obnovit originál použije dostupnou zálohu. Nabízeny jsou pouze aktuálně podporované VGA postupy.

## [text] Editor textu
Otevřete kompatibilní datový soubor typu GAMEPC, načtěte jej znovu, hledejte, zvolte kódování/kontext a potom Uložte, Uložte jako nebo Vytvořte variantu. Kompatibilní názvy jsou GAMEPC, GAMEPCSK, GAMEPCCZ, GAMEPCDE a MYMOD: rozhoduje podporovaný formát a kontext, nikoli doslovný název. GAMEPC při změně délky překladu automaticky znovu sestaví celý textový pool a zachová logické indexy i netextová data souboru. Původní B, Překlad B a Δ B jsou informativní počty kódovaných bajtů; pevný runtime limit délky textu Elviry I ani Elviry II se nyní nezobrazuje.

## [game-data] Herní datové soubory
Aktuální datový soubor je soubor právě otevřený v Editoru textu. Varianta má zobrazovaný název, datový soubor, povolení, pořadí a dostupnost. Katalog ukládá **ELVIRA_MODS.INI**. Samotné otevření Modů a spouštěče jej nikdy nevytvoří ani nepřepíše. Chybějící soubor zůstane v katalogu jako Chybí/nedostupný a nikdy se potichu nesmaže.

## [variants] Varianty a mody
Přidat, Upravit, Odebrat, Posunout výše, Posunout níže, Povolit/Zakázat a Otevřít datový soubor v Text Editoru spravují metadata katalogu. Odebrání varianty odstraní pouze metadata, ne fyzický datový soubor. Spouštěč zobrazí povolené dostupné varianty v pořadí katalogu.

## [mods-launcher] Mody a spouštěč
Nastavte soubor spouštěče, jméno moddera a výchozí variantu. Tabulka zobrazuje #, Název, Datový soubor, Stav a Povolená. Náhled BAT nic nezapisuje; Vytvořit / aktualizovat spouštěč zapíše BAT; Obnovit původní spouštěč zkopíruje neměnný .BAK stejného jména zpět do aktivního BAT.

Runtime menu: ENTER spustí DefaultVariant, 1..9/A..Z vybírá variantu, 0 je Sound Setup, ESC návrat do DOSu a neplatné/nepřiřazené klávesy se ignorují. Podporuje 35 explicitních variant a položky BAT skládá vodorovně, kde se vejdou.

## [sound] Nastavení zvuku
Generované spouštěče jsou specifické pro hru.

### Elvira I
`RUNVGA <DataFile> <sound option>`: /p PC Internal Speaker; /t IBM/Tandy 3 voice; /a AdLib sound card; /c CMS Gameblaster; /s Soundblaster music card; /m0 Generic Midi; /m1 Roland MT-32 / LAPC-1; /m2 Casio CT460.

### Elvira II
`RUNIT <DataFile> <sound option>`: /p PC Internal speaker; /t IBM/Tandy 3 voice; /a AdLib sound card; /s Soundblaster music card; /m Roland MT-32 / LAPC-1.

Sound Setup vytváří/aktualizuje `SET PI1SND=/s` v PI1SND.BAT, ne v ELVIRA_MODS.INI. Chybějící/neplatný PI1SND.BAT vynutí nastavení: Back/ESC/ENTER se ignorují do platné volby. Běžné nastavení umožňuje 0 = Back.

## [font] Editor fontu
Prohlížejte originální font, pracujte v editovatelné kopii a používejte **Import font...**, Export font..., Apply Font, Save Patched Copy, filtry, editor glyphů, Original/Edited, posuny a náhledy 1x/2x/4x/8x. „Import font...” záměrně podporuje více formátů než TTF.

Všechny obnovené renderery používají 8 řádků a zobrazují 6 sloupců z bitů 7..2; dva pravé bitmapové sloupce jsou neaktivní. Originální glyph 0x81 je `00 FC FC FC FC FC FC 00`, chráněný HUD erase/blanking glyph. Patched font používá `FC FC FC FC FC FC FC FC` jako záměrný plný 6x8 HUD erase. Úpravy, kopírování/reset, import i každý výstup V5/V2 tuto hodnotu vynutí; otevření staršího chybného patched EXE zůstává jen pro čtení, dokud výslovně nevytvoříte výstup.

### Elvira I / Elvira II
Originál RUNVGA: 0x1A216..0x1A525, 784 bajtů, 98 glyphů, 0x20..0x81. V5: 0x3AE60..0x3B65F, 2048 bajtů, 256x8 CP852-compatible. Originál RUNIT: 0x168CA..0x16BD9, stejný rozsah. RUNIT V2 je split: LOW 0x20..0x81 originál; HIGH 0x82..0xFF na 0x28480..0x2886F (126x8=1008), helper 0x28870. Jde o ověřené podporované layouty, ne univerzální offsety.

## [backups] Zálohy a obnovení
Otevření/náhled nevytvoří zálohu. První trvalá úprava fontu/dat/grafiky chrání neměnné O-soubory, například RUNVGAO.EXE/RUNITO.EXE, GAMEPCO nebo 012O.VGA. Generování launcheru používá `ELVIRA.BAT` -> `ELVIRA.BAK`: BAK je neměnný, nikdy se nepřepíše/nesmaže a Obnovit jej zkopíruje zpět do aktivního BAT.

## [technical] Technické podrobnosti
Elvira I V5 poskytuje tabulku 256x8; Elvira II RUNIT V2 používá split low/high font. PI1MENU.COM je DOS helper projektu pro BIOS vstup, filtrování platných kláves, ENTER/Setup/Variant/ESC a pevné 17x8 Pi1 logo. Nespouští hry, negeneruje BAT, nespravuje varianty ani neukládá zvuk: BAT je logika, COM je UI/vstup. Logo používá CP437 0xDB, Pi atribut 07 a číslice 1 atribut 0F. Cíl: classic DOSBox, DOSBox-X, DOSBox Staging a kompatibilní DOS prostředí.

## [troubleshooting] Řešení potíží
Pokud hra není rozpoznána, vyberte správnou VGA instalaci; nepodporované EXE nelze patchovat. Zkontrolujte chybějící datový soubor/variantu a dostupnou výchozí variantu. Náhled nic nezapisuje. Při chybějícím/neplatném PI1SND dokončete vynucený Sound Setup. Import/Apply Font je dostupný až po načtení podporovaného zdroje/pracovní kopie; 0x81 se záměrně nedá upravit. Nepřepisujte BAT zálohu bez kontroly ručně upraveného launcheru.

## [supported] Podporované / nepodporované
### Podporované ve v1.0
- **Elvira: Mistress of the Dark — VGA**
- **Elvira II: The Jaws of Cerberus — VGA**

### Aktuálně nepodporované
`RUNEGA` a EGA patchování EXE/fontů jsou mimo podporovaný rozsah. **Editor RUNEGA nikdy nemění a patche V5/V2 se na něj nevztahují.**

## [about] O programu / Credits
**π1 Elvira I & II Editor v1.0** podporuje VGA grafiku, textové/datové varianty kompatibilní s GAMEPC, rozšířené fonty kompatibilní s CP852, Mody a spouštěč a přenosný DOS helper. Glyph `0x81` je chráněný glyph pro mazání HUD; RUNEGA/EGA není podporován a nikdy se nemění.

Projekt: `https://github.com/Pivan781005/Pi1-Elvira-I-II-Editor`

Remember these crews? :) Phrozen Crew, UCF, CORE, PARADOX, Razor 1911, Fairlight, TRSi, INC, Hybrid, Prestige, CLASS, MYTH, Drink Or Die, DREAD, ORiON, ECLiPSE. Malá pocta scéně, která tvořila DOS historii; bez afiliace nebo podpory.
