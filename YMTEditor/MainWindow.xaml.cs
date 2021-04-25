using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static YMTEditor.ComponentData;

namespace YMTEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ObservableCollection<ComponentData> Components;
        public static ObservableCollection<ComponentDrawable> Drawables;

        public MainWindow()
        {
            

            InitializeComponent();
            Components = new ObservableCollection<ComponentData>();
            ComponentsItemsControl.ItemsSource = Components;
        }

        private void OpenXML_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog xmlFile = new OpenFileDialog
            {
                DefaultExt = ".ymt.xml",
                Filter = "Codewalker YMT XML (*.ymt.xml)|*.ymt.xml"
            };
            bool? result = xmlFile.ShowDialog();
            if (result == true)
            {
                string filename = xmlFile.FileName;
                XMLHandler.LoadXML(filename);
            }
        }

    }
}
