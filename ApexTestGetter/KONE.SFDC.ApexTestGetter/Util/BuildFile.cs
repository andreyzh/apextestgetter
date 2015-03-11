using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace KONE.SFDC.ApexTestGetter.Util
{
    /// <summary>
    /// Operates on the build file
    /// </summary>
    class BuildFile
    {
        static bool useFilter = false;
        readonly string filePath;
        List<string> testNames;
        XmlDocument xmlDocument = new XmlDocument();

        /// <summary>
        /// Initiailizes build file into XML document.
        /// </summary>
        /// <param name="path">Path to build file</param>
        public BuildFile(string path)
        {
            filePath = path;

            try
            {
                xmlDocument.PreserveWhitespace = false;
                xmlDocument.Load(path);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Returns the path to properties file
        /// </summary>
        public string PropertyPath
        {
            get
            {
                string propertyPath = getPropertiesPath();
                if(File.Exists(propertyPath))
                    return propertyPath;
                else
                    return Path.GetDirectoryName(filePath) + "\\" + propertyPath;
            }
        }
        /// <summary>
        /// Determines whether to filter certain unit tests
        /// </summary>
        public static bool UseFilter
        {
            get { return useFilter; }
            set { useFilter = value; }
        }

        public void WriteTestNodes(string targetName, List<string> testNamesInput)
        {
            this.testNames = testNamesInput;

            XmlNode testsParentNode = null;
            XmlNodeList targetNodes;
            XmlElement rootElement = xmlDocument.DocumentElement;
            targetNodes = rootElement.SelectNodes("target");

            // Go through target nodes, select one which has the targetName
            if(targetNodes.Count > 0)
            {
                foreach(XmlNode node in targetNodes)
                {
                    XmlAttributeCollection attributeCollection = node.Attributes;
                    foreach (XmlAttribute attribute in attributeCollection)
                    {
                        if (attribute.Name == "name" && attribute.Value == targetName)
                            testsParentNode = node.FirstChild; 
                    }                    
                }
            }

            if (testsParentNode == null)
            { 
                Console.WriteLine("Target " + targetName + " was not found. Aborting.");
                return;
            }

            // Remove all child <RunTest> nodes
            while (testsParentNode.FirstChild != null)
                testsParentNode.RemoveChild(testsParentNode.FirstChild);

            // Remove filtered classes
            if (useFilter)
                filterTestClasses();

            // Create new test nodes
            foreach(string testName in testNames)
            {
                XmlNode node = xmlDocument.CreateNode("element", "runTest", "");
                node.InnerText = testName;
                testsParentNode.AppendChild(node);
            }

            // Write XML
            writeXml();
        }

        private void filterTestClasses()
        {
            // Get list of classes to remove
            List<string> testsToRemove = FilterFile.GetExcludedTests(Path.GetDirectoryName(filePath));
            if(testsToRemove != null)
            {
                foreach(string testToRemove in testsToRemove)
                {
                    // Remove ignoring the case
                    testNames.RemoveAll(n => n.Equals(testToRemove, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        /// <summary>
        /// Finds path to the build.properties file
        /// </summary>
        /// <returns></returns>
        private string getPropertiesPath()
        {
            string propertyFileName = null;

            XmlNode property;
            XmlElement rootElement = xmlDocument.DocumentElement;
            property = rootElement.SelectSingleNode("property");

            XmlAttributeCollection attributeCollection = property.Attributes;
            foreach(XmlAttribute attribute in attributeCollection)
            {
                if (attribute.Name == "file")
                    propertyFileName = attribute.Value;
            }

            return propertyFileName;
        }

        // Helper method for writing the XML
        private void writeXml()
        {
            // Create XML Writer settings for indentation
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };

            //TODO: Try/Catch
            using (XmlWriter writer = XmlWriter.Create(filePath, settings))
            {
                xmlDocument.PreserveWhitespace = true;
                xmlDocument.Save(writer);
                writer.Close();
            }
        }
    }
}
