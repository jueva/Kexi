using Kexi.Common;
using Kexi.ViewModel.Popup;
using Xunit;

namespace Kexi.Test
{
    public class CommandsPopupViewModelTest
    {
        [Theory]
        [InlineData("AddTemporaryBookmarksCommand", "Add Temporary Bookmarks")]
        [InlineData("AddTemporaryBookmarkscommand", "Add Temporary Bookmarkscommand")]
        [InlineData("CommandAlphaBravo", "Alpha Bravo")]
        [InlineData("Copy Command", "Copy")]
        public void CleanupCommandName(string baseName, string expected)
        {
            var model = new CommandsPopupViewModel(null, new Options(), null);
            Assert.Equal(expected, model.GetName(baseName));
        }
    }
}