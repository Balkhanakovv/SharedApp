using System;
using System.Collections.Generic;

#nullable disable

namespace SharedApp
{
    public partial class CountriesTbl
    {
        public CountriesTbl()
        {
            UsersTbls = new HashSet<UsersTbl>();
        }

        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public virtual ICollection<UsersTbl> UsersTbls { get; set; }
    }
}
