namespace ElviraVgaEditor;

internal static class TranslationVariantService
{
    public static VariantEntry Create(
        string installationDirectory,
        ElviraGameProfile game,
        VariantCatalog catalog,
        string name,
        string code)
    {
        VariantEntry candidate = VariantNaming.Create(name, code, game, true, catalog.Entries.Count + 1);
        string directory = Path.GetFullPath(installationDirectory);
        string dataDestination = Path.Combine(directory, candidate.DataFile);
        string exeDestination = Path.Combine(directory, candidate.ExeFile);
        string originalData = Path.Combine(directory, GamePcOriginalService.OriginalFileName);
        string baseExe = Path.Combine(directory, VariantNaming.DeriveExeFile("EN", game));
        if (File.Exists(dataDestination) || File.Exists(exeDestination))
            throw new IOException("The new variant data file or executable already exists and will not be overwritten.");
        if (catalog.FindByDataFile(candidate.DataFile) is not null)
            throw new InvalidOperationException($"A variant already uses {candidate.DataFile}.");
        if (!File.Exists(originalData) || !File.Exists(baseExe))
            throw new FileNotFoundException("The protected GAMEPCO source or active base executable is missing.");

        string dataTemp = dataDestination + ".pi1_new_" + Guid.NewGuid().ToString("N") + ".tmp";
        string exeTemp = exeDestination + ".pi1_new_" + Guid.NewGuid().ToString("N") + ".tmp";
        bool dataCreated = false, exeCreated = false, registered = false;
        try
        {
            // Copy, strict-parse, then move: no partly initialized data file becomes visible.
            File.Copy(originalData, dataTemp, overwrite: false);
            int expected = GamePcTextEditor.LoadEntries(originalData, game).Count;
            GamePcTextEditor.ValidateSerializedData(dataTemp, expected, game);
            File.Copy(baseExe, exeTemp, overwrite: false);
            File.Move(dataTemp, dataDestination);
            dataCreated = true;
            File.Move(exeTemp, exeDestination);
            exeCreated = true;
            VariantEntry created = catalog.Add(candidate.DisplayName, candidate.Code, candidate.DataFile, candidate.ExeFile);
            registered = true;
            VariantConfigurationService.Save(catalog);
            return created;
        }
        catch
        {
            if (registered) catalog.Remove(candidate.DataFile);
            if (exeCreated) try { File.Delete(exeDestination); } catch { }
            if (dataCreated) try { File.Delete(dataDestination); } catch { }
            throw;
        }
        finally
        {
            try { if (File.Exists(dataTemp)) File.Delete(dataTemp); } catch { }
            try { if (File.Exists(exeTemp)) File.Delete(exeTemp); } catch { }
        }
    }
}
