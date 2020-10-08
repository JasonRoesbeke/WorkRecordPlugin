using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Xml.Linq;

namespace WpfAppImportGPX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource cancelTokenSource;
        private static readonly ComboBox[] inputCombos = new ComboBox[4];
        private static readonly TextBox[] inputTexts = new TextBox[4];

        public MainWindow()
        {
            InitializeComponent();
            inputCombos[0] = Component1Combo;
            inputCombos[1] = Component2Combo;
            inputCombos[2] = Component3Combo;
            inputCombos[3] = Component4Combo;
            inputTexts[0] = Component1TextBox;
            inputTexts[1] = Component2TextBox;
            inputTexts[2] = Component3TextBox;
            inputTexts[3] = Component4TextBox;
            foreach (ComboBox combo in inputCombos)
                combo.SelectedIndex = 0;
            // Default values for TEST purposes:
            inputTexts[0].Text = @"C:\tmp-in\COURSE_42957969.gpx";
            inputCombos[0].SelectedIndex = 1;
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
            // Timestamp 28:48 in IAmTimCorey C# Async / Await https://www.youtube.com/watch?v=2moh18sh5p4
            // Timestamp 48:42, 1:32:17 in JeremyBytes I'll Get Back to You: Task, Await https://www.youtube.com/watch?v=B2HDDKq4d3c
            cancelTokenSource = new CancellationTokenSource();
            Stopwatch watch = Stopwatch.StartNew();
            Task<List<Component>> loopTask = LoopThroughInputsParellelAsync(cancelTokenSource.Token);
            loopTask.ContinueWith(
                task => {
                    switch (task.Status)
                    {
                        case TaskStatus.RanToCompletion:
                            List<Component> components = task.Result;
                            if (components.Count == 0)
                                MessageBox.Show("There were no files set to be included in the processing.", "Notice", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            else
                            {
                                var invalids = components.Count<Component>(comp => comp.Type == ComponentType.ExceptionValue);
                                if (invalids > 0)
                                    MessageBox.Show($"{invalids} file" + ((invalids == 1) ? "" : "s") + " didn't have valid trackpoints."
                                        + "\r\nPlease check the source file" + ((invalids == 1) ? "" : "s") + ".",
                                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                else
                                {
                                    // DisconnectedContext and ContextSwitchDeadlock exceptions???????
                                    try
                                    {
                                        string results = ComponentFunctions.AggregateComponents(components);
                                        //MessageBox.Show(results);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show($"1 exception occurred.\r\n"
                                            + ex.Message
                                            + "\r\nPlease check the source files.",
                                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }
                            break;
                        case TaskStatus.Canceled:
                            break;
                        case TaskStatus.Faulted:
                            int exs = task.Exception.Flatten().InnerExceptions.Count;
                            MessageBox.Show($"{exs} exception" + ((exs == 1) ? "" : "s") + " occurred during the processing.\r\n"
                                + task.Exception.Flatten().InnerExceptions[0].Message + ((exs == 1) ? "" : "; …")
                                + "\r\nPlease check the source files.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                    }
                    watch.Stop();
                    MessageBox.Show($"Total execution time: {watch.ElapsedMilliseconds}");
                },
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Timestamp 1:26:16 in JeremyBytes I'll Get Back to You: Task, Await https://www.youtube.com/watch?v=B2HDDKq4d3c
            cancelTokenSource.Cancel();
        }

        private static async Task<List<Component>> LoopThroughInputsParellelAsync(CancellationToken cancelToken = new CancellationToken())
        {
            // Timestamp 23:27, 26:48 in IAmTimCorey C# Async / Await https://www.youtube.com/watch?v=2moh18sh5p4
            List<Task<Component>> tasks = new List<Task<Component>>();
            for (int i = 0; i < inputTexts.Length; i++)
            {
                cancelToken.ThrowIfCancellationRequested();
                if (!(inputCombos[i].SelectedItem is ComponentType))
                    throw new InvalidOperationException("The ComboBox must be binded to the expected enum.");
                if ((ComponentType)inputCombos[i].SelectedItem == ComponentType.ExceptionValue)
                    continue;// No processing needed
                // Need the following 2 vars to avoid System.InvalidOperationException: 'The calling thread cannot access this object because a different thread owns it.'
                string path = inputTexts[i].Text;
                ComponentType componentType = (ComponentType)inputCombos[i].SelectedItem;
                tasks.Add(Task.Run(() => ComponentFunctions.ProcessGPXFile(path, componentType)));
            }
            var results = await Task.WhenAll(tasks);
            return results.ToList<Component>();
        }
    }

    internal static class ComponentFunctions
    {
        public static Component ProcessGPXFile(string path, ComponentType componentType)
        {
            XDocument doc = XDocument.Load(path);
            List<PointXYZ> trackpoints = new List<PointXYZ>();
            XElement root = doc.Root;
            XNamespace xNs = root.GetDefaultNamespace();
            foreach (XElement item in doc.Descendants(xNs + "trkpt").ToList())
            {
                List<XElement> ele = item.Descendants(xNs + "ele").ToList();
                if (ele.Count == 1)
                    trackpoints.Add(new PointXYZ(
                        decimal.Parse(item.Attribute("lat").Value, CultureInfo.InvariantCulture),
                        decimal.Parse(item.Attribute("lon").Value, CultureInfo.InvariantCulture),
                        decimal.Parse(ele[0].Value, CultureInfo.InvariantCulture)
                        ));
            }
            if (trackpoints.Count == 0)
                return new Component(new List<PointXYZ>(), ComponentType.ExceptionValue);
            else
                return new Component(trackpoints, componentType);
        }

        internal static string AggregateComponents(List<Component> components)
        {
            return "test";
            //throw new NotImplementedException();
        }
    }

    internal class PointXYZ
    {
        public PointXYZ(decimal lat, decimal lon, decimal ele )
        {
            Lat = lat;
            Lon = lon;
            Ele = ele;
        }

        public decimal Lat { get; }
        public decimal Lon { get; }
        public decimal Ele { get; }
    }

    internal class Component
    {
        public Component(List<PointXYZ> points, ComponentType type)
        {
            Points = points;
            Type = type;
        }

        public List<PointXYZ> Points { get; }
        public ComponentType Type { get; }
    }
}
