using System;
using System.Xml;
using System.Collections.ObjectModel;

namespace ModSwitcherLib
{
    static public class XMLConfig
    {
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

        public static void ReadXML(ObservableCollection<string> modList, ref string currentModName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            XmlNodeList modNameNodes = xmlDoc.SelectNodes("//ModSwitcherConfig/ModList/Mod/ModName");
            XmlNode currentModNameNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/CurrentMod/ModName");
            
            currentModName = currentModNameNode.InnerText;

            modList.Clear();
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

        public static string ReadCurrentModName()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            XmlNode currentModNameNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/CurrentMod/ModName");

            return currentModNameNode.InnerText;
        }

        public static void SetCurrentModName(string currentModName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            
            XmlNode currentModNameNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/CurrentMod/ModName");

            if(currentModNameNode == null)
            {
                throw new Exception("could not find the node //ModSwitcherConfig/CurrentMod/ModName.");
            }

            currentModNameNode.InnerText = currentModName;

            xmlDoc.Save("config.xml");
        }

        public static Mod ReadMod(string modName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");

            XmlNode modNode = GetModNode(modName, xmlDoc);

            return ModNodeToMod(modNode);
        }

        public static void AddMod(Mod mod)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            XmlNode modListNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/ModList");

            XmlNode childNode = Node("Mod", xmlDoc);
            modListNode.AppendChild(childNode);

            childNode.AppendChild(Node("ModName", mod.ModName, xmlDoc));
            childNode.AppendChild(Node("modType", Convert.ToString(mod.modType), xmlDoc));
            childNode.AppendChild(Node("ModPath", mod.ModPath, xmlDoc));
            childNode.AppendChild(Node("Flag", mod.Flag, xmlDoc));
            childNode.AppendChild(Node("UsingModPath", Convert.ToString(mod.UsingModPath), xmlDoc));
            childNode.AppendChild(Node("OverrideGamePath", Convert.ToString(mod.OverrideGamePath), xmlDoc));
            childNode.AppendChild(Node("GamePath", mod.GamePath, xmlDoc));
            childNode.AppendChild(Node("SetVersion", Convert.ToString(mod.SetVersion), xmlDoc));
            childNode.AppendChild(Node("Version", mod.Version, xmlDoc));

            xmlDoc.Save("config.xml");
        }

        public static void EditMod(Mod mod, string selectedMod)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");

            XmlNode modNode = GetModNode(selectedMod, xmlDoc);

            modNode.ChildNodes[0].InnerText = mod.ModName;
            modNode.ChildNodes[1].InnerText = Convert.ToString(mod.modType);
            modNode.ChildNodes[2].InnerText = mod.ModPath;
            modNode.ChildNodes[3].InnerText = mod.Flag;
            modNode.ChildNodes[4].InnerText = Convert.ToString(mod.UsingModPath);
            modNode.ChildNodes[5].InnerText = Convert.ToString(mod.OverrideGamePath);
            modNode.ChildNodes[6].InnerText = mod.GamePath;
            modNode.ChildNodes[7].InnerText = Convert.ToString(mod.SetVersion);
            modNode.ChildNodes[8].InnerText = mod.Version;

            xmlDoc.Save("config.xml");
        }

        public static void RemoveMod(string selectedMod)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            
            XmlNode modListNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/ModList");
            XmlNode modNode = GetModNode(selectedMod, xmlDoc);

            modListNode.RemoveChild(modNode);

            xmlDoc.Save("config.xml");
        }

        public static bool Exists(string modName, int indexOfSelectedMod = -1)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            XmlNodeList modNameNodes = xmlDoc.SelectNodes("//ModSwitcherConfig/ModList/Mod/ModName");

            int i = 0;
            while (i < modNameNodes.Count)
            {
                if (modNameNodes[i].InnerText == modName && i != indexOfSelectedMod)
                {
                    return true;
                }
                i = i + 1;
            }
            return false;
        }

        public static int IndexOf(string modName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            XmlNodeList modNameNodes = xmlDoc.SelectNodes("//ModSwitcherConfig/ModList/Mod/ModName");

            int i = 0;
            while(i < modNameNodes.Count)
            {
                if(modNameNodes[i].InnerText == modName)
                {
                    return i;
                }
                i = i + 1;
            }
            return -1;
        }

        public static XmlNode GetModNode(string modName, XmlDocument xmlDoc)
        {
            XmlNodeList modNodes = xmlDoc.SelectNodes("//ModSwitcherConfig/ModList/Mod");
            foreach (XmlNode modNode in modNodes)
            {
                if (modNode.ChildNodes[0].InnerText == modName)
                {
                    return modNode;
                }
            }

            throw new Exception($"could not find {modName}.");
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

        private static Mod ModNodeToMod(XmlNode node)
        {
            Mod mod = new Mod();

            mod.ModName = node.ChildNodes[0].InnerText;
            mod.modType = (ModType)Enum.Parse(typeof(ModType), node.ChildNodes[1].InnerText, true);
            mod.ModPath = node.ChildNodes[2].InnerText;
            mod.Flag = node.ChildNodes[3].InnerText;
            mod.UsingModPath = Convert.ToBoolean(node.ChildNodes[4].InnerText);
            mod.OverrideGamePath = Convert.ToBoolean(node.ChildNodes[5].InnerText);
            mod.GamePath = node.ChildNodes[6].InnerText;
            mod.SetVersion = Convert.ToBoolean(node.ChildNodes[7].InnerText);
            mod.Version = node.ChildNodes[8].InnerText;

            return mod;
        }
    }
}
