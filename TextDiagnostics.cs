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
    public const int DialogueLimit = 96;
    public static readonly HashSet<int> ConfirmedAffected = new() { 417, 424 };
    public static readonly HashSet<int> ConfirmedSafe = new() { 298, 425, 627 };

    public static TextDiagnosticResult Evaluate(int index, string text, Encoding enc, bool ignored)
    {
        int bytes = enc.GetByteCount(text);
        var sim = DialogueLimitValidator.Simulate(text, enc, DialogueLimit);
        int over = Math.Max(0, bytes - DialogueLimit);

        if (ignored)
            return new(TextDiagnosticKind.Ignored, bytes, DialogueLimit, over, sim.VisibleText, sim.TruncatedText,
                "User override", "MANUAL");

        if (ConfirmedSafe.Contains(index))
            return new(TextDiagnosticKind.ConfirmedSafe, bytes, null, 0, text, string.Empty,
                index switch { 298 => "Object/room description", 425 => "Cutscene/script message", 627 => "Scroll/script message", _ => "Direct render" },
                "CONFIRMED SAFE PATH");

        if (ConfirmedAffected.Contains(index))
            return new(bytes > DialogueLimit ? TextDiagnosticKind.ConfirmedRisk : TextDiagnosticKind.ConfirmedSafe,
                bytes, DialogueLimit, over, sim.VisibleText, sim.TruncatedText,
                "Interactive NPC dialogue", "CONFIRMED RUNTIME CASE");

        if (bytes > DialogueLimit)
            return new(TextDiagnosticKind.PossibleRisk, bytes, DialogueLimit, over, sim.VisibleText, sim.TruncatedText,
                "Unknown / possible dialogue path", "POSSIBLE");

        return new(TextDiagnosticKind.None, bytes, DialogueLimit, 0, text, string.Empty,
            "Unknown", "N/A");
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
