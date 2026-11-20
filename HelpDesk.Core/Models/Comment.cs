using System;
using System.Collections.Generic;
using System.Text;

namespace HelpDesk.Core.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int AuthorId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
