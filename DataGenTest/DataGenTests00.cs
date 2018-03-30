using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DataGen;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
namespace DataGenTest
{
    [TestClass]
    public class DataGenTests00
    {
        public static int ItemCount=1280;
        public static int StoreCount=1000;
        public static double ItemDomRatio { get { return 1 - IntItemRatio; } }
        public static double IntItemRatio=0.75;
        public static int RDCCount=80;
        public static int IDCCount=12;
        public static int NormalIDCCount { get { return IDCCount / 2; } }
        public static int NoCarryIDC { get { return IDCCount / 2; } }
        public static int TimeBucketsCount=78;
        public static int StorageTypesCount=4;
        public static Int64 maxRecordsPerFile=5000000;
        public static int intSuppliersCount=1;
        public static int domSuppliersCount=1;
        public static DateTime StartDate;
        public static int demands = 6;
        public int StoreKeyStartIdx = 0;
        public int RdcKeyStartIdx = 5000;
        public int IdcKeyStartIdx = 6000;
        public int NoCarryIdcKeyStartIdx = 6500;
        public int IntSuppKeyStartIdx = 7000;
        public int DomSuppKeyStartIdx = 8000;
        public const string StorageRegx = @"^DC-(\d+)$";
        public int AvgDemand
        {
            get { return 105; }
        }

        public int AvgSSNumberAtStore
        {
            get { return 200; }
        }

        public int AvgSSNumberAtRDC
        {
            get { return AvgSSNumberAtStore * (StoreCount / RDCCount); }
        }

        public int AvgSSNumberAtIDC
        {
            get { return AvgSSNumberAtStore * (StoreCount / IDCCount); }
        }
        public double RegStorageAtRDC { get { return ItemCount * AvgSSNumberAtRDC; } }
        public double Ot1StorageAtRDC { get { return RegStorageAtRDC / 10; } }
        public double Ot2StorageAtRDC { get { return RegStorageAtRDC / 10; } }

        public double RegStorageAtIDC { get { return IntItemCount * AvgSSNumberAtIDC; } }
        public double Ot1StorageAtIDC { get { return RegStorageAtIDC / 10; } }
        public double Ot2StorageAtIDC { get { return RegStorageAtIDC / 10; } }
        public static double FillRate = 1.0;
        public static double FillRateHandlingAtRDC = 1.0;
        public static double FillRateHandlingAtIDC = 1.0;
        public static double FillRateOutBoundEnggAtStore = 1.0;
        public static double FillRateOutBoundEnggAtRDC = 1.0;
        public static double FillRateInBoundEnggAtRDC = 1.0;
        public static double FillRateInBoundEnggAtIDC = 1.0;
        public static double FillRateRegAtRDCToStore = 1.0;
        public static double FillRateRegAtIDCToRDC = 1.0;
        public static double FillRateRegAtIntToIDC = 1.0;
        public static double FillRateRegAtDomToRDC = 1.0;
        public static double FillRateItemSpecRegAtIntToIDC = 1.0;
        public static double FillRateItemSpecRegAtDomToRDC = 1.0;
        
        public double HandelingAtRDC { get { return FillRateHandlingAtRDC * InBoundEnggAtRDC * 2; } }
        public double HandelingAtRDCOt1 { get { return HandelingAtRDC / 10; } }
        public double HandelingAtRDCOt2 { get { return HandelingAtRDC / 10; } }

        public double HandelingAtIDC { get { return FillRateHandlingAtIDC * InBoundEnggAtIDC * 2; } }
        public double HandelingAtIDCOt1 { get { return HandelingAtIDC / 10; } }
        public double HandelingAtIDCOt2 { get { return HandelingAtIDC / 10; } }

        public double OutBoundEnggAtStore { get { return FillRateOutBoundEnggAtStore * ItemCount * AvgDemand; } }
        public double OutBoundEnggAtStoreOt1 { get { return OutBoundEnggAtStore / 10; } }
        public double OutBoundEnggAtStoreOt2 { get { return OutBoundEnggAtStore / 10; } }

        public double OutBoundEnggAtRDC { get { return FillRateOutBoundEnggAtRDC * InBoundEnggAtRDC; } }
        public double OutBoundEnggAtRDCOt1 { get { return OutBoundEnggAtRDC / 10; } }
        public double OutBoundEnggAtRDCOt2 { get { return OutBoundEnggAtRDC / 10; } }

        public double InBoundEnggAtRDC { get { return FillRateInBoundEnggAtRDC * ItemCount * AvgDemand * (StoreCount / RDCCount); } }
        public double InBoundEnggAtRDCOt1 { get { return InBoundEnggAtRDC / 10; } }
        public double InBoundEnggAtRDCOt2 { get { return InBoundEnggAtRDC / 10; } }

        public double InBoundEnggAtIDC { get { return FillRateInBoundEnggAtIDC * IntItemCount * AvgDemand * (StoreCount / IDCCount); } }
        public double InBoundEnggAtIDCOt1 { get { return InBoundEnggAtIDC / 10; } }
        public double InBoundEnggAtIDCOt2 { get { return InBoundEnggAtIDC / 10; } }

        public double RegAtRDCToStore { get { return FillRateRegAtRDCToStore * AvgDemand * ItemCount; } }
        public double RegAtRDCToStoreOt1 { get { return RegAtRDCToStore / 10; } }
        public double RegAtRDCToStoreOt2 { get { return RegAtRDCToStore / 10; } }

        public double RegAtIDCToRDC { get { return FillRateRegAtIDCToRDC * AvgDemand * IntItemCount * (StoreCount / RDCCount); } }
        public double RegAtIDCToRDCOt1 { get { return RegAtIDCToRDC / 10; } }
        public double RegAtIDCToRDCOt2 { get { return RegAtIDCToRDC / 10; } }

        public double RegAtIntToIDC { get { return FillRateRegAtIntToIDC * AvgDemand * IntItemCount * (StoreCount / IDCCount); } }
        public double RegAtIntToIDCOt1 { get { return RegAtIntToIDC / 10; } }
        public double RegAtIntToIDCOt2 { get { return RegAtIntToIDC / 10; } }

        public double RegAtDomToRDC { get { return FillRateRegAtDomToRDC * AvgDemand * DomItemCount * (StoreCount / RDCCount); } }
        public double RegAtDomToRDCOt1 { get { return RegAtDomToRDC / 10; } }
        public double RegAtDomToRDCOt2 { get { return RegAtDomToRDC / 10; } }

        public double ItemSpecRegAtIntToIDC { get { return FillRateItemSpecRegAtIntToIDC * AvgDemand * (StoreCount / IDCCount); } }
        public double ItemSpecRegAtIntToIDCOt1 { get { return ItemSpecRegAtIntToIDC / 10; } }
        public double ItemSpecRegAtIntToIDCOt2 { get { return ItemSpecRegAtIntToIDC / 10; } }

        public double ItemSpecRegAtDomToRDC { get { return FillRateItemSpecRegAtDomToRDC * AvgDemand * (StoreCount / RDCCount); } }
        public double ItemSpecRegAtDomToRDCOt1 { get { return ItemSpecRegAtDomToRDC / 10; } }
        public double ItemSpecRegAtDomToRDCOt2 { get { return ItemSpecRegAtDomToRDC / 10; } }
        public static string AssemblyBaseDirectory
        {
            get
            {
                
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                var settingFile = Path.GetDirectoryName(path) + @"\";
                String[] extract = Regex.Split(settingFile, "DataGenTest");  //split it in DataGenTest
                settingFile = extract[0].TrimEnd('\\');
                return Path.Combine(settingFile, "DataGen\\bin\\Release");
               
            }
        }

        public static string OutputDir
        {
            get { return Path.Combine(AssemblyBaseDirectory, string.Format("Output{0}", ItemCount)); } 
        }

        public static void InitVariablesFromXml(string settingsFile)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(settingsFile);
            XmlNodeList nodeList = xml.SelectNodes("/Settings");
            XmlNode xmlnode = nodeList[0].FirstChild;
            do
            {
                switch (xmlnode.Attributes["id"].Value)
                {
                    case "Stores":
                        if (!Int32.TryParse(xmlnode.InnerText, out StoreCount))
                            StoreCount = 1000;
                        break;
                    case "ItemCount":
                        if (!Int32.TryParse(xmlnode.InnerText, out ItemCount))
                            ItemCount = 1280;
                        break;
                    case "ItemRatio":
                        if (!Double.TryParse(xmlnode.InnerText, out IntItemRatio))
                            IntItemRatio = 0.75;
                        break;
                    case "RDC":
                        if (!Int32.TryParse(xmlnode.InnerText, out RDCCount))
                            RDCCount = 80;
                        break;
                    case "IDC":
                        if (!Int32.TryParse(xmlnode.InnerText, out IDCCount))
                            IDCCount = 12;
                        break;
                    case "Weeks":
                        if (!Int32.TryParse(xmlnode.InnerText, out TimeBucketsCount))
                            TimeBucketsCount = 78;
                        break;
                    case "StorageTypes":
                        if (!Int32.TryParse(xmlnode.InnerText, out StorageTypesCount))
                            StorageTypesCount = 4;
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
                        int day, month, year;
                        if (!Int32.TryParse(daymonthyear[0], out day))
                            day = 1;
                        if (!Int32.TryParse(daymonthyear[1], out month))
                            month = 1;
                        if (!Int32.TryParse(daymonthyear[2], out year))
                            year = 2017;
                        StartDate = new DateTime(year, month, day);
                        break;
                    case "Demands":
                        if (!Int32.TryParse(xmlnode.InnerText, out demands))
                            demands = 6;
                        break;

                }
                xmlnode = xmlnode.NextSibling;
            } while (xmlnode != null);

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
                        if (!Double.TryParse(currNode.InnerText, out FillRateHandlingAtIDC))
                            FillRateHandlingAtIDC = 1.0;
                        break;
                    case ("FillRateOutBoundEnggAtStore"):
                        if (!Double.TryParse(currNode.InnerText, out FillRateOutBoundEnggAtStore))
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
            } while (currNode != null);
            Console.WriteLine(FillRateInBoundEnggAtIDC);
        }
        #region Calulated Expects
        public static int DomItemCount
        {
            get { return (int)(ItemCount * ItemDomRatio); }
        }

        public static int IntItemCount
        {
            get { return (int)(ItemCount * (1 - ItemDomRatio)); }
        }

        public static int RDCToStoreActivityCount
        {
            get { return (int)(ItemCount * StoreCount * 1.1); }
        }

        public static int IDCToRDCActivityCount
        {
            get { return 2 * RDCCount * IntItemCount; }
        }

        public static int DomToRDCActivityCount
        {
            get { return RDCCount * DomItemCount; }
        }

        public static int IntToIDCActivityCount
        {
            get { return (IDCCount) * IntItemCount; }
        }

        public static int ActivityCount
        {
            get
            {
                return RDCToStoreActivityCount + IDCToRDCActivityCount +
                       IntToIDCActivityCount + DomToRDCActivityCount;
            }
        }

        #region Capacity Edge counts

        #region RDCToStore
        public static int RDCToStoreInBoundEnggEdgeCount
        {
            get { return RDCToStoreActivityCount; }
        }

        public static int RDCToStoreOutBoundEnggEdgeCount
        {
            get { return RDCToStoreActivityCount; }
        }

        public static int RDCToStoreRegularEdgeCount
        {
            get { return RDCToStoreActivityCount; }
        }

        public static int RDCToStoreHandelingEdgeCount
        {
            get { return RDCToStoreActivityCount; }
        }
        #endregion

        #region IDCToRDC
        public static int IDCToRDCInBoundEnggEdgeCount
        {
            get { return IDCToRDCActivityCount; }
        }

        public static int IDCToRDCOutBoundEnggEdgeCount
        {
            get { return IDCToRDCActivityCount; }
        }

        public static int IDCToRDCRegularEdgeCount
        {
            get { return IDCToRDCActivityCount; }
        }

        public static int IDCToRDCHandelingEdgeCount
        {
            get { return IDCToRDCActivityCount * 2; }
        }
        #endregion

        #region IntToIDC
        public static int IntToIDCInBoundEnggEdgeCount
        {
            get { return 0; }
        }

        public static int IntToIDCOutBoundEnggEdgeCount
        {
            get { return 0; }
        }

        public static int IntToIDCRegularEdgeCount
        {
            get { return IntToIDCActivityCount; }
        }

        public static int IntToIDCHandelingEdgeCount
        {
            get { return IntToIDCActivityCount; }
        }

        public static int ItemSpecIntToIDCRegularEdgeCount
        {
            get { return IntToIDCActivityCount; }
        }
        #endregion

        #region DomToRDC
        public static int DomToRDCInBoundEnggEdgeCount
        {
            get { return 0; }
        }

        public static int DomToRDCOutBoundEnggEdgeCount
        {
            get { return DomToRDCActivityCount; }
        }

        public static int DomToRDCRegularEdgeCount
        {
            get { return DomToRDCActivityCount; }
        }

        public static int DomToRDCHandelingEdgeCount
        {
            get { return DomToRDCActivityCount; }
        }

        public static int ItemSpecDomToRDCRegularEdgeCount
        {
            get { return DomToRDCActivityCount; }
        }

        #endregion

        public int InBoundEnggEdgeCount
        {
            get
            {
                return RDCToStoreInBoundEnggEdgeCount + IDCToRDCInBoundEnggEdgeCount + IntToIDCInBoundEnggEdgeCount +
                       DomToRDCInBoundEnggEdgeCount;
            }
        }

        public int OutBoundEnggEdgeCount
        {
            get
            {
                return RDCToStoreOutBoundEnggEdgeCount + IDCToRDCOutBoundEnggEdgeCount + IntToIDCOutBoundEnggEdgeCount +
                       DomToRDCOutBoundEnggEdgeCount;
            }
        }

        public int RegularEdgeCount
        {
            get
            {
                return RDCToStoreRegularEdgeCount + IDCToRDCRegularEdgeCount + IntToIDCRegularEdgeCount +
                       DomToRDCRegularEdgeCount;
            }
        }

        public int HandelingEdgeCount
        {
            get
            {
                return RDCToStoreHandelingEdgeCount + IDCToRDCHandelingEdgeCount + IntToIDCHandelingEdgeCount +
                       DomToRDCHandelingEdgeCount;
            }
        }

        public int ItemSpecEdgeCount
        {
            get { return ItemSpecIntToIDCRegularEdgeCount + ItemSpecDomToRDCRegularEdgeCount; }
        }

        public int TotalEdgeCount
        {
            get
            {
                return ItemSpecEdgeCount + HandelingEdgeCount + RegularEdgeCount + InBoundEnggEdgeCount +
                       OutBoundEnggEdgeCount;
            }
        }

        #endregion

        #region Capacity Count

        public int InBoundEnggAtRDCCount
        {
            get { return RDCCount; }
        }

        public int InBoundEnggAtStoreCount
        {
            get { return 0; }
        }

        public int InBoundEnggAtIDCCount
        {
            get { return NormalIDCCount + NoCarryIDC; }
        }

        public int OutBoundEnggAtStoreCount
        {
            get { return StoreCount; }
        }

        public int OutBoundEnggAtRDCCount
        {
            get { return RDCCount; }
        }

        public int OutBoundEnggAtIDCCount
        {
            get { return 0; }
        }

        public int RegularFromRDCToStoreCount
        {
            get { return RDCToStoreActivityCount; }
        }

        public int RegularFromIDCToRDCCount
        {
            get { return IDCToRDCActivityCount; }
        }

        public int RegularFromDomToRDCCount
        {
            get { return DomToRDCActivityCount; }
        }

        public int RegularFromIntoIDCCount
        {
            get { return IntToIDCActivityCount; }
        }

        public int ItemSpecRegularFromDomToRDCCount
        {
            get { return DomToRDCActivityCount; }
        }

        public int ItemSpecRegularFromIntToIDCCount
        {
            get { return IntToIDCActivityCount; }
        }

        public int HandelingAtRDCCount
        {
            get { return RDCCount; }
        }

        public int HandelingAtIDCCount
        {
            get { return NormalIDCCount + NoCarryIDC; }
        }

        public int HandelingCount
        {
            get { return HandelingAtIDCCount + HandelingAtRDCCount; }
        }

        public int RegularCount
        {
            get
            {
                return RegularFromRDCToStoreCount / ItemCount + RegularFromIDCToRDCCount / (IntItemCount) + RegularFromIntoIDCCount / (IntItemCount) +
                       RegularFromDomToRDCCount / (DomItemCount);
            }
        }

        public int InBoundEnggCount
        {
            get { return InBoundEnggAtStoreCount + InBoundEnggAtRDCCount + InBoundEnggAtIDCCount; }
        }

        public int OutBoundEnggCount
        {
            get { return OutBoundEnggAtStoreCount + OutBoundEnggAtRDCCount + OutBoundEnggAtIDCCount; }
        }

        public int ItemSpecCapCount
        {
            get { return ItemSpecRegularFromDomToRDCCount + ItemSpecRegularFromIntToIDCCount; }
        }

        public int TotalCapacityCount
        {
            get { return ItemSpecCapCount + RegularCount + InBoundEnggCount + OutBoundEnggCount + HandelingCount; }
        }
        #endregion

        #region Storage

        public int StorageNodesAtRDCCount
        { 
            get { return RDCCount*StorageTypesCount; }
        }

        public int StorageNodesAtIDC
        {
            get { return NormalIDCCount; }
        }

        public int StorageNodeCount
        {
            get { return StorageNodesAtRDCCount + StorageNodesAtIDC; }
        }

        public int StorageEdgeAtRDC
        {
            get { return RDCCount*ItemCount; }
        }

        public int StorageEdgeAtIDC
        {
            get { return NormalIDCCount*IntItemCount; }
        }

        public int StorageEdgeCount
        {
            get { return StorageEdgeAtRDC + StorageEdgeAtIDC; }
        }



        #endregion

        public static int ProductionEdgeCount
        {
            get { return ActivityCount; }
        }

        public static int ConsumptionEdgeCount
        {
            get { return ActivityCount; }
        }

        public static int CapacityEdgeCount
        {
            get
            {
                return RDCToStoreActivityCount * 4 + IDCToRDCActivityCount * 5 + IntToIDCActivityCount * 3 +
                       DomToRDCActivityCount * 4;
            }
        }
        public static int DemandQuantityCount
        {
            get
            {
                return StoreCount * ItemCount * TimeBucketsCount;
            }
        }

        public static int SimultaneousCapacityEdgeCount
        {
            get
            {
                return IntToIDCActivityCount * 2 + IDCToRDCActivityCount * 4 + RDCToStoreActivityCount * 2 + DomToRDCActivityCount * 4;

            }
        }

        public static int WIPCount
        {
            get { return ActivityCount * 4; }

        }
        #endregion
        public static Dictionary<int, string> LocationMap = new Dictionary<int, string>();
        public static Dictionary<int, string> ItemMap = new Dictionary<int, string>();
        public static Dictionary<int, string> ProductMap = new Dictionary<int, string>();
        public static Dictionary<int, string> ActivityMap = new Dictionary<int, string>();
        public static Dictionary<int, string> ResourceMap = new Dictionary<int, string>();
        public static Dictionary<int, string> StorageMap = new Dictionary<int, string>();

        public const string ActRegex = @"^Act-(\d+)-(\d+)$";
        public const string RDCToStoreActRegex = @"^Act-(5\d\d\d)-([1-4]?\d?\d?\d?)$";
        public const string IDCToRDCActRegex = @"^Act-(6\d\d\d)-(5\d\d\d)$";
        public const string IntToIDCActRegex = @"^Act-(7\d\d\d)-(6\d\d\d)$";
        public const string DomToRDCActRegex = @"^Act-(8\d\d\d)-(5\d\d\d)$";

        [ClassInitialize]
        public static void TestInit(TestContext context)
        {
            var settingFile = Path.Combine(AssemblyBaseDirectory, "resources\\walmartDeep.xml");
            InitVariablesFromXml(settingFile);
            const bool generateNew = true;
            var generator = new DataGenerator()
            {
                StoreCount = StoreCount,
                RDCCount = RDCCount,
                IDCCount = IDCCount,
                ItemCount = ItemCount,
                IntItemRatio = IntItemRatio,
                IntSupplierCount = intSuppliersCount,
                DomSupplierCount = domSuppliersCount,
                TimeBucketCount = TimeBucketsCount,
                StorageTypesCount = StorageTypesCount,
                StartDate = StartDate,
                RdcToStoreActCount = (int)(StoreCount * 1.1),
                IdcToRdcActCount = RDCCount * 2,
                OutputDir = OutputDir,
                MaxRecordsPerFile = maxRecordsPerFile,
                FillRate = FillRate,
                FillRateHandlingAtRDC = FillRateHandlingAtRDC,
                FillRateHandlingAtIDC = FillRateHandlingAtIDC,
                FillRateOutBoundEnggAtStore = FillRateOutBoundEnggAtStore,
                FillRateOutBoundEnggAtRDC = FillRateOutBoundEnggAtRDC,
                FillRateInBoundEnggAtRDC = FillRateInBoundEnggAtRDC,
                FillRateInBoundEnggAtIDC = FillRateInBoundEnggAtIDC,
                FillRateRegAtRDCToStore = FillRateRegAtRDCToStore,
                FillRateRegAtIDCToRDC = FillRateRegAtIDCToRDC,
                FillRateRegAtIntToIDC = FillRateRegAtIntToIDC,
                FillRateRegAtDomToRDC = FillRateRegAtDomToRDC,
                FillRateItemSpecRegAtIntToIDC = FillRateItemSpecRegAtIntToIDC,
                FillRateItemSpecRegAtDomToRDC = FillRateItemSpecRegAtDomToRDC,
                DemandTypes = demands
            };

            if (generateNew)
            {
                if (Directory.Exists(OutputDir))
                    Directory.Delete(OutputDir, true);
                Thread.Sleep(1000);
                Directory.CreateDirectory(OutputDir);
                generator.Generate();
            }
            
            ReadDimension("Dimension.Locations-0.csv", ref LocationMap);
            ReadDimension("Dimension.Items-0.csv", ref ItemMap);
            ReadDimension("Dimension.Activities-0.csv", ref ActivityMap);
            ReadDimension("Dimension.Resource-0.csv", ref ResourceMap);
            ReadDimension("Dimension.Storage-0.csv", ref StorageMap);
            ReadDimension("Dimension.Product-0.csv", ref ProductMap);
        }

        public static void ReadDimension(string fileName, ref Dictionary<int, string> _dictionary)
        {
            var path = Path.Combine(OutputDir, fileName);
            using (var sr = new StreamReader(path))
            {
                string line = sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    var arr = line.Split(',');
                    var key = Convert.ToInt32(arr[0]);
                    var name = arr[1];
                    _dictionary.Add(key, name);
                }
            }
        }

        [ClassCleanup]
        public static void TestCleanUp()
        {
            //Directory.Delete(OutputDir);
        }

        [TestMethod]
        public void TestFileExists()
        {
            var itemFilePath = Path.Combine(OutputDir, "Dimension.Items-0.csv");
            Assert.IsTrue(File.Exists(itemFilePath));
        }

        [TestMethod]
        public void TestCapacity()
        {
            int handlingCapCount = 0;
            int inBoundCapCount = 0;
            int outBoundCapCount = 0;
            int regularCapCount = 0;
            int itemSpecCapCount = 0;
            var lineNumber = 0;
            var files = Directory.GetFiles(OutputDir, "Dimension.Resource-*");
            foreach (var file in files)
            {
                var path = Path.Combine(OutputDir, file);
                using (var sr = new StreamReader(path))
                {
                    var line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var arr = line.Split(',');
                        var resKey = Convert.ToInt32(arr[0]);
                        var resource = ResourceMap[resKey];
                        Match match;
                        if ((match = Regex.Match(resource, InBoundEngg)).Success)
                        {
                            inBoundCapCount++;
                        }
                        else if ((match = Regex.Match(resource, OutBoundEngg)).Success)
                        {
                            outBoundCapCount++;
                        }
                        else if ((match = Regex.Match(resource, Handeling)).Success)
                        {
                            handlingCapCount++;
                        }
                        else if ((match = Regex.Match(resource, BODCapacities)).Success)
                        {
                            regularCapCount++;
                        }
                        else if ((match = Regex.Match(resource, ItemResRegx)).Success)
                        {
                            itemSpecCapCount++;
                        }
                        lineNumber++;
                    }
                }

            }
            Assert.AreEqual(InBoundEnggCount, inBoundCapCount);
            Assert.AreEqual(OutBoundEnggCount, outBoundCapCount);
            Assert.AreEqual(RegularCount, regularCapCount);
            Assert.AreEqual(HandelingCount, handlingCapCount);
            Assert.AreEqual(ItemSpecCapCount, itemSpecCapCount);
            Assert.AreEqual(TotalCapacityCount, lineNumber);
        }

        [TestMethod]
        public void TestCapacityAvail()
        {
            int handlingCapCount = 0;
            int inBoundCapCount = 0; 
            int outBoundCapCount = 0;
            int regularCapCount = 0;
            int itemSpecCapCount = 0;
            var lineNumber = 0;
            var files = Directory.GetFiles(OutputDir, "Fact.CapacityAvailability-*");
            foreach (var file in files)
            {
                var path = Path.Combine(OutputDir, file);
                using (var sr = new StreamReader(path))
                {
                    var line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var arr = line.Split(',');
                        var locationKey = Convert.ToInt32(arr[1]);
                        var resKey = Convert.ToInt32(arr[0]);
                        var CapacityAvail = Convert.ToDouble(arr[4]);
                        var ot1 = Convert.ToDouble(arr[5]);
                        var ot2 = Convert.ToDouble(arr[6]);
                        var resource = ResourceMap[resKey];
                        var location = LocationMap[locationKey];
                        Match match;
                        if ((match = Regex.Match(resource, InBoundEngg)).Success)
                        {
                            var fromLocation = match.Groups[1].Value;
                            Assert.AreEqual(fromLocation, location);
                            int loc = Convert.ToInt32(fromLocation);
                            if (loc >= RdcKeyStartIdx && loc < (RdcKeyStartIdx + RDCCount))
                            {
                                Assert.AreEqual(CapacityAvail, InBoundEnggAtRDC);
                                Assert.AreEqual(InBoundEnggAtRDCOt1,ot1);
                                Assert.AreEqual(InBoundEnggAtRDCOt2, ot2);

                            }
                            else if (loc >= IdcKeyStartIdx && loc < (IdcKeyStartIdx + NormalIDCCount) || loc >= NoCarryIdcKeyStartIdx && loc < (NoCarryIdcKeyStartIdx + NoCarryIDC))
                            {
                                Assert.AreEqual(CapacityAvail, InBoundEnggAtIDC);
                                Assert.AreEqual(InBoundEnggAtIDCOt1, ot1);
                                Assert.AreEqual(InBoundEnggAtIDCOt2, ot2);
                            }
                            inBoundCapCount++;
                        }
                        else if ((match = Regex.Match(resource, OutBoundEngg)).Success)
                        {
                            var toLocation = match.Groups[1].Value;
                            int toloc = Convert.ToInt32(toLocation);
                            if (toloc >= RdcKeyStartIdx && toloc < (RdcKeyStartIdx + RDCCount))
                            {
                                Assert.AreEqual(CapacityAvail, OutBoundEnggAtRDC);
                                Assert.AreEqual(OutBoundEnggAtRDCOt1, ot1);
                                Assert.AreEqual(OutBoundEnggAtRDCOt2, ot2);
                            }
                            else if(toloc<(StoreCount))
                            {
                                Assert.AreEqual(CapacityAvail, OutBoundEnggAtStore);
                                Assert.AreEqual(OutBoundEnggAtStoreOt1, ot1);
                                Assert.AreEqual(OutBoundEnggAtStoreOt2, ot2);
                            }
                            Assert.AreEqual(toLocation, location);
                            outBoundCapCount++;
                        }
                        else if ((match = Regex.Match(resource, Handeling)).Success)
                        {
                            Assert.AreEqual(location, match.Groups[1].Value);
                            int loc = Convert.ToInt32(location);
                            if (loc >= RdcKeyStartIdx && loc < (RdcKeyStartIdx + RDCCount))
                            {
                                Assert.AreEqual(CapacityAvail, HandelingAtRDC);
                                Assert.AreEqual(HandelingAtRDCOt1,ot1);
                                Assert.AreEqual(HandelingAtRDCOt2,ot2);
                              
                            }
                            else if((loc>=IdcKeyStartIdx && loc<(IdcKeyStartIdx+NormalIDCCount)) || (loc>=NoCarryIdcKeyStartIdx && loc<(NoCarryIdcKeyStartIdx+NoCarryIDC)))
                            {
                                Assert.AreEqual(HandelingAtIDCOt1,ot1);
                                Assert.AreEqual(HandelingAtIDCOt2,ot2);
                                Assert.AreEqual(CapacityAvail,HandelingAtIDC);
                            }
                            handlingCapCount++;
                        }
                        else if ((match = Regex.Match(resource, BODCapacities)).Success)
                        {
                            Assert.AreEqual(location, match.Groups[2].Value);
                            var fromloc = Convert.ToInt32(match.Groups[1].Value);
                            var toloc = Convert.ToInt32(match.Groups[1].Value);
                            if (fromloc >= IntSuppKeyStartIdx && fromloc < (IntSuppKeyStartIdx + intSuppliersCount) &&
                                (toloc >= IdcKeyStartIdx && toloc < (IdcKeyStartIdx + NormalIDCCount) ||
                                toloc >= NoCarryIdcKeyStartIdx && toloc < (NoCarryIdcKeyStartIdx + NoCarryIDC)))
                            {
                                
                                Assert.AreEqual(CapacityAvail, RegAtIntToIDC);
                                Assert.AreEqual(RegAtIntToIDCOt1,ot1);
                                Assert.AreEqual(RegAtIntToIDCOt2,ot2);
                            }
                            else if (fromloc >= IdcKeyStartIdx && fromloc < (IdcKeyStartIdx + NormalIDCCount) ||
                                     fromloc >= NoCarryIdcKeyStartIdx && fromloc < (NoCarryIdcKeyStartIdx + NoCarryIDC) && toloc >= RdcKeyStartIdx && toloc < (RdcKeyStartIdx + RDCCount))
                            {
                                Assert.AreEqual(CapacityAvail, RegAtIDCToRDC);
                                Assert.AreEqual(RegAtIDCToRDCOt1, ot1);
                                Assert.AreEqual(RegAtIDCToRDCOt2, ot2);
                            }
                            else if (fromloc >= RdcKeyStartIdx && fromloc < (RdcKeyStartIdx + RDCCount)&& toloc>=0 && toloc<StoreCount)
                            {
                                Assert.AreEqual(CapacityAvail,RegAtRDCToStore);
                                Assert.AreEqual(RegAtRDCToStoreOt1, ot1);
                                Assert.AreEqual(RegAtRDCToStoreOt2, ot2);
                            }
                            else if (fromloc >= DomSuppKeyStartIdx && fromloc < DomSuppKeyStartIdx + domSuppliersCount && toloc >= RdcKeyStartIdx && toloc < (RdcKeyStartIdx + RDCCount))
                            {
                                Assert.AreEqual(CapacityAvail,RegAtDomToRDC);
                                Assert.AreEqual(RegAtDomToRDCOt1, ot1);
                                Assert.AreEqual(RegAtDomToRDCOt2, ot2);
                            }
                            regularCapCount++;
                        }
                        else if ((match = Regex.Match(resource, ItemResRegx)).Success)
                        {
                            Assert.AreEqual(location, match.Groups[3].Value);
                            int loc = Convert.ToInt32(match.Groups[3].Value);
                            if (loc >= RdcKeyStartIdx && loc < (RdcKeyStartIdx + RDCCount))
                            {
                                Assert.AreEqual(CapacityAvail, ItemSpecRegAtDomToRDC);
                                Assert.AreEqual(ItemSpecRegAtDomToRDCOt1, ot1);
                                Assert.AreEqual(ItemSpecRegAtDomToRDCOt2, ot2);
                            }
                            else if((loc >= IdcKeyStartIdx && loc < (IdcKeyStartIdx + NormalIDCCount))||(loc >= NoCarryIdcKeyStartIdx && loc < (NoCarryIdcKeyStartIdx + NoCarryIDC)))
                            {
                                Assert.AreEqual(CapacityAvail, ItemSpecRegAtIntToIDC);
                                Assert.AreEqual(ItemSpecRegAtIntToIDCOt1, ot1);
                                Assert.AreEqual(ItemSpecRegAtIntToIDCOt2, ot2);
                            }
                            itemSpecCapCount++;
                        }
                        lineNumber++;
                    }
                }

            }
            Assert.AreEqual(InBoundEnggCount, inBoundCapCount / TimeBucketsCount);
            Assert.AreEqual(OutBoundEnggCount, outBoundCapCount / TimeBucketsCount);
            Assert.AreEqual(RegularCount, regularCapCount / TimeBucketsCount);
            Assert.AreEqual(HandelingCount, handlingCapCount / TimeBucketsCount);
            Assert.AreEqual(ItemSpecCapCount, itemSpecCapCount / TimeBucketsCount);
            Assert.AreEqual(TotalCapacityCount, lineNumber / TimeBucketsCount);
        }

        [TestMethod]
        public void TestActivity()
        {
            var rdcToStoreActCount = 0;
            var idcToRDCActCount = 0;
            var domToRDCActCount = 0;
            var intToIDCActCount = 0;
            
            var files = Directory.GetFiles(OutputDir, "Dimension.Activities-*");
            foreach (var file in files)
            {
                var path = Path.Combine(OutputDir, file);
                using (var sr = new StreamReader(path))
                {
                    var line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var arr = line.Split(',');
                        var resKey = Convert.ToInt32(arr[0]);
                        var activity = ActivityMap[resKey];
                        Match match;
                        if ((match = Regex.Match(activity, ActRegex)).Success)
                        {
                            if ((match = Regex.Match(activity, RDCToStoreActRegex)).Success)
                            {
                                rdcToStoreActCount++;
                            }
                            else if ((match = Regex.Match(activity, IDCToRDCActRegex)).Success)
                            {
                                idcToRDCActCount++;
                            }
                            else if ((match = Regex.Match(activity, IntToIDCActRegex)).Success)
                            {
                                intToIDCActCount++;
                            }
                            else if ((match = Regex.Match(activity, DomToRDCActRegex)).Success)
                            {
                                domToRDCActCount++;
                            }
                        }
                        else
                        {
                            Assert.IsTrue("BOH Activity" == activity || "Expected Receipt Activity" == activity);
                        }

                    }
                }
            } 
            Assert.AreEqual(RDCToStoreActivityCount / ItemCount, rdcToStoreActCount);
            Assert.AreEqual(IDCToRDCActivityCount / IntItemCount, idcToRDCActCount);
            Assert.AreEqual(IntToIDCActivityCount / IntItemCount, intToIDCActCount);
            Assert.AreEqual(DomToRDCActivityCount / DomItemCount, domToRDCActCount);
        }

        [TestMethod]
        public void TestActivityParameters()
        {
            var rdcToStoreActCount = 0;
            var idcToRDCActCount = 0;
            var domToRDCActCount = 0;
            var intToIDCActCount = 0;
            var total = 0;
            var files = Directory.GetFiles(OutputDir, "Fact.ActivityParameters-*");
            foreach (var file in files)
            {
                var path = Path.Combine(OutputDir, file);
                using (var sr = new StreamReader(path))
                {
                    var line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var arr = line.Split(',');
                        var resKey = Convert.ToInt32(arr[0]);
                        var activity = ActivityMap[resKey];
                        Match match;
                        if ((match = Regex.Match(activity, ActRegex)).Success)
                        {
                            if ((match = Regex.Match(activity, RDCToStoreActRegex)).Success)
                            {
                                rdcToStoreActCount++;
                            }
                            else if ((match = Regex.Match(activity, IDCToRDCActRegex)).Success)
                            {
                                idcToRDCActCount++;
                            }
                            else if ((match = Regex.Match(activity, IntToIDCActRegex)).Success)
                            {
                                intToIDCActCount++;
                            }
                            else if ((match = Regex.Match(activity, DomToRDCActRegex)).Success)
                            {
                                domToRDCActCount++;
                            }
                        }
                        else
                        {
                            Assert.IsTrue("BOH Activity" == activity || "Expected Receipt Activity" == activity);
                        }
                        total++;
                    }
                }
            }
            Assert.AreEqual(RDCToStoreActivityCount, rdcToStoreActCount);
            Assert.AreEqual(IDCToRDCActivityCount, idcToRDCActCount);
            Assert.AreEqual(IntToIDCActivityCount, intToIDCActCount);
            Assert.AreEqual(DomToRDCActivityCount, domToRDCActCount);
            Assert.AreEqual(ActivityCount, total);
        }

        [TestMethod]
        public void TestCapacityConsumptionGraph()
        {
            var lineNumber = 0;
            var handelingEdgeCount = 0;
            var inBoundEnggEdgeCount = 0;
            var outBoundEnggEdgeCount = 0;
            var regularEdgeCount = 0;
            var itemSpecEdgeCount = 0;
            var files = Directory.GetFiles(OutputDir, "Fact.CapacityConsumptionGraph-*");
            foreach (var file in files)
            {
                var path = Path.Combine(OutputDir, file);
                using (var sr = new StreamReader(path))
                {
                    var line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var arr = line.Split(',');
                        var actKey = Convert.ToInt32(arr[0]);
                        var resKey = Convert.ToInt32(arr[3]);
                        var locKey = Convert.ToInt32(arr[4]);
                        var capCat = arr[6];

                        var activity = ActivityMap[actKey];
                        var match = Regex.Match(activity, ActRegex);
                        Assert.IsTrue(match.Success);

                        var fromLocation = match.Groups[1].Value;
                        var toLocation = match.Groups[2].Value;
                        var resLocation = LocationMap[locKey];
                        var resource = ResourceMap[resKey];
                        if ((match = Regex.Match(resource, InBoundEngg)).Success)
                        {
                            var location = match.Groups[1].Value;
                            Assert.AreEqual(fromLocation, location);
                            inBoundEnggEdgeCount++;
                        }
                        else if ((match = Regex.Match(resource, OutBoundEngg)).Success)
                        {
                            var location = match.Groups[1].Value;
                            Assert.AreEqual(toLocation, location);
                            outBoundEnggEdgeCount++;
                        }
                        else if ((match = Regex.Match(resource, Handeling)).Success)
                        {
                            var location = match.Groups[1].Value;
                            switch (capCat)
                            {
                                case "O":
                                    Assert.AreEqual(location, toLocation);
                                    break;
                                case "I":
                                    Assert.AreEqual(location, fromLocation);
                                    break;
                                default:
                                    Assert.Fail();
                                    break;
                            }
                            Assert.AreEqual(resLocation, location);
                            handelingEdgeCount++;
                        }
                        else if ((match = Regex.Match(resource, BODCapacities)).Success)
                        {
                            Assert.AreEqual(fromLocation, match.Groups[1].Value);
                            Assert.AreEqual(toLocation, match.Groups[2].Value);
                            Assert.AreEqual(resLocation, match.Groups[2].Value);
                            regularEdgeCount++;
                        }
                        else if ((match = Regex.Match(resource, ItemResRegx)).Success)
                        {
                            var loc = match.Groups[3].Value;
                            Assert.IsTrue(toLocation == loc);
                            Assert.AreEqual(resLocation, match.Groups[3].Value);
                            itemSpecEdgeCount++;
                        }
                        lineNumber++;
                    }
                }

            }
            Assert.AreEqual(RegularEdgeCount, regularEdgeCount);
            Assert.AreEqual(InBoundEnggEdgeCount, inBoundEnggEdgeCount);
            Assert.AreEqual(OutBoundEnggEdgeCount, outBoundEnggEdgeCount);
            Assert.AreEqual(HandelingEdgeCount, handelingEdgeCount);
            Assert.AreEqual(ItemSpecEdgeCount, itemSpecEdgeCount);
            Assert.AreEqual(CapacityEdgeCount, lineNumber);
        }

        [TestMethod]
        public void TestProdcutionGraph()
        {
            var lineNumber = 0;
            var files = Directory.GetFiles(OutputDir, "Fact.MaterialProductionGraph-*");
            foreach (var file in files)
            {
                var path = Path.Combine(OutputDir, file);
                using (var sr = new StreamReader(path))
                {
                    
                    var line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var arr = line.Split(',');
                        var locationKey = Convert.ToInt32(arr[4]);
                        var actKey = Convert.ToInt32(arr[0]);
                        var itemKey = Convert.ToInt32(arr[3]);
                        var prodKey = Convert.ToInt32(arr[2]);
                        //var noBuild = Convert.ToBoolean(arr[11]);

                        var location = LocationMap[locationKey];
                        var item = ItemMap[itemKey];
                        var activity = ActivityMap[actKey];
                        var product = ProductMap[prodKey];

                        Assert.AreEqual(product, item);

                        var match = Regex.Match(activity, ActRegex);
                        Assert.IsTrue(match.Success);
                        var toLocation = match.Groups[2].Value;
                        Assert.AreEqual(toLocation, location);
                        lineNumber++;
                    }
                }
                Assert.AreEqual(ProductionEdgeCount, lineNumber);
            }
        }

        [TestMethod]
        public void TestConsumptionGraph()
        {
            var lineNumber = 0;
            var files = Directory.GetFiles(OutputDir, "Fact.MaterialConsumptionGraph-*");
            foreach (var file in files)
            {
                var path = Path.Combine(OutputDir, file);
                using (var sr = new StreamReader(path))
                {
                    var line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var arr = line.Split(',');
                        var locationKey = Convert.ToInt32(arr[4]);
                        var actKey = Convert.ToInt32(arr[0]);
                        var itemKey = Convert.ToInt32(arr[3]);
                        var prodKey = Convert.ToInt32(arr[2]);

                        var location = LocationMap[locationKey];
                        var item = ItemMap[itemKey];
                        var activity = ActivityMap[actKey];
                        var product = ProductMap[prodKey];

                        Assert.AreEqual(product, item);

                        var match = Regex.Match(activity, ActRegex);
                        Assert.IsTrue(match.Success);
                        var fromLocation = match.Groups[1].Value;
                        Assert.AreEqual(fromLocation, location);
                        lineNumber++;
                    }
                    
                }
            }
            Assert.AreEqual(ConsumptionEdgeCount, lineNumber);
        }

        [TestMethod]
        public void SimultaneousCapacityConsumptionGraphTest()
        {
            var lineNumber = 0;
            var files = Directory.GetFiles(OutputDir, "Fact.SimultaneousCapacityConsumptionGraph-*");
            foreach (var file in files)
            {
                var path = Path.Combine(OutputDir, file);
                using (var sr = new StreamReader(path))
                {
                    string line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var arr = line.Split(',');
                        var actKey = Convert.ToInt32(arr[1]);
                        var resKey = Convert.ToInt32(arr[4]);
                        var locKey = Convert.ToInt32(arr[5]);
                        var groupKey = Convert.ToInt32(arr[0]);

                        Assert.IsTrue(groupKey >= 0 && groupKey < 368);
                        var activity = ActivityMap[actKey];
                        var match = Regex.Match(activity, ActRegex);
                        Assert.IsTrue(match.Success);

                        var fromLocation = match.Groups[1].Value;
                        var toLocation = match.Groups[2].Value;
                        var resLocation = LocationMap[locKey];

                        var resource = ResourceMap[resKey];
                        if ((match = Regex.Match(resource, InBoundEngg)).Success)
                        {
                            var location = match.Groups[1].Value;
                            Assert.AreEqual(fromLocation, location);
                        }
                        else if ((match = Regex.Match(resource, OutBoundEngg)).Success)
                        {
                            var location = match.Groups[1].Value;
                            Assert.AreEqual(toLocation, location);
                        }
                        else if ((match = Regex.Match(resource, Handeling)).Success)
                        {
                            var location = match.Groups[1].Value;
                            Assert.AreEqual(resLocation, location);
                        }
                        else if ((match = Regex.Match(resource, BODCapacities)).Success)
                        {
                            Assert.AreEqual(fromLocation, match.Groups[1].Value);
                            Assert.AreEqual(toLocation, match.Groups[2].Value);
                        }
                        else if ((match = Regex.Match(resource, ItemResRegx)).Success)
                        {
                            var loc = match.Groups[3].Value;
                            Assert.IsTrue(toLocation == loc);
                        }
                        lineNumber++;
                    }

                }
            }
            Assert.AreEqual(SimultaneousCapacityEdgeCount,lineNumber);
        }

        [TestMethod]
        public void TestStorageAvail()
        {
            var count = 0;
            var files = Directory.GetFiles(OutputDir, "Fact.StorageAvailability-*");
            foreach (var file in files)
            {
                var path = Path.Combine(OutputDir, file);
                using (var sr = new StreamReader(path))
                {
                    var line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var arr = line.Split(',');
                        var stgKey = Convert.ToInt32(arr[0]);
                        var storage = StorageMap[stgKey];
                        Assert.IsNotNull(storage);
                        var storageAvail = Convert.ToDouble(arr[4]);
                        var band1_storage = Convert.ToDouble(arr[5]);
                        var band2_storage = Convert.ToDouble(arr[6]);
                        if (stgKey >= IdcKeyStartIdx && stgKey < (IdcKeyStartIdx + NormalIDCCount))
                        {
                            Assert.AreEqual(storageAvail,RegStorageAtIDC);
                            Assert.AreEqual(band1_storage, Ot1StorageAtIDC);
                            Assert.AreEqual(band2_storage,Ot2StorageAtIDC);
                        }
                        else if (stgKey >= RdcKeyStartIdx && stgKey<RdcKeyStartIdx+RDCCount) 
                        {
                            Assert.AreEqual(storageAvail,RegStorageAtRDC);
                            Assert.AreEqual(band1_storage, Ot1StorageAtRDC);
                            Assert.AreEqual(band2_storage, Ot2StorageAtRDC);
                        }
                        count++;
                    }
                }
            }
            Assert.AreEqual(StorageNodeCount, count/TimeBucketsCount);
        }

        [TestMethod]
        public void TestStorageGraph()
        {
            var count = 0;
            var files = Directory.GetFiles(OutputDir, "Fact.StorageGraph-*");
            foreach (var file in files)
            {
                var path = Path.Combine(OutputDir, file);
                using (var sr = new StreamReader(path))
                {
                    var line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var arr = line.Split(',');
                        var stgKey = Convert.ToInt32(arr[0]);
                        var prodKey = Convert.ToInt32(arr[1]);
                        var itemKey = Convert.ToInt32(arr[2]);
                        var locKey = Convert.ToInt32(arr[3]);

                        var storage = StorageMap[stgKey];
                        var prod = ProductMap[prodKey];
                        var item = ItemMap[itemKey];
                        var loc = LocationMap[locKey];
                        Match match;
                        if ((match = Regex.Match(storage, RDCStorageRegex)).Success)
                        {
                            var strgLoc = match.Groups[1].Value;
                            Assert.AreEqual(loc, strgLoc);
                            Assert.IsTrue(5000 <= stgKey);
                            Assert.IsTrue(6000 > stgKey);
                            Assert.AreEqual(prodKey, itemKey%StorageTypesCount + DomItemCount);
                        }
                        else if ((match = Regex.Match(storage, IDCStorageRegex)).Success)
                        {
                            var strgLoc = match.Groups[1].Value;
                            Assert.AreEqual(loc, strgLoc);
                            Assert.IsTrue(6000 <= stgKey);
                            Assert.IsTrue(7000 > stgKey);
                            Assert.AreEqual(DomItemCount, prodKey);
                        } 
                        else
                        {
                            Assert.Fail("Invalid Storage");
                        }
                        count++;
                    }
                }
            }
            Assert.AreEqual(StorageEdgeCount, count);
        }
        [TestMethod]
        public void TestLocations()
        {

            var path = Path.Combine(OutputDir, "Dimension.Locations-0.csv");
             HashSet<int> locations = new HashSet<int>();
             using (var sr = new StreamReader(path))
             {
                 var line = sr.ReadLine();
                 while ((line = sr.ReadLine()) != null)
                 {
                     var arr = line.Split(',');
                     int locKey = Convert.ToInt32(arr[0]);
                     int loc = Convert.ToInt32(arr[1]);
                     locations.Add(loc);
                     Assert.IsTrue(locKey == loc);
                     
                 }
             }
            for(int i=StoreKeyStartIdx;i<StoreCount;i++)
            {
                Assert.IsTrue(locations.Contains(i));
            }
            for (int i = IdcKeyStartIdx; i < IDCCount; i++)
            {
                Assert.IsTrue(locations.Contains(i));
            }
            for (int i = NoCarryIdcKeyStartIdx; i < NoCarryIDC; i++)
            {
                Assert.IsTrue(locations.Contains(i));
            }
            for (int i = RdcKeyStartIdx; i < RDCCount; i++)
            {
                Assert.IsTrue(locations.Contains(i));
            }
            
        }
        
        [TestMethod]
        public void TestDemandQuantity()
        {
            int i = 0;
            var lineNumber = 0;
            HashSet<String> time = new HashSet<string>();
            HashSet<String> locations = new HashSet<string>();
            Dictionary<int, int> dict = new Dictionary<int, int>();
            Dictionary<DateTime, int> Timedict = new Dictionary<DateTime, int>();
            DateTime tempDate = StartDate;
            for (int j = 0; j < TimeBucketsCount; j++)
            {
                Timedict.Add(tempDate,j);
                tempDate=tempDate.AddDays(7);
            }
            var files = Directory.GetFiles(OutputDir, "Fact.DemandQuantity-*");
            foreach (var file in files)
            {
                var path = Path.Combine(OutputDir,file);

                using (var sr = new StreamReader(path))
                {
                    string line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var arr = line.Split(',');
                        var orderkey = Convert.ToInt32(arr[0]);
                        var timeKey = arr[4];
                        var locationKey = arr[3];
                        string[] daymonthyear = Regex.Split(arr[4], "-");
                        int day, month, year;
                        year = Convert.ToInt32(daymonthyear[0]);
                        month = Convert.ToInt32(daymonthyear[1]); 
                        day = Convert.ToInt32(daymonthyear[2]);
                        DateTime CurrDate = new DateTime(year, month, day);
                        int currBucketNo = Timedict[CurrDate];
                        int calchunk = TimeBucketsCount*orderkey+currBucketNo;
                        int chunk = Convert.ToInt32(arr[10]);
                        Assert.AreEqual(chunk,calchunk);
                        var demandpriority = Convert.ToInt32(arr[8]);
                        var DemandQty = Convert.ToInt32(arr[9]);
                        Assert.AreEqual(orderkey,demandpriority);
                        Assert.AreEqual(0, DemandQty % 10);
                        Assert.IsTrue(DemandQty>=10 && DemandQty<=200);
                        locations.Add(locationKey);
                        time.Add(timeKey);
                        lineNumber++;
                    }
                }
                i++;
            }
           
            Assert.AreEqual(time.Count, TimeBucketsCount);
            Assert.AreEqual(locations.Count, StoreCount);
            Assert.AreEqual(DemandQuantityCount, lineNumber);

        }
        [TestMethod]
        public void TestStorage()
        {
            var path = Path.Combine(OutputDir,"Dimension.Storage-0.csv");
            
                 using (var sr = new StreamReader(path))
             {
                 var line = sr.ReadLine();
                 while ((line = sr.ReadLine()) != null)
                 {
                      var arr=line.Split(',');
                      var StorageKey=arr[0];
                      var StorageName=arr[1];
                      var match=Regex.Match(StorageName, StorageRegx);
                      Assert.IsTrue(match.Success);
                      string[] storages=Regex.Split(StorageName,"-");
                      int LocKey = Convert.ToInt16(storages[1]);
                      Assert.IsTrue((LocKey >= 6000 && LocKey <= 6005) || (LocKey >= 5000 && LocKey <= 5080));
                 }
                 }

        }

        [TestMethod]
        public void TestWIP()
        {
            int i = 0;
            var lineNumber = 0;
   
            var files = Directory.GetFiles(OutputDir, "Fact.MaterialProductionGraphPlannedWIP-*");
            foreach (var file in files)
            {
                var path = Path.Combine(OutputDir, file);

                using (var sr = new StreamReader(path))
                {
                    string line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var arr = line.Split(',');
                        int actKey = Convert.ToInt32(arr[2]);
                        var activity = ActivityMap[actKey];
                        var match = Regex.Match(activity, ActRegex);
                        Assert.IsTrue(match.Success);

                        var location = Convert.ToInt32(arr[1]);
                        var toLocation = Convert.ToInt32(match.Groups[2].Value);
                        Assert.AreEqual(toLocation,location);
                        lineNumber++;
                    }
                }
                i++;
            }
            Assert.AreEqual(WIPCount,lineNumber);

        }
        

        public const string RDCStorageRegex = @"^DC-(5\d\d\d)$";
        public const string IDCStorageRegex = @"^DC-(6\d\d\d)$";
        public const string InBoundEngg = @"^Res-(\d+)-I$";
        public const string OutBoundEngg = @"^Res-(\d+)-O$";
        public const string BODCapacities = @"^Res-(\d+)-(\d+)$";
        public const string Handeling = @"^Res-(\d+)$";
        public const string ItemResRegx = @"^Res-(\d+)-(\d+)-(\d+)$";

    }
}
