using System.Collections.Generic;

#nullable disable

namespace SharedApp
{
    public partial class AccessRightsTbl
    {
        public AccessRightsTbl()
        {
            DocumentsTbls = new HashSet<DocumentsTbl>();
        }

        public int RightId { get; set; }
        public string AccessRight { get; set; }

        public virtual ICollection<DocumentsTbl> DocumentsTbls { get; set; }
    }
}
