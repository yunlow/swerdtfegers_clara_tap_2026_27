using ConsoleGame.Render;
using GameLibrary.States;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace SpectreGame.Render
{
    public class MainMenuRenderer : IScreenRenderer
    {
        private readonly SpectreRenderableRenderManager _spectreRenderableRenderManager;

        public MainMenuRenderer(SpectreRenderableRenderManager spectre_differential_renderer_manager)
        {
            _spectreRenderableRenderManager = spectre_differential_renderer_manager;
        }

        public void Render(IState state)
        {
            MainMenuState main_menu_state = state as MainMenuState;

            List<Text> menu_entries = new List<Text>();

            for (int menu_entry_index = 0; menu_entry_index < main_menu_state.GetMenuItemTable().Count; menu_entry_index++)
            {
                Style style;

                if (main_menu_state.GetSelectedItemIndex() != menu_entry_index)
                {
                    style = new Style(Color.Green, Color.Black);
                }
                else
                {
                    style = new Style(Color.Black, Color.Green1);
                }

                Text row_text = new Text($"{main_menu_state.GetMenuItemTable()[menu_entry_index].GetText()}", style);

                menu_entries.Add(row_text);
            }

            Rows rows = new Rows(menu_entries);
            Panel panel = new Panel(rows);
            panel.Header("Main menu");
            _spectreRenderableRenderManager.Add(panel);

            _spectreRenderableRenderManager.Render();
        }
    }
}