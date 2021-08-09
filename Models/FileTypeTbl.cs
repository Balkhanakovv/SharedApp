using System;
using System.Collections.Generic;

#nullable disable

namespace SharedApp
{
    public partial class FileTypeTbl
    {
        public FileTypeTbl()
        {
            FilesTbls = new HashSet<FilesTbl>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; }

        public virtual ICollection<FilesTbl> FilesTbls { get; set; }
    }
}
