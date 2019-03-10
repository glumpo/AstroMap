using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroMap
{
    partial class MapApi
    {
        partial class HereApi
        {
            /// <summary>
            /// Class for transfering arguments to CreateUrl method
            /// </summary>
            private class UrlArgs
            {
                public int Zoom = 1;
            }

            private class MapVieArgs : UrlArgs
            {
                public int Row = 1;
                public int Column = 1;
                public int Size = 256;
            }

            private class HeatMapArgs : UrlArgs
            {
                public List<MeteorBase> Data = new List<MeteorBase>();
                public string Rad = "1000k";
                public int Width = 250;
                public int Height = 250;
                public int Opacity = 75;
                public int MaxDataCount = 150;
            }
        }
    }
}
