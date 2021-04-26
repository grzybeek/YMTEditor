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

        private void SaveXML_Click(object sender, RoutedEventArgs e)
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
                XMLHandler.SaveXML(filename);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string btn = Convert.ToString((sender as Button).DataContext);

            Console.WriteLine("DataContext: " + (sender as Button).DataContext);
            Console.WriteLine("Content: " + (sender as Button).Content);
            if (int.TryParse(btn, out int val))
            {
                //DataContext is number(int) so it was new texture clicked
                Console.WriteLine(btn);
            }
            else
            {
                //DataContext is string so it was new variation clicked
                Console.WriteLine(btn);
            }
            Console.WriteLine(btn);
        }

        private string GetParents(Object element, int parentLevel)
        {
            string returnValue = String.Format("[{0}] {1}", parentLevel, element.GetType());
            if (element is FrameworkElement)
            {
                if (((FrameworkElement)element).Parent != null)
                    returnValue += String.Format("{0}{1}",
                        Environment.NewLine, GetParents(((FrameworkElement)element).Parent, parentLevel + 1));
            }
            return returnValue;
        }
    }
}
