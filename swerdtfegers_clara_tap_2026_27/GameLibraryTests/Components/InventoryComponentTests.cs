using activity_00_tap_26_27.Data;
using GameLibrary;
using GameLibrary.Components;
using GameLibrary.Data;

namespace GameLibraryTests
{
    public class InventoryComponentTests
    {
        private static readonly ItemData COLT = new ItemData("COLT", "Colt Peacemaker", 50);
        private static readonly ItemData LASSO = new ItemData("LASSO", "Lasso", 15);

        [Test]
        public void GetEntryCount_ReturnsZeroForNewInventory()
        {
            InventoryComponent inventory = new InventoryComponent(null);

            int entry_count = inventory.GetEntryCount();

            Assert.That(entry_count, Is.EqualTo(0));
        }

        [Test]
        public void Add_CreatesAnEntryWithItsQuantity()
        {
            InventoryComponent inventory = new InventoryComponent(null);

            inventory.Add(COLT, 3);

            Assert.That(inventory.GetEntryCount(), Is.EqualTo(1));
            Assert.That(inventory.GetEntryAtIndex(0).GetItem(), Is.EqualTo(COLT));
            Assert.That(inventory.GetEntryAtIndex(0).GetQuantity(), Is.EqualTo(3));
        }

        [Test]
        public void Add_SameItemTwiceKeepsOneEntry()
        {
            InventoryComponent inventory = new InventoryComponent(null);

            inventory.Add(COLT, 3);
            inventory.Add(COLT, 2);

            Assert.That(inventory.GetEntryCount(), Is.EqualTo(1));
            Assert.That(inventory.GetQuantity("COLT"), Is.EqualTo(5));
        }

        [Test]
        public void Add_KeepsAddedOrder()
        {
            InventoryComponent inventory = new InventoryComponent(null);

            inventory.Add(LASSO, 1);
            inventory.Add(COLT, 3);

            Assert.That(inventory.GetEntryAtIndex(0).GetItem()._id, Is.EqualTo("LASSO"));
            Assert.That(inventory.GetEntryAtIndex(1).GetItem()._id, Is.EqualTo("COLT"));
        }

        [Test]
        public void Add_ThrowsForZeroOrNegativeQuantity()
        {
            InventoryComponent inventory = new InventoryComponent(null);

            Assert.Throws<ArgumentException>(() => inventory.Add(COLT, 0));
            Assert.Throws<ArgumentException>(() => inventory.Add(COLT, -1));
        }

        [Test]
        public void GetQuantity_ReturnsZeroForAbsentItem()
        {
            InventoryComponent inventory = new InventoryComponent(null);
            inventory.Add(COLT, 3);

            int quantity = inventory.GetQuantity("LASSO");

            Assert.That(quantity, Is.EqualTo(0));
        }

        [Test]
        public void GetEntry_ReturnsTheEntryOfTheItem()
        {
            InventoryComponent inventory = new InventoryComponent(null);
            inventory.Add(LASSO, 1);
            inventory.Add(COLT, 3);

            InventoryEntry entry = inventory.GetEntry("COLT");

            Assert.That(entry.GetItem(), Is.EqualTo(COLT));
            Assert.That(entry.GetQuantity(), Is.EqualTo(3));
        }

        [Test]
        public void GetEntry_ReturnsNullForAbsentItem()
        {
            InventoryComponent inventory = new InventoryComponent(null);
            inventory.Add(COLT, 3);

            Assert.That(inventory.GetEntry("LASSO"), Is.Null);
        }

        [Test]
        public void TryRemove_DecreasesQuantity()
        {
            InventoryComponent inventory = new InventoryComponent(null);
            inventory.Add(COLT, 3);

            bool is_removed = inventory.TryRemove("COLT", 2);

            Assert.That(is_removed, Is.True);
            Assert.That(inventory.GetQuantity("COLT"), Is.EqualTo(1));
        }

        [Test]
        public void TryRemove_ReturnsFalseAndChangesNothingWhenThereIsNotEnough()
        {
            InventoryComponent inventory = new InventoryComponent(null);
            inventory.Add(COLT, 3);

            bool is_removed = inventory.TryRemove("COLT", 4);

            Assert.That(is_removed, Is.False);
            Assert.That(inventory.GetQuantity("COLT"), Is.EqualTo(3));
        }

        [Test]
        public void TryRemove_ReturnsFalseForAbsentItem()
        {
            InventoryComponent inventory = new InventoryComponent(null);

            bool is_removed = inventory.TryRemove("COLT", 1);

            Assert.That(is_removed, Is.False);
        }

        [Test]
        public void TryRemove_KeepsEntryThatFallsToZero()
        {
            InventoryComponent inventory = new InventoryComponent(null);
            inventory.Add(COLT, 1);

            inventory.TryRemove("COLT", 1);

            Assert.That(inventory.GetEntryCount(), Is.EqualTo(1));
            Assert.That(inventory.GetEntryAtIndex(0).GetQuantity(), Is.EqualTo(0));
        }

        [Test]
        public void TryRemove_ThrowsForZeroOrNegativeQuantity()
        {
            InventoryComponent inventory = new InventoryComponent(null);
            inventory.Add(COLT, 3);

            Assert.Throws<ArgumentException>(() => inventory.TryRemove("COLT", 0));
            Assert.Throws<ArgumentException>(() => inventory.TryRemove("COLT", -1));
        }
    }
}
