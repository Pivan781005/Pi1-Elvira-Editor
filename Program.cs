namespace ElviraVgaEditor;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        if (args.Length == 2 && args[0].Equals("--v5-regression", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                byte[] packed = File.ReadAllBytes(args[1]);
                byte[] baseline = RunVgaBootstrapService.UnpackVerifiedOriginal(packed);
                string fixturePath = Path.Combine(Path.GetDirectoryName(args[1]) ?? ".", "_font_analysis", "test", "RUNVGA_EXTFONT_TEST_V5.EXE");
                if (!File.Exists(fixturePath))
                    fixturePath = Path.Combine(Path.GetDirectoryName(args[1]) ?? ".", "RUNVGAV5.EXE");
                byte[] fixture = File.ReadAllBytes(fixturePath);
                byte[] table = fixture.AsSpan(RunVgaBootstrapService.V5FontOffset, RunVgaBootstrapService.V5FontSize).ToArray();
                bool pass = RunVgaBootstrapService.VerifyHistoricalV5Regression(baseline, table);
                Console.WriteLine(pass ? "V5 regression: PASS" : "V5 regression: FAIL");
                Environment.ExitCode = pass ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("V5 regression: FAIL - " + ex.Message);
                Environment.ExitCode = 1;
            }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--v5-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunVgaBootstrapResult result = RunVgaBootstrapService.CreateExtendedCp852(args[1], args[2], GlyphRepository.CreateAllCp852Slots());
                FontLoadResult loaded = RunVgaFontService.LoadRunVga(result.OutputPath);
                bool pass = loaded.Layout == RunVgaFontLayout.ExtendedCp852V5 && loaded.LoadedGlyphCount == 256;
                Console.WriteLine(pass ? "V5 smoke: PASS" : "V5 smoke: FAIL");
                Environment.ExitCode = pass ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("V5 smoke: FAIL - " + ex.Message);
                Environment.ExitCode = 1;
            }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--v5-ttf-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                // args: pristine packed Elvira I RUNVGA, TTF/OTF source, fresh V5 output path.
                RunVgaBootstrapService.CreateExtendedCp852(args[1], args[3], GlyphRepository.CreateAllCp852Slots());
                FontLoadResult loaded = RunVgaFontService.LoadRunVga(args[3]);
                if (loaded.Game != ElviraGame.Elvira1 || loaded.Layout != RunVgaFontLayout.ExtendedCp852V5 ||
                    loaded.PhysicalSlotCount != 224 || loaded.EditableGlyphCount != 223 ||
                    !FontSlotMetadata.IsReserved(loaded, FontSlotMetadata.HudEraseGlyph))
                    throw new InvalidDataException("Generated Elvira I V5 did not expose the expected 224-slot / 223-editable reservation model.");

                byte[] beforeA = loaded.Glyphs[0x41].Original.ToArray();
                byte[] beforea = loaded.Glyphs[0x61].Original.ToArray();
                byte[] raster = VectorFontRasterizer.RasterizeCp852(args[2], new VectorFontRasterOptions("", 11, 0, 0, true));
                RunVgaFontService.ImportFontBytesIntoEdited(loaded, raster, Path.GetFileName(args[2]));
                byte[] canonical = FontSlotMetadata.GetCanonicalBytes(loaded, FontSlotMetadata.HudEraseGlyph);
                if (!loaded.Glyphs[0x81].Edited.SequenceEqual(canonical))
                    throw new InvalidDataException("Elvira I TTF import overwrote reserved glyph 0x81 in memory.");
                // Validate the final write boundary too: legacy/external callers can
                // still supply a corrupt in-memory bitmap, but it must never reach disk.
                loaded.Glyphs[0x81].ReplaceEdited(Convert.FromHexString("1122334455667788"));

                string saved = args[3] + ".ttf.EXE";
                if (File.Exists(saved)) throw new IOException("TTF smoke output already exists.");
                RunVgaFontService.SaveCopy(loaded, saved);
                FontLoadResult reopened = RunVgaFontService.LoadRunVga(saved);

                int[] normalSlots = [0x41, 0x61, 0x80, 0x82, 0x8E, 0xA0, 0xE1, 0xFF];
                bool generated = normalSlots.All(code => reopened.Glyphs[code].Original.SequenceEqual(raster.AsSpan(code * RunVgaFontService.GlyphBytes, RunVgaFontService.GlyphBytes).ToArray()));
                bool asciiChanged = !beforeA.SequenceEqual(reopened.Glyphs[0x41].Original) && !beforea.SequenceEqual(reopened.Glyphs[0x61].Original);
                bool preserved = reopened.Glyphs[0x81].Original.SequenceEqual(canonical);
                if (!generated || !asciiChanged || !preserved)
                    throw new InvalidDataException("Elvira I TTF import/save/reopen reserved-slot verification failed.");

                Console.WriteLine("V5 TTF smoke: PASS");
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("V5 TTF smoke: FAIL - " + ex.Message);
                Environment.ExitCode = 1;
            }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runit-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunItBootstrapResult result = RunItBootstrapService.CreateExtendedCp852(args[1], args[2], GlyphRepository.CreateAllCp852Slots());
                FontLoadResult loaded = RunVgaFontService.LoadRunVga(result.OutputPath);
                if (loaded.Layout != RunVgaFontLayout.ExtendedCp852RunIt || loaded.LoadedGlyphCount != 224 || loaded.FirstByteValue != 0x20 || loaded.LastByteValue != 0xFF)
                    throw new InvalidDataException("Generated RUNIT did not reopen as Extended CP852.");
                var expectedSlots = new Dictionary<int, byte[]>
                {
                    [0x20] = Convert.FromHexString("0102030405060708"),
                    [0x41] = Convert.FromHexString("1121314151617181"),
                    [0x61] = Convert.FromHexString("1222324252627282"),
                    [0x80] = Convert.FromHexString("1333435363737383"),
                    [0x81] = RunItBootstrapService.ReservedHudEraseGlyphBytes,
                    [0x82] = Convert.FromHexString("2050507050508800"),
                    [0x8E] = Convert.FromHexString("2151517151518900"),
                    [0xA0] = Convert.FromHexString("2252527252528A00"),
                    [0xE1] = Convert.FromHexString("2353537353538B00"),
                    [0xFF] = Convert.FromHexString("1444546474847484")
                };
                foreach ((int code, byte[] bytes) in expectedSlots)
                    loaded.Glyphs[code].ReplaceEdited(bytes);
                // The serializer, rather than caller discipline, is the final safety gate.
                loaded.Glyphs[0x81].ReplaceEdited(Convert.FromHexString("1444546474847484"));
                string roundTrip = Path.Combine(Path.GetDirectoryName(result.OutputPath) ?? ".", Path.GetFileNameWithoutExtension(result.OutputPath) + ".roundtrip.EXE");
                if (File.Exists(roundTrip)) File.Delete(roundTrip);
                RunVgaFontService.SaveCopy(loaded, roundTrip);
                FontLoadResult reopened = RunVgaFontService.LoadRunVga(roundTrip);
                byte[] disk = File.ReadAllBytes(roundTrip);
                bool slotsMatch = expectedSlots.All(kv =>
                    reopened.Glyphs[kv.Key].Original.SequenceEqual(kv.Value) &&
                    disk.AsSpan(kv.Key <= 0x81
                        ? RunItBootstrapService.OriginalFontOffset + (kv.Key - 0x20) * RunVgaFontService.GlyphBytes
                        : RunItBootstrapService.HighFontOffset + (kv.Key - 0x82) * RunVgaFontService.GlyphBytes,
                        RunVgaFontService.GlyphBytes).SequenceEqual(kv.Value));
                bool pass = reopened.Layout == RunVgaFontLayout.ExtendedCp852RunIt && slotsMatch;
                Console.WriteLine(pass ? "RUNIT smoke: PASS" : "RUNIT smoke: FAIL");
                Environment.ExitCode = pass ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("RUNIT smoke: FAIL - " + ex.Message);
                Environment.ExitCode = 1;
            }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--runit-regression", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                byte[] packed = File.ReadAllBytes(args[1]);
                if (RunItBootstrapService.DetectState(args[1]) != RunItBootstrapState.OriginalPacked)
                    throw new InvalidDataException("Packed RUNIT identification failed.");
                byte[] canonical = RunItBootstrapService.UnpackCanonicalOriginal(packed);
                RunItBootstrapService.ValidateCanonical(canonical);
                byte[] corruptedPacked = (byte[])packed.Clone(); corruptedPacked[0x200] ^= 1;
                bool wrongHashRejected = false;
                try { RunItBootstrapService.UnpackCanonicalOriginal(corruptedPacked); } catch (InvalidDataException) { wrongHashRejected = true; }
                if (!wrongHashRejected) throw new InvalidDataException("Wrong packed hash was accepted.");
                byte[] font = new byte[RunVgaFontService.ExtendedFontBytes];
                Array.Copy(canonical, RunItBootstrapService.OriginalFontOffset, font, 0x20 * 8, 98 * 8);
                byte[] extended = RunItBootstrapService.BuildExtended(canonical, font);
                RunItBootstrapService.ValidateExtended(extended);
                byte[] knownBadExtended = (byte[])extended.Clone();
                Convert.FromHexString("B600B103D3E28BF28E0612068CD805C2138ED890").CopyTo(knownBadExtended, 0x7B89);
                if (RunItBootstrapService.IsExtended(knownBadExtended))
                    throw new InvalidDataException("Known-bad legacy renderer order was accepted as valid Extended CP852.");
                byte[] badRenderer = (byte[])canonical.Clone(); badRenderer[0x7B89] ^= 1;
                bool rendererRejected = false;
                try { RunItBootstrapService.BuildExtended(badRenderer, font); } catch (InvalidDataException) { rendererRejected = true; }
                if (!rendererRejected) throw new InvalidDataException("Renderer expected-byte guard was bypassed.");
                Console.WriteLine("RUNIT regression: PASS");
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("RUNIT regression: FAIL - " + ex.Message);
                Environment.ExitCode = 1;
            }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runit-v2-golden", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                byte[] canonical = RunItBootstrapService.UnpackCanonicalOriginal(File.ReadAllBytes(args[1]));
                byte[] golden = File.ReadAllBytes(args[2]);
                RunItBootstrapService.ValidateExtended(golden);
                byte[] image = new byte[RunVgaFontService.ExtendedFontBytes];
                Array.Copy(canonical, RunItBootstrapService.OriginalFontOffset, image, 0x20 * 8, 98 * 8);
                Array.Copy(golden, RunItBootstrapService.HighFontOffset, image, 0x82 * 8, RunItBootstrapService.HighFontSize);
                byte[] generated = RunItBootstrapService.BuildExtended(canonical, image);
                bool pass = generated.SequenceEqual(golden);
                Console.WriteLine($"RUNIT V2 golden: {(pass ? "PASS" : "FAIL")} SHA-256 {Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(generated))}");
                if (!pass)
                {
                    int first = Enumerable.Range(0, generated.Length).First(i => generated[i] != golden[i]);
                    Console.Error.WriteLine($"First differing offset: 0x{first:X}");
                }
                Environment.ExitCode = pass ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("RUNIT V2 golden: FAIL - " + ex.Message);
                Environment.ExitCode = 1;
            }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--production-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                // args: pristine Elvira I EXE, pristine Elvira II EXE, empty isolated root.
                RunProductionSmoke(ElviraGame.Elvira1, args[1], args[3]);
                RunProductionSmoke(ElviraGame.Elvira2, args[2], args[3]);
                Console.WriteLine("Production backup workflow: PASS");
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Production backup workflow: FAIL - " + ex.Message);
                Environment.ExitCode = 1;
            }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--partial-backup-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunPartialBackupSmoke(args[1], args[2], args[3]);
                Console.WriteLine("Partial backup guards: PASS");
                Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("Partial backup guards: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--runit-ttf-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunItBootstrapService.CreateExtendedCp852(args[1], args[3], GlyphRepository.CreateAllCp852Slots());
                FontLoadResult loaded = RunVgaFontService.LoadRunVga(args[3]);
                byte[] beforeA = loaded.Glyphs[0x41].Original.ToArray(), beforea = loaded.Glyphs[0x61].Original.ToArray(), before0 = loaded.Glyphs[0x30].Original.ToArray();
                byte[] raster = VectorFontRasterizer.RasterizeCp852(args[2], new VectorFontRasterOptions("", 11, 0, 0, true));
                RunVgaFontService.ImportFontBytesIntoEdited(loaded, raster, Path.GetFileName(args[2]));
                string saved = args[3] + ".ttf.EXE";
                if (File.Exists(saved)) throw new IOException("TTF smoke output already exists.");
                RunVgaFontService.SaveCopy(loaded, saved);
                FontLoadResult reopened = RunVgaFontService.LoadRunVga(saved);
                bool changed = !beforeA.SequenceEqual(reopened.Glyphs[0x41].Original) && !beforea.SequenceEqual(reopened.Glyphs[0x61].Original) && !before0.SequenceEqual(reopened.Glyphs[0x30].Original);
                bool cp852 = new[] { 0xA0, 0x82, 0x8D, 0xE1, 0xFD }.All(c => reopened.Glyphs[c].Original.SequenceEqual(raster.AsSpan(c * 8, 8).ToArray()));
                if (!changed || !cp852) throw new InvalidDataException("TTF raster/import/save/reopen verification failed.");
                Console.WriteLine("RUNIT TTF smoke: PASS"); Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("RUNIT TTF smoke: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--runit-packed-preview", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                FontLoadResult loaded = RunVgaFontService.LoadRunVga(args[1]);
                bool pass = loaded.Game == ElviraGame.Elvira2 && loaded.Layout == RunVgaFontLayout.OriginalPackedAscii98 &&
                    loaded.LoadedGlyphCount == 98 && loaded.FirstByteValue == 0x20 && loaded.LastByteValue == 0x81 &&
                    loaded.Glyphs[0x30].Original.Any(b => b != 0) && loaded.Glyphs[0x41].Original.Any(b => b != 0) &&
                    loaded.Glyphs[0x4C].Original.Any(b => b != 0) && loaded.Glyphs[0x61].Original.Any(b => b != 0) &&
                    loaded.Glyphs[0x80].Original.Any(b => b != 0) && loaded.Glyphs[0x81].Original.Any(b => b != 0);
                Console.WriteLine($"RUNIT packed preview: {(pass ? "PASS" : "FAIL")} A={Convert.ToHexString(loaded.Glyphs[0x41].Original)} L={Convert.ToHexString(loaded.Glyphs[0x4C].Original)}");
                Environment.ExitCode = pass ? 0 : 1;
            }
            catch (Exception ex) { Console.Error.WriteLine("RUNIT packed preview: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runvga-preview-regression", StringComparison.OrdinalIgnoreCase))
        {
            string? temporaryCanonical = null;
            try
            {
                // Preview loading must never alter either input. The temporary baseline lets
                // the same test cover the packed and canonical-unpacked Elvira I paths.
                byte[] packedBytes = File.ReadAllBytes(args[1]);
                string packedHashBefore = Hash(args[1]);
                byte[] canonical = RunVgaBootstrapService.UnpackVerifiedOriginal(packedBytes);
                FontLoadResult packed = RunVgaFontService.LoadRunVga(args[1]);
                AssertElvira1OriginalPreview(packed, RunVgaFontLayout.OriginalPackedAscii98, canonical, "packed");
                if (Hash(args[1]) != packedHashBefore)
                    throw new InvalidDataException("Packed RUNVGA changed while previewing it.");

                temporaryCanonical = Path.Combine(Path.GetTempPath(), "ElviraVgaEditor-preview-" + Guid.NewGuid().ToString("N") + ".EXE");
                File.WriteAllBytes(temporaryCanonical, canonical);
                FontLoadResult unpacked = RunVgaFontService.LoadRunVga(temporaryCanonical);
                AssertElvira1OriginalPreview(unpacked, RunVgaFontLayout.OriginalAscii98, canonical, "unpacked");

                FontLoadResult v5 = RunVgaFontService.LoadRunVga(args[2]);
                if (v5.Game != ElviraGame.Elvira1 || v5.Layout != RunVgaFontLayout.ExtendedCp852V5 ||
                    v5.LoadedGlyphCount != 256 || !v5.Glyphs[0x81].Original.SequenceEqual(RunItBootstrapService.ReservedHudEraseGlyphBytes) ||
                    !FontSlotMetadata.IsReserved(v5, 0x81))
                    throw new InvalidDataException("Elvira I V5 preview/reserved-glyph validation failed.");

                Console.WriteLine("RUNVGA packed/unpacked/V5 preview: PASS");
                Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("RUNVGA packed/unpacked/V5 preview: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            finally { if (temporaryCanonical is not null && File.Exists(temporaryCanonical)) File.Delete(temporaryCanonical); }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--vga-backup-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunVgaBackupSmoke(args[1], args[2]);
                Console.WriteLine("VGA O-backup workflow: PASS"); Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("VGA O-backup workflow: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--runit-patched-preview", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                FontLoadResult loaded = RunVgaFontService.LoadRunVga(args[1]);
                bool pass = loaded.Game == ElviraGame.Elvira2 && loaded.Layout == RunVgaFontLayout.ExtendedCp852RunIt &&
                    loaded.LoadedGlyphCount == 224 && loaded.FirstByteValue == 0x20 && loaded.LastByteValue == 0xFF &&
                    new[] { 0x20, 0x41, 0x61, 0x80, 0x81, 0x82, 0x8E, 0xA0, 0xE1, 0xFF }.All(c => loaded.Glyphs[c].IsLoadedFromSource) &&
                    FontSlotMetadata.IsReserved(loaded, 0x81) && loaded.Glyphs[0x81].Original.SequenceEqual(RunItBootstrapService.ReservedHudEraseGlyphBytes);
                Console.WriteLine($"RUNIT patched preview: {(pass ? "PASS" : "FAIL")} loaded={loaded.LoadedGlyphCount}");
                Environment.ExitCode = pass ? 0 : 1;
            }
            catch (Exception ex) { Console.Error.WriteLine("RUNIT patched preview: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--gamepc-backup-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunGamePcBackupSmoke(args[1], args[2]);
                Console.WriteLine("GAMEPC O-backup workflow: PASS"); Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("GAMEPC O-backup workflow: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }

    private static void RunProductionSmoke(ElviraGame game, string pristineExe, string root)
    {
        string name = game == ElviraGame.Elvira1 ? "elvira1" : "elvira2";
        string directory = Path.Combine(root, name);
        if (Directory.Exists(directory)) throw new IOException($"Isolated test directory already exists: {directory}");
        Directory.CreateDirectory(directory);
        string activeExe = Path.Combine(directory, game == ElviraGame.Elvira1 ? "RUNVGA.EXE" : "RUNIT.EXE");
        string sourceDir = Path.GetDirectoryName(pristineExe) ?? throw new InvalidDataException("Source directory missing.");
        File.Copy(pristineExe, activeExe);
        File.Copy(Path.Combine(sourceDir, "GAMEPC"), Path.Combine(directory, "GAMEPC"));
        FontLoadResult initial = RunVgaFontService.LoadRunVga(activeExe);
        ProductionDeploymentResult first = GamePatchDeploymentService.Deploy(initial, initial.Glyphs);
        string originalExeHash = Hash(first.OriginalExecutable), originalGamePcHash = Hash(first.OriginalGamePc);
        FontLoadResult extended = RunVgaFontService.LoadRunVga(first.ActiveExecutable);
        extended.Glyphs[0x41].ReplaceEdited(Convert.FromHexString("A0B0C0D0E0F00000"));
        ProductionDeploymentResult second = GamePatchDeploymentService.Deploy(extended, extended.Glyphs);
        if (Hash(second.OriginalExecutable) != originalExeHash || Hash(second.OriginalGamePc) != originalGamePcHash)
            throw new InvalidDataException($"{name}: an O-file changed on the second deployment.");
        FontLoadResult reopened = RunVgaFontService.LoadRunVga(second.ActiveExecutable);
        if (!reopened.Glyphs[0x41].Original.SequenceEqual(Convert.FromHexString("A0B0C0D0E0F00000")))
            throw new InvalidDataException($"{name}: active glyph did not round-trip.");
    }

    private static string Hash(string path) => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(path)));

    private static void AssertElvira1OriginalPreview(FontLoadResult loaded, RunVgaFontLayout expectedLayout, byte[] canonical, string sourceKind)
    {
        if (loaded.Game != ElviraGame.Elvira1 || loaded.Layout != expectedLayout || loaded.LoadedGlyphCount != 98 ||
            loaded.FirstByteValue != 0x20 || loaded.LastByteValue != 0x81 || !FontSlotMetadata.IsReserved(loaded, 0x81))
            throw new InvalidDataException($"Elvira I {sourceKind} preview did not expose the expected 98-glyph range.");

        foreach (int code in new[] { 0x20, 0x30, 0x41, 0x42, 0x43, 0x45, 0x48, 0x49, 0x4D, 0x4F, 0x53, 0x54, 0x3F, 0x61, 0x6D, 0x7A, 0x81 })
        {
            byte[] expected = canonical.AsSpan(0x1A216 + (code - 0x20) * RunVgaFontService.GlyphBytes, RunVgaFontService.GlyphBytes).ToArray();
            if (!loaded.Glyphs[code].IsLoadedFromSource || !loaded.Glyphs[code].Original.SequenceEqual(expected))
                throw new InvalidDataException($"Elvira I {sourceKind} preview glyph 0x{code:X2} differs from the canonical table.");
        }
        if (!loaded.Glyphs[0x81].Original.SequenceEqual(RunItBootstrapService.ReservedHudEraseGlyphBytes))
            throw new InvalidDataException($"Elvira I {sourceKind} preview did not preserve reserved glyph 0x81.");
    }

    private static void RunPartialBackupSmoke(string pristineExe, string pristineGamePc, string root)
    {
        foreach (string state in new[] { "exe_o_only", "gamepc_o_only", "active_missing" })
        {
            string dir = Path.Combine(root, state);
            if (Directory.Exists(dir)) throw new IOException($"Isolated partial-state directory already exists: {dir}");
            Directory.CreateDirectory(dir);
            string active = Path.Combine(dir, "RUNIT.EXE"), gamepc = Path.Combine(dir, "GAMEPC");
            File.Copy(pristineExe, active); File.Copy(pristineGamePc, gamepc);
            if (state == "exe_o_only") File.Copy(active, Path.Combine(dir, "RUNITO.EXE"));
            if (state == "gamepc_o_only") File.Copy(gamepc, Path.Combine(dir, "GAMEPCO"));
            if (state == "active_missing") { File.Copy(active, Path.Combine(dir, "RUNITO.EXE")); File.Copy(gamepc, Path.Combine(dir, "GAMEPCO")); File.Delete(active); }
            string before = File.Exists(Path.Combine(dir, "RUNITO.EXE")) ? Hash(Path.Combine(dir, "RUNITO.EXE")) : "";
            bool rejected = false;
            try { GamePatchDeploymentService.Deploy(RunVgaFontService.LoadRunVga(active), GlyphRepository.CreateAllCp852Slots()); }
            catch (Exception) { rejected = true; }
            if (!rejected) throw new InvalidDataException($"Partial state {state} was accepted.");
            if (before.Length != 0 && Hash(Path.Combine(dir, "RUNITO.EXE")) != before) throw new InvalidDataException($"Partial state {state} overwrote RUNITO.EXE.");
        }
    }

    private static void RunVgaBackupSmoke(string sourceVga, string root)
    {
        if (Directory.Exists(root)) throw new IOException($"Isolated VGA test directory already exists: {root}");
        Directory.CreateDirectory(root);
        string active = Path.Combine(root, Path.GetFileName(sourceVga));
        File.Copy(sourceVga, active);
        string originalHash = Hash(active), oFile = SafeDeployer.OriginalBackupPath(active);
        if (File.Exists(oFile)) throw new InvalidDataException("Unexpected O-file before modification.");
        _ = new VgaImageTableParser(File.ReadAllBytes(active)).Parse(); // read/preview-equivalent: no O-file creation
        if (File.Exists(oFile)) throw new InvalidDataException("Read-only VGA parse created an O-file.");

        InstallHarmlessValidatedVgaVariant(active, 0xA5);
        if (!File.Exists(oFile) || Hash(oFile) != originalHash) throw new InvalidDataException("First VGA O-file does not equal the original active file.");
        string oHash = Hash(oFile);
        InstallHarmlessValidatedVgaVariant(active, 0x5A);
        if (Hash(oFile) != oHash) throw new InvalidDataException("Second VGA save overwrote the O-file.");
    }

    private static void InstallHarmlessValidatedVgaVariant(string active, byte marker)
    {
        byte[] bytes = File.ReadAllBytes(active);
        Array.Resize(ref bytes, bytes.Length + 1); bytes[^1] = marker; // parsers tolerate inert tail data; active hash changes for transaction coverage.
        string prepared = active + ".prepared";
        File.WriteAllBytes(prepared, bytes);
        _ = new VgaImageTableParser(File.ReadAllBytes(prepared)).Parse();
        SafeDeployer.ReplaceActiveWithPrepared(active, prepared);
    }

    private static void RunGamePcBackupSmoke(string sourceGamePc, string root)
    {
        if (Directory.Exists(root)) throw new IOException($"Isolated GAMEPC test directory already exists: {root}");
        Directory.CreateDirectory(root);
        string active = Path.Combine(root, "GAMEPC"); File.Copy(sourceGamePc, active);
        string originalHash = Hash(active), oFile = SafeDeployer.OriginalBackupPath(active);
        List<GamePcStringEntry> entries = GamePcTextEditor.LoadEntries(active);
        GamePcStringEntry entry = entries.First(e => e.ByteLength > 0);
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        var enc = GamePcTextEditor.GetEncoding("CP852");
        GamePcTextEditor.SaveInPlace(active, new Dictionary<int, string> { [entry.Index] = new string('X', entry.ByteLength) }, entries, enc);
        if (!File.Exists(oFile) || Hash(oFile) != originalHash) throw new InvalidDataException("GAMEPCO does not equal first active GAMEPC.");
        string oHash = Hash(oFile);
        entries = GamePcTextEditor.LoadEntries(active);
        entry = entries.First(e => e.ByteLength > 0);
        GamePcTextEditor.SaveInPlace(active, new Dictionary<int, string> { [entry.Index] = new string('Y', entry.ByteLength) }, entries, enc);
        if (Hash(oFile) != oHash) throw new InvalidDataException("Repeated GAMEPC save overwrote GAMEPCO.");
    }
}
