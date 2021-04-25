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
                        Console.WriteLine("adding: " + componentName.ToString());
                        MainWindow.Components.Add(componentName);
                        //Console.WriteLine(MainWindow.Components.ElementAt(compIndex).compList.Add();
                        compIndex++;
                    }
                    compId++;
                }
            }
            Console.WriteLine("<aComponentData3> start");
            int compItemIndex = 0;
            foreach (var node in xmlFile.Descendants("aComponentData3").Elements("Item"))
            {
                Console.WriteLine("<Item> start");
                ComponentData _curComp = MainWindow.Components.ElementAt(compItemIndex);
                int _curCompDrawablesCount = 0;
                int _curCompAvailTex = 0;
                int _curCompDrawableIndex = 0;

                foreach (var drawable_nodes in node.Descendants("aDrawblData3"))
                {
                    _curCompDrawablesCount = drawable_nodes.Elements("Item").Count();
                    _curCompAvailTex = drawable_nodes.Elements("Item").Elements("aTexData").Elements("Item").Count();
                }
                Console.WriteLine(_curComp.compType + " ma: " + _curCompDrawablesCount + "wariantow z łącznie: " + _curCompAvailTex + " teksturami");

                foreach (var drawable_node in node.Descendants("aDrawblData3").Elements("Item"))
                {
                    int texturesCount = drawable_node.Elements("aTexData").Elements("Item").Count();
                    Console.WriteLine("drawable numer: " + _curCompDrawableIndex + "ma tekstur: " + texturesCount);

                    _curComp.compListItemsControl.Add(new ComponentDrawable(_curCompDrawableIndex, texturesCount));
                    _curCompDrawableIndex++;
                }

                compItemIndex++;
            }

            ComponentData _cur = MainWindow.Components.ElementAt(1);
            Console.WriteLine("total:");
            Console.WriteLine(_cur.compListItemsControl.ElementAt(1).ToString());

        }

        public static void SaveXML(string filePath)
        {

        }


    }
}