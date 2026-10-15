using activity_00_tap_26_27.Data;
using GameLibrary;
using GameLibrary.Csv;
using GameLibrary.CSV;
using GameLibrary.Data;

namespace GameLibraryTests
{
    public class InventoryReaderTests
    {
        private static readonly string[] INVENTORY_HEADER = new string[] { "OwnerId", "ItemId", "Quantity" };

        private List<ShopData> CreateShops()
        {
            List<ShopData> shop_datas = new List<ShopData>();

            shop_datas.Add(new ShopData("GUN_SHOP", 8, 18));

            return shop_datas;
        }

        private List<HeroData> CreateHeroes()
        {
            List<HeroData> hero_datas = new List<HeroData>();

            hero_datas.Add(new HeroData("LUCKY_LUKE", "Lucky Luke", 100, 40));

            return hero_datas;
        }

        private Dictionary<string, ItemData> CreateItems()
        {
            Dictionary<string, ItemData> item_data_table = new Dictionary<string, ItemData>();

            item_data_table.Add("COLT", new ItemData("COLT", "Colt Peacemaker", 50));
            item_data_table.Add("RIFLE", new ItemData("RIFLE", "Winchester Rifle", 120));
            item_data_table.Add("LASSO", new ItemData("LASSO", "Lasso", 15));

            return item_data_table;
        }

        [Test]
        public void Read_ReturnsAllEntriesWithoutErrorForValidFile()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("inventories.csv");

            csv_table.SetHeader(INVENTORY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "GUN_SHOP", "COLT", "3" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "GUN_SHOP", "RIFLE", "1" }));
            csv_table.AddRow(new CsvRow(4, new string[] { "LUCKY_LUKE", "LASSO", "1" }));

            List<InventoryEntryData> entry_datas = InventoryReader.Read(csv_table, CreateShops(), CreateHeroes(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0));
            Assert.That(entry_datas.Count, Is.EqualTo(3));
            Assert.That(entry_datas[0]._ownerId, Is.EqualTo("GUN_SHOP"));
            Assert.That(entry_datas[0]._itemId, Is.EqualTo("COLT"));
            Assert.That(entry_datas[0]._quantity, Is.EqualTo(3));
            Assert.That(entry_datas[2]._ownerId, Is.EqualTo("LUCKY_LUKE"));
        }

        [Test]
        public void Read_AcceptsSameItemForTwoOwners()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("inventories.csv");

            csv_table.SetHeader(INVENTORY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "GUN_SHOP", "COLT", "3" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "LUCKY_LUKE", "COLT", "1" }));

            InventoryReader.Read(csv_table, CreateShops(), CreateHeroes(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0));
        }

        [Test]
        public void Read_AcceptsZeroQuantity()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("inventories.csv");

            csv_table.SetHeader(INVENTORY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "GUN_SHOP", "RIFLE", "0" }));

            InventoryReader.Read(csv_table, CreateShops(), CreateHeroes(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0));
        }

        [Test]
        public void Read_ReportsMissingColumn()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("inventories.csv");

            csv_table.SetHeader(new string[] { "OwnerId", "ItemId" });
            csv_table.AddRow(new CsvRow(2, new string[] { "GUN_SHOP", "COLT" }));

            InventoryReader.Read(csv_table, CreateShops(), CreateHeroes(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.GreaterThanOrEqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("inventories.csv"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("Quantity"));
        }

        [Test]
        public void Read_ReportsLocationThatIsNotAShop()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("inventories.csv");

            csv_table.SetHeader(INVENTORY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "GUN_SHOP", "COLT", "3" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "DAISY_TOWN", "COLT", "1" }));

            InventoryReader.Read(csv_table, CreateShops(), CreateHeroes(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("DAISY_TOWN"));
        }

        [Test]
        public void Read_ReportsUnknownShopWithItsLine()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("inventories.csv");

            csv_table.SetHeader(INVENTORY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "GUN_SHOP", "COLT", "3" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "DAISY_TOWN", "COLT", "1" }));

            InventoryReader.Read(csv_table, CreateShops(), CreateHeroes(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("DAISY_TOWN"));
        }

        [Test]
        public void Read_ReportsUnknownOwnerWithItsLine()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("inventories.csv");

            csv_table.SetHeader(INVENTORY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "CALAMITY_JANE", "COLT", "1" }));

            InventoryReader.Read(csv_table, CreateShops(), CreateHeroes(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("CALAMITY_JANE"));
        }

        [Test]
        public void Read_ReportsUnknownItemWithItsLine()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("inventories.csv");

            csv_table.SetHeader(INVENTORY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "GUN_SHOP", "COLTT", "3" }));

            InventoryReader.Read(csv_table, CreateShops(), CreateHeroes(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("COLTT"));
        }

        [Test]
        public void Read_ReportsSameItemTwiceForOneOwner()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("inventories.csv");

            csv_table.SetHeader(INVENTORY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "GUN_SHOP", "COLT", "3" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "GUN_SHOP", "RIFLE", "1" }));
            csv_table.AddRow(new CsvRow(4, new string[] { "GUN_SHOP", "COLT", "2" }));

            InventoryReader.Read(csv_table, CreateShops(), CreateHeroes(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 4"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
        }

        [Test]
        public void Read_ReportsQuantityThatIsNotAWholeNumber()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("inventories.csv");

            csv_table.SetHeader(INVENTORY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "GUN_SHOP", "COLT", "two" }));

            InventoryReader.Read(csv_table, CreateShops(), CreateHeroes(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("two"));
        }

        [Test]
        public void Read_ReportsNegativeQuantity()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("inventories.csv");

            csv_table.SetHeader(INVENTORY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "GUN_SHOP", "COLT", "-1" }));

            InventoryReader.Read(csv_table, CreateShops(), CreateHeroes(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("-1"));
        }

        [Test]
        public void Read_ReportsEveryErrorOfTheFile()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("inventories.csv");

            csv_table.SetHeader(INVENTORY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "DAISY_TOWN", "COLT", "1" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "GUN_SHOP", "COLTT", "3" }));
            csv_table.AddRow(new CsvRow(4, new string[] { "LUCKY_LUKE", "LASSO", "two" }));

            InventoryReader.Read(csv_table, CreateShops(), CreateHeroes(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(3));
        }

        [Test]
        public void Read_ReportsEmptyQuantity()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("inventories.csv");

            csv_table.SetHeader(INVENTORY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "GUN_SHOP", "COLT", "" }));

            InventoryReader.Read(csv_table, CreateShops(), CreateHeroes(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("Quantity"));
        }

        [Test]
        public void Read_ReturnsTheEntriesEvenIfTheReportAlreadyHasErrorsFromAnotherFile()
        {
            ContentReport report = new ContentReport();
            report.AddError("locations.csv", 4, "an error in another file");
            CsvTable csv_table = new CsvTable("inventories.csv");

            csv_table.SetHeader(INVENTORY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "GUN_SHOP", "COLT", "3" }));

            List<InventoryEntryData> inventory_datas = InventoryReader.Read(csv_table, CreateShops(), CreateHeroes(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(inventory_datas.Count, Is.EqualTo(1));
        }
    }
}
