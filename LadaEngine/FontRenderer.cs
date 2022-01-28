using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadaEngine
{
    public class FontRenderer
    {
        Tilemap tm;

        int[] chars = new int[2500];
        
        public void Load(Texture font_sheet, int string_length, int line_count)
        {
            //tm = new Tilemap(font_sheet, chars, new int[] { 0}, line_count, string_length, 16, 16);
        }
    }
}
