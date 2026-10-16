using activity_00_tap_26_27.Data;

using GameLibrary.CSV;
using GameLibrary.Data;

namespace GameLibraryTests.Data
{
    public class HeroReaderTests
    {
        private static readonly string[] HERO_HEADER = new string[] { "Id", "Name", "Health", "Gold" };

        private List<LocationData> CreateLocations()
        {
            List<LocationData> location_datas = new List<LocationData>();

            location_datas.Add(new LocationData("WORLD", "World", "", 0));
            location_datas.Add(new LocationData("DAISY_TOWN", "Daisy Town", "WORLD", 180));
            location_datas.Add(new LocationData("GUN_SHOP", "Gun Shop", "DAISY_TOWN", 5));

            return location_datas;
        }

        [Test]
        public void Read_ReturnsTheHeroWithoutErrorForValidFile()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("heroes.csv");

            csv_table.SetHeader(HERO_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "LUCKY_LUKE", "Lucky Luke", "100", "40" }));

            List<HeroData> hero_datas = HeroReader.Read(csv_table, CreateLocations(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0));
            Assert.That(hero_datas.Count, Is.EqualTo(1));
            Assert.That(hero_datas[0]._id, Is.EqualTo("LUCKY_LUKE"));
            Assert.That(hero_datas[0]._name, Is.EqualTo("Lucky Luke"));
            Assert.That(hero_datas[0]._health, Is.EqualTo(100));
            Assert.That(hero_datas[0]._gold, Is.EqualTo(40));
        }

        [Test]
        public void Read_ReportsMissingColumn()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("heroes.csv");

            csv_table.SetHeader(new string[] { "Id", "Name", "Health" });
            csv_table.AddRow(new CsvRow(2, new string[] { "LUCKY_LUKE", "Lucky Luke", "100" }));

            HeroReader.Read(csv_table, CreateLocations(), report);

            Assert.That(report.GetErrorCount(), Is.GreaterThanOrEqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("heroes.csv"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("Gold"));
        }

        [Test]
        public void Read_ReportsFileWithoutHero()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("heroes.csv");

            csv_table.SetHeader(HERO_HEADER);

            HeroReader.Read(csv_table, CreateLocations(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("heroes.csv"));
        }

        [Test]
        public void Read_ReportsMoreThanOneHero()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("heroes.csv");

            csv_table.SetHeader(HERO_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "LUCKY_LUKE", "Lucky Luke", "100", "40" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "JOLLY_JUMPER", "Jolly Jumper", "80", "0" }));

            HeroReader.Read(csv_table, CreateLocations(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("heroes.csv"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("2"));
        }

        [Test]
        public void Read_ReportsEmptyId()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("heroes.csv");

            csv_table.SetHeader(HERO_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "", "Lucky Luke", "100", "40" }));

            HeroReader.Read(csv_table, CreateLocations(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
        }

        [Test]
        public void Read_ReportsEmptyName()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("heroes.csv");

            csv_table.SetHeader(HERO_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "LUCKY_LUKE", "", "100", "40" }));

            HeroReader.Read(csv_table, CreateLocations(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
        }

        [Test]
        public void Read_ReportsIdAlreadyUsedByALocation()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("heroes.csv");

            csv_table.SetHeader(HERO_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "GUN_SHOP", "Gun Shop Keeper", "100", "40" }));

            HeroReader.Read(csv_table, CreateLocations(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("GUN_SHOP"));
        }

        [Test]
        public void Read_ReportsHealthThatIsNotAboveZero()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("heroes.csv");

            csv_table.SetHeader(HERO_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "LUCKY_LUKE", "Lucky Luke", "0", "40" }));

            HeroReader.Read(csv_table, CreateLocations(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("health"));
        }

        [Test]
        public void Read_ReportsHealthThatIsNotAWholeNumber()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("heroes.csv");

            csv_table.SetHeader(HERO_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "LUCKY_LUKE", "Lucky Luke", "strong", "40" }));

            HeroReader.Read(csv_table, CreateLocations(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("strong"));
        }

        [Test]
        public void Read_ReportsNegativeGold()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("heroes.csv");

            csv_table.SetHeader(HERO_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "LUCKY_LUKE", "Lucky Luke", "100", "-5" }));

            HeroReader.Read(csv_table, CreateLocations(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("-5"));
        }

        [Test]
        public void Read_ReportsGoldThatIsNotAWholeNumber()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("heroes.csv");

            csv_table.SetHeader(HERO_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "LUCKY_LUKE", "Lucky Luke", "100", "2,5" }));

            HeroReader.Read(csv_table, CreateLocations(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("2,5"));
        }

        [Test]
        public void Read_ReportsEveryErrorOfTheFile()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("heroes.csv");

            csv_table.SetHeader(HERO_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "LUCKY_LUKE", "", "0", "-1" }));

            HeroReader.Read(csv_table, CreateLocations(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(3));
        }

        [Test]
        public void Read_ReportsEmptyHealth()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("heroes.csv");

            csv_table.SetHeader(HERO_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "LUCKY_LUKE", "Lucky Luke", "", "40" }));

            HeroReader.Read(csv_table, CreateLocations(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("Health"));
        }

        [Test]
        public void Read_ReturnsTheHeroEvenIfTheReportAlreadyHasErrorsFromAnotherFile()
        {
            ContentReport report = new ContentReport();
            report.AddError("locations.csv", 4, "an error in another file");
            CsvTable csv_table = new CsvTable("heroes.csv");

            csv_table.SetHeader(HERO_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "LUCKY_LUKE", "Lucky Luke", "100", "40" }));

            List<HeroData> hero_datas = HeroReader.Read(csv_table, CreateLocations(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(hero_datas.Count, Is.EqualTo(1));
        }
    }
}
