using System;
using System.Collections.Generic;
using System.Text;
using HelpDesk.Core.Enums;

namespace HelpDesk.Core.Services
{
    public static class StatusTransitionValidator
    {
        public static bool IsTransitionAllowed(TicketStatus from, TicketStatus to)
        {
            return (from, to) switch
            {
                (TicketStatus.New, TicketStatus.InProgress) => true,
                (TicketStatus.New, TicketStatus.Cancelled) => true,
                (TicketStatus.InProgress, TicketStatus.Resolved) => true,
                (TicketStatus.InProgress, TicketStatus.Cancelled) => true,
                (TicketStatus.Resolved, TicketStatus.Closed) => true,
                (TicketStatus.Resolved, TicketStatus.InProgress) => true,
                _ => false
            };
        }
    }
}
