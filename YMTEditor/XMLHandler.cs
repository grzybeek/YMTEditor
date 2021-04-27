using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace YMTEditor
{
    class XMLHandler
    {
        private static string CPedVariationInfo = "TODO"; //TODO: if not set it will be "TODO", implement changing it in main window (only for new files)
        private static bool bHasTexVariations = false;
        private static bool bHasDrawblVariations = false;
        private static bool bHasLowLODs = false;
        private static bool bIsSuperLOD = false;
        private static string dlcName = "TODO";

        public static void LoadXML(string filePath)
        {
            XDocument xmlFile = XDocument.Load(filePath);
            string usedPath = filePath;

            CPedVariationInfo = xmlFile.Element("CPedVariationInfo").FirstAttribute.Value.ToString();
            bHasTexVariations = Convert.ToBoolean(xmlFile.Elements("CPedVariationInfo").Elements("bHasTexVariations").First().FirstAttribute.Value);
            bHasDrawblVariations = Convert.ToBoolean(xmlFile.Elements("CPedVariationInfo").Elements("bHasDrawblVariations").First().FirstAttribute.Value);
            bHasLowLODs = Convert.ToBoolean(xmlFile.Elements("CPedVariationInfo").Elements("bHasLowLODs").First().FirstAttribute.Value);
            bIsSuperLOD = Convert.ToBoolean(xmlFile.Elements("CPedVariationInfo").Elements("bIsSuperLOD").First().FirstAttribute.Value);
            dlcName = xmlFile.Elements("CPedVariationInfo").Elements("dlcName").First().Value.ToString();

            Console.WriteLine(CPedVariationInfo);

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
                    int drawablePropMask = Convert.ToInt16(drawable_node.Elements("propMask").First().FirstAttribute.Value);
                    int drawableNumAlternatives = Convert.ToInt16(drawable_node.Elements("numAlternatives").First().FirstAttribute.Value);
                    bool drawableCloth = Convert.ToBoolean(drawable_node.Elements("clothData").Elements("ownsCloth").First().FirstAttribute.Value.ToString());

                    int textureIndex = 0;
                    ComponentDrawable _curDrawable = new ComponentDrawable(_curCompDrawableIndex, texturesCount, drawablePropMask, drawableNumAlternatives, drawableCloth, new ObservableCollection<ComponentTexture>());
                    _curComp.compList.Add(_curDrawable);
                    foreach (var texture_node in drawable_node.Descendants("aTexData").Elements("Item"))
                    {
                        string texId = texture_node.Element("texId").FirstAttribute.Value;
                        string texLetter = Number2String(textureIndex, false);
                        _curDrawable.drawableTextures.Add(new ComponentTexture(texLetter, texId));
                        textureIndex++;
                    }
                    _curCompDrawableIndex++;
                }
                compItemIndex++;
            }
        }

        private static XElement XML_Schema(string filePath)
        {
            // TOP OF FILE || START -> CPedVariationInfo
            XElement xml = new XElement("CPedVariationInfo", new XAttribute("name", CPedVariationInfo));
            xml.Add(new XElement("bHasTexVariations", new XAttribute("value", bHasTexVariations)));
            xml.Add(new XElement("bHasDrawblVariations", new XAttribute("value", bHasDrawblVariations)));
            xml.Add(new XElement("bHasLowLODs", new XAttribute("value", bHasLowLODs)));
            xml.Add(new XElement("bIsSuperLOD", new XAttribute("value", bIsSuperLOD)));

            (int[] availComp, int availCompCount) = generateAvailComp();
            xml.Add(new XElement("availComp", String.Join(" ", availComp)));

            // START -> aComponentData3
            XElement components = new XElement("aComponentData3", new XAttribute("itemType", "CPVComponentData"));
            for (int i = 0; i < availCompCount; i++)
            {
                XElement compIndex = new XElement("Item");
                compIndex.Add(new XElement("numAvailTex", new XAttribute("value", countAvailTex(i))));
                XElement drawblData = new XElement("aDrawblData3", new XAttribute("itemType", "CPVDrawblData"));
                compIndex.Add(drawblData);

                for (int j = 0; j < MainWindow.Components.ElementAt(i).compList.Count(); j++)
                {
                    XElement drawblDataIndex = new XElement("Item");
                    int _propMask = MainWindow.Components.ElementAt(i).compList.ElementAt(j).drawablePropMask;
                    int _numAlternatives = MainWindow.Components.ElementAt(i).compList.ElementAt(j).drawableAlternatives;
                    drawblDataIndex.Add(new XElement("propMask", new XAttribute("value", _propMask)));
                    drawblDataIndex.Add(new XElement("numAlternatives", new XAttribute("value", _numAlternatives)));
                    XElement TexDataIndex = new XElement("aTexData", new XAttribute("itemType", "CPVTextureData"));
                    drawblDataIndex.Add(TexDataIndex);

                    for (int k = 0; k < MainWindow.Components.ElementAt(i).compList.ElementAt(j).drawableTextures.Count(); k++)
                    {
                        XElement TexDataItem = new XElement("Item");
                        int _texId = _propMask == 17 || _propMask == 19 || _propMask == 21 ? 1 : 0; // if propMask 17/19/21 -> texId = 1, otherwise texId = 0 --- there might be other values as well but those are most used
                        TexDataItem.Add(new XElement("texId", new XAttribute("value", _texId))); //I guess it doesn't need functionality to manually edit it (?)
                        TexDataItem.Add(new XElement("distribution", new XAttribute("value", 255)));
                        TexDataIndex.Add(TexDataItem);
                    }

                    XElement clothDataItem = new XElement("clothData");
                    bool _clothData = MainWindow.Components.ElementAt(i).compList.ElementAt(j).drawableHasCloth;
                    clothDataItem.Add(new XElement("ownsCloth", new XAttribute("value", _clothData)));
                    drawblDataIndex.Add(clothDataItem);
                    drawblData.Add(drawblDataIndex);
                }
                components.Add(compIndex);
            }
            xml.Add(components);
            // END -> aComponentData3

            // START -> aSelectionSets
            xml.Add(new XElement("aSelectionSets", new XAttribute("itemType", "CPedSelectionSet"))); //never seen it used anywhere i think(?)
            // END -> aSelectionSets

            // START -> compInfos
            XElement compInfo = new XElement("compInfos", new XAttribute("itemType", "CComponentInfo")); //not sure if game needs compInfos without any values set
            foreach (var c in MainWindow.Components)
            {
                foreach (var comp in c.compList)
                {
                    XElement compInfoItem = new XElement("Item");
                    compInfoItem.Add(new XElement("hash_2FD08CEF", "none")); //not sure what it does, currently it won't save if there is something set
                    compInfoItem.Add(new XElement("hash_FC507D28", "none")); //not sure what it does, currently it won't save if there is something set
                    compInfoItem.Add(new XElement("hash_07AE529D", "0 0 0 0 0"));  //TODO: implement adding "high heels" value in main window for shoes and something else for different components(?)
                    compInfoItem.Add(new XElement("flags", new XAttribute("value", 0))); //not sure what it does, currently it won't save if there is something set
                    compInfoItem.Add(new XElement("inclusions", "0")); //not sure what it does, currently it won't save if there is something set
                    compInfoItem.Add(new XElement("exclusions", "0")); //not sure what it does, currently it won't save if there is something set
                    compInfoItem.Add(new XElement("hash_6032815C", "PV_COMP_HEAD")); //probably everything has PV_COMP_HEAD (?)
                    compInfoItem.Add(new XElement("hash_7E103C8B", new XAttribute("value", 0))); //not sure what it does, currently it won't save if there is something set
                    compInfoItem.Add(new XElement("hash_D12F579D", new XAttribute("value", c.compId)));
                    compInfoItem.Add(new XElement("hash_FA1F27BF", new XAttribute("value", comp.drawableIndex)));
                    compInfo.Add(compInfoItem);
                }
            }
            xml.Add(compInfo);
            // END -> compInfos

            // START -> propInfo
            XElement propInfo = new XElement("propInfo"); //TODO: implement editing props in main window, for now it will remove all props from .YMT (!)
            propInfo.Add(new XElement("numAvailProps", new XAttribute("value", 0)));
            propInfo.Add(new XElement("aPropMetaData", new XAttribute("itemType", "CPedPropMetaData")));
            propInfo.Add(new XElement("aAnchors", new XAttribute("itemType", "CAnchorProps")));

            xml.Add(propInfo);
            // END -> propInfo


            // dlcName Field
            XElement dlcNameField = new XElement("dlcName", dlcName);
            xml.Add(dlcNameField);

            return xml;
            // END OF FILE || END -> CPedVariationInfo
        }

        public static void SaveXML(string filePath)
        {
            XElement xmlFile = XML_Schema(filePath);
            xmlFile.Save(filePath);
            MessageBox.Show("Saved to: " + filePath, "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static String Number2String(int number, bool isCaps)
        {
            Char c = (Char)((isCaps ? 65 : 97) + (number));
            return c.ToString();
        }

        private static (int[], int) generateAvailComp()
        {
            int[] genAvailComp = { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 };
            int compCount = 0;
            foreach (var comp in MainWindow.Components)
            {
                genAvailComp[comp.compId] = compCount;
                compCount++;
            }

            return (genAvailComp, compCount);
        }

        private static int countAvailTex(int componentIndex)
        {
            int _textures = 0;
            foreach (var comp in MainWindow.Components.ElementAt(componentIndex).compList)
            {
                _textures = _textures + comp.drawableTextureCount;
            }

            return _textures;
        }
    }
}