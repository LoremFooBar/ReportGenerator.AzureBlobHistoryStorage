namespace ReportGenerator.AzureBlobHistoryStorage;

public interface ICommandLineArgumentsParser
{
    Dictionary<string, string> GetCommandLineArguments();
}
