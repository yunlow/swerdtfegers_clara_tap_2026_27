using Spectre.Console;
using Spectre.Console.Rendering;

namespace SpectreGame.Render
{
    public class SpectreRenderableRenderManager
    {
        private const string ERASE_TO_END_OF_LINE = "\u001b[K";

        private readonly List<IRenderable> _renderableTable = new List<IRenderable>();
        private readonly StringWriter _offscreenWriter = new StringWriter();
        private readonly IAnsiConsole _offscreenConsole;

        private List<string> _previousLineTable = new List<string>();
        private int _previousWidth = -1;
        private int _previousHeight = -1;

        public SpectreRenderableRenderManager()
        {
            AnsiConsoleSettings settings = new AnsiConsoleSettings();

            settings.Ansi = AnsiSupport.Yes;
            settings.ColorSystem = ColorSystemSupport.TrueColor;
            settings.Interactive = InteractionSupport.No;
            settings.Out = new AnsiConsoleOutput(_offscreenWriter);

            _offscreenConsole = AnsiConsole.Create(settings);

            AnsiConsole.Clear();
            AnsiConsole.Cursor.Hide();
        }

        public void Add(IRenderable renderable)
        {
            _renderableTable.Add(renderable);
        }

        public void Render()
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            if ((width != _previousWidth) || (height != _previousHeight))
            {
                AnsiConsole.Clear();
                _previousLineTable.Clear();
                _previousWidth = width;
                _previousHeight = height;
            }

            List<string> line_table = RenderOffscreen(width);
            int visible_line_count = Math.Min(line_table.Count, height - 1);

            for (int line_index = 0; line_index < visible_line_count; line_index++)
            {
                bool is_new_line = line_index >= _previousLineTable.Count;

                if (is_new_line || (line_table[line_index] != _previousLineTable[line_index]))
                {
                    Console.SetCursorPosition(0, line_index);
                    Console.Out.Write(line_table[line_index] + ERASE_TO_END_OF_LINE);
                }
            }

            int previous_visible_line_count = Math.Min(_previousLineTable.Count, height - 1);

            for (int line_index = visible_line_count; line_index < previous_visible_line_count; line_index++)
            {
                Console.SetCursorPosition(0, line_index);
                Console.Out.Write(ERASE_TO_END_OF_LINE);
            }

            _previousLineTable = line_table;
            _renderableTable.Clear();
        }

        private List<string> RenderOffscreen(int width)
        {
            _offscreenWriter.GetStringBuilder().Clear();
            _offscreenConsole.Profile.Width = width;

            for (int renderable_index = 0; renderable_index < _renderableTable.Count; renderable_index++)
            {
                _offscreenConsole.Write(_renderableTable[renderable_index]);
            }

            string[] raw_line_table = _offscreenWriter.ToString().Split('\n');
            List<string> line_table = new List<string>();

            for (int line_index = 0; line_index < raw_line_table.Length; line_index++)
            {
                line_table.Add(raw_line_table[line_index].TrimEnd('\r'));
            }

            if ((line_table.Count > 0) && (line_table[line_table.Count - 1].Length == 0))
            {
                line_table.RemoveAt(line_table.Count - 1);
            }

            return line_table;
        }
    }
}