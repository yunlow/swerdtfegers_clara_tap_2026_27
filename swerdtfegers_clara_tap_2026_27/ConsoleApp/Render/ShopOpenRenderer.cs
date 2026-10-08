using GameLibrary.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace activity_00_tap_26_27.Render
{
    public class ShopOpenRenderer : IScreenRenderer
    {
        public void Render(IState state, ConsoleRenderManager render_manager)
        {
            ShopOpenState shop_open_state = state as ShopOpenState;
            ShopComponent shop_component = shop_open_state.GetShopComponent();

            render_manager.Draw(0, 0, $"Time: {shop_component.GetHour():00}:{shop_component.GetMinute():00}", ConsoleColor.Yellow, ConsoleColor.Black);
            render_manager.Draw(0, 1, $"Shop is Open. Close at {shop_component.GetShopData()._closingHour}:00", ConsoleColor.Red, ConsoleColor.Black);
            render_manager.Draw(0, 2, "Esc to return", ConsoleColor.Blue, ConsoleColor.Black);

            render_manager.Draw(0, 4, shop_open_state.GetCurrentSpeech(), ConsoleColor.Red, ConsoleColor.Yellow);

            int stock_display_first_line_index = 6;

            for (int item_index = 0; item_index < shop_component.GetItemCount(); item_index++)
            {
                int quantity_at_index = shop_component.GetQuantityAtIndex(item_index);
                bool is_item_selected = shop_open_state.GetSelectedItemIndex() == item_index;

                if (quantity_at_index > 0)
                {
                    if (is_item_selected)
                    {
                        render_manager.Draw(0, stock_display_first_line_index + item_index, $"{shop_component.GetItemAtIndex(item_index)._name} ({shop_component.GetItemAtIndex(item_index)._price}$) - qty: {quantity_at_index}", ConsoleColor.Black, ConsoleColor.Green);
                    }
                    else
                    {
                        render_manager.Draw(0, stock_display_first_line_index + item_index, $"{shop_component.GetItemAtIndex(item_index)._name} ({shop_component.GetItemAtIndex(item_index)._price}$) - qty: {quantity_at_index}", ConsoleColor.Green, ConsoleColor.Black);
                    }
                }
                else
                {
                    if (is_item_selected)
                    {
                        render_manager.Draw(0, stock_display_first_line_index + item_index, $"{shop_component.GetItemAtIndex(item_index)._name} ({shop_component.GetItemAtIndex(item_index)._price}$) - qty: out of stock", ConsoleColor.Black, ConsoleColor.DarkRed);
                    }
                    else
                    {
                        render_manager.Draw(0, stock_display_first_line_index + item_index, $"{shop_component.GetItemAtIndex(item_index)._name} ({shop_component.GetItemAtIndex(item_index)._price}$) - qty: out of stock", ConsoleColor.DarkRed, ConsoleColor.Black);
                    }
                }
            }
        }
    }
}
