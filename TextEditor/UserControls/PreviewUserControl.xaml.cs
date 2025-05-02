using System.Windows;
using System.Windows.Controls;

namespace TextEditor.UserControls
{
    /// <summary>
    /// Interaction logic for PreviewUserControl.xaml
    /// </summary>
    public partial class PreviewUserControl : UserControl
    {
        public PreviewUserControl()
        {
            InitializeComponent();
        }

        public event Action<UserControlTypes> UserControlSwitched;

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            UserControlSwitched?.Invoke(UserControlTypes.Edit);
        }
    }
}
