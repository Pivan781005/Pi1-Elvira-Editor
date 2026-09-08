namespace Pi1ElviraEditor;

internal static class AppInfo
{
    internal const string ProductName = "π1 Elvira Editor";

    // The assembly version is the single product-version authority. It must
    // never be sourced from a UI locale, because product identity is invariant.
    internal static string ProductVersion =>
        typeof(AppInfo).Assembly.GetName().Version?.ToString(2) ?? "0.0";

    internal static string ProductTitle => ProductName + " v" + ProductVersion;
}
