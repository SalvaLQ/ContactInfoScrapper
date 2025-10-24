using AllContactInfoScrapper.Domain.Google;
using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllContactInfoScrapper.Infraestructure.GoogleScrapper.Globalization
{
    public class LanguageReader
    {
        public Stream SourceFile { get; set; }
        public LanguageReader(Stream sourceFile)
        {
            SourceFile = sourceFile;
        }
        public List<Language> GetSupportedLanguages()
        {
            List<Language> Languages;
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ",", HasHeaderRecord = false };
            using (var reader = new StreamReader(SourceFile))

            using (var csv = new CsvReader(reader, config))
            {
                Languages = csv.GetRecords<Language>().ToList();
            }
            return (Languages);
        }
    }
}
