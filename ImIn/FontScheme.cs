using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImIn
{
    class FontScheme
    {
        private readonly static string font_family = "Times New Roman";


        public Font main_font;
        public Font sub_font;
        public Font header_font;
        public Font menu_font;


        public FontScheme(int window_size)
        {
            float multiplier = window_size / (float)1920;

            main_font = new Font(font_family, 20.0f * multiplier, FontStyle.Bold);
            sub_font = new Font(font_family, 14.0f * multiplier);
            header_font = new Font(font_family, 30.0f * multiplier, FontStyle.Bold);
            menu_font = new Font(font_family, 22.0f * multiplier, FontStyle.Bold);
        }
    }
}
