using System;

namespace activity_00_tap_26_27.Presentation
{
    public class ConsoleRenderManager
    {
        private struct Pixel
        {
            public char Character;
            public ConsoleColor Color;
        }

        private Pixel[,] _currentBuffer;
        private Pixel[,] _previousBuffer;
        private int _width;
        private int _height;

        public ConsoleRenderManager()
        {
            _width = Console.WindowWidth;
            _height = Console.WindowHeight;
            _currentBuffer = new Pixel[_width, _height];
            _previousBuffer = new Pixel[_width, _height];
            
            Console.CursorVisible = false;
            ClearBuffer(_currentBuffer);
            ClearBuffer(_previousBuffer);
        }

        private void ClearBuffer(Pixel[,] buffer)
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    buffer[x, y] = new Pixel { Character = ' ', Color = ConsoleColor.Gray };
                }
            }
        }

        public void Draw(int x, int y, string text, ConsoleColor color)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
            {
                return;
            }

            for (int character_index = 0; character_index < text.Length; character_index++)
            {
                if (x + character_index < _width)
                {
                    _currentBuffer[x + character_index, y] = new Pixel { Character = text[character_index], Color = color };
                }
            }
        }

        public void Render()
        {
            // The actual drawing to the console happens here by comparing buffers
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    Pixel current = _currentBuffer[x, y];
                    Pixel previous = _previousBuffer[x, y];

                    if (current.Character != previous.Character || current.Color != previous.Color)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = current.Color;
                        Console.Write(current.Character);
                        _previousBuffer[x, y] = current;
                    }
                }
            }

            // Reset current buffer for next frame
            ClearBuffer(_currentBuffer);
            
            Console.ResetColor();
        }
    }
}