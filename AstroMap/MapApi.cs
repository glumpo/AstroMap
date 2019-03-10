using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AstroMap
{
    partial class MapApi
    {
        private static partial class HereApi
        {
            /// <summary>
            /// Client for HERE REST API
            /// </summary>
            private static readonly HttpClient client;

            static HereApi()
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("User-Agent", "Map");
                client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                client.DefaultRequestHeaders.Add("accept", "*/*");
                client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate");
            }

            /// <summary>
            /// Create URL for HERE REST API
            /// </summary>
            /// <param name="args"></param>
            /// <param name="type"></param>
            /// <returns>Returns null on bad arguments or ready to use URL</returns>
            private static string CreateUrl(MapVieArgs args)
            {
                    int balancedServer = 1 + ((args.Row + args.Column) % 4);
                    string baseUrl = $"https://{balancedServer}.base.maps.api.here.com";
                    string apiVersion = "maptile/2.1";
                    string resource = "maptile";
                    string mapVersion = "newest";
                    string viewScheme = "normal.day";

                    string zoom = args.Zoom.ToString();
                    string column = args.Column.ToString();
                    string row = args.Row.ToString();
                    string size = args.Size.ToString();

                    string format = "png8";

                    string credentialParameters = $"?app_id={Credentials.AppId}&app_code={Credentials.AppCode}";

                    string res =
                        $"{baseUrl}/{apiVersion}/{resource}/{mapVersion}/{viewScheme}/" +
                        $"{zoom}/{column}/{row}/{size}/{format}" +
                        credentialParameters;

                    return res;
            }

            private static string CreateUrl(HeatMapArgs args)
            {
                string baseUrl = "https://image.maps.api.here.com/mia/1.6/heat?";
                string credentialParameters = $"app_id={Credentials.AppId}&app_code={Credentials.AppCode}";
                string otherParams = $"&z={args.Zoom}&h={args.Height}&w={args.Width}&op={args.Opacity}";

                string dataPoints = "";
                for (int i = 0; i < System.Math.Min(args.Data.Count, args.MaxDataCount); ++i)
                {
                    dataPoints += $"&a{i}={args.Data[i].PositionRaw}&rad{i}={args.Rad}&l{i}=1";
                }

                return baseUrl + credentialParameters + otherParams + dataPoints;
            }

            public async static Task<Bitmap> GetSquareAsync(int row, int column, int zoom)
            {
                Bitmap bitmapImage;

                try
                {
                    var requesParams = new MapVieArgs()
                    {
                        Zoom = zoom,
                        Column = column,
                        Row = row
                    };
                    string target = CreateUrl(requesParams);

                    using (var response = await client.GetAsync(target))
                    {
                        response.EnsureSuccessStatusCode();

                        using (Stream stream = await response.Content.ReadAsStreamAsync())
                        {
                            bitmapImage = new Bitmap(stream);
                        }
                    }

                    return bitmapImage;
                }
                catch
                {
                    throw;
                }
            }

            public async static Task<Bitmap> GetHeatMap(List<MeteorBase> data)
            {
                Bitmap bitmapImage;
                try
                {
                    var requesParams = new HeatMapArgs()
                    {
                        Data = data,
                        Zoom = 1,
                    };
                    string target = CreateUrl(requesParams);

                    using (var response = await client.GetAsync(target))
                    {
                        response.EnsureSuccessStatusCode();

                        using (Stream stream = await response.Content.ReadAsStreamAsync())
                        {
                            bitmapImage = new Bitmap(stream);
                        }
                    }

                    return bitmapImage;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Asynchronously get world map from REST API
        /// </summary>
        /// <remarks>Creates a Bitmap from fragments, gotten from REST API</remarks>
        /// <returns>Bitmap with world map</returns>
        public async Task<Bitmap> GetWorldMap()
        {
            // MapApi  is Square, so minRow == min Column and maxRow == max Column
            // 1 << zoom == Pow(2, zoom), google left shift to multiple 2. Fuck the Math.Pow
            const int zoom = 1;
            const int minRowAndColumn = 0;
            const int maxRowAndColumnPlusOne = (1 << zoom);

            var fragmentsOfMap = new List<List<Bitmap>>(maxRowAndColumnPlusOne);
            for (int i = minRowAndColumn; i < maxRowAndColumnPlusOne; ++i)
            {
                fragmentsOfMap.Add(new List<Bitmap>(maxRowAndColumnPlusOne));
                for (int j = minRowAndColumn; j < maxRowAndColumnPlusOne; ++j)
                {
                    fragmentsOfMap[i].Add(null );
                }
            }

            // TODO: Paralel load of fragments
            for (int i = minRowAndColumn; i < maxRowAndColumnPlusOne; ++i)
            {
                for (int j = minRowAndColumn; j < maxRowAndColumnPlusOne; ++j)
                {
                    fragmentsOfMap[i][j] = await HereApi.GetSquareAsync(i, j, zoom);
                }
            }

            // Fragments have equal size
            int fragmentWidth = fragmentsOfMap[0][0].Width;
            int fragmentHeight = fragmentsOfMap[0][0].Height;
            int resWidth  = fragmentWidth  * (maxRowAndColumnPlusOne - minRowAndColumn);
            int resHeight = fragmentHeight * (maxRowAndColumnPlusOne - minRowAndColumn);

            // TODO: Exception handler
            Bitmap res = new Bitmap(resWidth, resHeight);
            using (Graphics g = Graphics.FromImage(res))
            {
                g.Clear(Color.Gray);

                int rowOffset = 0;
                int columnOffset = 0;
                foreach (var column in fragmentsOfMap)
                {
                    foreach(var fragment in column)
                    {
                        g.DrawImage(fragment, new Rectangle(rowOffset, columnOffset, fragmentWidth, fragmentHeight));
                        rowOffset += fragmentWidth;
                    }
                    columnOffset += fragmentHeight;
                    rowOffset = 0;
                }
            }

                return res;
        }

        public async Task<Bitmap> GetHeatMap(List<MeteorBase> data)
        {
            Bitmap res = await HereApi.GetHeatMap(data);
            return res;
        }

    }
}
