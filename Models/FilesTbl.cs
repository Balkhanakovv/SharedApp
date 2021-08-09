using System;
using System.Collections.Generic;

#nullable disable

namespace SharedApp
{
    public partial class FilesTbl
    {
        public FilesTbl()
        {
            DocumentsTbls = new HashSet<DocumentsTbl>();
        }

        public int FileId { get; set; }
        public string FileNam { get; set; }
        public byte[] FileBin { get; set; }
        public DateTime CreateDate { get; set; }
        public int? TypeId { get; set; }
        public int FileSize { get; set; }

        public virtual FileTypeTbl Type { get; set; }
        public virtual ICollection<DocumentsTbl> DocumentsTbls { get; set; }
    }
}
