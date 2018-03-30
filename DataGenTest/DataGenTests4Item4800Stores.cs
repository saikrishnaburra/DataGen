using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataGen;
using System.Xml;

namespace DataGenTest
{
    [TestClass]
    public class DataGenTests4Item4800Stores
    {
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

        public static string OutputDir
        {
            get { return Path.Combine(AssemblyBaseDirectory, "output4800"); }
        }

        public static int ItemCount = 4;
        public static int StoreCount = 4800;
        public static double ItemDomRatio = .25;
        public static int RDCCount = 80;
        public static int IDCCount = 6;
        public static int NoCarryIDC = 6;
        public static int TimeBucketsCount = 53;

        #region Calulated Expects
        public static int DomItemCount
        {
            get { return (int) (ItemCount * ItemDomRatio); }
        }

        public static int IntItemCount
        {
            get { return (int)(ItemCount * (1-ItemDomRatio)); }
        }

        public static int RDCToStoreActivityCount
        {
            get { return (int) (ItemCount*StoreCount*1.1); }
        }

        public static int IDCToRDCActivityCount
        {
            get { return 2 * RDCCount * IntItemCount; }
        }

        public static int DomToRDCActivityCount
        {
            get { return RDCCount; }
        }

        public static int IntToIDCActivityCount
        {
            get { return (NoCarryIDC + IDCCount)*IntItemCount; }
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
            get { return IDCCount + NoCarryIDC; }
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
            get { return IDCCount + NoCarryIDC; }
        }

        public int HandelingCount
        {
            get { return HandelingAtIDCCount + HandelingAtRDCCount; }
        }

        public int RegularCount
        {
            get
            {
                return RegularFromRDCToStoreCount/ItemCount + RegularFromIDCToRDCCount/(IntItemCount) + RegularFromIntoIDCCount/(IntItemCount) +
                       RegularFromDomToRDCCount/(DomItemCount);
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
                return RDCToStoreActivityCount*4 + IDCToRDCActivityCount*5 + IntToIDCActivityCount*3 +
                       DomToRDCActivityCount*4;
            }
        }
#endregion

        public static Dictionary<int, string> LocationMap = new Dictionary<int, string>();
        public static Dictionary<int, string> ItemMap = new Dictionary<int, string>();
        public static Dictionary<int, string> ProductMap = new Dictionary<int, string>(); 
        public static Dictionary<int, string> ActivityMap = new Dictionary<int, string>();
        public static Dictionary<int, string> ResourceMap = new Dictionary<int, string>();
        public static Dictionary<int, string> StorageMap = new Dictionary<int, string>();

        public const string ActRegex = @"^Act-(\d+)-(\d+)$";
           
        [ClassInitialize]
        public static void TestInit(TestContext context)
        {
            //generate here
            var scriptPath = Path.Combine(AssemblyBaseDirectory, "resources\\walmartDeep.ps1");
            var settingFile = Path.Combine(AssemblyBaseDirectory, "resources\\walmartDeep.xml");

            Program.RunScript(scriptPath, settingFile, OutputDir);

            ReadDimension("Dimension.Locations.csv", ref LocationMap);
            ReadDimension("Dimension.Items.csv", ref ItemMap);
            ReadDimension("Dimension.Activities.csv", ref ActivityMap);
            ReadDimension("Dimension.Resource.csv", ref ResourceMap);
            ReadDimension("Dimension.Storage.csv", ref StorageMap);
            ReadDimension("Dimension.Product.csv", ref ProductMap);
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

        [TestMethod]
        public void TestItemFileExists()
        {
            var itemFilePath = Path.Combine(OutputDir, "Dimension.Items.csv");
            Assert.IsTrue(File.Exists(itemFilePath));
        }

        [TestMethod]
        public void TestCapacityConsumptionGraph()
        {
            var path = Path.Combine(OutputDir, "Fact.CapacityConsumptionGraph-0.csv");
            var lineNumber = 0;
            var handelingEdgeCount = 0;
            var inBoundEnggEdgeCount = 0;
            var outBoundEnggEdgeCount = 0;
            var regularEdgeCount = 0;
            var itemSpecEdgeCount = 0;
            using (var sr = new StreamReader(path))
            {
                string line = sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    var arr = line.Split(',');
                    var locationKey = Convert.ToInt32(arr[0]);
                    var actKey = Convert.ToInt32(arr[1]);
                    var resKey = Convert.ToInt32(arr[3]);
                    var prodKey = Convert.ToInt32(arr[5]);
                    var capCat = arr[6];

                    var activity = ActivityMap[actKey];
                    var match = Regex.Match(activity, ActRegex);
                    Assert.IsTrue(match.Success);

                    var fromLocation = match.Groups[1].Value;
                    var toLocation = match.Groups[2].Value;

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
                        handelingEdgeCount++;
                    }
                    else if ((match = Regex.Match(resource, BODCapacities)).Success)
                    {
                        Assert.AreEqual(fromLocation, match.Groups[1].Value);
                        Assert.AreEqual(toLocation, match.Groups[2].Value);
                        regularEdgeCount++;
                    }
                    else if ((match = Regex.Match(resource, ItemResRegx)).Success)
                    {
                        var loc = match.Groups[3].Value;
                        Assert.IsTrue(toLocation == loc);
                        itemSpecEdgeCount++;
                    }
                    lineNumber++;
                }
                Assert.AreEqual(CapacityEdgeCount, lineNumber);
                Assert.AreEqual(RegularEdgeCount, regularEdgeCount);
                Assert.AreEqual(InBoundEnggEdgeCount, inBoundEnggEdgeCount);
                Assert.AreEqual(OutBoundEnggEdgeCount, outBoundEnggEdgeCount);
                Assert.AreEqual(HandelingEdgeCount, handelingEdgeCount);
                Assert.AreEqual(ItemSpecEdgeCount, itemSpecEdgeCount);
            }
        }

        [TestMethod]
        public void TestProdcutionGraph()
        {
            var path = Path.Combine(OutputDir, "Fact.MaterialProductionGraph-0.csv");
            using (var sr = new StreamReader(path))
            {
                var lineNumber = 0;
                var line = sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {

                    var arr = line.Split(',');
                    var locationKey = Convert.ToInt32(arr[0]);
                    var actKey = Convert.ToInt32(arr[1]);
                    var itemKey = Convert.ToInt32(arr[2]);
                    var prodKey = Convert.ToInt32(arr[5]);
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
                Assert.AreEqual(ProductionEdgeCount, lineNumber);
            }
        }

        [TestMethod]
        public void TestConsumptionGraph()
        {
            var path = Path.Combine(OutputDir, "Fact.MaterialConsumptionGraph-0.csv");
            using (var sr = new StreamReader(path))
            {
                var lineNumber = 0;
                var line = sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {

                    var arr = line.Split(',');
                    var locationKey = Convert.ToInt32(arr[0]);
                    var actKey = Convert.ToInt32(arr[1]);
                    var itemKey = Convert.ToInt32(arr[2]);
                    var prodKey = Convert.ToInt32(arr[5]);
                    //var noBuild = Convert.ToBoolean(arr[11]);

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
                Assert.AreEqual(ConsumptionEdgeCount, lineNumber);
            }
        }

        [TestMethod]
        public void TestCapacity()
        {
            var path = Path.Combine(OutputDir, "Fact.CapacityAvailability-0.csv");
            using (var sr = new StreamReader(path))
            {
                int handlingCapCount = 0;
                int inBoundCapCount = 0;
                int outBoundCapCount = 0;
                int regularCapCount = 0;
                int itemSpecCapCount = 0;
                var lineNumber = 0;
                var line = sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    var arr = line.Split(',');
                    var locationKey = Convert.ToInt32(arr[1]);
                    
                    var resKey = Convert.ToInt32(arr[3]);

                    var resource = ResourceMap[resKey];
                    var location = LocationMap[locationKey];
                    Match match;
                    if ((match = Regex.Match(resource, InBoundEngg)).Success)
                    {
                        var fromLocation = match.Groups[1].Value;
                        Assert.AreEqual(fromLocation, location);
                        inBoundCapCount++;
                    }
                    else if ((match = Regex.Match(resource, OutBoundEngg)).Success)
                    {
                        var toLocation = match.Groups[1].Value;
                        Assert.AreEqual(toLocation, location);
                        outBoundCapCount++;
                    }
                    else if ((match = Regex.Match(resource, Handeling)).Success)
                    {
                        Assert.AreEqual(location, match.Groups[1].Value);
                        handlingCapCount++;
                    }
                    else if ((match = Regex.Match(resource, BODCapacities)).Success)
                    {
                        Assert.AreEqual(location, match.Groups[2].Value);
                        regularCapCount++;
                    }
                    else if ((match = Regex.Match(resource, ItemResRegx)).Success)
                    {
                        Assert.AreEqual(location, match.Groups[3].Value);
                        itemSpecCapCount++;
                    }
                    lineNumber++;
                }
                Assert.AreEqual(InBoundEnggCount, inBoundCapCount/TimeBucketsCount);
                Assert.AreEqual(OutBoundEnggCount, outBoundCapCount / TimeBucketsCount);
                Assert.AreEqual(RegularCount, regularCapCount / TimeBucketsCount);
                Assert.AreEqual(HandelingCount, handlingCapCount / TimeBucketsCount);
                Assert.AreEqual(ItemSpecCapCount, itemSpecCapCount/TimeBucketsCount);
                Assert.AreEqual(TotalCapacityCount, lineNumber/TimeBucketsCount);
            }
        }

        [ClassCleanup]
        public static void TestCleanUp()
        {
            //Directory.Delete(OutputDir);
        }

        [TestMethod]
        public void SimultaneousCapacityConsumptionGraphTest()
        {
            var path = Path.Combine(OutputDir, "Fact.SimultaneousCapacityConsumptionGraph-0.csv");
            var lineNumber = 0;
            using (var sr = new StreamReader(path))
            {
                 string line = sr.ReadLine();
                 while ((line = sr.ReadLine()) != null)
                 {
                     var arr = line.Split(',');
                     var actKey=Convert.ToInt32(arr[0]);
                     var resKey=Convert.ToInt32(arr[1]);
                     var locKey=Convert.ToInt32(arr[4]);
                     var groupKey=Convert.ToInt32(arr[6]);
                     Assert.IsTrue(groupKey>=0 && groupKey <368);
                     var activity = ActivityMap[actKey];
                    var match = Regex.Match(activity, ActRegex);
                    Assert.IsTrue(match.Success);
                    
                    var fromLocation = match.Groups[1].Value;
                    var toLocation = match.Groups[2].Value;

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
                        
                    }
                    else if ((match = Regex.Match(resource, BODCapacities)).Success)
                    {
                        Assert.AreEqual(fromLocation, match.Groups[1].Value);
                        Assert.AreEqual(toLocation, match.Groups[2].Value);
                    }
                    else if((match=Regex.Match(resource,ItemResRegx)).Success)
                    {
                        var loc=match.Groups[3].Value;
                        Assert.IsTrue(toLocation==loc);
                    }
                    lineNumber++;
                 }

            }
        }
      

        public const string InBoundEngg = @"^Res-(\d+)-I$";
        public const string OutBoundEngg = @"^Res-(\d+)-O$";
        public const string BODCapacities = @"^Res-(\d+)-(\d+)$";
        public const string Handeling = @"^Res-(\d+)$";
        public const string ItemResRegx=@"^Res-(\d+)-(\d+)-(\d+)$";
    
    }
}
