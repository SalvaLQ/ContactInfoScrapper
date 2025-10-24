using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AllContactInfoScrapper.Infraestructure.GoogleScrapper
{
    public class PhoneExtractor
    {
        public PhoneExtractor()
        {

        }
        public string GetPhones(string text)
        {
            string Phones = "";
            List<string> Patterns = new List<string>() {
                @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}",
                @"\+?[1-9][0-9]{7,14}",
                @"((\+|\+\s|\d{1}\s?|\()(\d\)?\s?[-\.\s\(]??){8,}\d{1}|\d{3}[-\.\s]??\d{3}[-\.\s]??\d{4}|\(\d{3}\)\s*\d{3}[-\.\s]??\d{4}|\d{3}[-\.\s]??\d{4})" };

            foreach (var pat in Patterns)
            {
                var Matches = System.Text.RegularExpressions.Regex.Matches(text,pat);
                foreach (Match m in Matches)
                    Phones += m.Value + " ";
            }                       
            return (Phones);

        }
    }
}
