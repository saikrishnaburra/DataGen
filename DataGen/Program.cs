using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Management.Automation;
using System.Xml;
using System.Text.RegularExpressions;
namespace DataGen
{
    public class Program
    {
        public static int itemCount = 1280;
        public static double intItemRatio = 0.75;
        public static int storeCount=1000;
        public static int rdcCount=80;
        public static int idcCount=12;
        public static int storageTypesCount=4;
        public static int timeBucketsCount=78;
        public static long maxRecordsPerFile=5000000;
        public static int intSuppliersCount = 1;
        public static int domSuppliersCount = 1;
        public static int demands = 6;
        public static double FillRate=1.0;
        public static double FillRateHandlingAtRDC=1.0;
        public static double FillRateHandlingAtIDC=1.0;
        public static double FillRateOutBoundEnggAtStore=1.0;
        public static double FillRateOutBoundEnggAtRDC=1.0;
        public static double FillRateInBoundEnggAtRDC=1.0;
        public static double FillRateInBoundEnggAtIDC=1.0;
        public static double FillRateRegAtRDCToStore=1.0;
        public static double FillRateRegAtIDCToRDC=1.0;
        public static double FillRateRegAtIntToIDC=1.0;
        public static double FillRateRegAtDomToRDC=1.0;
        public static double FillRateItemSpecRegAtIntToIDC=1.0;
        public static double FillRateItemSpecRegAtDomToRDC=1.0;
        public static DateTime StartDate;
        
        
        private static void Main(string[] args)
        {
            
            var settingsFile = Path.Combine(AssemblyBaseDirectory,"resources\\walmartDeep.xml");
            InitVariablesFromXml(settingsFile);
            string outputDir = string.Format("Output{0}",itemCount);
            if (args.Length > 0)
                outputDir = Path.Combine(args[0], outputDir);
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir,true);

            Thread.Sleep(1000);
            Directory.CreateDirectory(outputDir);
            var generator = new DataGenerator()
            {
                StoreCount = storeCount,
                RDCCount = rdcCount,
                IDCCount = idcCount,
                ItemCount = itemCount,
                IntItemRatio = intItemRatio,
                IntSupplierCount = intSuppliersCount,
                DomSupplierCount = domSuppliersCount,
                TimeBucketCount = timeBucketsCount,
                StorageTypesCount = storageTypesCount, 
                StartDate = StartDate,
                RdcToStoreActCount = (int)(storeCount * 1.1),
                IdcToRdcActCount = rdcCount * 2,
                OutputDir = outputDir,
                MaxRecordsPerFile = maxRecordsPerFile,
                FillRate=FillRate,
                FillRateHandlingAtRDC=FillRateHandlingAtRDC,
                FillRateHandlingAtIDC =FillRateHandlingAtIDC,
                FillRateOutBoundEnggAtStore=FillRateOutBoundEnggAtStore,
                FillRateOutBoundEnggAtRDC=FillRateOutBoundEnggAtRDC,
                FillRateInBoundEnggAtRDC=FillRateInBoundEnggAtRDC,
                FillRateInBoundEnggAtIDC=FillRateInBoundEnggAtIDC,
                FillRateRegAtRDCToStore=FillRateRegAtRDCToStore,
                FillRateRegAtIDCToRDC=FillRateRegAtIDCToRDC,
                FillRateRegAtIntToIDC=FillRateRegAtIntToIDC,
                FillRateRegAtDomToRDC=FillRateRegAtDomToRDC,
                FillRateItemSpecRegAtIntToIDC=FillRateItemSpecRegAtIntToIDC,
                FillRateItemSpecRegAtDomToRDC=FillRateItemSpecRegAtDomToRDC,
                DemandTypes = demands
            };
            
            generator.Generate();
            Console.ReadKey();
        }
           
        public static void InitVariablesFromXml(string settingsFile)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(settingsFile);
            XmlNodeList nodeList = xml.SelectNodes("/Settings");
            XmlNode xmlnode= nodeList[0].FirstChild;
            do{
                switch (xmlnode.Attributes["id"].Value)
                {
                    case "Stores":
                        if (!Int32.TryParse(xmlnode.InnerText, out storeCount))
                            storeCount = 1000;
                        break;
                    case "ItemCount":
                        if (!Int32.TryParse(xmlnode.InnerText, out itemCount))
                            itemCount = 1280;
                        break;
                    case "ItemRatio":
                        if (!Double.TryParse(xmlnode.InnerText, out intItemRatio))
                            intItemRatio = 0.75;
                        break;
                    case "RDC":
                        if (!Int32.TryParse(xmlnode.InnerText, out rdcCount))
                            rdcCount = 80;
                        break;
                    case "IDC":
                        if (!Int32.TryParse(xmlnode.InnerText, out idcCount))
                            idcCount = 12;
                        break;
                    case "Weeks":
                        if (!Int32.TryParse(xmlnode.InnerText, out timeBucketsCount))
                            timeBucketsCount = 78;
                        break;
                    case "StorageTypes":
                        if (!Int32.TryParse(xmlnode.InnerText, out storageTypesCount))
                            storageTypesCount = 4;
                        break;
                    case "MaxRecordsPerFile":
                        if (!Int64.TryParse(xmlnode.InnerText, out maxRecordsPerFile))
                            maxRecordsPerFile = 5000000;
                        break;
                    case "IntlSuppliers":
                        if (!Int32.TryParse(xmlnode.InnerText, out intSuppliersCount))
                           intSuppliersCount = 1;
                           break;
                    case "DomSuppliers":
                           if (!Int32.TryParse(xmlnode.InnerText, out intSuppliersCount))
                               domSuppliersCount = 1;
                           break;
                    case "StartDate":
                           var sd = xmlnode.InnerText.Trim();
                           string[] daymonthyear = Regex.Split(sd, "/");
                           int day,month,year;
                           if(!Int32.TryParse(daymonthyear[0],out day))
                               day=1;
                           if(!Int32.TryParse(daymonthyear[1],out month))
                               month=1;
                           if(!Int32.TryParse(daymonthyear[2],out year))
                               year=2017;
                           StartDate = new DateTime(year,month,day);
                           break;
                    case "Demands":
                        if (!Int32.TryParse(xmlnode.InnerText, out demands))
                            demands = 6;
                        break;
                } 
                xmlnode = xmlnode.NextSibling;
            }while(xmlnode != null);
            
            var nodes = xml.SelectNodes("/Settings/Setting[@id='FillRate']");
            var currNode = nodes[0].FirstChild;
               do
                {
                var name = currNode.Name;
                switch (name)
                {
                    case ("FillRate"):
                        if (!Double.TryParse(currNode.InnerText, out FillRate))
                            FillRate = 1.0;
                        break;
                    case ("FillRateHandlingAtRDC"):
                        if (!Double.TryParse(currNode.InnerText, out FillRateHandlingAtRDC))
                            FillRateHandlingAtRDC = 1.0;
                        break;
                    case ("FillRateHandlingAtIDC"):
                        if(!Double.TryParse(currNode.InnerText,out FillRateHandlingAtIDC))
                            FillRateHandlingAtIDC = 1.0;
                        break;
                    case ("FillRateOutBoundEnggAtStore"):
                        if(!Double.TryParse(currNode.InnerText,out FillRateOutBoundEnggAtStore))
                            FillRateOutBoundEnggAtStore = 1.0;
                        break;
                    case ("FillRateOutBoundEnggAtRDC"):
                        if (!Double.TryParse(currNode.InnerText, out FillRateOutBoundEnggAtRDC))
                            FillRateOutBoundEnggAtRDC = 1.0;
                        break;
                    case ("FillRateInBoundEnggAtRDC"):
                        if (!Double.TryParse(currNode.InnerText, out FillRateInBoundEnggAtRDC))
                            FillRateInBoundEnggAtRDC = 1.0;
                        break;
                    case ("FillRateInBoundEnggAtIDC"):
                        if (!Double.TryParse(currNode.InnerText, out FillRateInBoundEnggAtIDC))
                            FillRateInBoundEnggAtIDC = 1.0;
                        break;
                    case ("FillRateRegAtRDCToStore"):
                        if (!Double.TryParse(currNode.InnerText, out FillRateRegAtRDCToStore))
                            FillRateRegAtRDCToStore = 1.0;
                        break;
                    case ("FillRateRegAtIDCToRDC"):
                        if (!Double.TryParse(currNode.InnerText, out FillRateRegAtIDCToRDC))
                            FillRateRegAtIDCToRDC = 1.0;
                        break;
                    case ("FillRateRegAtIntToIDC"):
                        if (!Double.TryParse(currNode.InnerText, out FillRateRegAtIntToIDC))
                              FillRateRegAtIntToIDC = 1.0;
                        break;
                    case ("FillRateRegAtDomToRDC"):
                        if (!Double.TryParse(currNode.InnerText, out FillRateRegAtDomToRDC))
                              FillRateRegAtDomToRDC = 1.0;
                        break;
                    case ("FillRateItemSpecRegAtIntToIDC"):
                        if (!Double.TryParse(currNode.InnerText, out FillRateItemSpecRegAtIntToIDC))
                              FillRateItemSpecRegAtIntToIDC = 1.0;
                        break;
                    case ("FillRateItemSpecRegAtDomToRDC"):
                        if (!Double.TryParse(currNode.InnerText, out FillRateItemSpecRegAtDomToRDC))
                              FillRateItemSpecRegAtDomToRDC = 1.0;
                        break;
                }
                currNode = currNode.NextSibling;
            }while (currNode != null);
               Console.WriteLine(FillRateInBoundEnggAtIDC);
        }
        
           
        public static string AssemblyBaseDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path) + @"\";
            }
        }

        public static void RunScript(string scriptPath, string settingPath, string outputPath)
        {
            var scriptText = File.ReadAllText(scriptPath);
            using (PowerShell powerShellInstance = PowerShell.Create())
            {
                powerShellInstance.AddScript(scriptText);
                powerShellInstance.AddParameter("SettingsFile", settingPath);
                powerShellInstance.AddParameter("OutputDirectory", outputPath);
                var result = powerShellInstance.BeginInvoke();

                while (result.IsCompleted == false)
                {
                    Console.WriteLine("Waiting for pipeline to finish...");
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
