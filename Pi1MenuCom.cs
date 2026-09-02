namespace ElviraVgaEditor;

/// <summary>Deterministic project-owned 8086 .COM keyboard helper. No timer, DOS clock, or timeout code exists.</summary>
internal static class Pi1MenuCom
{
    internal const string FileName = "PI1MENU.COM";
    internal const int LogoRow = 4; // BIOS zero-based: visible row 5
    internal const int LogoColumn = 43; // BIOS zero-based: visible column 44

    internal static byte[] Build()
    {
        var b = new List<byte>(); var labels = new Dictionary<string, int>(); var jumps = new List<(int, string)>();
        void A(params byte[] x) => b.AddRange(x);
        void L(string name) => labels[name] = b.Count;
        void J(byte op, string target) { A(op, 0); jumps.Add((b.Count - 1, target)); }
        // Any command-tail argument is the explicit /logo render mode; no argument is keyboard mode.
        A(0xA0,0x80,0x00,0x3C,0x00); J(0x75, "logo");
        // BIOS AH=00h immediately returns any already-buffered key. GOG's preceding
        // CHOICE can leave a make/repeat event there, so drain every pending BIOS key
        // first, then perform the one blocking read below. This has no timer or delay:
        // a fresh key is accepted as soon as the buffer is observed empty.
        L("drain_keyboard"); A(0xB4,0x01,0xCD,0x16); J(0x74, "read_keyboard"); A(0xB4,0x00,0xCD,0x16); J(0xEB, "drain_keyboard");
        L("read_keyboard");
        // Keyboard result: ENTER=1, 0=2, 1..9=3..11, A..Z/a..z=12..37, otherwise 0.
        A(0xB4,0x00,0xCD,0x16,0x3C,0x1B); J(0x75,"not_escape"); A(0xB0,0x26); J(0xEB,"exit");
        L("not_escape"); A(0x3C,0x0D); J(0x75,"not_enter"); A(0xB0,0x01); J(0xEB,"exit");
        L("not_enter"); A(0x3C,0x30); J(0x75,"not_zero"); A(0xB0,0x02); J(0xEB,"exit");
        L("not_zero"); A(0x3C,0x31); J(0x72,"invalid"); A(0x3C,0x39); J(0x77,"letter"); A(0x2C,0x31,0x04,0x03); J(0xEB,"exit");
        L("letter"); A(0x24,0xDF,0x3C,0x41); J(0x72,"invalid"); A(0x3C,0x5A); J(0x77,"invalid"); A(0x2C,0x41,0x04,0x0C); J(0xEB,"exit");
        L("invalid"); A(0x30,0xC0); J(0xEB,"exit");
        L("logo");
        // BIOS cursor save, then one BIOS AH=09h cell write for every exact logical cell.
        A(0xB4,0x03,0xB7,0x00,0xCD,0x10,0x52,0xBE,0,0); int matrixAddress = b.Count - 2;
        A(0xBD,0x08,0x00,0xB6,(byte)LogoRow); L("row"); A(0xB2,(byte)LogoColumn,0xB9,0x11,0x00); L("cell");
        A(0xAC,0x3C,0x50); J(0x75,"not_one"); A(0xB3,0x07,0xB0,0xDB); J(0xEB,"draw");
        L("not_one"); A(0x3C,0x31); J(0x75,"blank"); A(0xB3,0x0F,0xB0,0xDB); J(0xEB,"draw");
        L("blank"); A(0xB3,0x00,0xB0,0x20); L("draw");
        A(0x51,0xB4,0x02,0x30,0xFF,0xCD,0x10,0xB4,0x09,0xB9,0x01,0x00,0xCD,0x10,0x59,0xFE,0xC2); J(0xE2,"cell");
        A(0xFE,0xC6,0x4D); J(0x75,"row"); A(0x5A,0xB4,0x02,0xB7,0x00,0xCD,0x10,0xB8,0x00,0x4C,0xCD,0x21);
        L("exit"); A(0xB4,0x4C,0xCD,0x21);
        int data = b.Count; foreach (string row in Pi1Logo.Matrix) A(System.Text.Encoding.ASCII.GetBytes(row));
        b[matrixAddress] = (byte)(0x100 + data); b[matrixAddress + 1] = (byte)((0x100 + data) >> 8);
        foreach ((int at, string target) in jumps) b[at] = unchecked((byte)(sbyte)(labels[target] - at - 1));
        return b.ToArray();
    }

    // Test-only contract model for the BIOS boundary above. Pending entries are
    // discarded exactly as AH=01h/AH=00h does; only the next post-drain key is mapped.
    internal static int SimulateFreshKeyAfterDrain(IEnumerable<byte> pendingAscii, byte freshAscii)
    {
        _ = pendingAscii.Count();
        return ErrorLevelForAscii(freshAscii);
    }

    internal static bool HasKeyboardDrain(byte[] image)
    {
        ReadOnlySpan<byte> signature = [0xB4,0x01,0xCD,0x16,0x74,0x00,0xB4,0x00,0xCD,0x16,0xEB,0x00,0xB4,0x00,0xCD,0x16];
        for (int i = 0; i <= image.Length - signature.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < signature.Length; j++)
            {
                // The two signed relative displacements are resolved at build time.
                if ((j == 5 || j == 11) ? false : image[i + j] != signature[j]) { match = false; break; }
            }
            if (match) return true;
        }
        return false;
    }

    private static int ErrorLevelForAscii(byte ascii)
    {
        if (ascii == 0x1B) return 0x26;
        if (ascii == 0x0D) return 1;
        if (ascii == (byte)'0') return 2;
        if (ascii is >= (byte)'1' and <= (byte)'9') return ascii - (byte)'1' + 3;
        byte upper = (byte)(ascii & 0xDF);
        return upper is >= (byte)'A' and <= (byte)'Z' ? upper - (byte)'A' + 12 : 0;
    }
}

/// <summary>Author-provided immutable Pi1 design, retained cell-for-cell for regression checks.</summary>
internal static class Pi1Logo
{
    internal static readonly string[] Matrix = [
        ".PPPPPPPPPPP.....", "PP..PP..PP.......", "....PP..PP.......", "....PP..PP....111",
        ".PP.PP..PP...1111", "..PPP...PP..11.11", "...............11", "...............11"];
    internal const byte FullBlock = 0xDB;
    internal static (byte Glyph, byte Attribute) Cell(char c) => c switch
    { 'P' => (FullBlock, 0x07), '1' => (FullBlock, 0x0F), '.' => (0x20, 0x00), _ => throw new InvalidDataException("Invalid Pi1 logo cell.") };
}

internal static class Pi1SetupLayout
{
    internal static bool Fits(ElviraGameProfile game, bool forced)
    {
        var occupied = new bool[25, 80];
        void Text(int row, string value)
        {
            if (row is < 0 or >= 25 || value.Length > 80) throw new InvalidDataException("Sound setup exceeds 80x25.");
            for (int col = 0; col < value.Length; col++) { if (occupied[row, col]) throw new InvalidDataException("Sound setup overlaps Pi1 logo."); occupied[row, col] = true; }
        }
        string title = game == ElviraGameProfile.Elvira1 ? "Elvira: Mistress of the Dark" : "Elvira II: The Jaws of Cerberus";
        Text(0, title); Text(2, "Sound Setup");
        foreach ((LauncherSoundOption option, int i) in LauncherService.SoundOptions(game).Select((s, i) => (s, i + 1))) Text(4 + i - 1, $"{i}. {option.DisplayName}");
        if (!forced) Text(12, "0. Back"); Text(14, "Current: Soundblaster music card"); Text(15, "Select:");
        Text(17, "Remember these crews? :)");
        Text(19, "Phrozen Crew, UCF, CORE, PARADOX, Razor 1911, Fairlight, TRSi, INC,");
        Text(20, "Hybrid, Prestige, CLASS, MYTH, Drink Or Die, DREAD, ORiON, ECLiPSE");
        Text(22, "A little tribute to the scene that made DOS history."); Text(23, "No affiliation or endorsement implied.");
        for (int r = 0; r < 8; r++) for (int c = 0; c < 17; c++)
        {
            int row = Pi1MenuCom.LogoRow + r, col = Pi1MenuCom.LogoColumn + c;
            if (row >= 25 || col >= 80 || occupied[row, col]) throw new InvalidDataException("Pi1 logo clips or overlaps.");
            occupied[row, col] = true;
        }
        return true;
    }
}
