using System.Text;
using System.Text.Json;

namespace ElviraVgaEditor;

internal enum TextDiagnosticKind
{
    None,
    ConfirmedSafe,
    PossibleRisk,
    ConfirmedRisk,
    Ignored,
    UnknownGame
}

internal sealed record TextDiagnosticResult(
    TextDiagnosticKind Kind,
    int Bytes,
    int? Limit,
    int OverBy,
    string VisibleText,
    string ClippedText,
    string Context,
    string Confidence)
{
    public bool IsRisk => Kind is TextDiagnosticKind.PossibleRisk or TextDiagnosticKind.ConfirmedRisk;
}

internal static class Elvira1TextMetadata
{
    public static TextDiagnosticResult Evaluate(int index, string text, Encoding enc, bool ignored)
    {
        int bytes = enc.GetByteCount(text);
        return new(TextDiagnosticKind.None, bytes, null, 0, text, string.Empty,
            "No fixed Elvira I runtime text-length limit is currently proven.", "PROVEN NO GLOBAL LIMIT");
    }
}

internal sealed class TextDiagnosticStore
{
    private sealed class State
    {
        public State() { }
        public HashSet<int> IgnoredWarnings { get; set; } = new();
    }

    private readonly HashSet<int> _ignored = new();
    public string PathName { get; }

    private TextDiagnosticStore(string path) => PathName = path;

    public static TextDiagnosticStore Load(string gameDirectory)
    {
        string path = System.IO.Path.Combine(gameDirectory, ".pi1-text-diagnostics.json");
        var store = new TextDiagnosticStore(path);
        try
        {
            if (File.Exists(path))
            {
                var state = JsonSerializer.Deserialize<State>(File.ReadAllText(path));
                if (state?.IgnoredWarnings is not null)
                    store._ignored.UnionWith(state.IgnoredWarnings);
            }
        }
        catch { /* A corrupt sidecar must never block opening the game. */ }
        return store;
    }

    public bool IsIgnored(int index) => _ignored.Contains(index);

    public void SetIgnored(int index, bool ignored)
    {
        if (ignored) _ignored.Add(index); else _ignored.Remove(index);
        Save();
    }

    private void Save()
    {
        try
        {
            var state = new State { IgnoredWarnings = new HashSet<int>(_ignored) };
            File.WriteAllText(PathName, JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch { /* non-fatal preference persistence */ }
    }
}
