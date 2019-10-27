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

namespace ModSwitcherLib
{
    public static class XMLVersion
    {
        public static List<string> GetVersions()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("versions.xml");

            XmlNodeList versionNameNodes = xmlDoc.SelectNodes("//VersionList/Version/VersionName");

            List<string> versionNames = new List<string>();
            foreach(XmlNode versionNameNode in versionNameNodes)
            {
                versionNames.Add(versionNameNode.InnerText);
            }

            return versionNames;
        }
    }
}
