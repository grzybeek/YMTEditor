using CodeWalker.GameFiles;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace YMTEditor
{
    public partial class MainWindow : Window
    {
        public static MenuItem _componentsMenu;
        public static MenuItem _propsMenu;
        public static MenuItem _removeAsk;
        public static MenuItem _enableLogs;
        public static TextBlock _logBar;
        public static MenuItem _version;
        public static MenuItem _creatureGenButton;
        public static MenuItem _renameButton;

        public static ObservableCollection<ComponentData> Components;
        public static ObservableCollection<PropData> Props;

        public static string fullName; //whole name "mp_m_freemode_01_XXXXXX"
        public static string dlcName; //only XXXXX

        private string openedPath;

        public MainWindow()
        {

            InitializeComponent();

            _componentsMenu = (MenuItem)FindName("ComponentsMenu");
            _propsMenu = (MenuItem)FindName("PropsMenu");
            _logBar = (TextBlock)FindName("logBar");
            _removeAsk = (MenuItem)FindName("RemovingAskCheck");
            _enableLogs = (MenuItem)FindName("EnableLogs");
            _version = (MenuItem)FindName("version");
            _creatureGenButton = (MenuItem)FindName("CreatureGen");
            _renameButton = (MenuItem)FindName("RenameBtn");

            _removeAsk.IsChecked = Properties.Settings.Default.removeAsk;
            _enableLogs.IsChecked = Properties.Settings.Default.enableLogs;
            SetLogMessage("Loaded options");

            Components = new ObservableCollection<ComponentData>();
            ComponentsItemsControl.ItemsSource = Components;

            Props = new ObservableCollection<PropData>();
            PropsItemsControl.ItemsSource = Props;

            CheckForUpdates();
        }

        private void OpenNEW_Click(object sender, RoutedEventArgs e)
        {
            ClearEverything(); //so if we import another file when something is imported it will clear

            NewYMTWindow newWindow = new NewYMTWindow();
            newWindow.ShowDialog();

            fullName = newWindow.fullName;

            if(newWindow.ped.IsChecked == true)
            {
                dlcName = ""; //non-mp peds doesn't have dlcName set
            }
            else
            {
                dlcName = newWindow.input;
            }
            
            XMLHandler.CPedVariationInfo = dlcName;
            XMLHandler.dlcName = dlcName;

            EnableMenus();

            SetLogMessage("Created new file");

            this.Title = "YMTEditor by grzybeek - editing " + fullName + ".ymt";
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
                OpenFileFunction(xmlFile.FileName);
            }
        }

        private void SaveXML_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog xmlFile = new SaveFileDialog
            {
                DefaultExt = ".ymt.xml",
                Filter = "Codewalker YMT XML (*.ymt.xml)|*.ymt.xml",
                FileName = fullName,
                InitialDirectory = openedPath
            };
            bool? result = xmlFile.ShowDialog();
            if (result == true)
            {
                try
                {
                    string filename = xmlFile.FileName;
                    XMLHandler.SaveXML(filename);
                    SetLogMessage("Saved XML to path: " + filename);
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to save XML YMT, please report it!\n\nReport it on github or discord: grzybeek#9100\nPlease include XML YMT you tried to save!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

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
                OpenFileFunction(ymtFile.FileName);
            }
        }

        private void SaveYMT_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog xmlFile = new SaveFileDialog
            {
                DefaultExt = ".ymt",
                Filter = "Peds YMT (*.ymt)|*.ymt",
                FileName = fullName,
                InitialDirectory = openedPath
            };
            bool? result = xmlFile.ShowDialog();
            if (result == true)
            {
                try
                {
                    string filename = xmlFile.FileName;
                    System.Xml.XmlDocument newXml = XMLHandler.SaveYMT(filename);
                    
                    Meta meta = XmlMeta.GetMeta(newXml);
                    byte[] newYmtBytes = ResourceBuilder.Build(meta, 2);

                    File.WriteAllBytes(filename, newYmtBytes);

                    SetLogMessage("Saved YMT to path: " + filename);

                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to save YMT, please report it!\n\nReport it on github or discord: grzybeek#9100\nPlease include YMT you tried to save!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            
            }
        }

        private void DragAndDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var file = files[0];
                string extension = Path.GetExtension(file);
                if(extension == ".ymt" || extension == ".xml")
                {
                    OpenFileFunction(file);
                }
            }
        }

        private void OpenFileFunction(String filePath)
        {
            string extension = Path.GetExtension(filePath);
            if (extension == ".ymt")
            {
                byte[] ymtBytes = File.ReadAllBytes(filePath);

                ClearEverything(); //so if we import another file when something is imported it will clear

                PedFile ymt = new PedFile();
                RpfFile.LoadResourceFile<PedFile>(ymt, ymtBytes, 2);
                string xml = MetaXml.GetXml(ymt.Meta);

                try
                {
                    XMLHandler.LoadXML(xml);
                    EnableMenus();

                    SetLogMessage("Loaded YMT from path: " + filePath);

                    fullName = Path.GetFileNameWithoutExtension(filePath);
                    openedPath = Path.GetDirectoryName(filePath);

                    this.Title = "YMTEditor by grzybeek - editing " + fullName + ".ymt";
                }
                catch (Exception)
                {
                    ClearEverything();
                    MessageBox.Show("Failed to load YMT, please report it!\n\nReport it on github or discord: grzybeek#9100\nPlease include YMT you tried to load!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if(extension == ".xml")
            {
                ClearEverything(); //so if we import another file when something is imported it will clear

                try
                {
                    XMLHandler.LoadXML(filePath);

                    SetLogMessage("Loaded XML from path: " + filePath);

                    fullName = Path.GetFileNameWithoutExtension(filePath); //removes .xml
                    fullName = Path.GetFileNameWithoutExtension(fullName); //removes .ymt
                    openedPath = Path.GetDirectoryName(filePath);

                    this.Title = "YMTEditor by grzybeek - editing " + fullName + ".ymt.xml";
                }
                catch (Exception)
                {
                    ClearEverything();
                    MessageBox.Show("Failed to load XML YMT, please report it!\n\nReport it on github or discord: grzybeek#9100\nPlease include XML YMT you tried to load!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GenerateCreature_Click(object sender, RoutedEventArgs e)
        {
            if(Components.Where(c => c.compId == 6).Count() > 0 || (Props.Where(p => p.propAnchorId == 0).Count() > 0))
            {
                SaveFileDialog CreatureMetadataFile = new SaveFileDialog
                {
                    DefaultExt = ".ymt",
                    Filter = "CreatureMetadata YMT (*.ymt)|*.ymt",
                    FileName = "mp_creaturemetadata_" + (dlcName.Length > 0 ? dlcName : fullName)
                };
                bool? result = CreatureMetadataFile.ShowDialog();
                if (result == true)
                {
                    try
                    {
                        string filename = CreatureMetadataFile.FileName;
                    
                        System.Xml.XmlDocument creature = XMLHandler.SaveCreature(filename);
                        RbfFile rbf = XmlRbf.GetRbf(creature);

                        File.WriteAllBytes(filename, rbf.Save());

                        SetLogMessage("Saved CreatureMetadata to path: " + filename);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Failed to generate CreatureMetadata, please report it!\n\nReport it on github or discord: grzybeek#9100\nPlease include YMT you tried to generate!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
            }
            else
            {
                MessageBox.Show("You need to add FEET or P_HEAD first!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            if (dlcName.Length > 0)
            {
                RenameWindow renameWindow = new RenameWindow(fullName, dlcName);
                renameWindow.ShowDialog();

                string _newDlcName = renameWindow.input;
                string _newFullName = renameWindow.NewfullName;

                XMLHandler.CPedVariationInfo = _newDlcName;
                XMLHandler.dlcName = _newDlcName;
                fullName = _newFullName;
                dlcName = _newDlcName;

                SetLogMessage("Renamed to " + _newFullName);
                this.Title = "YMTEditor by grzybeek - editing " + _newFullName + ".ymt";
            }
            else
            {
                MessageBox.Show("Your dlc name is empty!\nYou can't rename it in editor. Please create new ymt instead.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
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
                
                if (txtCount >= 26)
                {
                    MessageBox.Show("Can't add more textures, limit is 26! (a-z)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ComponentData compData = Components.Where(z => z.compType == drawableName.ToLower()).First(); //component index in our ymt

                ComponentDrawable comp = compData.compList.Where(c => c.drawableIndex == index).First(); //get component
                int drawableToAddTexture = compData.compList.IndexOf(comp); //get index of our clicked comonent in collection
                int _FirstTexId = compData.compList.ElementAt(drawableToAddTexture).drawableTextures.First().textureTexId; //get texId of first texture to keep same texid

                string txtLetter = XMLHandler.Number2String(txtCount, false);
                comp.drawableTextures.Add(new ComponentTexture(txtLetter, _FirstTexId));
                comp.drawableTextureCount++;
                SetLogMessage("Added new texture (letter " + txtLetter + ") to drawable " + String.Format("{0:D3}", drawableToAddTexture) + " in " + drawableName + " component | Count: " + comp.drawableTextureCount + " / 26");

            }
            else //else add new drawable number
            {

                ComponentData compData = Components.Where(z => z.compType == btn.ToLower()).First(); //index of our component
                int drawIndex = compData.compList.Count(); //drawable index (000, 001, 002 etc)

                if (drawIndex == 128)
                {
                    MessageBox.Show("More than 127 drawables in component might cause issues (people may see different models)", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                ComponentDrawable _newDrawable = new ComponentDrawable(compData.compId, drawIndex);
                compData.compList.Add(_newDrawable);

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
                
                string propName = Convert.ToString((sender as Button).Tag); //drawable we are adding txt (p_head, p_eyes etc)
                
                PropDrawable prop = Props.Where(p => p.propType == propName.ToLower()).First().propList.Where(p => p.propIndex == index).First(); // get prop
                int txtCount = prop.propTextureCount;

                if (txtCount >= 26)
                {
                    MessageBox.Show("Can't add more textures, limit is 26! (a-z)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string txtLetter = XMLHandler.Number2String(txtCount, false);
                prop.propTextureList.Add(new PropTexture(txtLetter, "0", "0", txtCount, 0, 0, 255));
                prop.propTextureCount++;

                SetLogMessage("Added new texture (letter " + txtLetter + ") to prop " + String.Format("{0:D3}", prop.propIndex) + " in " + propName + " prop | Count: " + prop.propTextureCount + " / 26");
            }
            else //else add new drawable number
            {

                string propName = btn; //drawable we are adding txt (p_head, p_eyes etc)
                int _index = (int)(YMTTypes.PropNumbers)Enum.Parse(typeof(YMTTypes.PropNumbers), propName.ToLower()); //0 = p_head, 1 = p_eyes etc -> YMTTypes.cs (it is also anchorId of prop)

                PropData prop = Props.Where(p => p.propType == propName.ToLower()).First(); //get prop
                int drawIndex = prop.propList.Count;

                string[] expressionMods = new string[] { "0", "0", "0", "0", "0" };

                //some props have renderFlag = "PRF_ALPHA", others nothing - since i don't know what it does, leave it as nothing - update: now it is editable and user can easily change it
                PropDrawable _newPropDrawable = new PropDrawable(drawIndex, 1, "none", expressionMods, new ObservableCollection<PropTexture>(), "", 0, 0, prop.propAnchorId, drawIndex, 0);
                _newPropDrawable.propTextureList.Add(new PropTexture("a", "0", "0", 0, 0, 0, 255));

                prop.propList.Add(_newPropDrawable);

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

                ComponentData compdata = Components.Where(z => z.compType == drawableName.ToLower()).First(); //component data

                ComponentDrawable comp = compdata.compList.Where(c => c.drawableIndex == drawableIndex).First(); //get component
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
                ComponentData compdata = Components.Where(z => z.compType == drawableName.ToLower()).First();

                ComponentDrawable comp = compdata.compList.Where(c => c.drawableIndex == clickedIndex).First();
                int drawableToRemoveIndex = compdata.compList.IndexOf(comp);

                if (compdata.compList.Count() > 1)
                {
                    if (_removeAsk.IsChecked)
                    {
                        MessageBoxResult result = MessageBox.Show(this, "Do you want to remove drawable " + String.Format("{0:D3}", drawableToRemoveIndex) + "?\nIt can't be restored.\n\nThis box can be disabled in options.", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.OK)
                        {
                            compdata.compList.RemoveAt(drawableToRemoveIndex);
                            UpdateDrawableComponentIndexes(compdata);
                            SetLogMessage("Removed drawable (number " + String.Format("{0:D3}", drawableToRemoveIndex) + ") from " + drawableName + " component | CHANGED ALL OTHER INDEX NUMBERS (!)");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        compdata.compList.RemoveAt(drawableToRemoveIndex);
                        UpdateDrawableComponentIndexes(compdata);
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

                PropDrawable prop = Props.Where(p => p.propType == drawableName.ToLower()).First().propList.Where(p => p.propIndex == drawableIndex).First(); //get prop
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

                PropData propData = Props.Where(p => p.propType == drawableName.ToLower()).First();
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
                ComponentData newComponent = new ComponentData(clicked_name, enumNumber, 0, new ObservableCollection<ComponentDrawable>());
                ComponentDrawable _newDrawable = new ComponentDrawable(newComponent.compId, newComponent.compList.Count);
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
            temp = new ObservableCollection<PropData>(SortProps.OrderBy(p => p.propAnchorId));
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
                //add first drawable(000), default compInfo, and one texture
                PropData newProp = new PropData(clicked_name, enumNumber, new ObservableCollection<PropDrawable>());
                PropDrawable newPropDrawable = new PropDrawable(0, 1, "none", new string[] { "0", "0", "0", "0", "0" }, new ObservableCollection<PropTexture>(), "", 0, 0, enumNumber, 0, 0);
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
                    PropData propData = Props.Where(p => p.propAnchorId == enumNumber).First();
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
            if (_enableLogs.IsChecked)
            {
                _logBar.Text = "Logs: " + message;
            }
            else
            {
                _logBar.Text = "";
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void numAlternatives_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            int _newNumAlternatives = (int)e.NewValue;

            string compName = Convert.ToString((sender as FrameworkElement).Parent.GetValue(TagProperty)).ToLower();
            int drawIndex = (int)(sender as FrameworkElement).Tag;

            int enumNumber = (int)(YMTTypes.ComponentNumbers)Enum.Parse(typeof(YMTTypes.ComponentNumbers), compName);
            int _index = Convert.ToInt32(Components.Where(z => z.compId == enumNumber).First().compIndex);

            ComponentDrawable comp = Components.ElementAt(_index).compList.Where(c => c.drawableIndex == drawIndex).First(); //get component
            comp.drawableAlternatives = _newNumAlternatives;

            SetLogMessage("numAlternatives changed to: " + _newNumAlternatives);
        }

        private void propMask_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (isSetFromTxtDropdown) return;

            int _newPropMask = (int) e.NewValue;

            string compName = Convert.ToString((sender as FrameworkElement).Parent.GetValue(TagProperty)).ToLower();
            int drawIndex = (int) (sender as FrameworkElement).Tag;

            ComponentData compData = Components.Where(z => z.compType == compName).First();
            ComponentDrawable comp = compData.compList.Where(c => c.drawableIndex == drawIndex).First(); //get component
            comp.drawablePropMask = _newPropMask;
            int _newTexId = _newPropMask == 17 || _newPropMask == 19 || _newPropMask == 21 || _newPropMask == 25 || _newPropMask == 27 || _newPropMask == 49 ? 1 : 0;

            foreach (var txt in comp.drawableTextures)
            {
                txt.textureTexId = _newTexId;
            }

            comp.dTexturesTexId = Convert.ToInt32(_newTexId); //used to set proper dropdown value

            switch (_newPropMask)
            {
                case 17:
                case 19:
                case 21:
                case 25: // 
                case 27: // found in some gta ymt's
                case 49: //
                    SetLogMessage("propMask changed to: " + _newPropMask + " | Your .ydd model should end with _r, and .ytd texture with _whi/bla/chi/lat/etc...");
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

        private void EnableLogs_Click(object sender, RoutedEventArgs e)
        {
            bool? value = (sender as MenuItem).IsChecked;

            Properties.Settings.Default.enableLogs = (bool)value;
            Properties.Settings.Default.Save();
            SetLogMessage("Saved options, will be restored when opening the program");
        }

        private void HelpBtn_Click(object sender, RoutedEventArgs e)
        {
            string clicked_btn = (sender as MenuItem).Name;
            if(clicked_btn == "tutorial")
            {
                Process.Start("https://forum.cfx.re/t/how-to-stream-clothes-and-props-as-addons-for-mp-freemode-models/3345474");
            }
            else if(clicked_btn == "tutorial_heels")
            {
                Process.Start("https://forum.cfx.re/t/how-to-create-addon-heels-or-hide-hair-with-addon-hat/4209989");
            }
            else if(clicked_btn == "help")
            {
                string text = "Small explanation of how everything works, check other buttons for better explanation:\n\n\n" +
                    "Component = Each have own number and \"slot\" on our ped, it requires skinning/rigging. (head, berd, hair, uppr, lowr, hand, feet, teef, accs, task, decl, jbib)\n\n" +
                    "Prop = Each have own number and \"slot\" on our ped model, it doesn't need skinning/rigging. (p_head, p_eyes, p_ears, p_lwrist, p_rwrist)\n\n" +
                    "Drawable = Each .ydd file is different drawable, each have different number starting at 000, up to 127 (not sure about max number)\n\n" +
                    "Texture = Each .ytd file is a texture, one drawable can have up to 26 textures (a-z)\n\n" +
                    "propMask = Determines if our skin should be _u or _r - what is that is explained later\n\n" +
                    "texture texId = it defines 'race' for our skin\n\n" +
                    "numAlternatives = it defines how much alternate models our drawable use (it has _1 _2 _3 etc. after _u/_r in name - requires alternatevariations.meta file to work)\n\n" +
                    "clothData = true/false if our drawable is using .yld file - mostly false\n\n" +
                    "Component/prop properties explained in other buttons";

                MessageBox.Show(text, "General help", MessageBoxButton.OK, MessageBoxImage.Question);
            }
            else if(clicked_btn == "components")
            {
                string text =
                    "Components:\n" +
                    "0 - head (head models)\n" +
                    "1 - berd (masks, beards)\n" +
                    "2 - hair (hair models)\n" +
                    "3 - uppr (torso, body, arms)\n" +
                    "4 - lowr (pants, legs)\n" +
                    "5 - hand (backpacks, bags)\n" +
                    "6 - feet (feets, shoes)\n" +
                    "7 - teef (tie, scarf, necklace)\n" +
                    "8 - accs (inner top models, under t-shirt, etc)\n" +
                    "9 - task (vests)\n" +
                    "10 - decl (stickers, decals)\n" +
                    "11 - jbib (outer top models, hoodies, above t-shirt, etc)\n\n" +
                    "Component suffix (_u or _r): \n" +
                    "To set your model _u or _r, you have to change propMask value, check other help button for it\n" +
                    "_u = universal\n" +
                    "_r = race\n\n" +
                    "Component textures: \n" +
                    "If your model is _u, you have to use suffix _uni in your texture name\n" +
                    "If your model is _r, you have to use suffix _whi/bla/chi/etc... in your texture name\n\n" +
                    "texId values: \n" +
                    "0 = _uni (universal) \n" +
                    "1 = _whi (white) \n" +
                    "2 = _bla (black) \n" +
                    "3 = _chi (chinese) \n" +
                    "4 = _lat (latino) \n" +
                    "5 = _ara (arabic) \n" +
                    "6 = _bal (baltic) \n" +
                    "7 = _jam (jamaican) \n" +
                    "8 = _kor (korean) \n" +
                    "9 = _ita (italian) \n" +
                    "10 = _pak (pakistani - resembles indians mostly, like shopkeepers) \n" +
                    "";
                   
                MessageBox.Show(text, "Components help", MessageBoxButton.OK, MessageBoxImage.Question);
            }
            else if(clicked_btn == "props")
            {
                string text =
                    "Props - there are more known, but game only uses these: \n" +
                    "0 - p_head (hats)\n" +
                    "1 - p_eyes (glasses)\n" +
                    "2 - p_ears (earrings)\n" +
                    "6 - p_lwrist (watches, bracelets on left wrist)\n" +
                    "7 - p_rwrist (watches, bracelets on right wrist)\n";

                MessageBox.Show(text, "Props help", MessageBoxButton.OK, MessageBoxImage.Question);
            }
            else if(clicked_btn == "propmask")
            {
                string text =
                    "To set your model _u or _r you have to change propMask value: \n\n" +
                    "propMask values for _r: 17, 19, 21, 25, 27, 49\n" +
                    "and there might be more, usage of different numbers is not known.\n\n" +
                    "Simply for _u use anything else, most common is value: 1\n\n\n" +

                    "If you are using _r, your .ydd model should contain skin/body\n";


                MessageBox.Show(text, "Propmask help", MessageBoxButton.OK, MessageBoxImage.Question);
            }
            else if (clicked_btn == "compproperties")
            {
                string text =
                    "hash_2FD08CEF - usage unknown, something related to audio (game.dat151.rel file) \n\n" +
                    "hash_FC507D28 - usage unknown \n\n" +
                    "hash_07AE529D - expressionMods(?) - used to make heels for example - currently disabled in editor because i don't know how to do it well :( \n\n" +
                    "flags - i don't know usage of it :( \n\n" +
                    "inclusions - i don't know usage of it :( \n\n" +
                    "exclusions - i don't know usage of it :( \n\n" +
                    "hash_6032815C - usage unknown, it's always PV_COMP_HEAD i think, so it's disabled in editor \n\n" +
                    "hash_7E103C8B - usage unknown\n\n\n" +

                    "If you know what something of these do, please contact me!\n";


                MessageBox.Show(text, "Component properties help", MessageBoxButton.OK, MessageBoxImage.Question);
            }
            else if (clicked_btn == "propproperties")
            {
                string text =
                    "AudioId - something related to audio, as name says, idk much about it \n\n" +
                    "ExpressionMods - used to hats hide hair for example - it requires more files than only this value - currently disabled in editor because i don't know how to do it well :( \n\n" +
                    "RenderFlags - i don't know usage of it :( \n\n" +
                    "PropFlags - i don't know usage of it :( \n\n" +
                    "Flags - i don't know usage of it :( \n\n" +
                    "hash_AC887A91 - i don't know usage of it :( \n\n" +

                    "If you know what something of these do, please contact me!\n";


                MessageBox.Show(text, "Prop properties help", MessageBoxButton.OK, MessageBoxImage.Question);
            }
            else if (clicked_btn == "creaturemetadata")
            {
                string text =
                    "This file is required for addon heels and addon hat (so it does hide hair under hat) \n\n" +
                    "Everything is explained in second tutorial! \n\n";

                MessageBox.Show(text, "Prop properties help", MessageBoxButton.OK, MessageBoxImage.Question);
            }
            else if(clicked_btn == "contact")
            {
                string text =
                    "You can find me here: \n\n" +
                    "Github - grzybeek \n" +
                    "Discord acc - grzybeek#9100 \n" +
                    "Discord server - discord.gg/xUXbrupFhN \n" +
                    "forum.cfx.re acc - grzybeek\n";

                MessageBox.Show(text, "Contact info", MessageBoxButton.OK, MessageBoxImage.Question);
            }
        }

        bool isSetFromTxtDropdown = false;
        private void TXTCombo_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            if(cmb.SelectedItem != null)
            {
                isSetFromTxtDropdown = true;
                string btn = Convert.ToString(cmb.DataContext);
                string[] btn_parts = btn.Split((char)32);
                int index = Convert.ToInt32(btn_parts[0]); //index 000, 001, 002 etc
                int val = cmb.SelectedIndex;

                string compName = (string)(sender as FrameworkElement).Tag; //name jbib, lowr, hand etc
                ComponentData compData = Components.Where(z => z.compType == compName.ToLower()).First();
                ComponentDrawable comp = compData.compList.Where(c => c.drawableIndex == index).First(); //get component

                foreach (var txt in comp.drawableTextures)
                {
                    txt.textureTexId = val;
                }

                if(Convert.ToInt32(val) >= 1) //if we change dropdown menu with texid, update propmask also
                {
                    comp.drawablePropMask = 17; //_r _whi/bla/chi/lat/etc propmask
                }
                else
                {
                    comp.drawablePropMask = 1; //_u _uni propmask
                }
                isSetFromTxtDropdown = false;
            }
        }

        private void RenderFlagsCombo_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            if (cmb.SelectedItem != null)
            {
                string btn = Convert.ToString(cmb.DataContext);
                string propName = Convert.ToString(cmb.Tag); //name p_eyes, p_head, etc
                string[] btn_parts = btn.Split((char)32);
                int index = Convert.ToInt32(btn_parts[0]); //index 000, 001, 002 etc

                PropData propData = Props.Where(p => p.propType == propName.ToLower()).First();
                PropDrawable prop = propData.propList.Where(p => p.propIndex == index).First(); //get our prop
                
                string val = (string)cmb.SelectedValue.ToString(); //selected option from combobox

                prop.propRenderFlags = val;
            }
        }

        private void ClothCombo_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            if (cmb.SelectedItem != null)
            {
                string btn = Convert.ToString(cmb.DataContext);
                string[] btn_parts = btn.Split((char)32);
                int index = Convert.ToInt32(btn_parts[0]); //index 000, 001, 002 etc

                string compName = (string)(sender as FrameworkElement).Tag; //name jbib, lowr, hand etc
                int _index = Convert.ToInt32(Components.Where(z => z.compType == compName.ToLower()).First().compIndex);

                bool val = Convert.ToBoolean(cmb.SelectedValue.ToString()); //selected option from combobox

                ComponentDrawable comp = Components.ElementAt(_index).compList.Where(c => c.drawableIndex == index).First(); //get component
                comp.drawableHasCloth = val;
            }
        }

        private void ClearEverything()
        {
            var allItemsComps = _componentsMenu.Items.Cast<MenuItem>().ToArray();
            var allItemsProps = _propsMenu.Items.Cast<MenuItem>().ToArray();

            foreach (var item in allItemsComps)
            {
                item.IsChecked = false;
            }

            foreach (var item in allItemsProps)
            {
                item.IsChecked = false;
            }

            Components.Clear();
            Props.Clear(); 
        }

        private void EnableMenus()
        {
            _renameButton.IsEnabled = true;
            _renameButton.ToolTip = "Change current dlc name";

            _componentsMenu.IsEnabled = true;
            _componentsMenu.ToolTip = "Check/Uncheck components";
            _propsMenu.IsEnabled = true;
            _propsMenu.ToolTip = "Check/Uncheck props";

            _creatureGenButton.IsEnabled = true;
            _creatureGenButton.ToolTip = "Generate creaturemetadata file - only used with heels(FEET) and hats(P_HEAD)";
        }

        // https://stackoverflow.com/a/60736900 idk if that's a good way of checking
        private bool CheckInternetConnection()
        {
            if (NetworkInterface.GetIsNetworkAvailable() && new Ping().Send(new IPAddress(new byte[] { 8, 8, 8, 8 }), 2000).Status == IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //version compare taken and edited from https://github.com/smallo92/Ymap-YbnMover/blob/master/ymapmover/Startup.cs
        private void CheckForUpdates()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion.ToString();
            _version.Header = "Version: " + version;

            if (CheckInternetConnection())
            {
                WebClient webclient = new WebClient();
                Stream stream = webclient.OpenRead("https://raw.githubusercontent.com/grzybeek/YMTEditor/master/YMTEditor/version.txt");
                StreamReader reader = new StreamReader(stream);

                string githubVersion = reader.ReadToEnd().ToString();

                if (version != githubVersion)
                {
                    _version.Header += " (Update available)";
                    _version.IsEnabled = true;
                }
            }
            else
            {
                //no internet, open editor
            }
        }

        private void NewVersion_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/grzybeek/YMTEditor/releases");
        }

        private void Button_Click_AddMultipleDrawables(object sender, RoutedEventArgs e)
        {
            AddMultipleDrawables window;
            string btn = Convert.ToString((sender as Button).DataContext);

            if(btn.Substring(0,1) == "P")
            {
                PropData curProp = Props.Where(p => p.propType == btn.ToLower()).First();

                window = new AddMultipleDrawables(curProp);
            } 
            else
            {
                ComponentData curComp = Components.Where(z => z.compType == btn.ToLower()).First();

                window = new AddMultipleDrawables(curComp);
            }

            window.ShowDialog();
        }
    }
}
