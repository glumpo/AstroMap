using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroMap
{
    partial class Map
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
                public List<DataParser.Meteor> Data = new List<DataParser.Meteor>();
                public string Rad = "3000k";
                public int Width = 250;
                public int Height = 250;
                public int Opacity = 75;
            }
        }
    }
}
