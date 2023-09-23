using CliWrap;
using ReportGenerator.AzureBlobHistoryStorage.Tests.BDD;

namespace ReportGenerator.AzureBlobHistoryStorage.Tests.GitRepositoryAccessorTests;

public class GitRepositoryAccessorSpecificationBase : SpecificationBase
{
    protected GitRepositoryAccessor GitRepositoryAccessor { get; private set; }
    protected DirectoryInfo WorkingDir { get; private set; }
    protected DirectoryInfo SourceDir { get; private set; }

    protected override async Task BeforeAllTestsAsync()
    {
        await base.BeforeAllTestsAsync();

        WorkingDir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
        WorkingDir.Create();
        SourceDir = new DirectoryInfo(Path.Combine(WorkingDir.FullName, "dummy"));

        await Cli.Wrap("git")
            .WithArguments($"clone https://github.com/lazyboy1/Dummy.git \"{SourceDir.FullName}\"")
            .WithValidation(CommandResultValidation.ZeroExitCode)
            .WithWorkingDirectory(WorkingDir.FullName)
            .WithStandardErrorPipe(PipeTarget.ToDelegate(Console.Write))
            .ExecuteAsync();
    }

    protected override Task CleanUpAsync()
    {
        base.CleanUp();

        DeleteCreatedDirectories();

        return Task.CompletedTask;
    }

    private void DeleteCreatedDirectories()
    {
        if (!WorkingDir.Exists) return;

        SetAttributesNormal(WorkingDir);
        WorkingDir.Delete(true);
    }

    private static void SetAttributesNormal(DirectoryInfo path)
    {
        // BFS folder permissions normalizer
        var dirs = new Queue<DirectoryInfo>();
        dirs.Enqueue(path);

        while (dirs.Count > 0) {
            var dir = dirs.Dequeue();
            dir.Attributes = FileAttributes.Normal;
            Parallel.ForEach(dir.EnumerateFiles(), e => e.Attributes = FileAttributes.Normal);

            foreach (var subDir in dir.GetDirectories()) {
                dirs.Enqueue(subDir);
            }
        }
    }

    protected override Task GivenAsync()
    {
        base.GivenAsync();

        GitRepositoryAccessor = new GitRepositoryAccessor(SourceDir.FullName);

        return Task.CompletedTask;
    }
}
