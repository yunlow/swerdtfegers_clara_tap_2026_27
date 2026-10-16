using GameLibrary.Csv;
using GameLibrary.Data;

namespace GameLibraryTests.Data
{
    // Fiche 05, partie 2 : mines.csv.
    // Un test par règle du tableau de la fiche. Chaque test d'erreur part d'un fichier
    // valide et casse une seule chose : il attend donc exactement une erreur.
    public class MineReaderTests
    {
        private static readonly string[] MINE_HEADER = new string[] { "LocationId", "MinutesPerStrike", "Stock" };

        private List<LocationData> CreateLocations()
        {
            List<LocationData> location_datas = new List<LocationData>();

            location_datas.Add(new LocationData("WORLD", "World", "", 0));
            location_datas.Add(new LocationData("DAISY_TOWN", "Daisy Town", "WORLD", 180));
            location_datas.Add(new LocationData("GUN_SHOP", "Gun Shop", "DAISY_TOWN", 5));
            location_datas.Add(new LocationData("SILVER_MINE", "Silver Mine", "WORLD", 240));
            location_datas.Add(new LocationData("MINE_LEVEL_1", "Mine Level 1", "SILVER_MINE", 15));
            location_datas.Add(new LocationData("MINE_LEVEL_2", "Mine Level 2", "MINE_LEVEL_1", 20));

            return location_datas;
        }

        private List<ShopData> CreateShops()
        {
            List<ShopData> shop_datas = new List<ShopData>();

            shop_datas.Add(new ShopData("GUN_SHOP", 8, 18));

            return shop_datas;
        }

        private CsvTable CreateTable()
        {
            CsvTable csv_table = new CsvTable("mines.csv");

            csv_table.SetHeader(MINE_HEADER);

            return csv_table;
        }

        [Test]
        public void Read_ReturnsAllMinesWithoutErrorForValidFile()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "30", "10" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "MINE_LEVEL_2", "45", "4" }));

            List<MineData> mine_datas = MineReader.Read(csv_table, CreateLocations(), CreateShops(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0));
            Assert.That(mine_datas.Count, Is.EqualTo(2));
            Assert.That(mine_datas[0]._locationId, Is.EqualTo("MINE_LEVEL_1"));
            Assert.That(mine_datas[0]._minutesPerStrike, Is.EqualTo(30));
            Assert.That(mine_datas[0]._stock, Is.EqualTo(10));
        }

        [Test]
        public void Read_AcceptsAStockOfZero()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "30", "0" }));

            List<MineData> mine_datas = MineReader.Read(csv_table, CreateLocations(), CreateShops(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0));
            Assert.That(mine_datas[0]._stock, Is.EqualTo(0));
        }

        [Test]
        public void Read_ReportsUnknownLocationWithItsLine()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "30", "10" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "MINE_LEVEL_3", "30", "10" }));

            MineReader.Read(csv_table, CreateLocations(), CreateShops(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("MINE_LEVEL_3"));
        }

        [Test]
        public void Read_ReportsLocationThatIsAlreadyAShop()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "30", "10" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "GUN_SHOP", "30", "10" }));

            MineReader.Read(csv_table, CreateLocations(), CreateShops(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("GUN_SHOP"));
        }

        [Test]
        public void Read_ReportsLocationDeclaredTwice()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "30", "10" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "MINE_LEVEL_1", "20", "5" }));

            MineReader.Read(csv_table, CreateLocations(), CreateShops(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("MINE_LEVEL_1"));
        }

        [Test]
        public void Read_ReportsMinutesPerStrikeThatIsNotAWholeNumber()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "half", "10" }));

            MineReader.Read(csv_table, CreateLocations(), CreateShops(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("half"));
        }

        // La borne : 0 est refusé (un coup instantané n'a pas de sens), 1 est accepté.
        [Test]
        public void Read_ReportsMinutesPerStrikeOfZero()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "0", "10" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "MINE_LEVEL_2", "1", "10" }));

            MineReader.Read(csv_table, CreateLocations(), CreateShops(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("MinutesPerStrike"));
        }

        [Test]
        public void Read_ReportsStockThatIsNotAWholeNumber()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "30", "ten" }));

            MineReader.Read(csv_table, CreateLocations(), CreateShops(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("ten"));
        }

        [Test]
        public void Read_ReportsNegativeStock()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "30", "-1" }));

            MineReader.Read(csv_table, CreateLocations(), CreateShops(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("-1"));
        }

        [Test]
        public void Read_ReportsEveryErrorOfTheFile()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_3", "30", "10" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "MINE_LEVEL_1", "0", "10" }));
            csv_table.AddRow(new CsvRow(4, new string[] { "MINE_LEVEL_2", "30", "-1" }));

            MineReader.Read(csv_table, CreateLocations(), CreateShops(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(3));
        }
    }
}
