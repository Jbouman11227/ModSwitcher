namespace ModSwitcherLib
{
    using System;
    using System.Xml;
    using System.Linq;
    using System.Collections.ObjectModel;

    public static class ExtensionMethods
    {
        public static string AddPeriod(this string str)
        {
            return str + (str.EndsWith(".") ? string.Empty : ".");
        }

        public static void AddPath(this XmlDocument xmlDoc, string path)
        {
            var nodeNameList = path.Split('/');

            var partialPath = "//" + nodeNameList[0];
            var rootNodeTuple = FindOrCreateXmlNode(xmlDoc, partialPath);
            if (rootNodeTuple.Item2)
            {
                xmlDoc.AppendChild(rootNodeTuple.Item1);
            }

            var previousXmlNode = rootNodeTuple.Item1; 
            for (int i = 1; i < nodeNameList.Length; i++)
            {
                partialPath = partialPath + '/' + nodeNameList[i];
                var nodeTuple = FindOrCreateXmlNode(xmlDoc, partialPath);
                if (nodeTuple.Item2)
                {
                    previousXmlNode.AppendChild(nodeTuple.Item1);
                }
                previousXmlNode = nodeTuple.Item1;
            }
        }

        private static (XmlNode, bool) FindOrCreateXmlNode(XmlDocument xmlDoc, string path)
        {
            var nodes = xmlDoc.SelectNodes(path);
            if(nodes.Count > 0)
            {
                return (nodes[0], false);
            }

            var bottomNodeName = path.Split('/').Last();
            var bottomNode = xmlDoc.CreateElement(bottomNodeName);
            return (bottomNode, true);
        }
    }
}
