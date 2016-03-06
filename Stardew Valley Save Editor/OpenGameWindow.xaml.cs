using System;
using System.Collections.Generic;
using System.IO;
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

namespace Stardew_Valley_Save_Editor
{
    /// <summary>
    /// Interaction logic for OpenGameWindow.xaml
    /// </summary>
    public partial class OpenGameWindow : Window
    {
        public SavedGame SelectedSavedGame { get; internal set; } = null;

        public OpenGameWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string stardewValleySavesPath = Path.Combine(appDataPath, "StardewValley", "Saves");

            string[] saves = Directory.GetDirectories(stardewValleySavesPath);
            foreach (string save in saves)
            {
                SavedGame savedGame = GetSavedGameFromPath(save);

                comboBox.Items.Add(savedGame);
            }

            if (comboBox.Items.Count == 0)
            {
                MessageBox.Show("Could not find any saved games.");
                comboBox.IsEnabled = false;
                openButton.IsEnabled = false;
            }
            else
            {
                comboBox.SelectedIndex = 0;
            }
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedSavedGame = comboBox.SelectedItem as SavedGame;
            DialogResult = true;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public static SavedGame GetSavedGameFromPath(string path)
        {
            SavedGame savedGame = new SavedGame();
            savedGame.Value = Path.GetFileName(path);
            savedGame.Name = savedGame.Value;
            if (savedGame.Name.IndexOf("_") > 0)
            {
                savedGame.Name = savedGame.Name.Substring(0, savedGame.Name.IndexOf("_"));
            }
            savedGame.FullPath = path;
            return savedGame;
        }
    }
}
