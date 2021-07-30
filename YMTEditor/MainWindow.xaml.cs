using CodeWalker.GameFiles;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
        public static TextBlock _logBar;
        public static MenuItem _version;
        public static MenuItem _creatureGenButton;

        public static ObservableCollection<ComponentData> Components;
        public static ObservableCollection<PropData> Props;

        public static string fullName; //whole name "mp_m_freemode_01_XXXXXX"
        public static string dlcName; //only XXXXX

        public MainWindow()
        {

            InitializeComponent();

            _componentsMenu = (MenuItem)FindName("ComponentsMenu");
            _propsMenu = (MenuItem)FindName("PropsMenu");
            _logBar = (TextBlock)FindName("logBar");
            _removeAsk = (MenuItem)FindName("RemovingAskCheck");
            _version = (MenuItem)FindName("version");
            _creatureGenButton = (MenuItem)FindName("CreatureGen");

            _removeAsk.IsChecked = Properties.Settings.Default.removeAsk;
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
            dlcName = newWindow.input;
            XMLHandler.CPedVariationInfo = dlcName;
            XMLHandler.dlcName = dlcName;

            _componentsMenu.IsEnabled = true;
            _componentsMenu.ToolTip = "Check/Uncheck components";
            _propsMenu.IsEnabled = true;
            _propsMenu.ToolTip = "Check/Uncheck props";

            _creatureGenButton.IsEnabled = true;
            _creatureGenButton.ToolTip = "Generate creaturemetadata file - only used with heels(FEET) and hats(P_HEAD)";

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
                ClearEverything(); //so if we import another file when something is imported it will clear
                
                string filename = xmlFile.FileName;
                try
                {
                    XMLHandler.LoadXML(filename);
                    _componentsMenu.IsEnabled = true;
                    _componentsMenu.ToolTip = "Check/Uncheck components";
                    _propsMenu.IsEnabled = true;
                    _propsMenu.ToolTip = "Check/Uncheck props";

                    _creatureGenButton.IsEnabled = true;
                    _creatureGenButton.ToolTip = "Generate creaturemetadata file - only used with heels(FEET) and hats(P_HEAD)";

                    SetLogMessage("Loaded XML from path: " + filename);

                    fullName = Path.GetFileNameWithoutExtension(filename); //removes .xml
                    fullName = Path.GetFileNameWithoutExtension(fullName); //removes .ymt

                    this.Title = "YMTEditor by grzybeek - editing " + fullName + ".ymt.xml";
                }
                catch (Exception)
                {
                    ClearEverything();
                    MessageBox.Show("Failed to load XML YMT, please report it!\n\nReport it on github or discord: grzybeek#9100\nPlease include XML YMT you tried to load!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private void SaveXML_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog xmlFile = new SaveFileDialog
            {
                DefaultExt = ".ymt.xml",
                Filter = "Codewalker YMT XML (*.ymt.xml)|*.ymt.xml",
                FileName = fullName
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
                string filename = ymtFile.FileName;
                byte[] ymtBytes = File.ReadAllBytes(filename);

                ClearEverything(); //so if we import another file when something is imported it will clear

                PedFile ymt = new PedFile();
                RpfFile.LoadResourceFile<PedFile>(ymt, ymtBytes, 2);
                string xml = MetaXml.GetXml(ymt.Meta);

                try
                {
                    XMLHandler.LoadXML(xml);
                    _componentsMenu.IsEnabled = true;
                    _componentsMenu.ToolTip = "Check/Uncheck components";
                    _propsMenu.IsEnabled = true;
                    _propsMenu.ToolTip = "Check/Uncheck props";

                    _creatureGenButton.IsEnabled = true;
                    _creatureGenButton.ToolTip = "Generate creaturemetadata file - only used with heels(FEET) and hats(P_HEAD)";

                    SetLogMessage("Loaded YMT from path: " + filename);

                    fullName = Path.GetFileNameWithoutExtension(filename);

                    this.Title = "YMTEditor by grzybeek - editing " + fullName + ".ymt";
                }
                catch (Exception)
                {
                    ClearEverything();
                    MessageBox.Show("Failed to load YMT, please report it!\n\nReport it on github or discord: grzybeek#9100\nPlease include YMT you tried to load!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            
            }
        }

        private void SaveYMT_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog xmlFile = new SaveFileDialog
            {
                DefaultExt = ".ymt",
                Filter = "Peds YMT (*.ymt)|*.ymt",
                FileName = fullName
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

        private void GenerateCreature_Click(object sender, RoutedEventArgs e)
        {
            if(Components.Where(c => c.compId == 6).Count() > 0 || (Props.Where(p => p.propId == 0).Count() > 0))
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
                        Meta meta = XmlMeta.GetMeta(creature);
                        byte[] newYmtBytes = ResourceBuilder.Build(meta, 2);

                        File.WriteAllBytes(filename, newYmtBytes);

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

                string[] expressionMods = new string[] { "0", "0", "0", "0", "0" };

                //some props have renderFlag = "PRF_ALPHA", others nothing - since i don't know what it does, leave it as nothing - update: now it is editable and user can easily change it
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

                string[] expressionMods = new string[] { "0", "0", "0", "0", "0" };

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
            int _newPropMask = (int) e.NewValue;

            string compName = Convert.ToString((sender as FrameworkElement).Parent.GetValue(TagProperty)).ToLower();
            int drawIndex = (int) (sender as FrameworkElement).Tag;

            int enumNumber = (int)(YMTTypes.ComponentNumbers)Enum.Parse(typeof(YMTTypes.ComponentNumbers), compName);
            int _index = Convert.ToInt32(Components.Where(z => z.compId == enumNumber).First().compIndex);

            ComponentDrawable comp = Components.ElementAt(_index).compList.Where(c => c.drawableIndex == drawIndex).First(); //get component
            comp.drawablePropMask = _newPropMask;
            string _newTexId = _newPropMask == 17 || _newPropMask == 19 || _newPropMask == 21 || _newPropMask == 25 || _newPropMask == 27 || _newPropMask == 49 ? "1" : "0";

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
                    "Discord server - discord.gg/txjjzdr \n" +
                    "forum.cfx.re acc - grzybeek\n";

                MessageBox.Show(text, "Contact info", MessageBoxButton.OK, MessageBoxImage.Question);
            }
        }

        private void TXTCombo_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            if(cmb.SelectedItem != null)
            {
                string btn = Convert.ToString(cmb.DataContext);
                string[] btn_parts = btn.Split((char)32);
                int index = Convert.ToInt32(btn_parts[0]); //index 000, 001, 002 etc

                string compName = (string)(sender as FrameworkElement).Tag; //name jbib, lowr, hand etc
                int enumNumber = (int)(YMTTypes.ComponentNumbers)Enum.Parse(typeof(YMTTypes.ComponentNumbers), compName.ToLower());
                int _index = Convert.ToInt32(Components.Where(z => z.compId == enumNumber).First().compIndex);

                string val = (string) cmb.SelectedValue.ToString().Substring(0,1); //selected option from combobox, only number

                ComponentDrawable comp = Components.ElementAt(_index).compList.Where(c => c.drawableIndex == index).First(); //get component

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

                int enumNumber = (int)(YMTTypes.PropNumbers)Enum.Parse(typeof(YMTTypes.PropNumbers), propName.ToLower());

                PropData propData = Props.Where(p => p.propId == enumNumber).First();
                PropDrawable prop = propData.propList.Where(p => p.propIndex == index).First(); //get our prop
                
                string val = (string)cmb.SelectedValue.ToString(); //selected option from combobox

                prop.propRenderFlags = val;
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

        //version compare taken and edited from https://github.com/smallo92/Ymap-YbnMover/blob/master/ymapmover/Startup.cs
        private void CheckForUpdates()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion.ToString();
            _version.Header = "Current version: " + version;

            WebClient webclient = new WebClient();
            Stream stream = webclient.OpenRead("https://raw.githubusercontent.com/grzybeek/YMTEditor/master/YMTEditor/version.txt");
            StreamReader reader = new StreamReader(stream);

            string githubVersion = reader.ReadToEnd().ToString();

            if(version != githubVersion)
            {
                MessageBoxResult result = MessageBox.Show(this, "There is new version of YMTEditor available!\nYour version: " + version + "\nAvailable version: " + githubVersion + " \n\nDo you want to download it now?\n\nClick YES to open website and close editor\nClick NO to open editor", "New version available!", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    //probably better would be auto-updater but idk how to do it yet
                    Process.Start("https://github.com/grzybeek/YMTEditor/releases");
                    Environment.Exit(0);
                }
                else if (result == MessageBoxResult.No)
                {
                    //open editor
                }
            }
            else
            {
                //good version, open editor
            }
        }

        
    }
}
