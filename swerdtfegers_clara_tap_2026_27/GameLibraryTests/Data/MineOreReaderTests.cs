using GameLibrary.Csv;
using GameLibrary.Data;

namespace GameLibraryTests.Data
{
    // Fiche 05, partie 2 : mine_ores.csv.
    // Attention à la règle « le total des poids d'une mine est > 0 » : chaque mine reçue
    // par le lecteur doit avoir au moins un minerai de poids > 0, sinon c'est une erreur.
    // Les tests ne passent donc que les mines qu'ils remplissent.
    public class MineOreReaderTests
    {
        private static readonly string[] MINE_ORE_HEADER = new string[] { "LocationId", "ItemId", "Weight" };

        private List<MineData> CreateMines(string[] mine_id_table)
        {
            List<MineData> mine_datas = new List<MineData>();

            for (int mine_index = 0; mine_index < mine_id_table.Length; mine_index++)
            {
                mine_datas.Add(new MineData(mine_id_table[mine_index], 30, 10));
            }

            return mine_datas;
        }

        private List<MineData> CreateFirstMineOnly()
        {
            return CreateMines(new string[] { "MINE_LEVEL_1" });
        }

        private Dictionary<string, ItemData> CreateItems()
        {
            Dictionary<string, ItemData> item_datas = new Dictionary<string, ItemData>();

            item_datas.Add("COPPER_ORE", new ItemData("COPPER_ORE", "Copper ore", 2));
            item_datas.Add("SILVER_ORE", new ItemData("SILVER_ORE", "Silver ore", 8));
            item_datas.Add("GOLD_NUGGET", new ItemData("GOLD_NUGGET", "Gold nugget", 40));
            item_datas.Add("COLT", new ItemData("COLT", "Colt Peacemaker", 50));

            return item_datas;
        }

        private CsvTable CreateTable()
        {
            CsvTable csv_table = new CsvTable("mine_ores.csv");

            csv_table.SetHeader(MINE_ORE_HEADER);

            return csv_table;
        }

        [Test]
        public void Read_ReturnsAllOresWithoutErrorForValidFile()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "COPPER_ORE", "60" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "MINE_LEVEL_1", "SILVER_ORE", "35" }));
            csv_table.AddRow(new CsvRow(4, new string[] { "MINE_LEVEL_1", "GOLD_NUGGET", "5" }));

            List<MineOreData> mine_ore_datas = MineOreReader.Read(csv_table, CreateFirstMineOnly(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0));
            Assert.That(mine_ore_datas.Count, Is.EqualTo(3));
            Assert.That(mine_ore_datas[1]._locationId, Is.EqualTo("MINE_LEVEL_1"));
            Assert.That(mine_ore_datas[1]._itemId, Is.EqualTo("SILVER_ORE"));
            Assert.That(mine_ore_datas[1]._weight, Is.EqualTo(35));
        }

        // Un poids de 0 est permis : le designer « éteint » un minerai sans effacer sa ligne.
        [Test]
        public void Read_AcceptsAWeightOfZeroWhenTheMineHasOtherOres()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "COPPER_ORE", "60" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "MINE_LEVEL_1", "GOLD_NUGGET", "0" }));

            MineOreReader.Read(csv_table, CreateFirstMineOnly(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0));
        }

        // La clé est le couple (mine, minerai) : le même minerai peut sortir de deux mines.
        [Test]
        public void Read_AcceptsTheSameOreInTwoMines()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "SILVER_ORE", "35" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "MINE_LEVEL_2", "SILVER_ORE", "70" }));

            MineOreReader.Read(csv_table, CreateMines(new string[] { "MINE_LEVEL_1", "MINE_LEVEL_2" }), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0));
        }

        [Test]
        public void Read_ReportsLocationThatIsNotAMine()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "COPPER_ORE", "60" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "LAUGHING_HORSE_INN", "COPPER_ORE", "10" }));

            MineOreReader.Read(csv_table, CreateFirstMineOnly(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("LAUGHING_HORSE_INN"));
        }

        [Test]
        public void Read_ReportsUnknownItem()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "COPPER_ORE", "60" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "MINE_LEVEL_1", "SILVER_ORR", "35" }));

            MineOreReader.Read(csv_table, CreateFirstMineOnly(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("SILVER_ORR"));
        }

        [Test]
        public void Read_ReportsWeightThatIsNotAWholeNumber()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "COPPER_ORE", "60" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "MINE_LEVEL_1", "SILVER_ORE", "rare" }));

            MineOreReader.Read(csv_table, CreateFirstMineOnly(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("rare"));
        }

        [Test]
        public void Read_ReportsNegativeWeight()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "COPPER_ORE", "60" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "MINE_LEVEL_1", "SILVER_ORE", "-5" }));

            MineOreReader.Read(csv_table, CreateFirstMineOnly(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("-5"));
        }

        [Test]
        public void Read_ReportsOreDeclaredTwiceForTheSameMine()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "COPPER_ORE", "60" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "MINE_LEVEL_1", "SILVER_ORE", "35" }));
            csv_table.AddRow(new CsvRow(4, new string[] { "MINE_LEVEL_1", "SILVER_ORE", "10" }));

            MineOreReader.Read(csv_table, CreateFirstMineOnly(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 4"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("SILVER_ORE"));
        }

        [Test]
        public void Read_ReportsMineWhoseWeightsAreAllZero()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "COPPER_ORE", "0" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "MINE_LEVEL_1", "SILVER_ORE", "0" }));

            MineOreReader.Read(csv_table, CreateFirstMineOnly(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("MINE_LEVEL_1"));
        }

        // Même règle : une mine sans aucune ligne a un total de 0.
        [Test]
        public void Read_ReportsMineWithoutAnyOre()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "COPPER_ORE", "60" }));

            MineOreReader.Read(csv_table, CreateMines(new string[] { "MINE_LEVEL_1", "MINE_LEVEL_2" }), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("MINE_LEVEL_2"));
        }

        [Test]
        public void Read_ReportsEveryErrorOfTheFile()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = CreateTable();
            csv_table.AddRow(new CsvRow(2, new string[] { "MINE_LEVEL_1", "COPPER_ORE", "60" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "LAUGHING_HORSE_INN", "COPPER_ORE", "10" }));
            csv_table.AddRow(new CsvRow(4, new string[] { "MINE_LEVEL_1", "SILVER_ORR", "35" }));
            csv_table.AddRow(new CsvRow(5, new string[] { "MINE_LEVEL_1", "GOLD_NUGGET", "rare" }));

            MineOreReader.Read(csv_table, CreateFirstMineOnly(), CreateItems(), report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(3));
        }
    }
}
