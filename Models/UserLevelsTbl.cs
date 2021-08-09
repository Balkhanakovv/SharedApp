using System;
using System.Collections.Generic;

#nullable disable

namespace SharedApp
{
    public partial class UserLevelsTbl
    {
        public UserLevelsTbl()
        {
            UsersTbls = new HashSet<UsersTbl>();
        }

        public int LevelId { get; set; }
        public string LevelName { get; set; }

        public virtual ICollection<UsersTbl> UsersTbls { get; set; }
    }
}
