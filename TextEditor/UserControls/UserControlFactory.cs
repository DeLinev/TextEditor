using System.Windows.Controls;

namespace TextEditor.UserControls
{
    public interface IUserControlFactory
    {
        UserControl CreateUserControl(UserControlTypes userControl, Action<UserControlTypes> userControlHandler);
    }

    public class UserControlFactory : IUserControlFactory
    {
        public UserControl CreateUserControl(UserControlTypes userControl, Action<UserControlTypes> userControlHandler)
        {
            switch (userControl)
            {
                case UserControlTypes.Edit:
                    var editUserControl = new EditUserControl();
                    editUserControl.UserControlSwitched += userControlHandler;
                    return editUserControl;
                case UserControlTypes.Preview:
                    var previewUserControl = new PreviewUserControl();
                    previewUserControl.UserControlSwitched += userControlHandler;
                    return previewUserControl;
                default:
                    throw new ArgumentException("Invalid user control type");
            }
        }
    }
}
