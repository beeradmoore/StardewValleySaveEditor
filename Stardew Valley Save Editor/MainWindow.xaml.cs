using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Stardew_Valley_Save_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SavedGame _savedGame = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {

            OpenGameWindow openGameWindow = new OpenGameWindow();

            bool didOpen = openGameWindow.ShowDialog() ?? false;
            if (didOpen)
            {
                SavedGame savedGame = openGameWindow.SelectedSavedGame;
                if (savedGame == null)
                {
                    MessageBox.Show("No saved game selected.", "Error");
                }
                else
                {
                    OpenSavedGame(savedGame);
                }
            }
        }

        private void OpenSavedGame(SavedGame savedGame)
        {
            _savedGame = savedGame;
        }

        private void CreateBackup_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Stardew Valley backup (*.svbu)|*.svbu";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    ZipFile.CreateFromDirectory(_savedGame.FullPath, saveFileDialog.FileName, CompressionLevel.Optimal, true);
                    MessageBox.Show("Backup created.");
                }
                catch (Exception err)
                {
                    MessageBox.Show($"Unable to create backup. ({err.Message})", "Error");
                }
            }
        }

        private void RestoreBackup_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Stardew Valley backup (*.svbu)|*.svbu";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string stardewValleySavesPath = Path.Combine(appDataPath, "StardewValley", "Saves");

                    string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                    Directory.CreateDirectory(tempDirectory);

                    ZipFile.ExtractToDirectory(openFileDialog.FileName, tempDirectory);

                    string[] directories = Directory.GetDirectories(tempDirectory);
                    if (directories.Length == 1)
                    {
                        string tempBackupDirectory = directories[0];
                        string savedGameName = Path.GetFileName(tempBackupDirectory);

                        string savedGameDestination = Path.Combine(stardewValleySavesPath, savedGameName);


                        if (Directory.Exists(savedGameDestination))
                        {
                            MessageBoxResult result = MessageBox.Show($"The saved game {savedGameName} exists, overwrite?", "Error", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
                            if (result != MessageBoxResult.Yes)
                                return;


                            string savedGameTempDestination = savedGameDestination + "_" + ((int)new Random().Next(1, Int32.MaxValue));
                            Directory.Move(savedGameDestination, savedGameTempDestination);
                            Directory.Delete(savedGameTempDestination, true);
                        }

                        Directory.Move(tempBackupDirectory, savedGameDestination);

                        MessageBoxResult openResult = MessageBox.Show("Backup restored. Open it now?", "Success", MessageBoxButton.YesNo);
                        if (openResult == MessageBoxResult.Yes)
                        {
                            SavedGame savedGame = OpenGameWindow.GetSavedGameFromPath(savedGameDestination);
                            OpenSavedGame(savedGame);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Backup appears to be malformed.", "Error");
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show($"Unable to restore backup. ({err.Message})", "Error");
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            CreateBackup.IsEnabled = _savedGame != null;
        }
    }
}
