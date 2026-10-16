using activity_00_tap_26_27.Data;

using GameLibrary.CSV;
using GameLibrary.Data;

namespace GameLibraryTests.Data
{
    public class ItemReaderTests
    {
        private static readonly string[] ITEM_HEADER = new string[] { "Id", "Name", "Price" };

        [Test]
        public void Read_ReturnsAllItemsWithoutErrorForValidFile()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("items.csv");

            csv_table.SetHeader(ITEM_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "COLT", "Colt Peacemaker", "50" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "RIFLE", "Winchester Rifle", "120" }));

            Dictionary<string, ItemData> item_datas = ItemReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0));
            Assert.That(item_datas.Count, Is.EqualTo(2));
            Assert.That(item_datas["COLT"]._id, Is.EqualTo("COLT"));
            Assert.That(item_datas["COLT"]._name, Is.EqualTo("Colt Peacemaker"));
            Assert.That(item_datas["COLT"]._price, Is.EqualTo(50));
        }

        [Test]
        public void Read_AcceptsFreeItem()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("items.csv");

            csv_table.SetHeader(ITEM_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "FEATHER", "Feather", "0" }));

            ItemReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0));
        }

        [Test]
        public void Read_ReportsMissingColumn()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("items.csv");

            csv_table.SetHeader(new string[] { "Id", "Name" });
            csv_table.AddRow(new CsvRow(2, new string[] { "COLT", "Colt Peacemaker" }));

            ItemReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.GreaterThanOrEqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("items.csv"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("Price"));
        }

        [Test]
        public void Read_ReportsEmptyId()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("items.csv");

            csv_table.SetHeader(ITEM_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "", "Colt Peacemaker", "50" }));

            ItemReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
        }

        [Test]
        public void Read_ReportsEmptyName()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("items.csv");

            csv_table.SetHeader(ITEM_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "COLT", "", "50" }));

            ItemReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
        }

        [Test]
        public void Read_ReportsIdDeclaredTwice()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("items.csv");

            csv_table.SetHeader(ITEM_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "COLT", "Colt Peacemaker", "50" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "COLT", "Colt Navy", "40" }));

            ItemReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("COLT"));
        }

        [Test]
        public void Read_ReportsNegativePrice()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("items.csv");

            csv_table.SetHeader(ITEM_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "COLT", "Colt Peacemaker", "-50" }));

            ItemReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("-50"));
        }

        [Test]
        public void Read_ReportsPriceThatIsNotAWholeNumber()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("items.csv");

            csv_table.SetHeader(ITEM_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "COLT", "Colt Peacemaker", "2.5" }));

            ItemReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("2.5"));
        }

        [Test]
        public void Read_ReportsEveryErrorOfTheFile()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("items.csv");

            csv_table.SetHeader(ITEM_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "", "Colt Peacemaker", "50" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "RIFLE", "", "120" }));
            csv_table.AddRow(new CsvRow(4, new string[] { "KNIFE", "Bowie Knife", "2.5" }));

            ItemReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(3));
        }

        [Test]
        public void Read_ReportsEmptyPrice()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("items.csv");

            csv_table.SetHeader(ITEM_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "COLT", "Colt Peacemaker", "" }));

            ItemReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("Price"));
        }

        [Test]
        public void Read_ReturnsTheValidItemsAfterAWrongLine()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("items.csv");

            csv_table.SetHeader(ITEM_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "COLT", "Colt Peacemaker", "cheap" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "RIFLE", "Winchester Rifle", "120" }));

            Dictionary<string, ItemData> item_datas = ItemReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(item_datas.ContainsKey("RIFLE"), Is.True);
        }

        [Test]
        public void Read_ReturnsTheItemsEvenIfTheReportAlreadyHasErrorsFromAnotherFile()
        {
            ContentReport report = new ContentReport();
            report.AddError("locations.csv", 4, "an error in another file");
            CsvTable csv_table = new CsvTable("items.csv");

            csv_table.SetHeader(ITEM_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "COLT", "Colt Peacemaker", "50" }));

            Dictionary<string, ItemData> item_datas = ItemReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(item_datas.Count, Is.EqualTo(1));
        }
    }
}
