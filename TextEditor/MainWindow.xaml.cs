using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
            RecentFiles = new ObservableCollection<KeyValuePair<string, string>>();
        }

        private IUserControlFactory userControlFactory;
        private UserControl currentUserControl;
        private EditUserControl? currentEditUserControl;
        public event PropertyChangedEventHandler PropertyChanged;

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
                        editUserControl.FileSaved += UpdateRecentFiles;
                    }

                    if (currentUserControl is PreviewUserControl previewUserControl)
                    {
                        previewUserControl.FileSaved += UpdateRecentFiles;
                    }
                }
            }
        }

        private ObservableCollection<KeyValuePair<string, string>> recentFiles;

        public ObservableCollection<KeyValuePair<string, string>> RecentFiles
        {
            get { return recentFiles; }
            set
            {
                recentFiles = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RecentFiles)));
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
                Filter = FileManager.FileFilter
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                OpenFile(openFileDialog.FileName);
            }
        }

        private void RecentFilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listbox && listbox.SelectedItem is KeyValuePair<string, string> keyValue)
            {
                OpenFile(keyValue.Value);
            }
        }

        private void UpdateRecentFiles(KeyValuePair<string, string> keyValuePair)
        {
            if (RecentFiles.Contains(keyValuePair))
            {
                RecentFiles.Remove(keyValuePair);
            }

            RecentFiles.Insert(0, keyValuePair);
        }

        private async void OpenFile(string filePath)
        {
            var fileManager = new FileManager();

            try
            {
                string content = await fileManager.Open(filePath);
                var document = new Document(filePath, content);
                CurrentUserControl = userControlFactory.CreateUserControl(UserControlTypes.Edit, document, SwitchUserControl);

                var fileName = Path.GetFileName(filePath);
                var keyValuePair = new KeyValuePair<string, string>(fileName, filePath);
                UpdateRecentFiles(keyValuePair);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}