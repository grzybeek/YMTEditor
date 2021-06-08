using CodeWalker.GameFiles;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
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
        public static MenuItem _propsMenu;
        public static MenuItem _removeAsk;
        public static TextBlock _logBar;

        public static ObservableCollection<ComponentData> Components;
        public static ObservableCollection<PropData> Props;

        public RpfFileEntry entry;

        public MainWindow()
        {
            InitializeComponent();

            _componentsMenu = (MenuItem)FindName("ComponentsMenu");
            _propsMenu = (MenuItem)FindName("PropsMenu");
            _logBar = (TextBlock)FindName("logBar");
            _removeAsk = (MenuItem)FindName("RemovingAskCheck");

            _removeAsk.IsChecked = Properties.Settings.Default.removeAsk;
            SetLogMessage("Loaded options");

            Components = new ObservableCollection<ComponentData>();
            ComponentsItemsControl.ItemsSource = Components;

            Props = new ObservableCollection<PropData>();
            PropsItemsControl.ItemsSource = Props;
        }

        private void OpenNEW_Click(object sender, RoutedEventArgs e)
        {
            Components.Clear(); //so if we import another file when something is imported it will clear
            Props.Clear();

            _componentsMenu.IsEnabled = true;
            _componentsMenu.ToolTip = "Check/Uncheck components";
            _propsMenu.IsEnabled = true;
            _propsMenu.ToolTip = "Check/Uncheck props";

            SetLogMessage("Created new file");
            
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
                Components.Clear(); //so if we import another file when something is imported it will clear
                Props.Clear();
                string filename = xmlFile.FileName;
                XMLHandler.LoadXML(filename);
                _componentsMenu.IsEnabled = true;
                _componentsMenu.ToolTip = "Check/Uncheck components";
                _propsMenu.IsEnabled = true;
                _propsMenu.ToolTip = "Check/Uncheck props";
                SetLogMessage("Loaded XML from path: " + filename);
            }
        }

        private void SaveXML_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog xmlFile = new SaveFileDialog
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

        private void OpenYMT_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ymtFile = new OpenFileDialog
            {
                DefaultExt = ".ymt",
                Filter = "Peds YMT (*.ymt)|*.ymt"
            };
            bool? result = ymtFile.ShowDialog();
            if (result == true)
            {
                string filename = ymtFile.FileName;
                byte[] ymtBytes = File.ReadAllBytes(filename);

                Components.Clear(); //so if we import another file when something is imported it will clear
                Props.Clear();

                PedFile ymt = new PedFile();
                RpfFile.LoadResourceFile<PedFile>(ymt, ymtBytes, 2);
                string xml = MetaXml.GetXml(ymt.Meta);

                XMLHandler.LoadXML(xml);
                _componentsMenu.IsEnabled = true;
                _componentsMenu.ToolTip = "Check/Uncheck components";
                _propsMenu.IsEnabled = true;
                _propsMenu.ToolTip = "Check/Uncheck props";
                SetLogMessage("Loaded YMT from path: " + filename);
            }
        }

        private void SaveYMT_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog xmlFile = new SaveFileDialog
            {
                DefaultExt = ".ymt.xml",
                Filter = "Peds YMT (*.ymt)|*.ymt"
            };
            bool? result = xmlFile.ShowDialog();
            if (result == true)
            {
                string filename = xmlFile.FileName;
                System.Xml.XmlDocument newXml = XMLHandler.SaveYMT(filename);
                
                PedFile ymt = new PedFile();
                Meta meta = XmlMeta.GetMeta(newXml);
                byte[] newYmtBytes = ResourceBuilder.Build(meta, 2);

                File.WriteAllBytes(filename, newYmtBytes);
                
                SetLogMessage("Saved YMT to path: " + filename);
            }
        }

        private void Button_Click_AddComponent(object sender, RoutedEventArgs e)
        {
            string btn = Convert.ToString((sender as Button).DataContext);
            string[] btn_parts = btn.Split((char)32);
            if(btn_parts.Length > 1) // if more than 1 then add texture
            {
                int index = Convert.ToInt32(btn_parts[0]); //clicked number 000/001/002 etc
                int txtCount = Convert.ToInt32(btn_parts[1]); // current textures
                
                string drawableName = Convert.ToString((sender as Button).Tag); //drawable we are adding txt jbib/teef/lowr etc
                int enumNumber = (int)(YMTTypes.ComponentNumbers)Enum.Parse(typeof(YMTTypes.ComponentNumbers), drawableName.ToLower());
                
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

                int _index = Convert.ToInt32(Components.Where(z => z.compType == btn.ToLower()).First().compIndex); //index of our component
                int compId = Convert.ToInt32(Components.Where(z => z.compType == btn.ToLower()).First().compId); //id of component (11 = jbib, 4 = lowr, etc)
                int drawIndex = Components.ElementAt(_index).compList.Count(); //drawable index (000, 001, 002 etc)

                ComponentInfo defaultInfo = new ComponentInfo("none", "none", new string[] { "0", "0", "0", "0", "0" }, 0, "0", "0", "PV_COMP_HEAD", 0, compId, drawIndex); // default compInfo values

                ComponentDrawable _newDrawable = new ComponentDrawable(drawIndex, 1, 1, 0, false, new ObservableCollection<ComponentTexture>(), new ObservableCollection<ComponentInfo>());
                _newDrawable.drawableTextures.Add(new ComponentTexture("a", "0"));
                _newDrawable.drawableInfo.Add(defaultInfo);
                Components.ElementAt(_index).compList.Add(_newDrawable);

                SetLogMessage("Added new drawable (number " + String.Format("{0:D3}", drawIndex) + ") to " + btn + " component");
            }
        }

        private void Button_Click_AddProp(object sender, RoutedEventArgs e)
        {

            string btn = Convert.ToString((sender as Button).DataContext);
            string[] btn_parts = btn.Split((char)32);
            if (btn_parts.Length > 1) // if more than 1 then add texture
            {
                int index = Convert.ToInt32(btn_parts[0]); //clicked number 000/001/002 etc
                int txtCount = Convert.ToInt32(btn_parts[1]); // current textures

                string propName = Convert.ToString((sender as Button).Tag); //drawable we are adding txt (p_head, p_eyes etc)
                int enumNumber = (int)(YMTTypes.PropNumbers)Enum.Parse(typeof(YMTTypes.PropNumbers), propName.ToLower()); //0 = p_head, 1 = p_eyes etc -> YMTTypes.cs (it is also anchorId of prop)

                if (txtCount >= 26)
                {
                    MessageBox.Show("Can't add more textures, limit is 26! (a-z)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }            

                PropDrawable prop = Props.Where(p => p.propId == enumNumber).First().propList.Where(p => p.propIndex == index).First(); // get prop
                int drawableToAddTexture = Props.Where(p => p.propId == enumNumber).First().propList.IndexOf(prop); //get index of our clicked prop in collection

                string txtLetter = XMLHandler.Number2String(txtCount, false);
                prop.propTextureList.Add(new PropTexture(txtLetter, "0", "0", txtCount, 0, 0, 255));

                prop.propTextureCount++;
                SetLogMessage("Added new texture (letter " + txtLetter + ") to prop " + String.Format("{0:D3}", drawableToAddTexture) + " in " + propName + " prop | Count: " + prop.propTextureCount + " / 26");
                
            }
            else //else add new drawable number
            {

                string propName = btn; //drawable we are adding txt (p_head, p_eyes etc)
                int _index = (int)(YMTTypes.PropNumbers)Enum.Parse(typeof(YMTTypes.PropNumbers), propName.ToLower()); //0 = p_head, 1 = p_eyes etc -> YMTTypes.cs (it is also anchorId of prop)
                
                int drawIndex = Props.Where(p => p.propId == _index).First().propList.Count(); //prop drawable index (000, 001, 002 etc)

                //currently there is no ability to change it in window, so let's hardcode hats so it scale hair properly
                //TODO: fix ^above^ because if someone add new addon hat which shouldn't scale down hair, it will anyway :(
                string[] expressionMods;
                if (propName == "P_HEAD")
                {
                    expressionMods = new string[] { "-0.5", "0", "0", "0", "0" }; 
                }
                else
                {
                    expressionMods = new string[] { "0", "0", "0", "0", "0" };
                }

                //some props have renderFlag = "PRF_ALPHA", others nothing - since i don't know what it does, leave it as nothing   
                PropDrawable _newPropDrawable = new PropDrawable(drawIndex, "none", expressionMods, new ObservableCollection<PropTexture>(), "", 0, 0, _index, drawIndex, 0);
                _newPropDrawable.propTextureList.Add(new PropTexture("a", "0", "0", 0, 0, 0, 255));

                Props.Where(p => p.propId == _index).First().propList.Add(_newPropDrawable);

                SetLogMessage("Added new prop (number " + String.Format("{0:D3}", drawIndex) + ") to " + btn + " prop");
            }
        }

        private void Button_Click_RemoveComponent(object sender, RoutedEventArgs e)
        {
            string btn = Convert.ToString((sender as Button).DataContext);

            string[] btn_parts = btn.Split((char)32);
            if (btn_parts.Length > 1) // if more than 1 then remove last texture
            {
                
                int drawableIndex = Convert.ToInt32(btn_parts[0]);
                int lastTxtIndex = Convert.ToInt32(btn_parts[1]);
                string drawableName = Convert.ToString((sender as Button).Tag);

                int enumNumber = (int)(YMTTypes.ComponentNumbers)Enum.Parse(typeof(YMTTypes.ComponentNumbers), drawableName.ToLower());
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
                            UpdateDrawableComponentIndexes(Components.ElementAt(_index));
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
                        UpdateDrawableComponentIndexes(Components.ElementAt(_index));
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

        private void Button_Click_RemoveProp(object sender, RoutedEventArgs e)
        {
            string btn = Convert.ToString((sender as Button).DataContext);

            string[] btn_parts = btn.Split((char)32);
            if (btn_parts.Length > 1) // if more than 1 then remove last texture
            {

                int drawableIndex = Convert.ToInt32(btn_parts[0]); //000, 001, 002 etc
                int lastTxtIndex = Convert.ToInt32(btn_parts[1]); //last txt
                string drawableName = Convert.ToString((sender as Button).Tag); //prop name (p_head, p_eyes, etc)

                int enumNumber = (int)(YMTTypes.PropNumbers)Enum.Parse(typeof(YMTTypes.PropNumbers), drawableName.ToLower()); //0 = p_head, 1 = p_eyes etc -> YMTTypes.cs (it is also anchorId of prop)

                PropDrawable prop = Props.Where(p => p.propId == enumNumber).First().propList.Where(p => p.propIndex == drawableIndex).First(); //get prop
                int txtCount = prop.propTextureCount;

                if (txtCount <= 1)
                {
                    MessageBox.Show("Can't remove texture, minimum is 1! \nRemove drawable instead", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                //no need to show box with confirmation to remove texture - it can be added with just one click
                prop.propTextureList.RemoveAt(txtCount - 1); //remove last texture
                prop.propTextureCount--;
                SetLogMessage("Removed texture from prop drawable " + String.Format("{0:D3}", drawableIndex) + " in " + drawableName + " prop | Count: " + prop.propTextureCount + " / 26");
                
            }
            else //else remove drawable
            {
                int clickedIndex = Convert.ToInt32(btn_parts[0]);

                string drawableName = Convert.ToString((sender as Button).Tag);
                int enumNumber = (int)(YMTTypes.PropNumbers)Enum.Parse(typeof(YMTTypes.PropNumbers), drawableName.ToLower()); //0 = p_head, 1 = p_eyes etc -> YMTTypes.cs (it is also anchorId of prop)

                PropData propData = Props.Where(p => p.propId == enumNumber).First();
                PropDrawable prop = propData.propList.Where(p => p.propIndex == clickedIndex).First();
                int drawableToRemoveIndex = propData.propList.IndexOf(prop); 

                if (propData.propList.Count() > 1)
                {
                    if (_removeAsk.IsChecked)
                    {
                        MessageBoxResult result = MessageBox.Show(this, "Do you want to remove prop drawable " + String.Format("{0:D3}", drawableToRemoveIndex) + "?\nIt can't be restored.\n\nThis box can be disabled in options.", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.OK)
                        {
                            propData.propList.RemoveAt(drawableToRemoveIndex);
                            UpdateDrawablePropIndexes(propData);
                            SetLogMessage("Removed prop drawable (number " + String.Format("{0:D3}", drawableToRemoveIndex) + ") from " + drawableName + " prop | CHANGED ALL OTHER INDEX NUMBERS (!)");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        propData.propList.RemoveAt(drawableToRemoveIndex);
                        UpdateDrawablePropIndexes(propData);
                        SetLogMessage("Removed prop drawable (number " + String.Format("{0:D3}", drawableToRemoveIndex) + ") from " + drawableName + " prop | CHANGED ALL OTHER INDEX NUMBERS (!)");
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

            bool isChecked = (sender as MenuItem).IsChecked;
            string clicked_name = Convert.ToString((sender as MenuItem).Name);
            int enumNumber = (int)(YMTTypes.ComponentNumbers)Enum.Parse(typeof(YMTTypes.ComponentNumbers), clicked_name.ToLower());

            if (isChecked) // if true it changed from false to true, so add new component
            {
                //add first drawable(000), default compInfo, and one texture
                ComponentData newComponent = new ComponentData(clicked_name, enumNumber, 0, new ObservableCollection<ComponentDrawable>()) { compHeader = clicked_name.ToUpper() };
                ComponentInfo defaultInfo = new ComponentInfo("none", "none", new string[] { "0", "0", "0", "0", "0" }, 0, "0", "0", "PV_COMP_HEAD", 0, enumNumber, 0); // default compInfo values
                ComponentDrawable _newDrawable = new ComponentDrawable(000, 1, 1, 0, false, new ObservableCollection<ComponentTexture>(), new ObservableCollection<ComponentInfo>());
                _newDrawable.drawableTextures.Add(new ComponentTexture("a", "0"));
                _newDrawable.drawableInfo.Add(defaultInfo);

                newComponent.compList.Add(_newDrawable);

                //insert at 0, it will be sorted below
                Components.Insert(0, newComponent);

                SortComponents(Components); //sort components by compId
                UpdateAllComponentsIndexes(); //update comp.compIndex of each component - i don't remember if any functions rely on this, i think it does -_-
            }
            else // it changed from true to false, so delete clicked component
            {

                //don't check options, always ask to remove because it can be missclicked if someone forgots he have option disabled
                MessageBoxResult result = MessageBox.Show(this, "Do you want to remove WHOLE component " + clicked_name.ToUpper() + "?\n\nIt will REMOVE ALL DRAWABLES and it can't be restored.\n", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    ComponentData compData = Components.Where(c => c.compId == enumNumber).First();
                    int removeIndex = Components.IndexOf(compData);

                    Components.RemoveAt(removeIndex);

                    SortComponents(Components); //sort components by compId
                    UpdateAllComponentsIndexes(); //update comp.compIndex of each component - i don't remember if any functions rely on this, i think it does -_-
                }
                else
                {
                    return;
                }
            }
        }

        //taken and edited from https://stackoverflow.com/a/39043757
        private static ObservableCollection<ComponentData> SortComponents(ObservableCollection<ComponentData> SortComponents)
        {
            ObservableCollection<ComponentData> temp;
            temp = new ObservableCollection<ComponentData>(SortComponents.OrderBy(p => p.compId));
            SortComponents.Clear();
            foreach (var j in temp) SortComponents.Add(j);
            return SortComponents;
        }

        private static ObservableCollection<PropData> SortProps(ObservableCollection<PropData> SortProps)
        {
            ObservableCollection<PropData> temp;
            temp = new ObservableCollection<PropData>(SortProps.OrderBy(p => p.propId));
            SortProps.Clear();
            foreach (var j in temp) SortProps.Add(j);
            return SortProps;
        }

        private void PropsMenu_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as MenuItem).IsChecked;
            string clicked_name = Convert.ToString((sender as MenuItem).Name);
            int enumNumber = (int)(YMTTypes.PropNumbers)Enum.Parse(typeof(YMTTypes.PropNumbers), clicked_name.ToLower());

            if (isChecked) // if true it changed from false to true, so add new component
            {

                //currently there is no ability to change it in window, so let's hardcode hats so it scale hair properly
                //TODO: fix ^above^ because if someone add new addon hat which shouldn't scale down hair, it will anyway :(
                string[] expressionMods;
                if (clicked_name == "P_HEAD")
                {
                    expressionMods = new string[] { "-0.5", "0", "0", "0", "0" };
                }
                else
                {
                    expressionMods = new string[] { "0", "0", "0", "0", "0" };
                }

                //add first drawable(000), default compInfo, and one texture
                PropData newProp = new PropData(clicked_name, enumNumber, new ObservableCollection<PropDrawable>()) { propHeader = clicked_name.ToUpper() };
                PropDrawable newPropDrawable = new PropDrawable(0, "none", expressionMods, new ObservableCollection<PropTexture>(), "", 0, 0, enumNumber, 0, 0);
                newPropDrawable.propTextureList.Add(new PropTexture("a", "0", "0", 0, 0, 0, 255));
                newProp.propList.Add(newPropDrawable);

                //insert at 0, it will be sorted below
                Props.Insert(0, newProp);

                SortProps(Props); //sort components by compId
            }
            else // it changed from true to false, so delete clicked component
            {

                //don't check options, always ask to remove because it can be missclicked if someone forgots he have option disabled
                MessageBoxResult result = MessageBox.Show(this, "Do you want to remove WHOLE prop " + clicked_name.ToUpper() + "?\n\nIt will REMOVE ALL DRAWABLES and it can't be restored.\n", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    PropData propData = Props.Where(p => p.propId == enumNumber).First();
                    int removeIndex = Props.IndexOf(propData);

                    Props.RemoveAt(removeIndex);

                    SortProps(Props); 
                }
                else
                {
                    return;
                }
            }
        }

        private void UpdateAllComponentsIndexes()
        {
            int compsCount = MainWindow.Components.Count();
            for (int i = 0; i < compsCount; i++)
            {
                MainWindow.Components.ElementAt(i).compIndex = i;
            }
        }

        private void UpdateDrawableComponentIndexes(ComponentData component)
        {
            int drawCount = component.compList.Count();
            for (int i = 0; i < drawCount; i++)
            {
                component.compList.ElementAt(i).drawableIndex = i;
            }
        }

        private void UpdateDrawablePropIndexes(PropData prop)
        {
            int drawCount = prop.propList.Count();
            for (int i = 0; i < drawCount; i++)
            {
                prop.propList.ElementAt(i).propIndex = i;
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

            int enumNumber = (int)(YMTTypes.ComponentNumbers)Enum.Parse(typeof(YMTTypes.ComponentNumbers), compName);
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
