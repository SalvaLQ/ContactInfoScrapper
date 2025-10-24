using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllContactInfoScrapper.Domain
{
    public class ContactInfoProvider
    {
        public string Name { get; set; }
        public string SearchUrl { get; set; }
        public bool Active { get; set; }
    }
}
