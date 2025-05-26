using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TextEditor.Models;
using TextEditor.Models.FileManager;
using TextEditor.UserControls;
using TextEditor.Strategies; 

namespace TextEditor
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private IUserControlFactory userControlFactory = null!;
        private IUserControlSwitchStrategy userControlSwitchStrategy = null!;
        private UserControl currentUserControl = null!;
        private EditUserControl? currentEditUserControl = null!;
        public event PropertyChangedEventHandler? PropertyChanged = null!;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            userControlFactory = new UserControlFactory();
            userControlSwitchStrategy = new DefaultUserControlSwitchStrategy(userControlFactory);

            CurrentUserControl = userControlSwitchStrategy.GetUserControl(UserControlTypes.Edit, new Document());
            RecentFiles = new ObservableCollection<KeyValuePair<string, string>>();
        }

        public UserControl CurrentUserControl
        {
            get => currentUserControl;
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

        private ObservableCollection<KeyValuePair<string, string>> recentFiles = new();

        public ObservableCollection<KeyValuePair<string, string>> RecentFiles
        {
            get => recentFiles;
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

            CurrentUserControl = userControlSwitchStrategy.GetUserControl(userControl, document);
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            CurrentUserControl = userControlSwitchStrategy.GetUserControl(UserControlTypes.Edit, new Document());
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
                CurrentUserControl = userControlSwitchStrategy.GetUserControl(UserControlTypes.Edit, document);

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