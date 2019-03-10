using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroMap
{
    class DataProcessor<T> where T : MeteorBase
    {
        private IDataParser<T> Parser;
        private List<MeteorBase> Meteors;
        private Type MeteorsType;

        private DataProcessor()
        {
            Parser = null;
            Meteors = null;
            MeteorsType = typeof(T);
        }

        public DataProcessor(IDataParser<T> _parser)
        {
            Parser = _parser;
            Meteors = null;
            MeteorsType = typeof(T);

            Init();
        }

        private void Init()
        {
            var parsedMeteors = Parser.ParseCSV();
            Meteors = new List<MeteorBase>(parsedMeteors.Count);
            foreach (var it in parsedMeteors)
            {
                Meteors.Add(it);
            }
        }

        public List<MeteorBase> GetMeteors()
        {
            Contract.Requires(Meteors != null);
            return Meteors;
        }

        public Type GetMeteorsType()
        {
            return MeteorsType;
        }

        public static implicit operator DataProcessor<T>(DataProcessor<NasaCsv.Meteor> v)
        {
            var res = new DataProcessor<T>();
            res.MeteorsType = v.MeteorsType;
            res.Meteors = new List<MeteorBase>(v.Meteors);
            return res;
        }
    }
}
