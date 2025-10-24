using AllContactInfoScrapper.Domain.Google;
using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllContactInfoScrapper.Domain;

namespace AllContactInfoScrapper.Infraestructure.ContactInfoEngine.Provider
{
    public class ProviderReader
    {
        public Stream SourceFile { get; set; }
        public ProviderReader(Stream sourceFile)
        {
            SourceFile = sourceFile;
        }
        public List<ContactInfoProvider> GetProviders()
        {
            List<ContactInfoProvider> Providers;
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ",", HasHeaderRecord = false };
            using (var reader = new StreamReader(SourceFile))

            using (var csv = new CsvReader(reader, config))
            {
                Providers = csv.GetRecords<ContactInfoProvider>().ToList();
            }
            return (Providers);
        }
    }
}
