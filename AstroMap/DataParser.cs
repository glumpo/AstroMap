using System.Collections.Generic;

namespace AstroMap
{
    abstract class MeteorBase
    {
        public abstract double Latitude { get; set; }
        public abstract double Longitude { get; set; }
        public abstract string PositionRaw { get; set; }

        public abstract string Description();
    }

    interface IDataParser<T> where T : MeteorBase
    {
        List<T> ParseCSV();
    }
}
