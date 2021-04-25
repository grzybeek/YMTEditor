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
            //Drawables = new ObservableCollection<ComponentDrawable>();
            /*List<HeadComponent> items = new List<HeadComponent>();
            items.Add(new HeadComponent() {index = 0, texturesAmount = 5 });
            items.Add(new HeadComponent() {index = 1, texturesAmount = 15 });
            items.Add(new HeadComponent() { index = 9, texturesAmount = 15 });
            items.Add(new HeadComponent() { index = 15, texturesAmount = 15 });

            HeadItemsControl.ItemsSource = items;*/
            //Components.Add("dupa");
            //Components.Add(Drawables);
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
                foreach (var item in Components)
                {
                    Console.WriteLine("dsadsa");
                    Console.WriteLine(item);
                }

            }
        }

    }
}
