using System;
using System.Collections.Generic;
using System.Text;
using HelpDesk.Core.Enums;
using HelpDesk.Core.Services;
using Xunit;

namespace HelpDesk.Tests.Unit
{
    public class StatusTransitionValidatorTests
    {
        [Theory]
        // Разрешённые переходы
        [InlineData(TicketStatus.New, TicketStatus.InProgress, true)]
        [InlineData(TicketStatus.New, TicketStatus.Cancelled, true)]
        [InlineData(TicketStatus.InProgress, TicketStatus.Resolved, true)]
        [InlineData(TicketStatus.InProgress, TicketStatus.Cancelled, true)]
        [InlineData(TicketStatus.Resolved, TicketStatus.Closed, true)]
        [InlineData(TicketStatus.Resolved, TicketStatus.InProgress, true)]
        // Запрещённые переходы (включая все из Closed и Cancelled)
        [InlineData(TicketStatus.New, TicketStatus.New, false)]
        [InlineData(TicketStatus.New, TicketStatus.Resolved, false)]
        [InlineData(TicketStatus.New, TicketStatus.Closed, false)]
        [InlineData(TicketStatus.InProgress, TicketStatus.New, false)]
        [InlineData(TicketStatus.InProgress, TicketStatus.InProgress, false)]
        [InlineData(TicketStatus.InProgress, TicketStatus.Closed, false)]
        [InlineData(TicketStatus.Resolved, TicketStatus.New, false)]
        [InlineData(TicketStatus.Resolved, TicketStatus.Resolved, false)]
        [InlineData(TicketStatus.Resolved, TicketStatus.Cancelled, false)]
        [InlineData(TicketStatus.Closed, TicketStatus.New, false)]
        [InlineData(TicketStatus.Closed, TicketStatus.InProgress, false)]
        [InlineData(TicketStatus.Closed, TicketStatus.Resolved, false)]
        [InlineData(TicketStatus.Closed, TicketStatus.Closed, false)]
        [InlineData(TicketStatus.Closed, TicketStatus.Cancelled, false)]
        [InlineData(TicketStatus.Cancelled, TicketStatus.New, false)]
        [InlineData(TicketStatus.Cancelled, TicketStatus.InProgress, false)]
        [InlineData(TicketStatus.Cancelled, TicketStatus.Resolved, false)]
        [InlineData(TicketStatus.Cancelled, TicketStatus.Closed, false)]
        [InlineData(TicketStatus.Cancelled, TicketStatus.Cancelled, false)]
        public void IsTransitionAllowed_AllMatrixCombinations_ReturnsExpectedResult(
            TicketStatus from, TicketStatus to, bool expected)
        {
            // Act
            var result = StatusTransitionValidator.IsTransitionAllowed(from, to);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
