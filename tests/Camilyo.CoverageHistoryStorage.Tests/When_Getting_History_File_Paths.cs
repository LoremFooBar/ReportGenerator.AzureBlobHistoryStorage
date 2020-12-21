using System;
using System.Collections.Generic;
using System.Linq;
using Camilyo.Framework.Testing;
using FluentAssertions;

namespace Camilyo.CoverageHistoryStorage.Tests
{
    public class When_Getting_History_File_Paths : HistoryStorageSpecificationBase
    {
        private List<string> _historyFilePaths;

        protected override void When()
        {
            base.When();

            _historyFilePaths = HistoryStorage.GetHistoryFilePaths().ToList();
        }

        [Then]
        public void It_Should_Return_Correct_Blob_Urls()
        {
            _historyFilePaths.Should().NotBeEmpty();
            _historyFilePaths.Should().HaveCount(3);

            var uris = _historyFilePaths.Select(s => new UriBuilder(s)).ToList();
            uris.Should().OnlyContain(u =>
                u.Host == ContainerUri.Host && u.Query == ContainerUri.Query && u.Scheme == ContainerUri.Scheme &&
                u.Uri.IsDefaultPort);
            uris.Should().SatisfyRespectively(
                first => first.Path.Should().Be($"{ContainerUri.AbsolutePath}/commit1/{FakeCoverageFileName}"),
                second => second.Path.Should().Be($"{ContainerUri.AbsolutePath}/commit2/{FakeCoverageFileName}"),
                third => third.Path.Should().Be($"{ContainerUri.AbsolutePath}/commit3/{FakeCoverageFileName}"));
        }
    }
}
