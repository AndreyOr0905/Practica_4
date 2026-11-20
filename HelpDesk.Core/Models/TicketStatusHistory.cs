using System;
using System.Collections.Generic;
using System.Text;
using HelpDesk.Core.Enums;

namespace HelpDesk.Core.Models
{
    public class TicketStatusHistory
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public TicketStatus? FromStatus { get; set; }
        public TicketStatus ToStatus { get; set; }
        public DateTimeOffset ChangedAt { get; set; }
        public int ChangedById { get; set; }
    }
}
