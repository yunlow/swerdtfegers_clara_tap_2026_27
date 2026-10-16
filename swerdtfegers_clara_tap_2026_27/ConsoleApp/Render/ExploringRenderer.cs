using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Render;
using GameLibrary;
using GameLibrary.Components;
using GameLibrary.States;
using System;

namespace ConsoleGame.Render
{
    public class ExploringRenderer
    {
        public void Render(IState state, ConsoleRenderManager render_manager)
        {
            ExploringState exploring_state = state as ExploringState;

            string game_in_progress_message = "Game in progress...";

            if (exploring_state.GetIsReplaying())
            {
                game_in_progress_message += $" - REPLAY:x{exploring_state.GetReplaySpeed()}";
            }

            int current_line = 0;

            render_manager.Draw(0, current_line++, game_in_progress_message, ConsoleColor.Magenta, ConsoleColor.Black);

            LocationComponent current_location = exploring_state.GetCurrentLocation();

            GameObject hero = exploring_state.GetHeroObject();
            HealthComponent health_component = hero.GetComponent<HealthComponent>();
            WalletComponent wallet_component = hero.GetComponent<WalletComponent>();

            render_manager.Draw(0, current_line++, $"{hero.GetName()} - {health_component.GetHealth()} HP - Money: {wallet_component.GetMoney()}", ConsoleColor.Blue, ConsoleColor.DarkBlue);

            InventoryComponent inventory_component = hero.GetComponent<InventoryComponent>();

            for (int item_index = 0; item_index < inventory_component.GetEntryCount(); item_index++)
            {
                render_manager.Draw(0, current_line++, $"{inventory_component.GetEntryAtIndex(item_index).GetItem()._name}", ConsoleColor.Blue, ConsoleColor.DarkBlue);
            }

            render_manager.Draw(0, current_line++, $"Exploring {current_location.GetName()}", ConsoleColor.Cyan, ConsoleColor.Black);

            for (int location_index = 0; location_index < current_location.GetDestinationCount(); location_index++)
            {
                Connection current_connection = current_location.GetDestinationAtIndex(location_index);

                if (location_index == exploring_state.GetSelectedDestinationIndex())
                {
                    render_manager.Draw(0, current_line++, $"{location_index + 1}. {current_connection.GetDestinationLocation().GetName()}: Duration: {current_connection.GetTimeToReachDestination()}", ConsoleColor.Black, ConsoleColor.Green);
                }
                else
                {
                    render_manager.Draw(0, current_line++, $"{location_index + 1}. {current_connection.GetDestinationLocation().GetName()}: Duration: {current_connection.GetTimeToReachDestination()}", ConsoleColor.Green, ConsoleColor.Black);
                }
            }
        }
    }
}