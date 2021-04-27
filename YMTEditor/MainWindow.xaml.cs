using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace YMTEditor
{
    public partial class MainWindow : Window
    {
        public static ObservableCollection<ComponentData> Components;
        
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
            string[] btn_parts = btn.Split((char)32);
            if(btn_parts.Length > 1) // if more than 1 then add texture
            {
                int index = Convert.ToInt32(btn_parts[0]);
                int txtCount = Convert.ToInt32(btn_parts[1]);
                

                string drawableName = Convert.ToString((sender as Button).Tag);
                int enumNumber = (int)(ComponentTypes.ComponentNumbers)Enum.Parse(typeof(ComponentTypes.ComponentNumbers), drawableName.ToLower());
                

                if (txtCount >= 26)
                {
                    MessageBox.Show("Can't add more textures, limit is 26! (a-z)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int _index = Convert.ToInt32(Components.Where(z => z.compId == enumNumber).First().compIndex);
                string _FirstTexId = Components.ElementAt(_index).compList.ElementAt(index).drawableTextures[0].textureTexId;
                Components.ElementAt(_index).compList.ElementAt(index).drawableTextures.Add(new ComponentTexture(XMLHandler.Number2String(txtCount, false), _FirstTexId));
                Components.ElementAt(_index).compList.ElementAt(index).drawableTextureCount++;
                ComponentsItemsControl.ItemsSource = Components;
            }
            else //else add new variation
            {

                int _index = Convert.ToInt32(Components.Where(z => z.compType == btn.ToLower()).First().compIndex);
                int drawIndex = Components.ElementAt(_index).compList.Count();

                ComponentDrawable _newDrawable = new ComponentDrawable(drawIndex, 1, 1, 0, false, new ObservableCollection<ComponentTexture>());
                _newDrawable.drawableTextures.Add(new ComponentTexture("a", "0"));
                Components.ElementAt(_index).compList.Add(_newDrawable);
                    
            }
        }
    }
}
