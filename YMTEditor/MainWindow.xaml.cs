using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;

namespace YMTEditor
{
    public partial class MainWindow : Window
    {
        public static MenuItem _componentsMenu;
        public static MenuItem _removeAsk;
        public static TextBlock _logBar;

        public static ObservableCollection<ComponentData> Components;
        
        public MainWindow()
        {
            InitializeComponent();

            _componentsMenu = (MenuItem)FindName("ComponentsMenu");
            _logBar = (TextBlock)FindName("logBar");
            _removeAsk = (MenuItem)FindName("RemovingAskCheck");

            _removeAsk.IsChecked = Properties.Settings.Default.removeAsk;
            SetLogMessage("Loaded options");

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
                _componentsMenu.IsEnabled = true;
                _componentsMenu.ToolTip = "Check/Uncheck components";
                SetLogMessage("Loaded XML from path: " + filename);
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
                SetLogMessage("Saved XML to path: " + filename);
            }
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            string btn = Convert.ToString((sender as Button).DataContext);
            string[] btn_parts = btn.Split((char)32);
            if(btn_parts.Length > 1) // if more than 1 then add texture
            {
                int index = Convert.ToInt32(btn_parts[0]); //clicked number 000/001/002 etc
                int txtCount = Convert.ToInt32(btn_parts[1]); // current textures
                
                string drawableName = Convert.ToString((sender as Button).Tag); //drawable we are adding txt jbib/teef/lowr etc
                int enumNumber = (int)(ComponentTypes.ComponentNumbers)Enum.Parse(typeof(ComponentTypes.ComponentNumbers), drawableName.ToLower());
                
                if (txtCount >= 26)
                {
                    MessageBox.Show("Can't add more textures, limit is 26! (a-z)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int _index = Convert.ToInt32(Components.Where(z => z.compId == enumNumber).First().compIndex); //component index in our ymt

                ComponentDrawable comp = Components.ElementAt(_index).compList.Where(c => c.drawableIndex == index).First(); //get component
                int drawableToAddTexture = Components.ElementAt(_index).compList.IndexOf(comp); //get index of our clicked comonent in collection
                string _FirstTexId = Components.ElementAt(_index).compList.ElementAt(drawableToAddTexture).drawableTextures.First().textureTexId; //get texId of first texture to keep same texid

                string txtLetter = XMLHandler.Number2String(txtCount, false);
                comp.drawableTextures.Add(new ComponentTexture(txtLetter, _FirstTexId));
                comp.drawableTextureCount++;
                SetLogMessage("Added new texture (letter " + txtLetter + ") to drawable " + String.Format("{0:D3}", drawableToAddTexture) + " in " + drawableName + " component | Count: " + comp.drawableTextureCount + " / 26");

            }
            else //else add new drawable number
            {

                int _index = Convert.ToInt32(Components.Where(z => z.compType == btn.ToLower()).First().compIndex);
                int drawIndex = Components.ElementAt(_index).compList.Count();

                ComponentDrawable _newDrawable = new ComponentDrawable(drawIndex, 1, 1, 0, false, new ObservableCollection<ComponentTexture>());
                _newDrawable.drawableTextures.Add(new ComponentTexture("a", "0"));
                Components.ElementAt(_index).compList.Add(_newDrawable);

                SetLogMessage("Added new drawable (number " + String.Format("{0:D3}", drawIndex) + ") to " + btn + " component");
            }
        }

        private void Button_Click_Remove(object sender, RoutedEventArgs e)
        {
            string btn = Convert.ToString((sender as Button).DataContext);

            string[] btn_parts = btn.Split((char)32);
            if (btn_parts.Length > 1) // if more than 1 then remove last texture
            {
                
                int drawableIndex = Convert.ToInt32(btn_parts[0]);
                int lastTxtIndex = Convert.ToInt32(btn_parts[1]);
                string drawableName = Convert.ToString((sender as Button).Tag);

                int enumNumber = (int)(ComponentTypes.ComponentNumbers)Enum.Parse(typeof(ComponentTypes.ComponentNumbers), drawableName.ToLower());
                int _index = Convert.ToInt32(Components.Where(z => z.compId == enumNumber).First().compIndex); //component index in our ymt

                ComponentDrawable comp = Components.ElementAt(_index).compList.Where(c => c.drawableIndex == drawableIndex).First(); //get component
                int txtCount = comp.drawableTextureCount;

                if (txtCount <= 1)
                {
                    MessageBox.Show("Can't remove texture, minimum is 1! \nRemove drawable instead", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //no need to show box with confirmation to remove texture - it can be added with just one click
                comp.drawableTextures.RemoveAt(txtCount - 1); //remove last texture
                comp.drawableTextureCount--;
                SetLogMessage("Removed texture from drawable " + String.Format("{0:D3}", drawableIndex) + " in " + drawableName + " component | Count: " + comp.drawableTextureCount + " / 26");

            }
            else //else remove drawable
            {
                int clickedIndex = Convert.ToInt32(btn_parts[0]);

                string drawableName = Convert.ToString((sender as Button).Tag);
                int _index = Convert.ToInt32(Components.Where(z => z.compType == drawableName.ToLower()).First().compIndex);

                ComponentDrawable comp = Components.ElementAt(_index).compList.Where(c => c.drawableIndex == clickedIndex).First();
                int drawableToRemoveIndex = Components.ElementAt(_index).compList.IndexOf(comp);

                if (Components.ElementAt(_index).compList.Count() > 1)
                {
                    if (_removeAsk.IsChecked)
                    {
                        MessageBoxResult result = MessageBox.Show(this, "Do you want to remove drawable " + String.Format("{0:D3}", drawableToRemoveIndex) + "?\nIt can't be restored.\n\nThis box can be disabled in options.", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.OK)
                        {
                            Components.ElementAt(_index).compList.RemoveAt(drawableToRemoveIndex);
                            UpdateDrawableIndexes(Components.ElementAt(_index));
                            SetLogMessage("Removed drawable (number " + String.Format("{0:D3}", drawableToRemoveIndex) + ") from " + drawableName + " component | CHANGED ALL OTHER INDEX NUMBERS (!)");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        Components.ElementAt(_index).compList.RemoveAt(drawableToRemoveIndex);
                        UpdateDrawableIndexes(Components.ElementAt(_index));
                        SetLogMessage("Removed drawable (number " + String.Format("{0:D3}", drawableToRemoveIndex) + ") from " + drawableName + " component | CHANGED ALL OTHER INDEX NUMBERS (!)");
                    }
                }
                else
                {
                    MessageBox.Show("Can't remove drawable, minimum is 1! \nRemove component instead", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void ComponentsMenu_Click(object sender, RoutedEventArgs e)
        {
            SetLogMessage("not implemented :(");
        }

        private void UpdateDrawableIndexes(ComponentData component)
        {
            int drawCount = component.compList.Count();
            for (int i = 0; i < drawCount; i++)
            {
                component.compList.ElementAt(i).drawableIndex = i;
            }

        }

        public void SetLogMessage(string message)
        {
            _logBar.Text = "Logs: " + message;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void numAlternatives_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SetLogMessage("numAlternatives changed to: " + e.NewValue);
        }

        private void propMask_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            int _newPropMask = (int) e.NewValue;

            string compName = Convert.ToString((sender as FrameworkElement).Parent.GetValue(TagProperty)).ToLower();
            int drawIndex = (int) (sender as FrameworkElement).Tag;

            int enumNumber = (int)(ComponentTypes.ComponentNumbers)Enum.Parse(typeof(ComponentTypes.ComponentNumbers), compName);
            int _index = Convert.ToInt32(Components.Where(z => z.compId == enumNumber).First().compIndex);

            ComponentDrawable comp = Components.ElementAt(_index).compList.Where(c => c.drawableIndex == drawIndex).First(); //get component
            comp.drawablePropMask = _newPropMask;
            string _newTexId = _newPropMask == 17 || _newPropMask == 19 || _newPropMask == 21 ? "1" : "0";

            foreach (var txt in comp.drawableTextures)
            {
                txt.textureTexId = _newTexId;
            }

            switch (_newPropMask)
            {
                case 17:
                case 19:
                case 21:
                    SetLogMessage("propMask changed to: " + _newPropMask + " | Your .ydd model should end with _r, and .ytd texture with _whi");
                    break;
                default:
                    SetLogMessage("propMask changed to: " + _newPropMask + " | Your .ydd model should end with _u, and .ytd texture with _uni");
                    break;
            }
        }

        private void RemovingAsk_Click(object sender, RoutedEventArgs e)
        {
            bool? value = (sender as MenuItem).IsChecked;

            Properties.Settings.Default.removeAsk = (bool) value;
            Properties.Settings.Default.Save();
            SetLogMessage("Saved options, will be restored when opening the program");
        }

    }

}
