using System.Windows;
using System.Windows.Controls;

namespace TextEditor.UserControls
{
    /// <summary>
    /// Interaction logic for EditUserControl.xaml
    /// </summary>
    public partial class EditUserControl : UserControl
    {
        public EditUserControl()
        {
            InitializeComponent();
        }

        public event Action<UserControlTypes> UserControlSwitched;

        private void PreviewBtn_Click(object sender, RoutedEventArgs e)
        {
            UserControlSwitched?.Invoke(UserControlTypes.Preview);
        }
    }
}
