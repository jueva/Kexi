using System.Linq;
using Kexi.Common;
using Kexi.ViewModel.Item;
using Xunit;

namespace Kexi.Test
{
    public class ItemFilterTest
    {
        [Fact]
        public void MatchItem()
        {
            var items = new[]
            {
                new FileItem(@"c:\test"),
                new FileItem(@"c:\mytestmy"),
                new FileItem(@"c:\test1")
            };

            var filter = new ItemFilter<FileItem>(items, "test");
            var result = filter.Matches.Count();
            Assert.Equal(3, result);
            result = filter.MatchesContaining.Count();
            Assert.Equal(3, result);
            result = filter.MatchesBeginning.Count();
            Assert.Equal(2, result);
            result = filter.MatchesEquals.Count();
            Assert.Equal(1, result);
        }
    }
}