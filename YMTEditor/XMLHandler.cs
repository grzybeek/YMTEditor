using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace YMTEditor
{
    class XMLHandler
    {

        public static void LoadXML(string filePath)
        {
            XDocument xmlFile = XDocument.Load(filePath);
            foreach (var node in xmlFile.Descendants("availComp"))
            {
                Console.WriteLine(node.Value);
                var availComponents = node.Value.Split((char)32); //split on space
                int compId = 0; //components id's
                int compIndex = 0; //order of our components in ymt
                foreach(var comp in availComponents)
                {
                    if(comp != "255")
                    {
                        string _name = Enum.GetName(typeof(ComponentTypes.ComponentNumbers), compId);
                        ComponentData componentName = new ComponentData(_name, compId, compIndex, new ObservableCollection<ComponentDrawable>()) { compHeader = _name.ToUpper()};
                        MainWindow.Components.Add(componentName);
                        compIndex++;
                    }
                    compId++;
                }
            }

            int compItemIndex = 0; //order of our components in ymt
            foreach (var node in xmlFile.Descendants("aComponentData3").Elements("Item"))
            {
                ComponentData _curComp = MainWindow.Components.ElementAt(compItemIndex); //current component (jbib/lowr/teef etc)
                int _curCompDrawablesCount = 0; //count how many component has variations (000, 001, 002, etc)
                int _curCompAvailTex = 0; // not used by game probably, total amount of textures component has (numAvailTex)
                int _curCompDrawableIndex = 0; //current drawable index in component (000, 001, 002, etc)

                foreach (var drawable_nodes in node.Descendants("aDrawblData3"))
                {
                    _curCompDrawablesCount = drawable_nodes.Elements("Item").Count();
                    _curCompAvailTex = drawable_nodes.Elements("Item").Elements("aTexData").Elements("Item").Count();
                }

                foreach (var drawable_node in node.Descendants("aDrawblData3").Elements("Item"))
                {
                    int texturesCount = drawable_node.Elements("aTexData").Elements("Item").Count();

                    _curComp.compListItemsControl.Add(new ComponentDrawable(_curCompDrawableIndex, texturesCount));
                    _curCompDrawableIndex++;
                }

                compItemIndex++;
            }
        }

        public static void SaveXML(string filePath)
        {

        }

 
    }
}