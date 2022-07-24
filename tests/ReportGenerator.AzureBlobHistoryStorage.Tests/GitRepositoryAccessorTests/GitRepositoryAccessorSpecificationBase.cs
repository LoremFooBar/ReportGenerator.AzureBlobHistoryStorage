using CliWrap;
using ReportGenerator.AzureBlobHistoryStorage.Tests.BDD;

namespace ReportGenerator.AzureBlobHistoryStorage.Tests.GitRepositoryAccessorTests;

public class GitRepositoryAccessorSpecificationBase : SpecificationBase
{
    protected GitRepositoryAccessor GitRepositoryAccessor { get; private set; }
    private DirectoryInfo SourceDir { get; set; }

    protected override async Task BeforeAllTestsAsync()
    {
        await base.BeforeAllTestsAsync();

        var currentDir = new DirectoryInfo(Environment.CurrentDirectory);
        SourceDir = new DirectoryInfo(Path.Combine(currentDir.FullName, "dummy"));

        if (SourceDir.Exists) {
            DeleteSourceDirectory();
            SourceDir.Create();
        }

        await Cli.Wrap("git")
            .WithArguments($"clone https://github.com/lazyboy1/Dummy.git \"{SourceDir.FullName}\"")
            .WithValidation(CommandResultValidation.ZeroExitCode)
            .WithWorkingDirectory(Environment.CurrentDirectory)
            .WithStandardErrorPipe(PipeTarget.ToDelegate(Console.Write))
            .ExecuteAsync();
    }

    protected override void AfterAllTests()
    {
        base.AfterAllTests();

        DeleteSourceDirectory();
    }

    private void DeleteSourceDirectory()
    {
        if (!SourceDir.Exists) return;

        SetAttributesNormal(SourceDir);
        SourceDir.Delete(true);
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

    protected override void Given()
    {
        base.Given();

        GitRepositoryAccessor = new GitRepositoryAccessor(SourceDir.FullName);
    }
}
