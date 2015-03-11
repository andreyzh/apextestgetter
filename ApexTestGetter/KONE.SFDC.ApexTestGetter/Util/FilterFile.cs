using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KONE.SFDC.ApexTestGetter.Util
{
    /// <summary>
    /// This class is used to get the list of classes
    /// that have to be excluded from the tests list "excludetests.txt".
    /// </summary>
    static class FilterFile
    {
        /// <summary>
        /// Scans though the excludetests file and gets list of test classes to exclude
        /// </summary>
        /// <param name="projectPath">Path to project folder (contains build file)</param>
        /// <returns>List of test classes that need to be excluded from the build file</returns>
        public static List<string> GetExcludedTests(string projectPath)
        {
            List<string> excludedTests = new List<string>();

            if (!File.Exists(projectPath + "\\excludetests.txt"))
            { 
                Console.WriteLine("Filter file excludetests.txt does not exist in " + projectPath + ". Skipping...");
                return null;
            }

            using (StreamReader file = new StreamReader(projectPath + "\\excludetests.txt"))
            {
                string line;

                while ((line = file.ReadLine()) != null)
                {
                    excludedTests.Add(line.Trim());
                }

                file.Close();
            }

            return excludedTests;
        }
    }
}
