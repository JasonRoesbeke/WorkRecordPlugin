using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WpfAppImportGPX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string doNotInclude = "Do Not Include";
        private static readonly string[] componentComboEntries = new string[] {
            doNotInclude,
            ComponentType.FieldBoundary.GetDescription(),
            ComponentType.DrivenHeadland.GetDescription(),
            ComponentType.ABLine.GetDescription(),
            ComponentType.ABCurve.GetDescription()
        };

        public MainWindow()
        {
            InitializeComponent();
            Component1Combo.ItemsSource = componentComboEntries;
            Component1Combo.SelectedItem = doNotInclude;
            Component2Combo.ItemsSource = componentComboEntries;
            Component2Combo.SelectedItem = doNotInclude;
            Component3Combo.ItemsSource = componentComboEntries;
            Component3Combo.SelectedItem = doNotInclude;
            Component4Combo.ItemsSource = componentComboEntries;
            Component4Combo.SelectedItem = doNotInclude;
        }

        private void Component1Button_Click(object sender, RoutedEventArgs e) { ComponentBrowseButtonClick(Component1TextBox); /*ComponentBrowseButtonClick((Button)sender, Component1TextBox);*/ }
        private void Component2Button_Click(object sender, RoutedEventArgs e) { ComponentBrowseButtonClick(Component2TextBox); }
        private void Component3Button_Click(object sender, RoutedEventArgs e) { ComponentBrowseButtonClick(Component3TextBox); }
        private void Component4Button_Click(object sender, RoutedEventArgs e) { ComponentBrowseButtonClick(Component4TextBox); }

        private void ComponentBrowseButtonClick (TextBox textBox)
        {
            // OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog {
                Title = "Select gpx file to include",
                Filter = "GPX Files (*.gpx)|*.gpx",
                Multiselect = false
            };
            Nullable<bool> dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == true)
                textBox.Text = openFileDialog.FileName;
        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
