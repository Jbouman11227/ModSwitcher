using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using ModSwitcherLib;
using ModSwitcherLib.Types;

namespace ModSwitcherLib
{
    static public class XMLConfig
    {
        public static void WriteXML(ObservableCollection<Mod> modList, Mod currentMod)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootnode, node, childNode;

            rootnode = Node("ModSwitcherConfig", xmlDoc);
            xmlDoc.AppendChild(rootnode);

            node = Node("CurrentMod", xmlDoc);
            rootnode.AppendChild(node);
            node.AppendChild(Node("ModName", currentMod == null ? string.Empty : currentMod.ModName, xmlDoc));
            
            node = Node("ModList", xmlDoc);
            rootnode.AppendChild(node);
            
            foreach(Mod mod in modList)
            {
                childNode = Node("Mod", xmlDoc);
                node.AppendChild(childNode);
                childNode.AppendChild(Node("ModName", mod.ModName, xmlDoc));
                childNode.AppendChild(Node("GamePath", mod.GamePath, xmlDoc));
                childNode.AppendChild(Node("modType", Convert.ToString(mod.modType), xmlDoc));
                childNode.AppendChild(Node("ModPath", mod.ModPath, xmlDoc));
                childNode.AppendChild(Node("Flag", mod.Flag, xmlDoc));
                childNode.AppendChild(Node("UsingModPath", Convert.ToString(mod.UsingModPath), xmlDoc));
            }
            xmlDoc.Save("config.xml");
        }

        public static void ReadXML(ObservableCollection<Mod> modList, ref string currentModName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            XmlNodeList modNodes = xmlDoc.SelectNodes("//ModSwitcherConfig/ModList/Mod");
            XmlNode currentModNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/CurrentMod");
            
            currentModName = currentModNode.ChildNodes[0].InnerText;

            foreach (XmlNode modNode in modNodes)
            {
                modList.Add(ModeNodeToMod(modNode));
            }
        }

        private static XmlNode Node(string nodeName, string value, XmlDocument xmlDoc)
        {
            XmlNode xmlNode = xmlDoc.CreateElement(nodeName);
            xmlNode.InnerText = value;
            return xmlNode;
        }

        private static XmlNode Node(string nodeName, XmlDocument xmlDoc)
        {
            XmlNode xmlNode = xmlDoc.CreateElement(nodeName);
            return xmlNode;
        }

        private static Mod ModeNodeToMod(XmlNode node)
        {
            Mod mod = new Mod();

            mod.ModName = node.ChildNodes[0].InnerText;
            mod.GamePath = node.ChildNodes[1].InnerText;
            mod.modType = (ModType)Enum.Parse(typeof(ModType), node.ChildNodes[2].InnerText, true);
            mod.ModPath = node.ChildNodes[3].InnerText;
            mod.Flag = node.ChildNodes[4].InnerText;
            mod.UsingModPath = Convert.ToBoolean(node.ChildNodes[5].InnerText);

            return mod;
        }
    }
}
