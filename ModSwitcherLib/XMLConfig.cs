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
                childNode.AppendChild(Node("modType", Convert.ToString(mod.modType), xmlDoc));
                childNode.AppendChild(Node("ModPath", mod.ModPath, xmlDoc));
                childNode.AppendChild(Node("Flag", mod.Flag, xmlDoc));
                childNode.AppendChild(Node("UsingModPath", Convert.ToString(mod.UsingModPath), xmlDoc));
            }
            xmlDoc.Save("config.xml");
        }

        public static void ReadXML(ObservableCollection<string> modList, ref string currentModName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            XmlNodeList modNameNodes = xmlDoc.SelectNodes("//ModSwitcherConfig/ModList/Mod/ModName");
            XmlNode currentModNameNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/CurrentMod/ModName");
            
            currentModName = currentModNameNode.InnerText;

            foreach (XmlNode modNameNode in modNameNodes)
            {
                modList.Add(modNameNode.InnerText);
            }
        }

        public static void SetGamePath(string gamePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            XmlNode gamePathNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/DefaultGamePath");

            gamePathNode.InnerText = gamePath;

            xmlDoc.Save("config.xml");
        }

        public static string ReadGamePath()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            XmlNode gamePathNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/DefaultGamePath");

            return gamePathNode.InnerText;
        }

        public static void WriteNewConfig()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootnode, node, childNode;

            rootnode = Node("ModSwitcherConfig", xmlDoc);
            xmlDoc.AppendChild(rootnode);

            rootnode.AppendChild(Node("DefaultGamePath", string.Empty, xmlDoc));

            node = Node("CurrentMod", xmlDoc);
            rootnode.AppendChild(node);
            node.AppendChild(Node("ModName", string.Empty, xmlDoc));

            node = Node("ModList", xmlDoc);
            rootnode.AppendChild(node);

            xmlDoc.Save("config.xml");
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
            mod.modType = (ModType)Enum.Parse(typeof(ModType), node.ChildNodes[1].InnerText, true);
            mod.ModPath = node.ChildNodes[2].InnerText;
            mod.Flag = node.ChildNodes[3].InnerText;
            mod.UsingModPath = Convert.ToBoolean(node.ChildNodes[4].InnerText);

            return mod;
        }
    }
}
