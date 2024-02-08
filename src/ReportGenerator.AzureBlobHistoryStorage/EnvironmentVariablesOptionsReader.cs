namespace ReportGenerator.AzureBlobHistoryStorage;

public static class EnvironmentVariablesOptionsReader
{
    public static Dictionary<string, string> ReadOptionsFromEnvironment()
    {
        string[] variables =
            ["HISTORYCONTAINERURL", "WRITESASTOKEN", "REPOSITORYNAME", "SOURCEDIRECTORY", "COMMITIDS", "DEBUG"];
        var res = new Dictionary<string, string>();

        foreach (string variable in variables) {
            string? value = Environment.GetEnvironmentVariable(variable);

            if (string.IsNullOrWhiteSpace(value)) continue;

            res.Add(variable, value);
        }

        return res;
    }
}
