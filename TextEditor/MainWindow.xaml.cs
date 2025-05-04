using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using TextEditor.Models;
using TextEditor.Models.FileManager;
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
            CurrentUserControl = userControlFactory.CreateUserControl(UserControlTypes.Edit, null, SwitchUserControl);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private UserControlFactory userControlFactory;

        private UserControl currentUserControl;
        private EditUserControl? currentEditUserControl;

        public UserControl CurrentUserControl
        {
            get { return currentUserControl; }
            set
            {
                if (currentUserControl != value)
                {
                    currentUserControl = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentUserControl)));
                    if (currentUserControl is EditUserControl editUserControl)
                    {
                        currentEditUserControl = editUserControl;
                    }
                }
            }
        }

        private void SwitchUserControl(UserControlTypes userControl, Document document)
        {
            if (userControl == UserControlTypes.Edit && currentEditUserControl != null)
            {
                CurrentUserControl = currentEditUserControl;
                return;
            }

            CurrentUserControl = userControlFactory.CreateUserControl(userControl, document, SwitchUserControl);
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentUserControl = userControlFactory.CreateUserControl(UserControlTypes.Edit, new Document(), SwitchUserControl);
        }

        private void ToggleSideBarBtn_Click(object sender, RoutedEventArgs e)
        {
            SideBarColumn.Width = SideBarColumn.Width == new GridLength(0) ? new GridLength(200) : new GridLength(0);
            ToggleSideBarBtn.Content = SideBarColumn.Width == new GridLength(0) ? ">" : "<";
        }

        private async void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            var fileManager = new FileManager();
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = fileManager.FileFilter
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filePath = openFileDialog.FileName;
                string content = await fileManager.Open(filePath);
                var document = new Document(filePath, content);
                CurrentUserControl = userControlFactory.CreateUserControl(UserControlTypes.Edit, document, SwitchUserControl);
            }
        }
    }
}