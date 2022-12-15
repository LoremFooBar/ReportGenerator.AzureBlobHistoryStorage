using CliWrap;
using CliWrap.Buffered;

namespace ReportGenerator.AzureBlobHistoryStorage;

public class GitRepositoryAccessor : IGitRepositoryAccessor
{
    private readonly string _workingDirectory;
    private string? _repositoryPath;

    public GitRepositoryAccessor(string? workingDirectory) => _workingDirectory =
        string.IsNullOrEmpty(workingDirectory) ? Environment.CurrentDirectory : workingDirectory;

    private string RepositoryPath =>
        _repositoryPath ??= Cli.Wrap("git")
            .WithArguments("rev-parse --show-toplevel")
            .WithValidation(CommandResultValidation.ZeroExitCode)
            .WithWorkingDirectory(_workingDirectory)
            .ExecuteBufferedAsync(new CancellationTokenSource(TimeSpan.FromSeconds(3)).Token)
            .GetAwaiter().GetResult()
            .StandardOutput
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
            .TrimEnd('\n').TrimEnd('\r');

    public async Task<IEnumerable<string>> GetCommitIdsAsync(int numOfCommits)
    {
        var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(1)).Token;

        var bufferedCommandResult = await Cli.Wrap("git")
            .WithArguments($"log --format=\"%H\" -n {numOfCommits.ToString()}")
            .WithValidation(CommandResultValidation.ZeroExitCode)
            .WithWorkingDirectory(RepositoryPath)
            .ExecuteBufferedAsync(cancellationToken);

        return bufferedCommandResult.StandardOutput.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
    }

    public async Task<string> GetHeadCommitIdAsync()
    {
        var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
        var bufferedCommandResult = await Cli.Wrap("git")
            .WithArguments("rev-parse HEAD")
            .WithValidation(CommandResultValidation.ZeroExitCode)
            .WithWorkingDirectory(RepositoryPath)
            .ExecuteBufferedAsync(cancellationToken);

        return bufferedCommandResult.StandardOutput.TrimEnd();
    }
}

/*
    public class GitRepositoryNotFoundException : Exception
    {
        public GitRepositoryNotFoundException() : base("Git repo not found")
        {
        }
    }
*/
