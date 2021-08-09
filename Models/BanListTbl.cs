using System;
using System.Collections.Generic;

#nullable disable

namespace SharedApp
{
    public partial class BanListTbl
    {
        public string UserId { get; set; }
        public int? BanId { get; set; }

        public virtual BansTbl Ban { get; set; }
        public virtual UsersTbl User { get; set; }
    }
}
