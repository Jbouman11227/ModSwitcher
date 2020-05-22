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

            xmlDoc.AddPath("ModSwitcherConfig/Settings/DefaultGamePath");
            xmlDoc.AddPath("ModSwitcherConfig/Settings/GameFile");
            xmlDoc.AddPath("ModSwitcherConfig/Settings/PatchSwitcher");
            xmlDoc.AddPath("ModSwitcherConfig/CurrentMod/ModName");
            xmlDoc.AddPath("ModSwitcherConfig/ModList");

            var gameFileNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/Settings/GameFile");
            gameFileNode.InnerText = "lotrbfme2ep1.exe";

            var patchSwitcherNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/Settings/PatchSwitcher");
            patchSwitcherNode.InnerText = "202_launcher.exe";

            var modNameNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/CurrentMod/ModName");
            modNameNode.InnerText = string.Empty;

            xmlDoc.Save("config.xml");
        }

        public static void ReadXML(ObservableCollection<string> modList, ref string currentModName)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            var modNameNodes = xmlDoc.SelectNodes("//ModSwitcherConfig/ModList/Mod/ModName");
            var currentModNameNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/CurrentMod/ModName");
            
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
            var gamePathNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/Settings/DefaultGamePath");

            gamePathNode.InnerText = gamePath;

            xmlDoc.Save("config.xml");
        }

        public static string ReadGamePath()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");

            return xmlDoc.SelectSingleNode("//ModSwitcherConfig/Settings/DefaultGamePath").InnerText;
        }

        public static string ReadCurrentModName()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            var currentModNameNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/CurrentMod/ModName");

            return currentModNameNode.InnerText;
        }

        public static void SetCurrentModName(string currentModName)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            
            var currentModNameNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/CurrentMod/ModName");

            if(currentModNameNode == null)
            {
                throw new Exception("could not find the node //ModSwitcherConfig/CurrentMod/ModName.");
            }

            currentModNameNode.InnerText = currentModName;

            xmlDoc.Save("config.xml");
        }

        public static Mod ReadMod(string modName)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");

            var modNode = GetModNode(modName, xmlDoc);

            return ModNodeToMod(modNode);
        }

        public static void AddMod(Mod mod)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            XmlNode modListNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/ModList");

            var childNode = Node("Mod", xmlDoc);
            modListNode.AppendChild(childNode);

            childNode.AppendChild(Node("ModName", mod.ModName, xmlDoc));
            childNode.AppendChild(Node("modType", Convert.ToString(mod.modType), xmlDoc));
            childNode.AppendChild(Node("ModPath", mod.ModPath, xmlDoc));
            childNode.AppendChild(Node("ExtraFlags", mod.ExtraFlags, xmlDoc));
            childNode.AppendChild(Node("OverrideGamePath", Convert.ToString(mod.OverrideGamePath), xmlDoc));
            childNode.AppendChild(Node("GamePath", mod.GamePath, xmlDoc));

            xmlDoc.Save("config.xml");
        }

        public static void EditMod(Mod mod, string selectedMod)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");

            var modNode = GetModNode(selectedMod, xmlDoc);

            modNode.ChildNodes[0].InnerText = mod.ModName;
            modNode.ChildNodes[1].InnerText = Convert.ToString(mod.modType);
            modNode.ChildNodes[2].InnerText = mod.ModPath;
            modNode.ChildNodes[3].InnerText = mod.ExtraFlags;
            modNode.ChildNodes[4].InnerText = Convert.ToString(mod.OverrideGamePath);
            modNode.ChildNodes[5].InnerText = mod.GamePath;

            xmlDoc.Save("config.xml");
        }

        public static void RemoveMod(string selectedMod)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            
            var modListNode = xmlDoc.SelectSingleNode("//ModSwitcherConfig/ModList");
            var modNode = GetModNode(selectedMod, xmlDoc);

            modListNode.RemoveChild(modNode);

            xmlDoc.Save("config.xml");
        }

        public static string ReadPatchSwitcher()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");

            return xmlDoc.SelectSingleNode("//ModSwitcherConfig/Settings/PatchSwitcher").InnerText;
        }

        public static string ReadGameFile()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");

            return xmlDoc.SelectSingleNode("//ModSwitcherConfig/Settings/GameFile").InnerText;
        }

        public static bool Exists(string modName, int indexOfSelectedMod = -1)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            var modNameNodes = xmlDoc.SelectNodes("//ModSwitcherConfig/ModList/Mod/ModName");

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
            var xmlDoc = new XmlDocument();
            xmlDoc.Load("config.xml");
            var modNameNodes = xmlDoc.SelectNodes("//ModSwitcherConfig/ModList/Mod/ModName");

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
            var modNodes = xmlDoc.SelectNodes("//ModSwitcherConfig/ModList/Mod");
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
            var xmlNode = xmlDoc.CreateElement(nodeName);
            xmlNode.InnerText = value;
            return xmlNode;
        }

        private static XmlNode Node(string nodeName, XmlDocument xmlDoc)
        {
            var xmlNode = xmlDoc.CreateElement(nodeName);
            return xmlNode;
        }

        private static Mod ModNodeToMod(XmlNode node)
        {
            var mod = new Mod();

            mod.ModName = node.ChildNodes[0].InnerText;
            mod.modType = (ModType)Enum.Parse(typeof(ModType), node.ChildNodes[1].InnerText, true);
            mod.ModPath = node.ChildNodes[2].InnerText;
            mod.ExtraFlags = node.ChildNodes[3].InnerText;
            mod.OverrideGamePath = Convert.ToBoolean(node.ChildNodes[4].InnerText);
            mod.GamePath = node.ChildNodes[5].InnerText;

            return mod;
        }
    }
}
