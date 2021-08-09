using System;
using System.Collections.Generic;

#nullable disable

namespace SharedApp
{
    public partial class TransactionsTbl
    {
        public int TransactionId { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public int? DocumentId { get; set; }
        public DateTime? TransactionTime { get; set; }

        public virtual DocumentsTbl Document { get; set; }
        public virtual UsersTbl ReceiverNavigation { get; set; }
        public virtual UsersTbl SenderNavigation { get; set; }
    }
}
