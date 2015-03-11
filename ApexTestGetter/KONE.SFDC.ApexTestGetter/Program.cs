using KONE.SFDC.ApexTestGetter.SForce;
using KONE.SFDC.ApexTestGetter.Util;
using Salesforce.Partner;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Services.Protocols;
using System.Xml;

namespace KONE.SFDC.ApexTestGetter
{
    class Program
    {
        static void Main(string[] args)
        {

            if(!validateArguments(args))
                return;

            filterArguments(args);

            string path = args[0];

            // Initlilize build file
            BuildFile buildFile = new BuildFile(path);
            //
            PropertyFile propertyFile = new PropertyFile(buildFile.PropertyPath);
            string login = propertyFile.Login;
            string password = propertyFile.Password;
            string url = propertyFile.ServerUrl;

            Service API = Service.Instance;
            API.SfLogin(login, password);

            SearchResult searchResult = null;
            string query = @"FIND {@isTest OR testmethod} IN ALL FIELDS RETURNING ApexClass(Name)";

            try
            { 
                searchResult = API.SalesForce.search(query);
            }
            catch (SoapException sEx) 
            {
                Console.WriteLine(sEx.Message);
            }

            List<string> classNames = new List<string>();

            if(searchResult != null)
            {
                foreach(SearchRecord searchRecord in searchResult.searchRecords)
                {
                    sObject obj = searchRecord.record;
                    XmlElement[] elements = obj.Any;
                    classNames.Add(elements[0].InnerText);
                }
            }

            // Sort list and update build file
            classNames.Sort();
            buildFile.WriteTestNodes(args[1], classNames);

            API.SalesForce.logout();
        }

        /// <summary>
        /// Extracts paramters
        /// </summary>
        /// <param name="args"></param>
        private static void filterArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch(args[i])
                {
                    case "/f":
                        BuildFile.UseFilter = true;
                        break;

                }
            }
        }

        /// <summary>
        /// Checks wheter supplied arguments are relevant and valid
        /// </summary>
        /// <param name="inputArgs"></param>
        /// <returns></returns>
        private static bool validateArguments(string[] inputArgs)
        {
            bool pathOk = false;
            string path;

            // Check if argument exists
            if (inputArgs.Length > 1)
                path = inputArgs[0];
            else
            {
                Console.WriteLine("APEX test classes getter");
                Console.WriteLine("Usage: atestgetter.exe <build file path> <target> [/f]");
                Console.WriteLine("build file path: path to the build file, for instance \"C:\\Build.xml\"");
                Console.WriteLine("target: target name in build.xml");
                Console.WriteLine("/f: Use filtering");
                return pathOk;
            }

            if (File.Exists(path))
                pathOk = true;
            else
                Console.WriteLine("build.xml is not found at specificed path");

            return pathOk;
        }
    }
}
