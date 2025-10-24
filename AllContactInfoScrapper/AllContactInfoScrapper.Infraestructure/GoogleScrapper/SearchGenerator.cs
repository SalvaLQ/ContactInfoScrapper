using AllContactInfoScrapper.Domain;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllContactInfoScrapper.Infraestructure.GoogleScrapper
{
    public class SearchGenerator
    {
        public string Keywords { get; set; }
        public string Emails { get; set; }
        public ContactInfoProvider Provider { get; set; }
        public bool AnyWord { get; set; }
        public SearchGenerator(string Keywords, string Emails, bool AnyWord, ContactInfoProvider Provider)
        {
            this.Keywords = Keywords;
            this.Emails = Emails;
            this.AnyWord = AnyWord;
            if (Provider != null)
            {
                this.Provider = Provider;
            }

        }
        //gerente +34 OR @hotmail.es OR @gmail.com site:linkedin.com
        //dance ceo (@gmail.com OR hotmail.com OR yahoo.com OR @outlook.com) AND  site:tiktok.com
        //+44 whatsapp gmail.com intext:pinterest
        public string GetSearchTerm()
        {
            string SearchTerm = string.Empty;
            if (AnyWord)
            {
                if (!string.IsNullOrEmpty(Keywords))
                    SearchTerm += Keywords.Replace(",", " OR ") + " OR ";

                if (!string.IsNullOrEmpty(Emails))
                    SearchTerm += Emails.Replace(",", " OR ") + " OR ";

                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    if (SearchTerm.EndsWith(" OR "))
                        SearchTerm = SearchTerm.TrimEnd(' ', 'O', 'R', ' ');

                    SearchTerm = "( " + SearchTerm + " ) ";
                }
                
            }
            else
            {
                if (!string.IsNullOrEmpty(Keywords))
                    SearchTerm +="(" + Keywords.Replace(",", " | ") + ") ";

                if (!string.IsNullOrEmpty(Emails))
                    SearchTerm += " " + Emails.Replace(",", " | ");
                
            }
            //SearchTerm += " " + Provider.SearchUrl.Split('.')[0] + " OR site:" + Provider.SearchUrl + "";
            SearchTerm += " site:" + Provider.SearchUrl + "";

            return (SearchTerm);
        }

    }
}
