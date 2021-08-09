using System;
using System.Collections.Generic;

#nullable disable

namespace SharedApp
{
    public partial class TrafficPlanTbl
    {
        public TrafficPlanTbl()
        {
            UsersTbls = new HashSet<UsersTbl>();
        }

        public int PlanId { get; set; }
        public int TrafficPlan { get; set; }

        public virtual ICollection<UsersTbl> UsersTbls { get; set; }
    }
}
