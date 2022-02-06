using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadaEngine
{
    public class TextRenderer
    {
        public Tilemap tm;
        int[] map;
        string alphabet;
        public TextRenderer(Texture symbol_sheet, string alphabet, int col_number, int row_number, int symbol_sheet_length, int symbol_sheet_width)
        {
            this.alphabet = alphabet;
            map = new int[col_number * row_number];
            for (int i = 0; i < col_number * row_number; i++)
                map[i] = -1;
            tm = new Tilemap(symbol_sheet, map, row_number, col_number, symbol_sheet_length, symbol_sheet_width);
        }

        public void SetText(string text)
        {
            for (int i = 0; i < tm.tm_width * tm.tm_height; i++)
                map[i] = alphabet.IndexOf(" ");
            for (int i = 0; i < Math.Min(tm.tm_width * tm.tm_height, text.Length); i++)
                map[i] = alphabet.IndexOf(text[i]);

            tm.map = map;
            tm.UpdateMap();
        }
        public void Render()
        {
            tm.Render();
        }
    }
}
