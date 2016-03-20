using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Xml;

namespace Stardew_Valley_Save_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SavedGame _savedGame = null;
        private string _originalData = null;
        private string _changedData = null;
        JsonConfig _config = null;
        private bool _loadingXml = false;
        private XmlDocument _xmlDoc = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            string jsonConfigString = File.ReadAllText("config.json");
            _config = JsonConvert.DeserializeObject<JsonConfig>(jsonConfigString);


            for (int tabIndex = 0; tabIndex < _config.Tabs.Length; ++tabIndex)
            {
                TabItem tabItem = new TabItem();
                tabItem.Header = _config.Tabs[tabIndex].Title;
                Grid grid = new Grid();
                grid.HorizontalAlignment = HorizontalAlignment.Stretch;
                grid.VerticalAlignment = VerticalAlignment.Stretch;
                grid.ShowGridLines = true;

                ColumnDefinition gridColumn1 = new ColumnDefinition();
                ColumnDefinition gridColumn2 = new ColumnDefinition();
                grid.ColumnDefinitions.Add(gridColumn1);
                grid.ColumnDefinitions.Add(gridColumn2);

                if (_config.Tabs[tabIndex].Items != null)
                {
                    for (int itemIndex = 0; itemIndex < _config.Tabs[tabIndex].Items.Length; ++itemIndex)
                    {
                        _config.Tabs[tabIndex].Items[itemIndex].ParentTab = new WeakReference<JsonTab>(_config.Tabs[tabIndex]);

                        RowDefinition gridRow = new RowDefinition();
                        gridRow.Height = GridLength.Auto;
                        grid.RowDefinitions.Add(gridRow);


                        TextBlock textBlock = new TextBlock();
                        textBlock.Margin = new Thickness(10);
                        textBlock.Text = _config.Tabs[tabIndex].Items[itemIndex].Title;
                        textBlock.FontWeight = FontWeights.Bold;
                        textBlock.VerticalAlignment = VerticalAlignment.Top;
                        textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                        _config.Tabs[tabIndex].Items[itemIndex].GridRow = itemIndex;
                        Grid.SetRow(textBlock, itemIndex);
                        Grid.SetColumn(textBlock, 0);
                        gridRow.ToolTip = _config.Tabs[tabIndex].Items[itemIndex].Hint;

                        if (_config.Tabs[tabIndex].Items[itemIndex].Type == ItemType.Number)
                        {
                            Xceed.Wpf.Toolkit.DecimalUpDown numberTextBox = new Xceed.Wpf.Toolkit.DecimalUpDown();
                            numberTextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                            numberTextBox.VerticalAlignment = VerticalAlignment.Stretch;
                            numberTextBox.FormatString = "N0";
                            numberTextBox.ValueChanged += NumberTextBox_ValueChanged;
                            numberTextBox.Tag = _config.Tabs[tabIndex].Items[itemIndex];
                            numberTextBox.Minimum = _config.Tabs[tabIndex].Items[itemIndex].Min;
                            numberTextBox.Maximum = _config.Tabs[tabIndex].Items[itemIndex].Max;
                            numberTextBox.IsEnabled = false;
                            Grid.SetRow(numberTextBox, itemIndex);
                            Grid.SetColumn(numberTextBox, 1);

                            grid.Children.Add(numberTextBox);
                            _config.Tabs[tabIndex].Items[itemIndex].InputReference = new WeakReference<object>(numberTextBox);
                        }
                        else if (_config.Tabs[tabIndex].Items[itemIndex].Type == ItemType.Season)
                        {
                            ComboBox seasonComboBox = new ComboBox();
                            seasonComboBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                            seasonComboBox.VerticalAlignment = VerticalAlignment.Stretch;
                            seasonComboBox.SelectionChanged += SeasonComboBox_SelectionChanged;
                            seasonComboBox.Items.Add("Spring");
                            seasonComboBox.Items.Add("Summer");
                            seasonComboBox.Items.Add("Fall");
                            seasonComboBox.Items.Add("Winter");
                            seasonComboBox.SelectedIndex = 0;
                            seasonComboBox.Tag = _config.Tabs[tabIndex].Items[itemIndex];
                            seasonComboBox.IsEnabled = false;


                            Grid.SetRow(seasonComboBox, itemIndex);
                            Grid.SetColumn(seasonComboBox, 1);

                            grid.Children.Add(seasonComboBox);
                            _config.Tabs[tabIndex].Items[itemIndex].InputReference = new WeakReference<object>(seasonComboBox);
                        }
                        else if (_config.Tabs[tabIndex].Items[itemIndex].Type == ItemType.String)
                        {
                            TextBox textBox = new TextBox();
                            textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                            textBox.VerticalAlignment = VerticalAlignment.Stretch;
                            textBox.TextChanged += TextBox_TextChanged;
                            textBox.Tag = _config.Tabs[tabIndex].Items[itemIndex];
                            textBox.IsEnabled = false;

                            Grid.SetRow(textBox, itemIndex);
                            Grid.SetColumn(textBox, 1);

                            grid.Children.Add(textBox);
                            _config.Tabs[tabIndex].Items[itemIndex].InputReference = new WeakReference<object>(textBox);
                        }
                        else if (_config.Tabs[tabIndex].Items[itemIndex].Type == ItemType.Boolean)
                        {
                            CheckBox checkBox = new CheckBox();
                            checkBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                            checkBox.VerticalAlignment = VerticalAlignment.Stretch;
                            checkBox.Checked += CheckBox_Checked;
                            checkBox.Tag = _config.Tabs[tabIndex].Items[itemIndex];
                            checkBox.IsEnabled = false;

                            Grid.SetRow(checkBox, itemIndex);
                            Grid.SetColumn(checkBox, 1);

                            grid.Children.Add(checkBox);
                            _config.Tabs[tabIndex].Items[itemIndex].InputReference = new WeakReference<object>(checkBox);
                        }
                        else if (_config.Tabs[tabIndex].Items[itemIndex].Type == ItemType.Decimal)
                        {
                            Xceed.Wpf.Toolkit.DecimalUpDown decimalTextBox = new Xceed.Wpf.Toolkit.DecimalUpDown();
                            decimalTextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                            decimalTextBox.VerticalAlignment = VerticalAlignment.Stretch;
                            decimalTextBox.FormatString = "F3";
                            decimalTextBox.ValueChanged += DecimalTextBox_ValueChanged;
                            decimalTextBox.Tag = _config.Tabs[tabIndex].Items[itemIndex];
                            decimalTextBox.Minimum = _config.Tabs[tabIndex].Items[itemIndex].Min;
                            decimalTextBox.Maximum = _config.Tabs[tabIndex].Items[itemIndex].Max;
                            decimalTextBox.Increment = _config.Tabs[tabIndex].Items[itemIndex].Increment;
                            decimalTextBox.IsEnabled = false;
                            Grid.SetRow(decimalTextBox, itemIndex);
                            Grid.SetColumn(decimalTextBox, 1);

                            grid.Children.Add(decimalTextBox);
                            _config.Tabs[tabIndex].Items[itemIndex].InputReference = new WeakReference<object>(decimalTextBox);

                        }
                        else if (_config.Tabs[tabIndex].Items[itemIndex].Type == ItemType.Name)
                        {
                            TextBox nameTextBox = new TextBox();
                            nameTextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                            nameTextBox.VerticalAlignment = VerticalAlignment.Stretch;
                            nameTextBox.TextChanged += NameTextBox_TextChanged;
                            nameTextBox.Tag = _config.Tabs[tabIndex].Items[itemIndex];
                            nameTextBox.IsEnabled = false;
                            Grid.SetRow(nameTextBox, itemIndex);
                            Grid.SetColumn(nameTextBox, 1);

                            grid.Children.Add(nameTextBox);
                            _config.Tabs[tabIndex].Items[itemIndex].InputReference = new WeakReference<object>(nameTextBox);

                        }
                        else if (_config.Tabs[tabIndex].Items[itemIndex].Type == ItemType.Gender)
                        {
                            ComboBox genderComboBox = new ComboBox();
                            genderComboBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                            genderComboBox.VerticalAlignment = VerticalAlignment.Stretch;
                            genderComboBox.SelectionChanged += GenderComboBox_SelectionChanged;
                            genderComboBox.Items.Add("Male");
                            genderComboBox.Items.Add("Female");
                            genderComboBox.SelectedIndex = 0;
                            genderComboBox.Tag = _config.Tabs[tabIndex].Items[itemIndex];
                            genderComboBox.IsEnabled = false;


                            Grid.SetRow(genderComboBox, itemIndex);
                            Grid.SetColumn(genderComboBox, 1);

                            grid.Children.Add(genderComboBox);
                            _config.Tabs[tabIndex].Items[itemIndex].InputReference = new WeakReference<object>(genderComboBox);

                        }






                        grid.Children.Add(textBlock);
                    }
                }

                ScrollViewer scrollViewer = new ScrollViewer();
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                scrollViewer.Content = grid;

                tabItem.Content = scrollViewer;
                tabControl.Items.Add(tabItem);
            }
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_loadingXml)
                return;

            // TODO: Requires moving of game directory and some other funky business.
        }

        private void GenderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loadingXml)
                return;

            ComboBox genderComboBox = sender as ComboBox;
            if (genderComboBox == null)
                return;

            JsonItem item = genderComboBox.Tag as JsonItem;
            if (item == null)
                return;

            JsonTab tab = null;
            if (item.ParentTab.TryGetTarget(out tab) == false)
                return;

            string xmlPath = tab.Path + item.Path + "/text()";

            string selectedGender = genderComboBox.SelectedItem.ToString();
            switch (selectedGender)
            {
                case "Male":
                    _xmlDoc.SelectSingleNode(xmlPath).Value = "true";
                    break;

                case "Female":
                    _xmlDoc.SelectSingleNode(xmlPath).Value = "false";
                    break;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (_loadingXml)
                return;

            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null)
                return;

            JsonItem item = checkBox.Tag as JsonItem;
            if (item == null)
                return;

            JsonTab tab = null;
            if (item.ParentTab.TryGetTarget(out tab) == false)
                return;

            string xmlPath = tab.Path + item.Path + "/text()";
            _xmlDoc.SelectSingleNode(xmlPath).Value = ((checkBox.IsChecked == false || checkBox.IsChecked == null) ? "false" : "true");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_loadingXml)
                return;

            TextBox textBox = sender as TextBox;
            if (textBox == null)
                return;

            JsonItem item = textBox.Tag as JsonItem;
            if (item == null)
                return;

            JsonTab tab = null;
            if (item.ParentTab.TryGetTarget(out tab) == false)
                return;

            string xmlPath = tab.Path + item.Path + "/text()";
            _xmlDoc.SelectSingleNode(xmlPath).Value = textBox.Text;
        }

        private void DecimalTextBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_loadingXml)
                return;

            Xceed.Wpf.Toolkit.DecimalUpDown decimalTextBox = sender as Xceed.Wpf.Toolkit.DecimalUpDown;
            if (decimalTextBox == null)
                return;

            JsonItem item = decimalTextBox.Tag as JsonItem;
            if (item == null)
                return;

            JsonTab tab = null;
            if (item.ParentTab.TryGetTarget(out tab) == false)
                return;

            string xmlPath = tab.Path + item.Path + "/text()";
            _xmlDoc.SelectSingleNode(xmlPath).Value = e.NewValue.ToString();
        }

        private void SeasonComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loadingXml)
                return;

            ComboBox seasonComboBox = sender as ComboBox;
            if (seasonComboBox == null)
                return;

            JsonItem item = seasonComboBox.Tag as JsonItem;
            if (item == null)
                return;

            JsonTab tab = null;
            if (item.ParentTab.TryGetTarget(out tab) == false)
                return;

            string xmlPath = tab.Path + item.Path + "/text()";

            string selectedSeason = seasonComboBox.SelectedItem.ToString();
            switch (selectedSeason)
            {
                case "Spring":
                    _xmlDoc.SelectSingleNode(xmlPath).Value = "spring";
                    break;

                case "Summer":
                    _xmlDoc.SelectSingleNode(xmlPath).Value = "summer";
                    break;

                case "Fall":
                    _xmlDoc.SelectSingleNode(xmlPath).Value = "fall";
                    break;

                case "Winter":
                    _xmlDoc.SelectSingleNode(xmlPath).Value = "winter";
                    break;
            }
        }

        private void NumberTextBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_loadingXml)
                return;

            Xceed.Wpf.Toolkit.DecimalUpDown numberTextBox = sender as Xceed.Wpf.Toolkit.DecimalUpDown;
            if (numberTextBox == null)
                return;

            JsonItem item = numberTextBox.Tag as JsonItem;
            if (item == null)
                return;

            JsonTab tab = null;
            if (item.ParentTab.TryGetTarget(out tab) == false)
                return;

            string xmlPath = tab.Path + item.Path + "/text()";
            _xmlDoc.SelectSingleNode(xmlPath).Value = e.NewValue.ToString();
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
            _savedGame = null;

            //try
            {
                string path = Path.Combine(savedGame.FullPath, GetSavedFileName(savedGame));
                string xmlData = File.ReadAllText(path);
                _originalData = xmlData;
                _changedData = xmlData;

                _loadingXml = true;

                _xmlDoc = new XmlDocument();
                _xmlDoc.LoadXml(xmlData);

                foreach (JsonTab tab in _config.Tabs)
                {
                    if (tab.Items != null)
                    {
                        foreach (JsonItem item in tab.Items)
                        {
                            string xmlPath = tab.Path + item.Path + "/text()";
                            if (item.Type == ItemType.Number)
                            {
                                object inputObject = null;
                                if (item.InputReference.TryGetTarget(out inputObject))
                                {
                                    Xceed.Wpf.Toolkit.DecimalUpDown inputTextBox = inputObject as Xceed.Wpf.Toolkit.DecimalUpDown;
                                    if (inputTextBox != null)
                                    {
                                        inputTextBox.IsEnabled = true;
                                        decimal value = 0;
                                        if (decimal.TryParse(_xmlDoc.SelectSingleNode(xmlPath).Value, out value))
                                        {
                                            inputTextBox.Value = value;
                                        }
                                    }
                                }
                            }
                            else if (item.Type == ItemType.Season)
                            {
                                object inputObject = null;
                                if (item.InputReference.TryGetTarget(out inputObject))
                                {
                                    ComboBox sesaonComboBox = inputObject as ComboBox;
                                    if (sesaonComboBox != null)
                                    {
                                        sesaonComboBox.IsEnabled = true;
                                        foreach (string seasonItem in sesaonComboBox.Items)
                                        {
                                            if (seasonItem.ToLower() == _xmlDoc.SelectSingleNode(xmlPath).Value)
                                            {
                                                sesaonComboBox.SelectedItem = seasonItem;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (item.Type == ItemType.String)
                            {
                                object inputObject = null;
                                if (item.InputReference.TryGetTarget(out inputObject))
                                {
                                    TextBox inputTextBox = inputObject as TextBox;
                                    if (inputTextBox != null)
                                    {
                                        inputTextBox.IsEnabled = true;
                                        inputTextBox.Text = _xmlDoc.SelectSingleNode(xmlPath).Value;
                                    }
                                }
                            }
                            else if (item.Type == ItemType.Boolean)
                            {
                                object inputObject = null;
                                if (item.InputReference.TryGetTarget(out inputObject))
                                {
                                    CheckBox inputCheckBox = inputObject as CheckBox;
                                    if (inputCheckBox != null)
                                    {
                                        inputCheckBox.IsEnabled = true;
                                        inputCheckBox.IsChecked = (_xmlDoc.SelectSingleNode(xmlPath).Value == "true");
                                    }
                                }
                            }
                            else if (item.Type == ItemType.Decimal)
                            {
                                object inputObject = null;
                                if (item.InputReference.TryGetTarget(out inputObject))
                                {
                                    Xceed.Wpf.Toolkit.DecimalUpDown inputTextBox = inputObject as Xceed.Wpf.Toolkit.DecimalUpDown;
                                    if (inputTextBox != null)
                                    {
                                        inputTextBox.IsEnabled = true;
                                        decimal value = 0;
                                        if (decimal.TryParse(_xmlDoc.SelectSingleNode(xmlPath).Value, out value))
                                        {
                                            inputTextBox.Value = value;
                                        }
                                    }
                                }
                            }
                            else if (item.Type == ItemType.Name)
                            {
                                object inputObject = null;
                                if (item.InputReference.TryGetTarget(out inputObject))
                                {
                                    TextBox inputTextBox = inputObject as TextBox;
                                    if (inputTextBox != null)
                                    {
                                        // Don't enable editing of name just yet.
                                        inputTextBox.IsEnabled = false;
                                        inputTextBox.Text = _xmlDoc.SelectSingleNode(xmlPath).Value;
                                    }
                                }
                            }
                            else if (item.Type == ItemType.Gender)
                            {
                                object inputObject = null;
                                if (item.InputReference.TryGetTarget(out inputObject))
                                {
                                    ComboBox genderComboBox = inputObject as ComboBox;
                                    if (genderComboBox != null)
                                    {
                                        genderComboBox.IsEnabled = true;
                                        string value = _xmlDoc.SelectSingleNode(xmlPath).Value;
                                        if (_xmlDoc.SelectSingleNode(xmlPath).Value == "true")
                                        {
                                            genderComboBox.SelectedItem = "Male";
                                        }
                                        else
                                        {
                                            genderComboBox.SelectedItem = "Female";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                _loadingXml = false;


                _savedGame = savedGame;
            }
            //catch (Exception err)
            // {
            //    _savedGame = null;
            //    MessageBox.Show($"Could not load saved game. ({err.Message})", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            // }
        }

        public string GetSavedFileName(SavedGame savedGame)
        {
            return Path.Combine(savedGame.FullPath, Path.GetFileName(savedGame.FullPath));
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
            SaveMenu.IsEnabled = _savedGame != null;
        }

        private void SaveMenu_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = Save();
            if (errorMessage != null)
            {
                MessageBox.Show($"Could not save game data. ({errorMessage})", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string Save()
        {
            if (_savedGame == null)
            {
                return "No game currently loaded.";
            }

            try
            {

                string path = Path.Combine(_savedGame.FullPath, GetSavedFileName(_savedGame));
                using (XmlTextWriter textWriter = new XmlTextWriter(path, Encoding.UTF8))
                {
                    textWriter.Formatting = System.Xml.Formatting.None;
                    _xmlDoc.Save(textWriter);
                }

                path = Path.Combine(_savedGame.FullPath, "SaveGameInfo");
                XmlDocument saveGameInfo = new XmlDocument();
                saveGameInfo.Load(path);

                XmlNode playerNode = _xmlDoc.SelectSingleNode("/SaveGame/player");

                saveGameInfo.IterateThroughAllNodes(delegate (XmlNode node)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        try
                        {
                            string smallSavePath = FindXPath(node);
                            string largeSavePath = smallSavePath.Replace("Farmer[1]", "SaveGame/player");

                            XmlNode largeNode = _xmlDoc.SelectSingleNode(largeSavePath);

                            if (node.HasChildNodes && node.FirstChild.NodeType == XmlNodeType.Text && largeNode.HasChildNodes && largeNode.FirstChild.NodeType == XmlNodeType.Text)
                            {
                                string data = largeNode.InnerXml;
                                node.InnerXml = data;
                                Debug.WriteLine(smallSavePath + ", " + data);
                            }

                        }
                        catch (Exception err)
                        {
                            // TODO: Handle errors
                            Debug.WriteLine($"Error: {err.Message}");
                        }
                    }
                });

                using (XmlTextWriter textWriter = new XmlTextWriter(path, Encoding.UTF8))
                {
                    textWriter.Formatting = System.Xml.Formatting.None;
                    saveGameInfo.Save(textWriter);
                }

            }
            catch (Exception err)
            {
                return err.Message;
            }

            return null;
        }

        //http://stackoverflow.com/questions/241238/how-to-get-xpath-from-an-xmlnode-instance
        private string FindXPath(XmlNode node)
        {
            StringBuilder builder = new StringBuilder();
            while (node != null)
            {
                switch (node.NodeType)
                {
                    case XmlNodeType.Attribute:
                        builder.Insert(0, "/@" + node.Name);
                        node = ((XmlAttribute)node).OwnerElement;
                        break;
                    case XmlNodeType.Element:
                        int index = FindElementIndex((XmlElement)node);
                        builder.Insert(0, "/" + node.Name + "[" + index + "]");
                        node = node.ParentNode;
                        break;
                    case XmlNodeType.Document:
                        return builder.ToString();
                    default:
                        throw new ArgumentException("Only elements and attributes are supported");
                }
            }
            throw new ArgumentException("Node was not in a document");
        }

        private int FindElementIndex(XmlElement element)
        {
            XmlNode parentNode = element.ParentNode;
            if (parentNode is XmlDocument)
            {
                return 1;
            }
            XmlElement parent = (XmlElement)parentNode;
            int index = 1;
            foreach (XmlNode candidate in parent.ChildNodes)
            {
                if (candidate is XmlElement && candidate.Name == element.Name)
                {
                    if (candidate == element)
                    {
                        return index;
                    }
                    index++;
                }
            }
            throw new ArgumentException("Couldn't find element within parent");
        }

        private void LaunchNow_Click(object sender, RoutedEventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = "steam://run/413150";
            process.Start();
        }

        private void LaunchSave_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = Save();
            if (errorMessage == null)
            {
                Process process = new Process();
                process.StartInfo.FileName = "steam://run/413150";
                process.Start();
            }
            else
            {
                MessageBox.Show($"Could not save game data. ({errorMessage})", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    //http://stackoverflow.com/a/20629818/1253832
    public static class XmlDocumentExtensions
    {
        public static void IterateThroughAllNodes(
            this XmlDocument doc,
            Action<XmlNode> elementVisitor)
        {
            if (doc != null && elementVisitor != null)
            {
                foreach (XmlNode node in doc.ChildNodes)
                {
                    doIterateNode(node, elementVisitor);
                }
            }
        }

        private static void doIterateNode(
            XmlNode node,
            Action<XmlNode> elementVisitor)
        {
            elementVisitor(node);

            foreach (XmlNode childNode in node.ChildNodes)
            {
                doIterateNode(childNode, elementVisitor);
            }
        }
    }
}