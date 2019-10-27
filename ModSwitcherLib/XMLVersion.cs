using System;
using System.Collections.Generic;
using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using ModSwitcherLib;
using ModSwitcherLib.Types;
using System.Linq;
using System.IO;

namespace ModSwitcherLib
{
    public static class XMLVersion
    {
        public static List<string> GetVersions()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("versions.xml");

            XmlNodeList versionNodes = xmlDoc.SelectNodes("//VersionList/Version");

            List<string> versionNames = new List<string>();
            foreach(XmlNode versionNode in versionNodes)
            {
                versionNames.Add(versionNode.Attributes["Name"].InnerText);
            }

            return versionNames;
        }

        public static void SetVersion(string versionName, string gameFolder)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("versions.xml");

            XmlNode versionNode = GetVersion(versionName, xmlDoc);

            foreach (XmlNode fileChangeNode in versionNode.ChildNodes)
            {
                string oldFile = gameFolder + "\\" + fileChangeNode.ChildNodes[0].InnerText,
                       newFile = gameFolder + "\\" + fileChangeNode.ChildNodes[1].InnerText;

                try
                {
                    File.Move(oldFile, newFile);
                }
                catch (FileNotFoundException) { }
            }
        }

        public static XmlNode GetVersion(string versionName, XmlDocument xmlDoc)
        {
            XmlNodeList versionNodes = xmlDoc.SelectNodes("//VersionList/Version");

            foreach (XmlNode versionNode in versionNodes)
            {
                if(versionNode.Attributes["Name"].InnerText == versionName)
                {
                    return versionNode;
                }
            }

            return null;
        }
    }
}
