using activity_00_tap_26_27.Render;
using GameLibrary.Components;
using GameLibrary.States;
using System;

namespace ConsoleGame.Render
{
    public class ShopOpenRenderer : IScreenRenderer
    {
        private ConsoleRenderManager _renderManager
        public ShopOpenRenderer(ConsoleRenderManager render_manager)
        {
            _renderManager = render_manager;
        }
        public void Render(IState state)
        {
            ShopOpenState shop_open_state = state as ShopOpenState;
            string game_in_progress_message = "Shop...";

            if (shop_open_state.GetIsReplaying())
            {
                game_in_progress_message += $" - REPLAY:x{shop_open_state.GetReplaySpeed()}";
            }

            int current_line = 0;

            _renderManager.Draw(0, current_line++, game_in_progress_message, ConsoleColor.Magenta, ConsoleColor.Black);

            InventoryComponent inventory_component = shop_open_state.GetInventoryComponent();

            {
                ShopComponent shop_component = shop_open_state.GetShopComponent();
                _renderManager.Draw(0, current_line++, $"Time: {shop_component.GetHour():00}:{shop_component.GetMinute():00}", ConsoleColor.Yellow, ConsoleColor.Black);
                _renderManager.Draw(0, current_line++, $"Shop is Open. Close at {shop_component.GetShopData()._closingHour}:00", ConsoleColor.Red, ConsoleColor.Black);
                _renderManager.Draw(0, current_line++, "Esc to return", ConsoleColor.Blue, ConsoleColor.Black);
                _renderManager.Draw(0, current_line++, shop_open_state.GetCurrentSpeech(), ConsoleColor.Red, ConsoleColor.Yellow);
            }

            for (int item_index = 0; item_index < inventory_component.GetEntryCount(); item_index++)
            {
                InventoryEntry inventory_entry = inventory_component.GetEntryAtIndex(item_index);
                int quantity_at_index = inventory_entry.GetQuantity();
                bool is_item_selected = shop_open_state.GetSelectedItemIndex() == item_index;
                ConsoleColor foreground_color = ConsoleColor.Green;
                ConsoleColor background_color = ConsoleColor.Black;
                string quantity_text = quantity_at_index.ToString();

                if (quantity_at_index <= 0)
                {
                    foreground_color = ConsoleColor.DarkRed;
                    quantity_text = "out of stock";
                }

                if (is_item_selected)
                {
                    ConsoleColor temporary_foreground_color = foreground_color;

                    foreground_color = background_color;
                    background_color = temporary_foreground_color;
                }

                _renderManager.Draw(0, current_line++, $"{inventory_entry.GetItem()._name} ({inventory_entry.GetItem()._price}$) - qty: {quantity_text}", foreground_color, background_color);
            }
        }
    }
}