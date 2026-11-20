using System;
using System.Collections.Generic;

namespace HelpDesk.Data.EfModels;

public partial class TicketStatusHistory
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public string? FromStatus { get; set; }

    public string ToStatus { get; set; } = null!;

    public DateTime ChangedAt { get; set; }

    public int ChangedById { get; set; }

    public virtual Employee ChangedBy { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
