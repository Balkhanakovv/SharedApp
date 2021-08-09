using System;
using System.Collections.Generic;

#nullable disable

namespace SharedApp
{
    public partial class DocumentsTbl
    {
        public DocumentsTbl()
        {
            TransactionsTbls = new HashSet<TransactionsTbl>();
        }

        public int DocumentId { get; set; }
        public string UserId { get; set; }
        public int? FileId { get; set; }
        public int? RightId { get; set; }

        public virtual FilesTbl File { get; set; }
        public virtual AccessRightsTbl Right { get; set; }
        public virtual UsersTbl User { get; set; }
        public virtual ICollection<TransactionsTbl> TransactionsTbls { get; set; }
    }
}
