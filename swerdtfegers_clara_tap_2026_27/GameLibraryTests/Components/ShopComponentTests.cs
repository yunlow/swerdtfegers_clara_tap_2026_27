using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Data;
using GameLibrary;
using GameLibrary.Components;
using GameLibrary.Data;

namespace GameLibraryTests
{
    public class ShopComponentTests
    {
        private ShopComponent CreateShop(int opening_hour, int closing_hour)
        {
            ShopData shop_data = new ShopData("GUN_SHOP", opening_hour, closing_hour);

            return new ShopComponent(shop_data, new GameObject("GUN_SHOP"));
        }

        [Test]
        public void GetHour_ReturnsEightAtCreation()
        {
            ShopComponent shop = CreateShop(8, 18);

            int hour = shop.GetHour();

            Assert.That(hour, Is.EqualTo(8));
            Assert.That(shop.GetMinute(), Is.EqualTo(0));
        }

        [Test]
        public void FixedUpdate_AdvancesTenGameMinutesPerRealSecond()
        {
            ShopComponent shop = CreateShop(8, 18);

            shop.FixedUpdate(1.0f);

            Assert.That(shop.GetHour(), Is.EqualTo(8));
            Assert.That(shop.GetMinute(), Is.EqualTo(10));
        }

        [Test]
        public void FixedUpdate_AccumulatesSmallSteps()
        {
            ShopComponent shop = CreateShop(8, 18);

            shop.FixedUpdate(0.5f);
            shop.FixedUpdate(0.5f);

            Assert.That(shop.GetMinute(), Is.EqualTo(10));
        }

        [Test]
        public void FixedUpdate_CanAdvanceSeveralHoursInOneCall()
        {
            ShopComponent shop = CreateShop(8, 18);

            shop.FixedUpdate(30.0f);

            Assert.That(shop.GetHour(), Is.EqualTo(13));
            Assert.That(shop.GetMinute(), Is.EqualTo(0));
        }

        [Test]
        public void FixedUpdate_GoesToMidnightAfterTwentyThreeFiftyNine()
        {
            ShopComponent shop = CreateShop(8, 18);

            shop.FixedUpdate(96.0f);

            Assert.That(shop.GetHour(), Is.EqualTo(0));
            Assert.That(shop.GetMinute(), Is.EqualTo(0));
        }

        [Test]
        public void FixedUpdate_ReturnsToEightAfterTwentyFourHours()
        {
            ShopComponent shop = CreateShop(8, 18);

            shop.FixedUpdate(144.0f);

            Assert.That(shop.GetHour(), Is.EqualTo(8));
            Assert.That(shop.GetMinute(), Is.EqualTo(0));
        }

        [Test]
        public void GetIsOpen_ReturnsTrueAtOpeningHour()
        {
            ShopComponent shop = CreateShop(8, 18);

            bool is_open = shop.GetIsOpen();

            Assert.That(is_open, Is.True);
        }

        [Test]
        public void GetIsOpen_ReturnsFalseAtClosingHour()
        {
            ShopComponent shop = CreateShop(8, 18);

            shop.FixedUpdate(60.0f);

            Assert.That(shop.GetHour(), Is.EqualTo(18));
            Assert.That(shop.GetIsOpen(), Is.False);
        }

        [Test]
        public void GetIsOpen_ReturnsFalseBeforeOpeningHour()
        {
            ShopComponent shop = CreateShop(9, 17);

            bool is_open = shop.GetIsOpen();

            Assert.That(is_open, Is.False);
        }

        [Test]
        public void GetIsOpen_ReturnsTrueOnceOpeningHourIsReached()
        {
            ShopComponent shop = CreateShop(9, 17);

            shop.FixedUpdate(6.0f);

            Assert.That(shop.GetHour(), Is.EqualTo(9));
            Assert.That(shop.GetIsOpen(), Is.True);
        }
    }
}
