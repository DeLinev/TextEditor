using System.Windows.Controls;
using TextEditor.Models;
using TextEditor.UserControls;

namespace TextEditor.Strategies
{
    public interface IUserControlSwitchStrategy
    {
        UserControl GetUserControl(UserControlTypes type, Document document);
    }
}

