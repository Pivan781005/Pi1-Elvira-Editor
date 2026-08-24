using System.Text;
using System.Text.RegularExpressions;

namespace ElviraVgaEditor;

internal sealed record DialogueLimitResult(string VisibleText, string TruncatedText, int VisibleBytes, int TotalBytes);

internal static class DialogueLimitValidator
{
    public static DialogueLimitResult Simulate(string text, Encoding encoding, int limit)
    {
        int total = encoding.GetByteCount(text);
        if (total <= limit)
            return new DialogueLimitResult(text, string.Empty, total, total);

        var matches = Regex.Matches(text, @"\S+\s*");
        var visible = new StringBuilder();
        int visibleBytes = 0;
        int consumedChars = 0;

        foreach (Match match in matches)
        {
            string token = match.Value;
            int tokenBytes = encoding.GetByteCount(token);
            if (visibleBytes + tokenBytes > limit)
                break;

            visible.Append(token);
            visibleBytes += tokenBytes;
            consumedChars = match.Index + match.Length;
        }

        string truncated = consumedChars < text.Length ? text[consumedChars..] : string.Empty;
        return new DialogueLimitResult(visible.ToString().TrimEnd(), truncated, visibleBytes, total);
    }
}
