using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using TextEditor.Models;
using TextEditor.UserControls;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            userControlFactory = new UserControlFactory();
            CurrentUserControl = userControlFactory.CreateUserControl(UserControlTypes.Edit, new Document(), SwitchUserControl);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private UserControlFactory userControlFactory;

        private UserControl currentUserControl;

        public UserControl CurrentUserControl
        {
            get { return currentUserControl; }
            set
            {
                if (currentUserControl != value)
                {
                    currentUserControl = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentUserControl)));
                }
            }
        }

        private void SwitchUserControl(UserControlTypes userControl, Document document)
        {
            CurrentUserControl = userControlFactory.CreateUserControl(userControl, document, SwitchUserControl);
        }
    }
}