using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DataGen
{
    public class DataGenerator 
    {
        public string OutputDir;
        public long MaxRecordsPerFile;
        public int ItemCount { get; set; }
        public int StorageTypesCount { get; set; }
        public double IntItemRatio { get; set; }
        public Random Random = new Random(0);

        public DataGenerator()
        {
            FillRate = 1.0;
            NumChunksPerPriority = 7;
            MaxRecordsPerFile = 5*1000*1000;
        }

        public int DomItemCount
        {
            get { return (int) (ItemCount*(1 - IntItemRatio)); }
        }

        public int IntItemCount
        {
            get { return (int) (ItemCount*IntItemRatio); }

        }
        public  int DemandTypes { get; set; }
        public int StoreCount { get; set; }
        public int RDCCount { get; set; }
        public int IDCCount { get; set; }
        public int IntSupplierCount { get; set; }
        public int DomSupplierCount { get; set; }
        public int TimeBucketCount { get; set; }
        public DateTime StartDate { get; set; }
        public int RdcToStoreActCount { get; set; }
        public int IdcToRdcActCount { get; set;}
        public int[] Values1 = {150, 200, 250};
        public int[] Values2 = {100, 150};
        public int[] wipValues = {25,50,75,100};
        public int ERUpToBucket = 3;
        public int NumChunksPerPriority {get; set;}

        public int BAL { get { return 3; } }
        public int BLL { get { return 3; } }

        #region SSNumbers
        public double SsTargetAtStore { get { return Values1[Random.Next(Values1.Length)]; } }
        public double SsTargetAtRDC { get { return SsTargetAtStore * (StoreCount/RDCCount)/2; } }
        public double SsTargetAtIDC { get { return SsTargetAtStore * (StoreCount/IDCCount)/3; } }
        #endregion

        #region BOH / ER Numbers
        public double BOHAtStores { get { return 200; } }
        public double BOHAtRDC { get { return BOHAtStores * (StoreCount / RDCCount); } }
        public double BOHAtIDC { get { return BOHAtRDC*(RDCCount/IDCCount); } }

        public double? ERAtStores { get { return Values2[Random.Next(Values2.Length)]; } }
        public double? ERAtRDC { get { return null; } }
        public double? ERAtIDC { get { return null; } }
        #endregion

        #region StorageNumbers
        public double RegStorageAtRDC {get { return ItemCount*AvgSSNumberAtRDC; }}
        public double Ot1StorageAtRDC { get { return RegStorageAtRDC/10; } }
        public double Ot2StorageAtRDC { get { return RegStorageAtRDC/10; } }

        public double RegStorageAtIDC {get { return IntItemCount*AvgSSNumberAtIDC; }}
        public double Ot1StorageAtIDC { get { return RegStorageAtIDC/10; } }
        public double Ot2StorageAtIDC { get { return RegStorageAtIDC/10; } }
        #endregion

        #region CapacityNumbers

        public double FillRate { get; set; }
        public double FillRateHandlingAtRDC { get; set; }
        public double FillRateHandlingAtIDC { get; set; }
        public double FillRateOutBoundEnggAtStore { get; set; }
        public double FillRateOutBoundEnggAtRDC { get; set; }
        public double FillRateInBoundEnggAtRDC { get; set; }
        public double FillRateInBoundEnggAtIDC { get; set; }
        public double FillRateRegAtRDCToStore { get; set; }
        public double FillRateRegAtIDCToRDC { get; set; }
        public double FillRateRegAtIntToIDC { get; set; }
        public double FillRateRegAtDomToRDC { get; set; }
        public double FillRateItemSpecRegAtIntToIDC { get; set; }
        public double FillRateItemSpecRegAtDomToRDC { get; set; }

        public double HandelingAtRDC { get { return FillRateHandlingAtRDC * InBoundEnggAtRDC * 2; } }
        public double HandelingAtRDCOt1 { get { return HandelingAtRDC/10; } }
        public double HandelingAtRDCOt2 { get { return HandelingAtRDC/10; } }

        public double HandelingAtIDC { get { return FillRateHandlingAtIDC * InBoundEnggAtIDC * 2; } }
        public double HandelingAtIDCOt1 { get { return HandelingAtIDC/10; } }
        public double HandelingAtIDCOt2 { get { return HandelingAtIDC/10; } }

        public double OutBoundEnggAtStore { get { return FillRateOutBoundEnggAtStore * ItemCount * AvgDemand; } }
        public double OutBoundEnggAtStoreOt1 { get { return OutBoundEnggAtStore/10; } }
        public double OutBoundEnggAtStoreOt2 { get { return OutBoundEnggAtStore/10; } }

        public double OutBoundEnggAtRDC { get { return FillRateOutBoundEnggAtRDC * InBoundEnggAtRDC; } }
        public double OutBoundEnggAtRDCOt1 { get { return OutBoundEnggAtRDC/10; } }
        public double OutBoundEnggAtRDCOt2 { get { return OutBoundEnggAtRDC/10; } }

        public double InBoundEnggAtRDC { get { return FillRateInBoundEnggAtRDC * ItemCount * AvgDemand * (StoreCount / RDCCount); } }
        public double InBoundEnggAtRDCOt1 { get { return InBoundEnggAtRDC/10; } }
        public double InBoundEnggAtRDCOt2 { get { return InBoundEnggAtRDC/10; } }

        public double InBoundEnggAtIDC { get { return FillRateInBoundEnggAtIDC * IntItemCount * AvgDemand * (StoreCount / IDCCount); } }
        public double InBoundEnggAtIDCOt1 { get { return InBoundEnggAtIDC/10; } }
        public double InBoundEnggAtIDCOt2 {get { return InBoundEnggAtIDC/10; } }

        public double RegAtRDCToStore { get { return FillRateRegAtRDCToStore * AvgDemand * ItemCount; } }
        public double RegAtRDCToStoreOt1 { get { return RegAtRDCToStore/10; } }
        public double RegAtRDCToStoreOt2 { get { return RegAtRDCToStore/10; } }

        public double RegAtIDCToRDC { get { return FillRateRegAtIDCToRDC * AvgDemand * IntItemCount * (StoreCount / RDCCount); } }
        public double RegAtIDCToRDCOt1 { get { return RegAtIDCToRDC/10; } }
        public double RegAtIDCToRDCOt2 { get { return RegAtIDCToRDC/10; } }

        public double RegAtIntToIDC { get { return FillRateRegAtIntToIDC * AvgDemand * IntItemCount * (StoreCount / IDCCount); } }
        public double RegAtIntToIDCOt1 { get { return RegAtIntToIDC/10; } }
        public double RegAtIntToIDCOt2 { get { return RegAtIntToIDC/10; } }

        public double RegAtDomToRDC { get { return FillRateRegAtDomToRDC * AvgDemand * DomItemCount * (StoreCount / RDCCount); } }
        public double RegAtDomToRDCOt1 { get { return RegAtDomToRDC/10; } }
        public double RegAtDomToRDCOt2 { get { return RegAtDomToRDC/10; } }

        public double ItemSpecRegAtIntToIDC { get { return FillRateItemSpecRegAtIntToIDC * AvgDemand * (StoreCount / IDCCount); } }
        public double ItemSpecRegAtIntToIDCOt1 { get { return ItemSpecRegAtIntToIDC/10; } }
        public double ItemSpecRegAtIntToIDCOt2 { get { return ItemSpecRegAtIntToIDC/10; } }

        public double ItemSpecRegAtDomToRDC { get { return FillRateItemSpecRegAtDomToRDC * AvgDemand * (StoreCount / RDCCount); } }
        public double ItemSpecRegAtDomToRDCOt1 { get { return ItemSpecRegAtDomToRDC/10; } }
        public double ItemSpecRegAtDomToRDCOt2 { get { return ItemSpecRegAtDomToRDC/10; } }
        #endregion

        public int DemandQuantity()
        {
            return Random.Next(20) * 10 + 10;
        }

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
            get { return AvgSSNumberAtStore*(StoreCount/RDCCount); }
        }

        public int AvgSSNumberAtIDC
        {
            get { return AvgSSNumberAtStore*(StoreCount/IDCCount); }
        }

        public ItemRecord[] DomesticItemRecords;
        public ItemRecord[] IntItemRecords;
        public LocationRecord[] StoreLocationRecords;
        public LocationRecord[] RdcLocationRecords;
        public LocationRecord[] IdcLocationRecords;
        public LocationRecord[] IntSupplierLocationRecords;
        public LocationRecord[] DomSupplierLocationRecords;
        public TimeRecord[] TimeRecords;


        public GroupRecord RegularCapEdgeGroup = new GroupRecord {GroupKey = 1, GroupName = "Regular Edge Group"};
        public GroupRecord InBoundCapEdgeGroup = new GroupRecord {GroupKey = 2, GroupName = "InBound Edge Group"};
        public GroupRecord OutBoundCapEdgeGroup = new GroupRecord {GroupKey = 3, GroupName = "OutBound Edge Group"};

        public  OrderRecord[] DemandOrderRecords;

        public  void InitDemandTypes()
        {
            DemandOrderRecords=new OrderRecord[DemandTypes];
            for(int i=0;i<DemandTypes;i++)
                DemandOrderRecords[i] = new OrderRecord { OrderKey = i, OrderName = i.ToString()};
        }
        
        public OrderRecord InventoryOrderRecord = new OrderRecord { OrderKey = -1, OrderName = "Inventory Demand" };

        public CustomerRecord DemandCustomerRecord = new CustomerRecord { CustomerKey = 0, CustomerName = "0" };
        public CustomerRecord InventoryCustomerRecord = new CustomerRecord { CustomerKey = -1, CustomerName = "Inventory Demand" };

        public ItemRecord ErItemRecord = new ItemRecord { ItemKey = -1, ItemName = "Expected Receipt Activity" };
        public ItemRecord BOHItemRecord = new ItemRecord { ItemKey = -2, ItemName = "BOH Activity" };

        public ActivityRecord ErActivityRecord = new ActivityRecord { ActivityKey = -1, ActivityName = "Expected Receipt Activity" };
        public ActivityRecord BOHActivityRecord = new ActivityRecord { ActivityKey = -2, ActivityName = "BOH Activity" };

        public TransportRecord ErTransportRecord = new TransportRecord { TransportKey = -1, TransportName = "Expected Receipt Activity" };
        public TransportRecord BOHTransportRecord = new TransportRecord { TransportKey = -2, TransportName = "BOH Activity" };
        public TransportRecord RegTransportRecord = new TransportRecord {TransportKey = 0, TransportName = "0"};

        public string HandelingNameFormat = "Res-{0}";
        public string InBoundEnggNameFormat = "Res-{0}-I";
        public string OutBoundEnggNameFormat = "Res-{0}-O";
        public string RegularNameFormart = "Res-{0}-{1}";
        public string ItemSpecResgularNameFormat = "Res-{0}-{1}-{2}";
        public string ActivityNameFormat = "Act-{0}-{1}";
        public string StorageNameFormat = "DC-{0}";

        public int OutBoundEnggKeyStartIndx = 10000;
        public int InBoundEnggKeyStartIndx = 20000;
        public int RegularResourceStartIdx = 30000;
        public int StoreKeyStartIdx = 0;
        public int RdcKeyStartIdx = 5000;
        public int IdcKeyStartIdx = 6000;
        public int NoCarryIdcKeyStartIdx = 6500;
        public int IntSuppKeyStartIdx = 7000;
        public int DomSuppKeyStartIdx = 8000;

        public int CurrentActKey = 0;
        public int CurrentRegResourceKey = 30000;

        public void Generate()
        {
            var sw0 = new Stopwatch();
            sw0.Start();
            Console.WriteLine("Started DataGen for {0} items", ItemCount);
            InitWriters();
            InitDemandTypes();
            GenerateTime();
            GenerateItems();
            GenerateLocations();
            GenerateItemLocations();
            GenerateGroups();
            GenerateOrders();
            GenerateMisc();
            GenerateActivities();

            var sw1 = new Stopwatch();
            Console.WriteLine("Started generating SS data for {0} Items", ItemCount);
            sw1.Start();
            GenerateSsTargets();
            sw1.Stop();
            Console.WriteLine("Finished generating SS data for {0} Items in {1} s", ItemCount, sw1.ElapsedMilliseconds/1000);

            sw1.Reset();
            Console.WriteLine("Started generating Demand data for {0} Items", ItemCount);
            sw1.Start();
            GenerateDemands();
            sw1.Stop();
            Console.WriteLine("Finished generating Demand data for {0} Items in {1} s", ItemCount, sw1.ElapsedMilliseconds/1000);

            CloseWriters();
            sw0.Stop();
            Console.WriteLine("Finished DataGen for {0} items in {1} s", ItemCount, sw0.ElapsedMilliseconds/1000);
        }

        public void InitWriters()
        {
            CustomerRecordWriter = new FileWriter(OutputDir, "Dimension.Customer-{0}.csv", CustomerHeader, MaxRecordsPerFile);
            OrderRecordWriter = new FileWriter(OutputDir, "Dimension.Demands-{0}.csv", OrderHeader, MaxRecordsPerFile);
            ItemRecordWriter = new FileWriter(OutputDir, "Dimension.Items-{0}.csv", ItemHeader, MaxRecordsPerFile);
            ProductRecordWriter = new FileWriter(OutputDir, "Dimension.Product-{0}.csv", ProductHeader, MaxRecordsPerFile);
            LocationRecordWriter = new FileWriter(OutputDir, "Dimension.Locations-{0}.csv", LocationHeader, MaxRecordsPerFile);
            TransportRecordWriter = new FileWriter(OutputDir, "Dimension.Transport-{0}.csv", TransportHeader, MaxRecordsPerFile);
            ActivityRecordWriter = new FileWriter(OutputDir, "Dimension.Activities-{0}.csv", ActivityHeader, MaxRecordsPerFile);
            StorageRecordWriter = new FileWriter(OutputDir, "Dimension.Storage-{0}.csv", StorageHeader, MaxRecordsPerFile);
            ResourceRecordWriter = new FileWriter(OutputDir, "Dimension.Resource-{0}.csv", ResourceHeader, MaxRecordsPerFile);
            TimeRecordWriter = new FileWriter(OutputDir, "Dimension.Time-{0}.csv", TimeHeader, MaxRecordsPerFile);
            GroupRecordWriter = new FileWriter(OutputDir, "Dimension.GroupComponents-{0}.csv", GroupHeader, MaxRecordsPerFile);
            MaterialNodeParamWriter = new FileWriter(OutputDir, "Fact.MaterialNodeParameters-{0}.csv", MaterialNodeParamHeader, MaxRecordsPerFile);
            MaterialNodeTimeParamWriter = new FileWriter(OutputDir, "Fact.MaterialNodeTimeParameters-{0}.csv", MaterialNodeTimeParamHeader, MaxRecordsPerFile);
            InventoryWriter = new FileWriter(OutputDir, "Fact.Inventory-{0}.csv", InventoryHeader, MaxRecordsPerFile);
            StorageAvailWriter = new FileWriter(OutputDir, "Fact.StorageAvailability-{0}.csv", StorageAvailParamHeader, MaxRecordsPerFile);
            StorageGraphWriter = new FileWriter(OutputDir, "Fact.StorageGraph-{0}.csv", StorageGraphHeader, MaxRecordsPerFile);
            CapacityAvailWriter = new FileWriter(OutputDir, "Fact.CapacityAvailability-{0}.csv", CapacityAvailParamHeader, MaxRecordsPerFile);
            ActivityParamWriter = new FileWriter(OutputDir, "Fact.ActivityParameters-{0}.csv", ActivityParamHeader, MaxRecordsPerFile);
            ProductionGraphWriter = new FileWriter(OutputDir, "Fact.MaterialProductionGraph-{0}.csv", ProductionGraphHeader, MaxRecordsPerFile);
            ConsumptionGraphWriter = new FileWriter(OutputDir, "Fact.MaterialConsumptionGraph-{0}.csv", ConsumptionGraphHeader, MaxRecordsPerFile);
            CapacityGraphGraphWriter = new FileWriter(OutputDir, "Fact.CapacityConsumptionGraph-{0}.csv", CapacityGraphHeader, MaxRecordsPerFile);
            SimCapacityGraphGraphWriter = new FileWriter(OutputDir, "Fact.SimultaneousCapacityConsumptionGraph-{0}.csv", SimCapacityGraphHeader, MaxRecordsPerFile);
            DemandRecordWriter = new FileWriter(OutputDir, "Fact.DemandQuantity-{0}.csv", DemandHeader, MaxRecordsPerFile);
            WIPRecordWriter = new FileWriter(OutputDir, "Fact.MaterialProductionGraphPlannedWIP-{0}.csv",MaterialProductionGraphPlannedWIPheader,MaxRecordsPerFile);
        }

        public void GenerateItems()
        {
            int i = 0;
            DomesticItemRecords = new ItemRecord[DomItemCount];
            ItemRecord record;
            for (; i < DomItemCount; i++)
            {
                DomesticItemRecords[i] = record =new ItemRecord {ItemKey = i, ItemName = i.ToString()};
                WriteItemRecord(record);
                WriteProductRecord(record);
            }
            IntItemRecords = new ItemRecord[IntItemCount];
            for (int j = 0; j < IntItemCount; i++, j++)
            {
                IntItemRecords[j] = record= new ItemRecord {ItemKey = i, ItemName = i.ToString()};
                WriteItemRecord(record);
                WriteProductRecord(record);
            }
            
        }

        public void GenerateLocations()
        {
            StoreLocationRecords = new LocationRecord[StoreCount];
            
            for (int i = 0; i < StoreCount; i++)
            {
                LocationRecord locationRecord;
                StoreLocationRecords[i] = locationRecord = new LocationRecord
                {
                    LocationKey = i,
                    LocationName = i.ToString(),
                    HandelingResource = default(ResourceRecord),
                        //new ResourceRecord {ResourceKey = i, ResourceName = string.Format(HandelingNameFormat, i)},
                    InBoundEnggResource = default(ResourceRecord),
                    OutBoundEnggResource =
                        new ResourceRecord
                        {
                            ResourceKey = i + OutBoundEnggKeyStartIndx,
                            ResourceName = string.Format(OutBoundEnggNameFormat, i)
                        }
                };
                
                foreach (var timeRecord in TimeRecords)
                {
                    WriteOutBoundEnggCapacityAvailability(locationRecord, timeRecord, OutBoundEnggAtStore, OutBoundEnggAtStoreOt1, OutBoundEnggAtStoreOt2);
                }
                WriteLocationRecord(locationRecord);
                //WriteResourceRecord(locationRecord.HandelingResource);
                WriteResourceRecord(locationRecord.OutBoundEnggResource);

            }

            RdcLocationRecords = new LocationRecord[RDCCount];
            for (int i = RdcKeyStartIdx; i < RDCCount + RdcKeyStartIdx; i++)
            {
                LocationRecord locationRecord;
                RdcLocationRecords[i - RdcKeyStartIdx] = locationRecord = new LocationRecord
                {
                    LocationKey = i,
                    LocationName = i.ToString(),
                    HandelingResource =
                        new ResourceRecord {ResourceKey = i, ResourceName = string.Format(HandelingNameFormat, i)},
                    InBoundEnggResource =
                        new ResourceRecord
                        {
                            ResourceKey = i + InBoundEnggKeyStartIndx,
                            ResourceName = string.Format(InBoundEnggNameFormat, i)
                        },
                    OutBoundEnggResource =
                        new ResourceRecord
                        {
                            ResourceKey = i + OutBoundEnggKeyStartIndx,
                            ResourceName = string.Format(OutBoundEnggNameFormat, i)
                        },
                    Storage = new StorageRecord {StorageKey = i, StorageName = string.Format(StorageNameFormat, i)}
                };

                foreach (var timeRecord in TimeRecords)
                {
                    WriteHadelingCapacityAvailability(locationRecord, timeRecord, HandelingAtRDC, HandelingAtRDCOt1, HandelingAtRDCOt2);
                    WriteInBoundEnggCapacityAvailability(locationRecord, timeRecord, InBoundEnggAtRDC, InBoundEnggAtRDCOt1, InBoundEnggAtRDCOt2);
                    WriteOutBoundEnggCapacityAvailability(locationRecord, timeRecord, OutBoundEnggAtRDC, OutBoundEnggAtRDCOt1, OutBoundEnggAtRDCOt2);
                }
                WriteLocationRecord(locationRecord);
                WriteResourceRecord(locationRecord.HandelingResource);
                WriteResourceRecord(locationRecord.InBoundEnggResource);
                WriteResourceRecord(locationRecord.OutBoundEnggResource);
            }

            IdcLocationRecords = new LocationRecord[IDCCount];
            for (int i = IdcKeyStartIdx; i < IDCCount/2 + IdcKeyStartIdx; i++)
            {
                LocationRecord locationRecord;
                IdcLocationRecords[i - IdcKeyStartIdx] = locationRecord = new LocationRecord
                {
                    LocationKey = i,
                    LocationName = i.ToString(),
                    HandelingResource =
                        new ResourceRecord {ResourceKey = i, ResourceName = string.Format(HandelingNameFormat, i)},
                    InBoundEnggResource =
                        new ResourceRecord
                        {
                            ResourceKey = i + InBoundEnggKeyStartIndx,
                            ResourceName = string.Format(InBoundEnggNameFormat, i)
                        },
                    OutBoundEnggResource = default(ResourceRecord),
                    Storage = new StorageRecord {StorageKey = i, StorageName = string.Format(StorageNameFormat, i)}
                };
                foreach (var timeRecord in TimeRecords)
                {
                    WriteHadelingCapacityAvailability(locationRecord, timeRecord, HandelingAtIDC, HandelingAtIDCOt1, HandelingAtIDCOt2);
                    WriteInBoundEnggCapacityAvailability(locationRecord, timeRecord, InBoundEnggAtIDC, InBoundEnggAtIDCOt1, InBoundEnggAtIDCOt2);
                }
                WriteLocationRecord(locationRecord);
                WriteResourceRecord(locationRecord.HandelingResource);
                WriteResourceRecord(locationRecord.InBoundEnggResource);
            }
            for (int i = NoCarryIdcKeyStartIdx; i < IDCCount/2 + NoCarryIdcKeyStartIdx; i++)
            {
                LocationRecord locationRecord;
                IdcLocationRecords[IDCCount/2 + i - NoCarryIdcKeyStartIdx] = locationRecord = new LocationRecord
                {
                    LocationKey = i,
                    LocationName = i.ToString(),
                    HandelingResource =
                        new ResourceRecord {ResourceKey = i, ResourceName = string.Format(HandelingNameFormat, i)},
                    InBoundEnggResource =
                        new ResourceRecord
                        {
                            ResourceKey = i + InBoundEnggKeyStartIndx,
                            ResourceName = string.Format(InBoundEnggNameFormat, i)
                        },
                    OutBoundEnggResource = default(ResourceRecord),
                    IsNoCarry = true
                };
                foreach (var timeRecord in TimeRecords)
                {
                    WriteHadelingCapacityAvailability(locationRecord, timeRecord, HandelingAtIDC, HandelingAtIDCOt1, HandelingAtIDCOt2);
                    WriteInBoundEnggCapacityAvailability(locationRecord, timeRecord, InBoundEnggAtIDC, InBoundEnggAtIDCOt1, InBoundEnggAtIDCOt2);
                }
                WriteLocationRecord(locationRecord);
                WriteResourceRecord(locationRecord.HandelingResource);
                WriteResourceRecord(locationRecord.InBoundEnggResource);
            }

            IntSupplierLocationRecords = new LocationRecord[IntSupplierCount];
            for (int i = IntSuppKeyStartIdx; i < IntSupplierCount + IntSuppKeyStartIdx; i++)
            {
                LocationRecord locationRecord;
                IntSupplierLocationRecords[i - IntSuppKeyStartIdx] = locationRecord = new LocationRecord
                {
                    LocationKey = i,
                    LocationName = i.ToString(),
                    HandelingResource = default(ResourceRecord),
                    InBoundEnggResource = default(ResourceRecord),
                    OutBoundEnggResource = default(ResourceRecord)
                };
                WriteLocationRecord(locationRecord);
            }

            DomSupplierLocationRecords = new LocationRecord[DomSupplierCount];
            for (int i = DomSuppKeyStartIdx; i < DomSupplierCount + DomSuppKeyStartIdx; i++)
            {
                LocationRecord locationRecord;
                DomSupplierLocationRecords[i - DomSuppKeyStartIdx] = locationRecord = new LocationRecord
                {
                    LocationKey = i,
                    LocationName = i.ToString(),
                    HandelingResource = default(ResourceRecord),
                    InBoundEnggResource = default(ResourceRecord),
                    OutBoundEnggResource = default(ResourceRecord)
                };
                WriteLocationRecord(locationRecord);
            }
        }

        public void GenerateItemLocations()
        {
            foreach (var storeLocation in StoreLocationRecords)
            {
                foreach (var item in IntItemRecords.Union(DomesticItemRecords))
                {
                    WriteMaterialNodeParameters(item, storeLocation);
                    for (int index = 0; index < ERUpToBucket; index++)
                    {
                        var timeRecord = TimeRecords[index];
                        if (index == 0)
                            WriteBOHAndER(item, storeLocation, timeRecord, Values1[Random.Next(Values1.Length)], null);
                        else
                            WriteBOHAndER(item, storeLocation, timeRecord, null, 100);
                    }
                }
            }

            foreach (var rdcLocations in RdcLocationRecords)
            {
                var productSet = new HashSet<ItemRecord>();
                foreach (var item in IntItemRecords.Union(DomesticItemRecords))
                {
                    WriteMaterialNodeParameters(item, rdcLocations);

                    var prodcutKey = item.ItemKey%StorageTypesCount;
                    var product = prodcutKey >= IntItemCount
                        ? DomesticItemRecords[prodcutKey - IntItemCount]
                        : IntItemRecords[prodcutKey];
                    WriteStorageGraph(rdcLocations, item, product);

                    for (int index = 0; index < ERUpToBucket; index++)
                    {
                        var timeRecord = TimeRecords[index];
                        if (index == 0)
                            WriteBOHAndER(item, rdcLocations, timeRecord, Values1[Random.Next(Values1.Length)], null);
                        else
                            WriteBOHAndER(item, rdcLocations, timeRecord, null, 100);
                    }

                    if (!productSet.Add(product)) continue;
                    foreach (var timeRecord in TimeRecords)
                    {
                        WriteStorageAvailability(rdcLocations, product, timeRecord, RegStorageAtRDC, Ot1StorageAtRDC,
                            Ot2StorageAtRDC);
                    }
                }
                WriteStorageRecord(rdcLocations.Storage);
            }

            foreach (var idcLocations in IdcLocationRecords)
            {
                var product = IntItemRecords[0];
                foreach (var item in IntItemRecords)
                {
                    WriteMaterialNodeParameters(item, idcLocations);
                    if (idcLocations.IsNoCarry) continue;

                    WriteStorageGraph(idcLocations, item, product);

                    for (int index = 0; index < ERUpToBucket; index++)
                    {
                        var timeRecord = TimeRecords[index];
                        if (index == 0)
                            WriteBOHAndER(item, idcLocations, timeRecord, Values1[Random.Next(Values1.Length)], null);
                        else
                            WriteBOHAndER(item, idcLocations, timeRecord, null, 100);
                    }

                }
                if (idcLocations.IsNoCarry) continue;
                foreach (var timeRecord in TimeRecords)
                {
                    WriteStorageAvailability(idcLocations, product, timeRecord, RegStorageAtIDC, Ot1StorageAtIDC,
                        Ot2StorageAtIDC);
                }
                WriteStorageRecord(idcLocations.Storage);
            }

            foreach (var intSuppLocations in IntSupplierLocationRecords)
            {
                foreach (var item in IntItemRecords)
                {
                    WriteMaterialNodeParameters(item, intSuppLocations);
                }
            }

            foreach (var domSuppLocations in DomSupplierLocationRecords)
            {
                foreach (var item in DomesticItemRecords)
                {
                    WriteMaterialNodeParameters(item, domSuppLocations);
                }
            }
        }

        public void GenerateTime() 
        {
            TimeRecords = new TimeRecord[TimeBucketCount];
            var date = StartDate;
            for (int i = 0; i < TimeBucketCount; i++, date = date.AddDays(7))
            {
                TimeRecord record;
                TimeRecords[i] =  record = new TimeRecord {TimeKey = date};
                WriteTimeRecord(record);
            }
        }

        public void GenerateGroups()
        {
            WriteGroupRecord(RegularCapEdgeGroup);
            WriteGroupRecord(InBoundCapEdgeGroup);
            WriteGroupRecord(OutBoundCapEdgeGroup);
        }

        public void GenerateOrders()
        {
            foreach (var orderRecord in DemandOrderRecords)
            {
                WriteOrderRecord(orderRecord);    
            }
            WriteOrderRecord(InventoryOrderRecord);
            WriteCustomerRecord(DemandCustomerRecord);
            WriteCustomerRecord(InventoryCustomerRecord);
        }

        public void GenerateMisc()
        {
            WriteProductRecord(BOHItemRecord);
            WriteProductRecord(ErItemRecord);
            WriteTransportRecord(RegTransportRecord);
            WriteTransportRecord(BOHTransportRecord);
            WriteTransportRecord(ErTransportRecord);
            WriteActivityRecord(BOHActivityRecord);
            WriteActivityRecord(ErActivityRecord);
        }

        public void GenerateSsTargets()
        {
            foreach (var storeLocation in StoreLocationRecords)
            {
                foreach (var item in IntItemRecords.Union(DomesticItemRecords))
                {
                    foreach (var record in TimeRecords)
                    {
                        WriteMaterialNodeTimeParameters(item, storeLocation, record, SsTargetAtStore);
                    }
                }
            }

            foreach (var rdcLocations in RdcLocationRecords)
            {
                foreach (var item in IntItemRecords.Union(DomesticItemRecords))
                {
                    foreach (var record in TimeRecords)
                    {
                        WriteMaterialNodeTimeParameters(item, rdcLocations, record, SsTargetAtRDC);
                    }
                }
            }

            foreach (var idcLocations in IdcLocationRecords)
            {
                if (idcLocations.IsNoCarry) continue;
                foreach (var item in IntItemRecords)
                {
                    foreach (var record in TimeRecords)
                    {
                        WriteMaterialNodeTimeParameters(item, idcLocations, record, SsTargetAtIDC);
                    }
                }
            }
        }

        public void GenerateDemands()
        {
            Console.WriteLine(DemandOrderRecords.Length);
            {
                for (int index = 0; index < DomesticItemRecords.Length; index++)
                {
                    var item = DomesticItemRecords[index];
                    var orderRecord = DemandOrderRecords[item.ItemKey%DemandOrderRecords.Length];
                    var mul = TimeRecords.Length * orderRecord.OrderKey;
                    foreach (var storeLocation in StoreLocationRecords)
                    {
                        for (int i = 0; i < TimeRecords.Length; i++)
                        {
                            var timeRecord = TimeRecords[i];
                            WriteDemandRecords(item, storeLocation, orderRecord, timeRecord, orderRecord.OrderKey,
                                DemandQuantity(), mul + i);
                        }
                    }
                }
            }

            {
                for (int index = 0; index < IntItemRecords.Length; index++)
                {
                    var item = IntItemRecords[index];
                    var orderRecord = DemandOrderRecords[item.ItemKey % DemandOrderRecords.Length];
                    var mul = TimeRecords.Length * orderRecord.OrderKey;
                    foreach (var storeLocation in StoreLocationRecords)
                    {
                        for (int i = 0; i < TimeRecords.Length; i++)
                        {
                            var timeRecord = TimeRecords[i];
                            WriteDemandRecords(item, storeLocation, orderRecord, timeRecord, orderRecord.OrderKey,
                                DemandQuantity(), mul + i);
                        }
                    }
                }
            }
        }
        
        private IEnumerable<ActivityRecord> GenerateRdcToStoreActivities()
        {
            for (int i = 0, storeIdx = 0, rdcIdx = -1; i < RdcToStoreActCount; i++, storeIdx++, rdcIdx++)
            {
                var rem = 0;
                var q = Math.DivRem(storeIdx, StoreCount, out rem);
                if (rem == 0) { rdcIdx = q; }
                var toLocation = StoreLocationRecords[rem];
                var fromLocation = RdcLocationRecords[rdcIdx%RDCCount];

                var actRecord = new ActivityRecord
                {
                    ActivityKey = CurrentActKey++,
                    ActivityName = string.Format(ActivityNameFormat, fromLocation.LocationName, toLocation.LocationName),
                    RegulaResourceRecord = new ResourceRecord
                    {
                        ResourceKey = CurrentRegResourceKey++,
                        ResourceName =
                            string.Format(RegularNameFormart, fromLocation.LocationName, toLocation.LocationName)
                    },
                    ItemSpecificResourceRecords = null,
                    FromLocationRecord = fromLocation,
                    ToLocationRecord = toLocation
                };
                WriteActivityRecord(actRecord);
                WriteResourceRecord(actRecord.RegulaResourceRecord);
                yield return actRecord;
            }
        }

        private IEnumerable<ActivityRecord> GenerateIdcToRdcActivities()
        {
            for (int rdcIdx = 0, idcIdx = 0; rdcIdx < RDCCount; rdcIdx++, idcIdx++)
            {
                var toLocationRec = RdcLocationRecords[rdcIdx];
                var idc1Idx = idcIdx%(IDCCount/2);
                var idc2Idx = idc1Idx + IDCCount/2;
                foreach (var fromIdx in new []{idc1Idx, idc2Idx})
                {
                    var fromLocationRec = IdcLocationRecords[fromIdx];
                    var actRecord = new ActivityRecord
                    {
                        ActivityKey = CurrentActKey++,
                        ActivityName =
                            string.Format(ActivityNameFormat, fromLocationRec.LocationName, toLocationRec.LocationName),
                        RegulaResourceRecord = new ResourceRecord
                        {
                            ResourceKey = CurrentRegResourceKey++,
                            ResourceName =
                                string.Format(RegularNameFormart, fromLocationRec.LocationName, toLocationRec.LocationName),
                        },
                        ItemSpecificResourceRecords = null,
                        ToLocationRecord = toLocationRec,
                        FromLocationRecord = fromLocationRec
                    };
                    WriteActivityRecord(actRecord);
                    WriteResourceRecord(actRecord.RegulaResourceRecord);
                    yield return actRecord;
                }
            }
        }

        private IEnumerable<ActivityRecord> GenerateIntSupplierToIdcActivities()
        {
            for (int i = 0, idcIdx = 0, suppIdx = 0; i < IDCCount; i++, suppIdx++, idcIdx++)
            {
                var fromLocationRec = IntSupplierLocationRecords[suppIdx%IntSupplierCount];
                var toLocationRec = IdcLocationRecords[idcIdx%IDCCount];

                var actRecord = new ActivityRecord
                {
                    ActivityKey = CurrentActKey++,
                    ActivityName =
                        string.Format(ActivityNameFormat, fromLocationRec.LocationName, toLocationRec.LocationName),
                    RegulaResourceRecord = new ResourceRecord
                    {
                        ResourceKey = CurrentRegResourceKey++,
                        ResourceName =
                            string.Format(RegularNameFormart, fromLocationRec.LocationName, toLocationRec.LocationName),
                    },
                    ItemSpecificResourceRecords = new List<ResourceRecord>(),
                    ToLocationRecord = toLocationRec,
                    FromLocationRecord = fromLocationRec
                };
                WriteActivityRecord(actRecord);
                WriteResourceRecord(actRecord.RegulaResourceRecord);
                yield return actRecord;
            }
        }

        private IEnumerable<ActivityRecord> GenerateDomToRdcActivities()
        {
            for (int i = 0, domIdx = 0, rdcIdx = 0; i < RDCCount; i++, domIdx++, rdcIdx++)
            {
                var fromLocationRec = DomSupplierLocationRecords[domIdx%DomSupplierCount];
                var toLocationRec = RdcLocationRecords[rdcIdx%RDCCount];

                var actRecord = new ActivityRecord
                {
                    ActivityKey = CurrentActKey++,
                    ActivityName =
                        string.Format(ActivityNameFormat, fromLocationRec.LocationName, toLocationRec.LocationName),
                    RegulaResourceRecord = new ResourceRecord
                    {
                        ResourceKey = CurrentRegResourceKey++,
                        ResourceName =
                            string.Format(RegularNameFormart, fromLocationRec.LocationName, toLocationRec.LocationName),
                    },
                    ItemSpecificResourceRecords = new List<ResourceRecord>(),
                    ToLocationRecord = toLocationRec,
                    FromLocationRecord = fromLocationRec
                };
                WriteActivityRecord(actRecord);
                WriteResourceRecord(actRecord.RegulaResourceRecord);
                yield return actRecord;
            }
        }

        private void WriteRdcToStoreComponents()
        {
            foreach (var actRecord in GenerateRdcToStoreActivities())
            {
                foreach (var itemRecord in IntItemRecords.Union(DomesticItemRecords))
                {
                    WriteActivityParams(actRecord, itemRecord);
                    WriteProductionGraph(actRecord, itemRecord);
                    WriteConsumptionGraph(actRecord, itemRecord);

                    WriteInBoundHandelingResConsumptionGraph(actRecord, itemRecord, true);
                    WriteInBoundEnggResConsumptionGraph(actRecord, itemRecord, true);

                    WriteRegularResConsumptionGraph(actRecord, itemRecord, false);

                    WriteOutBoundEnggResConsumptionGraph(actRecord, itemRecord, false);
                    for (int i = 0; i < 4; i++)
                    {
                        WriteMaterialProductionGraphPlannedWIP(actRecord, itemRecord, TimeRecords[i], RegTransportRecord, wipValues[Random.Next(wipValues.Length)]);

                    }

                }

                foreach (var timeRecord in TimeRecords)
                {
                    WriteRegularCapacityAvailability(actRecord, timeRecord, RegAtRDCToStore, RegAtRDCToStoreOt1, RegAtRDCToStoreOt2);
                    
                }
            }
        }

        

        private void WriteIdcToRdcComponents()
        {
            foreach (var actRecord in GenerateIdcToRdcActivities())
            {
                foreach (var itemRecord in IntItemRecords)
                {
                    WriteActivityParams(actRecord, itemRecord);
                    WriteProductionGraph(actRecord, itemRecord);
                    WriteConsumptionGraph(actRecord, itemRecord);

                    WriteInBoundHandelingResConsumptionGraph(actRecord, itemRecord, true);
                    WriteInBoundEnggResConsumptionGraph(actRecord, itemRecord, true);

                    WriteOutBoundHandelingResConsumptionGraph(actRecord, itemRecord, true);
                    WriteOutBoundEnggResConsumptionGraph(actRecord, itemRecord, true);

                    WriteRegularResConsumptionGraph(actRecord, itemRecord, false);
                    for (int i = 0; i < 4; i++)
                    {
                        WriteMaterialProductionGraphPlannedWIP(actRecord, itemRecord, TimeRecords[i],RegTransportRecord,wipValues[Random.Next(wipValues.Length)]);

                    }
                }

                foreach (var timeRecord in TimeRecords)
                {
                    WriteRegularCapacityAvailability(actRecord, timeRecord, RegAtIDCToRDC, RegAtIDCToRDCOt1, RegAtIDCToRDCOt2);
                }
            }
        }

        private void WriteIntToIdcComponents()
        {
            foreach (var actRecord in GenerateIntSupplierToIdcActivities())
            {
                foreach (var itemRecord in IntItemRecords)
                {
                    WriteActivityParams(actRecord, itemRecord);
                    WriteProductionGraph(actRecord, itemRecord);
                    WriteConsumptionGraph(actRecord, itemRecord);

                    WriteOutBoundHandelingResConsumptionGraph(actRecord, itemRecord, false);

                    WriteRegularResConsumptionGraph(actRecord, itemRecord, true);
                    var itemSpec = new ResourceRecord
                    {
                        ResourceKey = CurrentRegResourceKey++,
                        ResourceName =
                            string.Format(ItemSpecResgularNameFormat, itemRecord.ItemKey,
                                actRecord.FromLocationRecord.LocationName,
                                actRecord.ToLocationRecord.LocationName)
                    };
                    WriteItemSpecRegularResConsumptionGraph(actRecord, itemRecord, itemSpec, true);

                    WriteResourceRecord(itemSpec);
                    for (int i = 0; i < 4; i++)
                    {
                        WriteMaterialProductionGraphPlannedWIP(actRecord, itemRecord, TimeRecords[i], RegTransportRecord, wipValues[Random.Next(wipValues.Length)]);

                    }
                }

                foreach (var timeRecord in TimeRecords)
                {
                    WriteRegularCapacityAvailability(actRecord, timeRecord, RegAtIntToIDC, RegAtIntToIDCOt1, RegAtIntToIDCOt2);
                    WriteItemSpecRegularCapacityAvailability(actRecord, timeRecord, ItemSpecRegAtIntToIDC, ItemSpecRegAtIntToIDCOt1, ItemSpecRegAtIntToIDCOt2);
                }
            }
        }

        private void WriteDomToRdcComponents()
        {
            foreach (var actRecord in GenerateDomToRdcActivities())
            {
                foreach (var itemRecord in DomesticItemRecords)
                {
                    WriteActivityParams(actRecord, itemRecord);
                    WriteProductionGraph(actRecord, itemRecord);
                    WriteConsumptionGraph(actRecord, itemRecord);

                    WriteRegularResConsumptionGraph(actRecord, itemRecord, true);
                    var itemSpec = new ResourceRecord
                    {
                        ResourceKey = CurrentRegResourceKey++,
                        ResourceName =
                            string.Format(ItemSpecResgularNameFormat, itemRecord.ItemKey,
                                actRecord.FromLocationRecord.LocationName,
                                actRecord.ToLocationRecord.LocationName)
                    };
                    WriteItemSpecRegularResConsumptionGraph(actRecord, itemRecord, itemSpec, true);

                    WriteOutBoundEnggResConsumptionGraph(actRecord, itemRecord, true);
                    WriteOutBoundHandelingResConsumptionGraph(actRecord, itemRecord, true);

                    WriteResourceRecord(itemSpec);
                    for (int i = 0; i < 4; i++)
                    {
                        WriteMaterialProductionGraphPlannedWIP(actRecord, itemRecord, TimeRecords[i], RegTransportRecord, wipValues[Random.Next(wipValues.Length)]);

                    }
                }

                foreach (var timeRecord in TimeRecords)
                {
                    WriteRegularCapacityAvailability(actRecord, timeRecord, RegAtDomToRDC, RegAtDomToRDCOt1, RegAtDomToRDCOt2);
                    WriteItemSpecRegularCapacityAvailability(actRecord, timeRecord, ItemSpecRegAtDomToRDC, ItemSpecRegAtDomToRDCOt1, ItemSpecRegAtDomToRDCOt2);
                }
            }
        }

        public void GenerateActivities()
        {
            WriteRdcToStoreComponents();
            WriteIdcToRdcComponents();
            WriteIntToIdcComponents();
            WriteDomToRdcComponents();
        }

        public static readonly string DemandHeader =
        @"Demand.[DemandKey],Customer.[CustomerKey],Item.[ItemKey],Location.[LocationKey],Time.[FiscalWeekKey],Version.[VersionKey],Demand Build Ahead Lmit,Demand Build Late Limit,Demand Priority,Demand Quantity,Demand Chunk Measure";
        public static readonly string DemandRecordFormat = @"{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}";
        public FileWriter DemandRecordWriter;

        public void WriteDemandRecords(ItemRecord itemRecord, LocationRecord locRecord, OrderRecord orderRecord, TimeRecord timeRecord, int priority, double demandQty, int chunk)
        {
            var line = string.Format(DemandRecordFormat, orderRecord, DemandCustomerRecord, itemRecord, locRecord,
                timeRecord, 0, BAL, BLL, priority, demandQty, chunk);
            DemandRecordWriter.Write(line);
        }

        public static readonly string CustomerHeader =
        @"Customer.[CustomerKey],Customer.[Customer],Customer.[CustomerSubGroupKey],Customer.[Customer Sub Group],Customer.[CustomerGroupKey],Customer.[Customer Group]";
        public static readonly string CustomerRecordFormat = @"{0},{1},{2},{3},{4},{5}";
        public FileWriter CustomerRecordWriter;
        public void WriteCustomerRecord(CustomerRecord cutomerRecord)
        {
            var line = string.Format(CustomerRecordFormat, cutomerRecord.CustomerKey, cutomerRecord.CustomerName,
                cutomerRecord.SubGroupKey, cutomerRecord.SubGroupName, cutomerRecord.GroupKey,
                cutomerRecord.GroupName);
            CustomerRecordWriter.Write(line);
        }

        public static readonly string OrderHeader =
            @"Demand.[DemandKey],Demand.[Demand],Demand.[DemandSubGroupKey],Demand.[Demand Sub Group],Demand.[DemandGroupKey],Demand.[Demand Group]";
        public static readonly string OrderRecordFormat = @"{0},{1},{2},{3},{4},{5}";
        public FileWriter OrderRecordWriter;
        public void WriteOrderRecord(OrderRecord orderRecord)
        {
            var line = string.Format(OrderRecordFormat, orderRecord.OrderKey, orderRecord.OrderName,
                orderRecord.SubGroupKey, orderRecord.SubGroupName, orderRecord.GroupKey,
                orderRecord.GroupName);
            OrderRecordWriter.Write(line);
        }

        public static readonly string ItemHeader =
            @"Item.[ItemKey],Item.[Item],Item.[ItemSubGroupKey],Item.[Item Sub Group],Item.[ItemGroupKey],Item.[Item Group]";
        public static readonly string ItemRecordFormat = @"{0},{1},{2},{3},{4},{5}";
        public FileWriter ItemRecordWriter;
        public void WriteItemRecord(ItemRecord itemRecord)
        {
            var line = string.Format(ItemRecordFormat, itemRecord.ItemKey, itemRecord.ItemName,
                itemRecord.SubGroupKey, itemRecord.SubGroupName, itemRecord.GroupKey,
                itemRecord.GroupName);
            ItemRecordWriter.Write(line);
        }

        public static readonly string ProductHeader =
            @"Product.[ProductKey],Product.[Product],Product.[ProductSubGroupKey],Product.[Product Sub Group],Product.[ProductGroupKey],Product.[Product Group]";
        public static readonly string ProductRecordFormat = @"{0},{1},{2},{3},{4},{5}";
        public FileWriter ProductRecordWriter;
        public void WriteProductRecord(ItemRecord itemRecord)
        {
            var line = string.Format(ProductRecordFormat, itemRecord.ItemKey, itemRecord.ItemName,
                itemRecord.SubGroupKey, itemRecord.SubGroupName, itemRecord.GroupKey,
                itemRecord.GroupName);
            ProductRecordWriter.Write(line);
        }

        public static readonly string LocationHeader =
            @"Location.[LocationKey],Location.[Location],Location.[LocationSubGroupKey],Location.[Location Sub Group],Location.[LocationGroupKey],Location.[Location Group]";
        public static readonly string LocationRecordFormat = @"{0},{1},{2},{3},{4},{5}";
        public FileWriter LocationRecordWriter;
        public void WriteLocationRecord(LocationRecord locationRecord)
        {
            var line = string.Format(LocationRecordFormat, locationRecord.LocationKey, locationRecord.LocationName,
                locationRecord.SubGroupKey, locationRecord.SubGroupName, locationRecord.GroupKey,
                locationRecord.GroupName);
            LocationRecordWriter.Write(line);
        }

        public static readonly string TransportHeader =
            @"Transport.[TransportKey],Transport.[Transport],Transport.[TransportSubGroupKey],Transport.[Transport Sub Group],Transport.[TransportGroupKey],Transport.[Transport Group]";
        public static readonly string TransportRecordFormat = @"{0},{1},{2},{3},{4},{5}";
        public FileWriter TransportRecordWriter;
        public void WriteTransportRecord(TransportRecord trasRecord)
        {
            var line = string.Format(TransportRecordFormat, trasRecord.TransportKey, trasRecord.TransportName,
                trasRecord.SubGroupKey, trasRecord.SubGroupName, trasRecord.GroupKey,
                trasRecord.GroupName);
            TransportRecordWriter.Write(line);
        }

        public static readonly string ActivityHeader =
    @"Activity.[ActivityKey],Activity.[Activity],Activity.[ActivitySubGroupKey],Activity.[Activity Sub Group],Activity.[ActivityGroupKey],Activity.[Activity Group]";
        public static readonly string ActivityRecordFormat = @"{0},{1},{2},{3},{4},{5}";
        public FileWriter ActivityRecordWriter;
        public void WriteActivityRecord(ActivityRecord activityRecord)
        {
            var line = string.Format(TransportRecordFormat, activityRecord.ActivityKey, activityRecord.ActivityName,
                activityRecord.SubGroupKey, activityRecord.SubGroupName, activityRecord.GroupKey,
                activityRecord.GroupName);
            ActivityRecordWriter.Write(line);
        }


        public static readonly string StorageHeader =
@"Storage.[StorageKey],Storage.[Storage],Storage.[StorageSubGroupKey],Storage.[Storage Sub Group],Storage.[StorageGroupKey],Storage.[Storage Group]";
        public static readonly string StorageRecordFormat = @"{0},{1},{2},{3},{4},{5}";
        public FileWriter StorageRecordWriter;
        public void WriteStorageRecord(StorageRecord storageRecord)
        {
            var line = string.Format(StorageRecordFormat, storageRecord.StorageKey, storageRecord.StorageName,
                storageRecord.SubGroupKey, storageRecord.SubGroupName, storageRecord.GroupKey,
                storageRecord.GroupName);
            StorageRecordWriter.Write(line);
        }

        public static readonly string ResourceHeader =
@"Resource.[ResourceKey],Resource.[Resource],Resource.[ResourceSubGroupKey],Resource.[Resource Sub Group],Resource.[ResourceGroupKey],Resource.[Resource Group]";
        public static readonly string ResourceRecordFormat = @"{0},{1},{2},{3},{4},{5}";
        public FileWriter ResourceRecordWriter;
        public void WriteResourceRecord(ResourceRecord storageRecord)
        {
            var line = string.Format(ResourceRecordFormat, storageRecord.ResourceKey, storageRecord.ResourceName,
                storageRecord.SubGroupKey, storageRecord.SubGroupName, storageRecord.GroupKey,
                storageRecord.GroupName);
            ResourceRecordWriter.Write(line);
        }

        public static readonly string TimeHeader =
@"Time.[FiscalWeekKey],Time.[Fiscal Week],Time.[FiscalMonthKey],Time.[Fiscal Month],Time.[FiscalQuarterKey],Time.[Fiscal Quarter],Time.[FiscalYearKey],Time.[Fiscal Year]";
        public static readonly string TimeRecordFormat = @"{0},{1},{2},{3},{4},{5},{6},{7}";
        public FileWriter TimeRecordWriter;
        public void WriteTimeRecord(TimeRecord timeRecord)
        {
            var line = string.Format(TimeRecordFormat, timeRecord.TimeKey.ToString("yyyy-MM-dd"), timeRecord.TimeName,
                timeRecord.MonthKey.ToString("yyyy-MM-dd"), timeRecord.MonthName, timeRecord.QuarterKey.ToString("yyyy-MM-dd"), timeRecord.QuarterName, 
                timeRecord.YearKey.ToString("yyyy-MM-dd"), timeRecord.YearName);
            TimeRecordWriter.Write(line);
        }

        public static readonly string GroupHeader =
@"GroupComponents.[GroupIDKey],GroupComponents.[Group ID]";
        public static readonly string GroupRecordFormat = @"{0},{1}";
        public FileWriter GroupRecordWriter;
        public void WriteGroupRecord(GroupRecord groupRecord)
        {
            var line = string.Format(GroupRecordFormat, groupRecord.GroupKey, groupRecord.GroupName);
            GroupRecordWriter.Write(line);
        }


        public static readonly string MaterialNodeParamHeader =
            @"Item.[ItemKey],Location.[LocationKey],Version.[VersionKey],Infinite Inventory,No Carry,Safety Stock Priority";
        public static readonly string MaterialNodeParamRecordFormat = @"{0},{1},{2},{3},{4},{5}";
        public FileWriter MaterialNodeParamWriter;
        public void WriteMaterialNodeParameters(ItemRecord itemRecord, LocationRecord locationRecord)
        {
            var line = String.Format(MaterialNodeParamRecordFormat, itemRecord, locationRecord, 0,
                (locationRecord.LocationKey == 7000 || locationRecord.LocationKey == 8000) ? 1 : 0,
                locationRecord.IsNoCarry ? 1 : 0, 0);
            MaterialNodeParamWriter.Write(line);
        }

        public static readonly string MaterialNodeTimeParamHeader =
            @"Item.[ItemKey],Location.[LocationKey],Time.[FiscalWeekKey],Version.[VersionKey],SS Target Qty";
        public static readonly string MaterialNodeTimeParamRecordFormat = @"{0},{1},{2},{3},{4}";
        public FileWriter MaterialNodeTimeParamWriter;
        public void WriteMaterialNodeTimeParameters(ItemRecord itemRecord, LocationRecord locationRecord,
            TimeRecord timeRecord, double ssTarget)
        {
            var line = String.Format(MaterialNodeTimeParamRecordFormat, itemRecord, locationRecord, timeRecord, 0,
                ssTarget);
            MaterialNodeTimeParamWriter.Write(line);
        }

        public static readonly string InventoryHeader =
            @"Item.[ItemKey],Location.[LocationKey],Time.[FiscalWeekKey],Version.[VersionKey],BOH,Expected Receipts";

        public static readonly string InventoryRecordFormat = @"{0},{1},{2},{3},{4},{5}";
        public FileWriter InventoryWriter;

        public void WriteBOHAndER(ItemRecord itemRecord, LocationRecord locationRecord, TimeRecord timeRecord,
            double? boh, double? er)
        {
            var line = String.Format(InventoryRecordFormat, itemRecord, locationRecord, timeRecord, 0, boh, er);
            InventoryWriter.Write(line);
        }

        public static readonly string StorageAvailParamHeader =
            @"Storage.[StorageKey],Product.[ProductKey],Time.[FiscalWeekKey],Version.[VersionKey],Storage Availability,Band_1 Storage,Band_2 Storage";

        public static readonly string StorageAvailRecordFormat = @"{0},{1},{2},{3},{4},{5},{6}";

        public FileWriter StorageAvailWriter;

        public void WriteStorageAvailability(LocationRecord locationRecord, ItemRecord product, TimeRecord timeRecord,
            double reg, double ot1, double ot2)
        {
            var line = String.Format(StorageAvailRecordFormat, locationRecord.Storage, product, timeRecord, 0, reg, ot1,
                ot2);
            StorageAvailWriter.Write(line);
        }

        public static readonly string StorageGraphHeader =
            @"Storage.[StorageKey],Product.[ProductKey],Item.[ItemKey],Location.[LocationKey],Version.[VersionKey],Storage Graph Association,Storage Qty Per";

        public static readonly string StorageGraphRecordFormat = @"{0},{1},{2},{3},{4},{5},{6}";
        public FileWriter StorageGraphWriter;

        public void WriteStorageGraph(LocationRecord locationRecord, ItemRecord itemRecord, ItemRecord productRecord)
        {
            var line = String.Format(StorageGraphRecordFormat, locationRecord.Storage, productRecord, itemRecord,
                locationRecord, 0, 1, 1);
            StorageGraphWriter.Write(line);
        }

        public static readonly string CapacityAvailParamHeader =
            @"Resource.[ResourceKey],Location.[LocationKey],Time.[FiscalWeekKey],Version.[VersionKey],Capacity Availability,Overtime_1,Overtime_2";

        public static readonly string CapacityAvailRecordFormat = @"{0},{1},{2},{3},{4},{5},{6}";

        public FileWriter CapacityAvailWriter;

        public void WriteHadelingCapacityAvailability(LocationRecord locationRecord, TimeRecord timeRecord, double reg,
            double ot1, double ot2)
        {
            var line = String.Format(CapacityAvailRecordFormat, locationRecord.HandelingResource, locationRecord,
                timeRecord, 0, reg, ot1, ot2);
            CapacityAvailWriter.Write(line);
        }

        public void WriteInBoundEnggCapacityAvailability(LocationRecord locationRecord, TimeRecord timeRecord,
            double reg, double ot1, double ot2)
        {
            var line = String.Format(CapacityAvailRecordFormat, locationRecord.InBoundEnggResource, locationRecord,
                timeRecord, 0, reg, ot1, ot2);
            CapacityAvailWriter.Write(line);
        }

        public void WriteOutBoundEnggCapacityAvailability(LocationRecord locationRecord, TimeRecord timeRecord,
            double reg, double ot1, double ot2)
        {
            var line = String.Format(CapacityAvailRecordFormat, locationRecord.OutBoundEnggResource, locationRecord,
                timeRecord, 0, reg, ot1, ot2);
            CapacityAvailWriter.Write(line);
        }

        public void WriteRegularCapacityAvailability(ActivityRecord activityRecord, TimeRecord timeRecord, double reg,
            double ot1, double ot2)
        {
            var line = String.Format(CapacityAvailRecordFormat, activityRecord.RegulaResourceRecord,
                activityRecord.ToLocationRecord, timeRecord, 0, reg, ot1, ot2);
            CapacityAvailWriter.Write(line);
        }

        public void WriteItemSpecRegularCapacityAvailability(ActivityRecord activityRecord, TimeRecord timeRecord,
            double reg, double ot1, double ot2)
        {
            foreach (var resourceRecord in activityRecord.ItemSpecificResourceRecords)
            {
                var line = String.Format(CapacityAvailRecordFormat, resourceRecord, activityRecord.ToLocationRecord,
                    timeRecord, 0, reg, ot1, ot2);
                CapacityAvailWriter.Write(line);
            }

        }

        public static readonly string ActivityParamHeader =
            @"Activity.[ActivityKey],Transport.[TransportKey],Product.[ProductKey],Version.[VersionKey],Activity Lead Time,Solver Activity End Date,Solver Activity Start Date";

        public static readonly string ActivityParamRecordFormat = @"{0},{1},{2},{3},{4},{5},{6}";

        public FileWriter ActivityParamWriter;

        public void WriteActivityParams(ActivityRecord actRecord, ItemRecord itemRecord)
        {
            var line = string.Format(ActivityParamRecordFormat, actRecord, 0, itemRecord, 0, 1, null, null);
            ActivityParamWriter.Write(line);
        }

        public static readonly string ProductionGraphHeader =
            @"Activity.[ActivityKey],Transport.[TransportKey],Product.[ProductKey],Item.[ItemKey],Location.[LocationKey],Version.[VersionKey],Material Production Graph Association,Material Production Min Qty,Material Production Multiple Qty,Material Production Priority,Material Production Qty Per,No Build";

        public static readonly string ProductionGraphRecordFormat = @"{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}";

        public FileWriter ProductionGraphWriter;

        public void WriteProductionGraph(ActivityRecord actRecord, ItemRecord itemRecord)
        {
            string line;
            if (!actRecord.ToLocationRecord.IsNoCarry)
            {
                 line = string.Format(ProductionGraphRecordFormat, actRecord, 0, itemRecord, itemRecord,
                    actRecord.ToLocationRecord, 0, 1, 50, 25, 1, 1, null);
            }
            else
            {
                line = string.Format(ProductionGraphRecordFormat, actRecord, 0, itemRecord, itemRecord,
                    actRecord.ToLocationRecord, 0, 1, null, null, 1, 1, null);
            }

            ProductionGraphWriter.Write(line);
        }

        
        public static readonly string ConsumptionGraphHeader =
            @"Activity.[ActivityKey],Transport.[TransportKey],Product.[ProductKey],Item.[ItemKey],Location.[LocationKey],Version.[VersionKey],Material Consumption Graph Association,Material Consumption Qty Per";

        public static readonly string ConsumptionGraphRecordFormat = @"{0},{1},{2},{3},{4},{5},{6},{7}";

        public FileWriter ConsumptionGraphWriter;

        public void WriteConsumptionGraph(ActivityRecord actRecord, ItemRecord itemRecord)
        {
            var line = string.Format(ConsumptionGraphRecordFormat, actRecord, 0, itemRecord, itemRecord,
                actRecord.FromLocationRecord, 0, 1, 1);
            ConsumptionGraphWriter.Write(line);
        }

        public static readonly string CapacityGraphHeader =
            @"Activity.[ActivityKey],Transport.[TransportKey],Product.[ProductKey],Resource.[ResourceKey],Location.[LocationKey],Version.[VersionKey],Capacity Category,Capacity Consumption Graph Association,Capacity Consumption Qty Per,Capacity Graph Priority";

        public static readonly string CapacityGraphRecordFormat = @"{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}";

        public FileWriter CapacityGraphGraphWriter;

        public static readonly string SimCapacityGraphHeader =
            @"GroupComponents.[GroupIDKey],Activity.[ActivityKey],Transport.[TransportKey],Product.[ProductKey],Resource.[ResourceKey],Location.[LocationKey],Version.[VersionKey],Simultaneous Cap Cons Association";

        public static readonly string SimCapacityGraphRecordFormat = @"{0},{1},{2},{3},{4},{5},{6},{7}";
        public FileWriter SimCapacityGraphGraphWriter;
        public static readonly string MaterialProductionGraphPlannedWIPheader = @"Time.[FiscalWeekKey],Location.[LocationKey],Activity.[ActivityKey],Item.[ItemKey],Version.[VersionKey],Transport.[TransportKey],Product.[ProductKey],Material Production Planned WIP";
        public FileWriter WIPRecordWriter;
        public  static  readonly string MaterialProductionGraphPlannedWIPRecordFormat = @"{0},{1},{2},{3},{4},{5},{6},{7}";
        public void WriteMaterialProductionGraphPlannedWIP(ActivityRecord activityRecord,ItemRecord itemRecord,TimeRecord timeRecord,TransportRecord transportRecord,int wip)
        {

            var line = string.Format(MaterialProductionGraphPlannedWIPRecordFormat,timeRecord,activityRecord.ToLocationRecord,activityRecord,itemRecord,"0",transportRecord,itemRecord,wip);
            WIPRecordWriter.Write(line);

        }
        public void WriteRegularResConsumptionGraph(ActivityRecord actRecord, ItemRecord itemRecord,
            bool addToSimCapacity)
        {
            var line = string.Format(CapacityGraphRecordFormat, actRecord, 0, itemRecord, actRecord.RegulaResourceRecord,
                actRecord.ToLocationRecord, 0, null, 1, 1, 1);
            CapacityGraphGraphWriter.Write(line);
            if (!addToSimCapacity) return;
            var simLine = string.Format(SimCapacityGraphRecordFormat, RegularCapEdgeGroup, actRecord, 0, itemRecord,
                actRecord.RegulaResourceRecord, actRecord.ToLocationRecord, 0, 1);
            SimCapacityGraphGraphWriter.Write(simLine);
        }

        public void WriteItemSpecRegularResConsumptionGraph(ActivityRecord actRecord, ItemRecord itemRecord, ResourceRecord resourceRecord,
            bool addToSimCapacity)
        {
            ResourceRecord itemSpecResource;
            actRecord.ItemSpecificResourceRecords.Add(itemSpecResource = resourceRecord);
            var line = string.Format(CapacityGraphRecordFormat, actRecord, 0, itemRecord, itemSpecResource,
                actRecord.ToLocationRecord, 0, null, 1, 1, 1);
            CapacityGraphGraphWriter.Write(line);

            if (!addToSimCapacity) return;
            var simLine = string.Format(SimCapacityGraphRecordFormat, RegularCapEdgeGroup, actRecord, 0, itemRecord,
                itemSpecResource, actRecord.ToLocationRecord, 0, 1);
            SimCapacityGraphGraphWriter.Write(simLine);
        }

        public void WriteInBoundHandelingResConsumptionGraph(ActivityRecord actRecord, ItemRecord itemRecord,
            bool addToSimCapacity)
        {
            var line = string.Format(CapacityGraphRecordFormat, actRecord, 0, itemRecord,
                actRecord.FromLocationRecord.HandelingResource, actRecord.FromLocationRecord, 0, "I", 1, 1, 1);
            CapacityGraphGraphWriter.Write(line);
            if (!addToSimCapacity) return;
            var simLine = string.Format(SimCapacityGraphRecordFormat, InBoundCapEdgeGroup, actRecord, 0, itemRecord,
                actRecord.FromLocationRecord.HandelingResource, actRecord.FromLocationRecord, 0, 1);
            SimCapacityGraphGraphWriter.Write(simLine);
        }

        public void WriteOutBoundHandelingResConsumptionGraph(ActivityRecord actRecord, ItemRecord itemRecord,
            bool addToSimCapacity)
        {
            var line = string.Format(CapacityGraphRecordFormat, actRecord, 0, itemRecord,
                actRecord.ToLocationRecord.HandelingResource, actRecord.ToLocationRecord, 0, "O", 1, 1, 1);
            CapacityGraphGraphWriter.Write(line);
            if (!addToSimCapacity) return;
            var simLine = string.Format(SimCapacityGraphRecordFormat, OutBoundCapEdgeGroup, actRecord, 0, itemRecord,
                actRecord.ToLocationRecord.HandelingResource, actRecord.ToLocationRecord, 0, 1);
            SimCapacityGraphGraphWriter.Write(simLine);
        }

        public void WriteInBoundEnggResConsumptionGraph(ActivityRecord actRecord, ItemRecord itemRecord,
            bool addToSimCapacity)
        {
            var line = string.Format(CapacityGraphRecordFormat, actRecord, 0, itemRecord,
                actRecord.FromLocationRecord.InBoundEnggResource, actRecord.FromLocationRecord, 0, "I", 1, 1, 1);
            CapacityGraphGraphWriter.Write(line);
            if (!addToSimCapacity) return;
            var simLine = string.Format(SimCapacityGraphRecordFormat, InBoundCapEdgeGroup, actRecord, 0, itemRecord,
                actRecord.FromLocationRecord.InBoundEnggResource, actRecord.FromLocationRecord, 0, 1);
            SimCapacityGraphGraphWriter.Write(simLine);
        }

        public void WriteOutBoundEnggResConsumptionGraph(ActivityRecord actRecord, ItemRecord itemRecord,
            bool addToSimCapacity)
        {
            var line = string.Format(CapacityGraphRecordFormat, actRecord, 0, itemRecord,
                actRecord.ToLocationRecord.OutBoundEnggResource, actRecord.ToLocationRecord, 0, "O", 1, 1, 1);
            CapacityGraphGraphWriter.Write(line);
            if (!addToSimCapacity) return;
            var simLine = string.Format(SimCapacityGraphRecordFormat, OutBoundCapEdgeGroup, actRecord, 0, itemRecord,
                actRecord.ToLocationRecord.OutBoundEnggResource, actRecord.ToLocationRecord, 0, 1);
            SimCapacityGraphGraphWriter.Write(simLine);
        }

        public void CloseWriters()
        {
            ItemRecordWriter.Close();
            ProductRecordWriter.Close();
            LocationRecordWriter.Close();
            TimeRecordWriter.Close();
            StorageRecordWriter.Close();
            ResourceRecordWriter.Close();
            ActivityRecordWriter.Close();
            GroupRecordWriter.Close();
            TransportRecordWriter.Close();
            CustomerRecordWriter.Close();
            OrderRecordWriter.Close();
            

            ActivityParamWriter.Close();
            ProductionGraphWriter.Close();
            ConsumptionGraphWriter.Close();
            CapacityGraphGraphWriter.Close();
            SimCapacityGraphGraphWriter.Close();
            CapacityAvailWriter.Close();
            MaterialNodeParamWriter.Close();
            InventoryWriter.Close();
            StorageAvailWriter.Close();
            StorageGraphWriter.Close();

            DemandRecordWriter.Close();
            MaterialNodeTimeParamWriter.Close();
            WIPRecordWriter.Close();
        }



        public struct ActivityRecord
        {
            public int ActivityKey;
            public string ActivityName;

            public int SubGroupKey;
            public int GroupKey;

            public string SubGroupName
            {
                get { return string.Format(SubGroupNameFormat, SubGroupKey); }
            }

            public string GroupName
            {
                get { return string.Format(GroupNameFormat, GroupKey); }
            }

            public ResourceRecord RegulaResourceRecord;
            public List<ResourceRecord> ItemSpecificResourceRecords;
            public LocationRecord FromLocationRecord;
            public LocationRecord ToLocationRecord;

            /// <summary>Returns the fully qualified type name of this instance.</summary>
            /// <returns>The fully qualified type name.</returns>
            public override string ToString()
            {
#if DEBUG
                return ActivityName;
#else
            return ActivityKey.ToString();
#endif
            }
        }

        public struct OrderRecord
        {
            public int OrderKey;
            public string OrderName;
            public int SubGroupKey;
            public int GroupKey;

           
            public string SubGroupName
            {
                get { return string.Format(SubGroupNameFormat, SubGroupKey); }
            }

            public string GroupName
            {
                get { return string.Format(GroupNameFormat, GroupKey); }
            }

            public override string ToString()
            {
#if DEBUG
                return OrderName;
#else
                return OrderKey.ToString();
#endif
            }
        }

        public struct ItemRecord
        {
            public int ItemKey;
            public string ItemName;
            public int SubGroupKey;
            public int GroupKey;
            
            public string SubGroupName
            {
                get { return string.Format(SubGroupNameFormat, SubGroupKey); }
            }

            public string GroupName
            {
                get { return string.Format(GroupNameFormat, GroupKey); }
            }

            public override string ToString()
            {
#if DEBUG
                return ItemName;
#else
            return ItemKey.ToString();
#endif
            }
        }

        public static string SubGroupNameFormat = "SubGroup-{0}";
        public static string GroupNameFormat = "Group-{0}";

        public struct LocationRecord
        {
            public int LocationKey;
            public string LocationName;
            public int SubGroupKey;
            public int GroupKey;

            public string SubGroupName
            {
                get { return string.Format(SubGroupNameFormat, SubGroupKey); }
            }
            
            public string GroupName
            {
                get { return string.Format(GroupNameFormat, GroupKey); }
            }

            public ResourceRecord HandelingResource;
            public ResourceRecord OutBoundEnggResource;
            public ResourceRecord InBoundEnggResource;
            public StorageRecord Storage;
            public bool IsNoCarry;

            public override string ToString()
            {
#if DEBUG
                return LocationName;
#else
            return LocationKey.ToString();
#endif
            }
        }

        public struct ResourceRecord
        {
            public int ResourceKey;
            public string ResourceName;
            public int SubGroupKey;
            public int GroupKey;

            public string SubGroupName
            {
                get { return string.Format(SubGroupNameFormat, SubGroupKey); }
            }

            public string GroupName
            {
                get { return string.Format(GroupNameFormat, GroupKey); }
            }

            public override string ToString()
            {
#if DEBUG
                return ResourceName;
#else
                return ResourceKey.ToString();
#endif
            }
        }

        public class GroupRecord
        {
            public int GroupKey;
            public string GroupName;

            public override string ToString()
            {
#if DEBUG
                return GroupName;
#else
                return GroupKey.ToString();
#endif
            }
        }

        public class TimeRecord
        {
            public DateTime TimeKey;

            public string TimeName
            {
                get { return string.Format("W{0}-{1}", TimeKey.DayOfYear / 7, TimeKey.Year); }
            }

            public DateTime MonthKey
            {
                get { return new DateTime(TimeKey.Year, TimeKey.Month, 1); }
            }

            public string MonthName 
            {
                get { return string.Format("M{0}-{1}", MonthKey.Month, TimeKey.Year);  }
            }

            public DateTime QuarterKey
            {
                get {return new DateTime(TimeKey.Year, TimeKey.Month/4 + 1, 1);}
            }

            public string QuarterName
            {
                get { return string.Format("Q{0}-{1}", TimeKey.Month / 4 + 1, TimeKey.Year); }
            }

            public DateTime YearKey
            {
                get { return new DateTime(TimeKey.Year, 1, 1); }
            }

            public string YearName
            {
                get { return string.Format("Y{0}", TimeKey.Year); }
            }

            public override string ToString()
            {
#if DEBUG
                return TimeName;
#else
                return TimeKey.ToString("yyyy-MM-dd");
#endif
            }
        }

        public struct StorageRecord
        {
            public int StorageKey;
            public string StorageName;

            public int SubGroupKey;
            public int GroupKey;

            public string SubGroupName
            {
                get { return string.Format(SubGroupNameFormat, SubGroupKey); }
            }

            public string GroupName
            {
                get { return string.Format(GroupNameFormat, GroupKey); }
            }

            public override string ToString()
            {
#if DEBUG
                return StorageName;
#else
            return StorageKey.ToString();
#endif
            }
        }

        public struct TransportRecord 
        {
            public int TransportKey;
            public string TransportName;

            public int SubGroupKey;
            public int GroupKey;

            public string SubGroupName
            {
                get { return string.Format(SubGroupNameFormat, SubGroupKey); }
            }

            public string GroupName
            {
                get { return string.Format(GroupNameFormat, GroupKey); }
            }

            public override string ToString()
            {
#if DEBUG
                return TransportName;
#else
            return TransportKey.ToString();
#endif
            }
        }

        public struct CustomerRecord
        {
            public int CustomerKey;
            public string CustomerName;

            public int SubGroupKey;
            public int GroupKey;

            public string SubGroupName
            {
                get { return string.Format(SubGroupNameFormat, SubGroupKey); }
            }

            public string GroupName
            {
                get { return string.Format(GroupNameFormat, GroupKey); }
            }

            public override string ToString()
            {
#if DEBUG
                return CustomerName;
#else
            return CustomerKey.ToString();
#endif
            }
        }
    }
}

