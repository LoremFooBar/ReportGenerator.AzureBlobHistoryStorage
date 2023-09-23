namespace ReportGenerator.AzureBlobHistoryStorage;

public static class Utils
{
    public static string NormalizePathForGitSafeDirectory(string path) =>
        Path.DirectorySeparatorChar == '\\'
            ? path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            : path;
}
