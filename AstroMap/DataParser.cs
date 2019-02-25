using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AstroMap
{
    static class DataParser
    {
        public class Meteor
        {
            // name,id,nametype,recclass,mass (g),fall,year,reclat,reclong,GeoLocation
            [Name("name")]
            [Index(0)]
            public string Name { get; set; }

            [Name("id")]
            [Index(1)]
            public int Id { get; set; }

            [Name("nametype")]
            [Index(2)]
            public string NameType { get; set; }

            [Name("recclass")]
            [Index(3)]
            public string RecClass { get; set; }

            [Name("mass (g)")]
            [Index(4)]
            public double Mass { get; set; }

            [Name("fall")]
            [Index(5)]
            public string FallStatus { get; set; }

            [Name("year")]
            [Index(6)]
            [Ignore]
            public DateTime FallingTime { get; set; }

            [Name("reclat")]
            [Index(7)]
            public double Latitude { get; set; }

            [Name("reclong")]
            [Index(8)]
            public double Longitude { get; set; }

            [Name("GeoLocation")]
            [Index(9)]
            public string PositionRaw { get; set; }
        };

        private class DoubleConverter : ITypeConverter
        {
            public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return value.ToString();
            }

            public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                double result;

                if (!double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                    !double.TryParse(text, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                    !double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                {
                    result = 0;
                }

                return result;
            }
        }

        private class RawPositionConverter : ITypeConverter
        {
            public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return value.ToString();
            }

            public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                if (null == text)
                {
                    return "";
                }

                if (text.Length <= 2)
                {
                    return text;
                }

                // "(double, double)" to "double,double"
                text = text.Substring(1, text.Length - 2);
                text = text.Replace(" ", "");

                return text;
            }
        }

        private class MeteorCsvMap : ClassMap<Meteor>
        {
            public MeteorCsvMap()
            {
                AutoMap();

                Map(m => m.Mass).TypeConverter(new DoubleConverter());
                Map(m => m.Latitude).TypeConverter(new DoubleConverter());
                Map(m => m.Longitude).TypeConverter(new DoubleConverter());

                Map(m => m.PositionRaw).TypeConverter(new RawPositionConverter());
            }
        }

        /// <summary>
        /// Returns List of Meteor oblects, parsed from CSV file
        /// </summary>
        /// <param name="path">Path to CSV file</param>
        /// <returns></returns>
        public static List<Meteor> ParseCSV(string path)
        {
            List<Meteor> res = null;
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader))
            {
                //csv.Configuration.TypeConverterCache.RemoveConverter<double>();
                //csv.Configuration.TypeConverterCache.AddConverter<double>(new DoubleConverter());
                csv.Configuration.RegisterClassMap<MeteorCsvMap>();
                csv.Configuration.Delimiter = ",";

                var records = csv.GetRecords<Meteor>();
                res = records.ToList(); // NOTE: records is IEnumerable, here we load Full CSV file to memory
            }

            return res;
        }
    }
}
