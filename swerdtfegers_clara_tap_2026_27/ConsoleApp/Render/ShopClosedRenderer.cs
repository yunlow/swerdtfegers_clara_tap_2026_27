using GameLibrary.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace activity_00_tap_26_27.Render
{
    public class ShopClosedRenderer : IScreenRenderer
    {
        public void Render(IState state, ConsoleRenderManager render_manager)
        {
            ShopClosedState shop_closed_state = state as ShopClosedState;
            ShopComponent shop_component = shop_closed_state.GetShopComponent();

            render_manager.Draw(0, 0, $"Time: {shop_component.GetHour():00}:{shop_component.GetMinute():00}", ConsoleColor.Yellow, ConsoleColor.Black);
            render_manager.Draw(0, 1, $"Shop is closed. Open at {shop_component.GetShopData()._openingHour}:00", ConsoleColor.Red, ConsoleColor.Black);
            render_manager.Draw(0, 2, "Esc to return", ConsoleColor.Blue, ConsoleColor.Black);

        }
    }
}
