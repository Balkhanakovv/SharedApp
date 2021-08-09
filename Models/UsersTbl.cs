using System;
using System.Collections.Generic;

#nullable disable

namespace SharedApp
{
    public partial class UsersTbl
    {
        public UsersTbl()
        {
            DocumentsTbls = new HashSet<DocumentsTbl>();
            TransactionsTblReceiverNavigations = new HashSet<TransactionsTbl>();
            TransactionsTblSenderNavigations = new HashSet<TransactionsTbl>();
        }

        public string UserId { get; set; }
        public string Passwd { get; set; }
        public int? CountryId { get; set; }
        public int? PlanId { get; set; }
        public double MemorySize { get; set; }
        public int? LevelId { get; set; }

        public virtual CountriesTbl Country { get; set; }
        public virtual UserLevelsTbl Level { get; set; }
        public virtual TrafficPlanTbl Plan { get; set; }
        public virtual ICollection<DocumentsTbl> DocumentsTbls { get; set; }
        public virtual ICollection<TransactionsTbl> TransactionsTblReceiverNavigations { get; set; }
        public virtual ICollection<TransactionsTbl> TransactionsTblSenderNavigations { get; set; }
    }
}
