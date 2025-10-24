using AllContactInfoScrapper.Domain.Google;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllContactInfoScrapper.Infraestructure.GoogleScrapper.Globalization
{
    public class CountryReader
    {
        public Stream SourceFile { get; set; }
        public CountryReader(Stream sourceFile)
        {
            SourceFile = sourceFile;
        }
        public List<Country> GetSupportedCountries()
        {
            List<Country> Countries;
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ",", HasHeaderRecord = false };
            using (var reader = new StreamReader(SourceFile))

            using (var csv = new CsvReader(reader, config))
            {
                Countries = csv.GetRecords<Country>().ToList();
            }
            return (Countries);
        }
    }
}
