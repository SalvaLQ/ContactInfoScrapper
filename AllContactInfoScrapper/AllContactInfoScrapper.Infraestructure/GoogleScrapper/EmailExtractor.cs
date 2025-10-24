using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AllContactInfoScrapper.Infraestructure.GoogleScrapper
{
    public class EmailExtractor
    {
        public EmailExtractor()
        {

        }
        public string GetEmails(string text)
        {
            string Emails = string.Empty;
            List<string> Patterns = new List<string>() {
               @"[A-Za-z0-9_\-\+]+@[A-Za-z0-9\-]+\.([A-Za-z]{2,3})(?:\.[a-z]{2})?" };
            foreach (var pat in Patterns)
            {
                var Matches = Regex.Matches(text, pat);
                foreach (Match m in Matches)
                    Emails += m.Value + " ";
            }
            return Emails;

        }
    }
}
