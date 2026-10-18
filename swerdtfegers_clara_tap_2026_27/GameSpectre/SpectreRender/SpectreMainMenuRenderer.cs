using activity_00_tap_26_27.Core;
using GameLibrary;
using GameLibrary.States;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace activity_00_tap_26_27.Render
{
    public class SpectreMainMenuRenderer : IScreenRenderer
    {
       
        public void Render(IState state)
        {
            IState menu_state = state;
            if (menu_state != null)
            {
                
                render_manager.Draw(0, 0, "Main Menu", ConsoleColor.White, ConsoleColor.Black);
                render_manager.Draw(0, 1, "1. Start Game", ConsoleColor.White, ConsoleColor.Black);
                render_manager.Draw(0, 2, "2. Options", ConsoleColor.White, ConsoleColor.Black);
                render_manager.Draw(0, 3, "3. Exit", ConsoleColor.White, ConsoleColor.Black);
                render_manager.Render();
            }
            Panel panel = new Panel("Main Menu");
            {
                panel.Add(new Text("1. Start Game"));
                panel.Add(new Text("2. Options"));
                panel.Add(new Text("3. Exit"));
                AnsiConsole.Render(panel);
            }
        }
    }
}
