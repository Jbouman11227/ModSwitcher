using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace ModSwitcherLib
{
    public class XMLVersion
    {
        public XMLVersion(string gameFolder)
        {
            GameFolder = gameFolder;
        }

        private string GameFolder { get; set; }

        public static List<string> GetVersions()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("versions.xml");

            XmlNodeList versionNodes = xmlDoc.SelectNodes("//VersionConfig/VersionList/Version");

            List<string> versionNames = new List<string>();
            foreach(XmlNode versionNode in versionNodes)
            {
                versionNames.Add(versionNode.Attributes["Name"].InnerText);
            }

            return versionNames;
        }

        public void SetVersion(string versionName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("versions.xml");

            XmlNode versionNode = GetVersion(versionName, xmlDoc);
            XmlNodeList changes = versionNode.ChildNodes;

            for(int i = 0; i < changes.Count; i++)
            {
                switch (changes[i].Name)
                {
                    case "FileChange":
                        ExecuteFileChange(changes[i]);
                        break;
                    case "PatchChange":
                        ExecutePatchChange(changes[i], xmlDoc);
                        break;
                }
            }
        }

        private XmlNode GetVersion(string versionName, XmlDocument xmlDoc)
        {
            XmlNodeList versionNodes = xmlDoc.SelectNodes("//VersionConfig/VersionList/Version");

            foreach (XmlNode versionNode in versionNodes)
            {
                if(versionNode.Attributes["Name"].InnerText == versionName)
                {
                    return versionNode;
                }
            }

            throw new Exception($"versions.xml contains no version called {versionName}.");
        }

        private void ExecuteFileChange(XmlNode fileChangeNode)
        {
            string oldFile = GameFolder + "\\" + fileChangeNode.ChildNodes[0].InnerText,
                   newFile = GameFolder + "\\" + fileChangeNode.ChildNodes[1].InnerText;

            try
            {
                File.Move(oldFile, newFile);
            }
            catch (FileNotFoundException) { }
        }

        private void ExecutePatchChange(XmlNode patchChangeNode, XmlDocument xmlDoc)
        {
            XmlNodeList patchNodes = xmlDoc.SelectNodes("//VersionConfig/PatchHirarchy/Patch");
            string patchName = patchChangeNode.InnerText;

            bool extension = false;
            for(int i = 0; i < patchNodes.Count; i++)
            {
                if(patchNodes[i].InnerText == patchName)
                {
                    extension = true;
                }

                string oldFile = GameFolder + "\\" + patchNodes[i].InnerText + ( extension ? ".disabled" : ".big"),
                       newFile = GameFolder + "\\" + patchNodes[i].InnerText + (!extension ? ".disabled" : ".big");

                try
                {
                    File.Move(oldFile, newFile);
                }
                catch (FileNotFoundException) { }
            }
        }
    }
}
