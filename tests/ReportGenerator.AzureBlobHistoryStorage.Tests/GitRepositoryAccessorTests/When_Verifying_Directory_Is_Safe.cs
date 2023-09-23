using CliWrap;
using CliWrap.Buffered;
using FluentAssertions;
using ReportGenerator.AzureBlobHistoryStorage.Tests.BDD;

namespace ReportGenerator.AzureBlobHistoryStorage.Tests.GitRepositoryAccessorTests;

public class When_Verifying_Directory_Is_Safe : GitRepositoryAccessorSpecificationBase
{
    private string _safeDirectoryOutput;

    protected override async Task WhenAsync()
    {
        await base.WhenAsync();

        await GitRepositoryAccessor.VerifyDirectoryIsSafeAsync();

        var bufferedCommandResult = await Cli.Wrap("git")
            .WithArguments("config --get-all safe.directory")
            .WithValidation(CommandResultValidation.ZeroExitCode)
            .ExecuteBufferedAsync();

        _safeDirectoryOutput = bufferedCommandResult.StandardOutput;
    }

    [Then]
    public void It_Should_Mark_Working_Directory_As_Safe()
    {
        _safeDirectoryOutput.Should().ContainAll(
            Utils.NormalizePathForGitSafeDirectory(WorkingDir.FullName),
            Utils.NormalizePathForGitSafeDirectory(SourceDir.FullName));
    }
}
