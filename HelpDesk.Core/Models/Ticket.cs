using System;
using System.Collections.Generic;
using System.Text;
using HelpDesk.Core.Enums;

namespace HelpDesk.Core.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public int EquipmentId { get; set; }
        public int? AssigneeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Priority Priority { get; set; }
        public TicketStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
