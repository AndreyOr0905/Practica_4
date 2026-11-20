using System;
using System.Collections.Generic;
using System.Text;
using HelpDesk.Core.Enums;
using HelpDesk.Core.Exceptions;
using HelpDesk.Core.Models;
using HelpDesk.Core.Services;
using HelpDesk.Tests.Unit.Fakes;
using Xunit;

namespace HelpDesk.Tests.Unit
{
    public class TicketServiceTests
    {
        private readonly FakeTicketRepository _ticketRepo = new();
        private readonly FakeEmployeeRepository _empRepo = new();
        private readonly FakeEquipmentRepository _eqRepo = new();
        private readonly TicketService _service;

        public TicketServiceTests()
        {
            _empRepo.Items.Add(new Employee { Id = 1, FullName = "Иванов И.И.", IsDeleted = false });
            _empRepo.Items.Add(new Employee { Id = 2, FullName = "Удалённый Сотрудник", IsDeleted = true });
            _eqRepo.Items.Add(new Equipment { Id = 1, InventoryNumber = "INV-1", Name = "Принтер", IsDeleted = false });
            _eqRepo.Items.Add(new Equipment { Id = 2, InventoryNumber = "INV-2", Name = "Старый ПК", IsDeleted = true });

            _service = new TicketService(_ticketRepo, _empRepo, _eqRepo);
        }

        [Fact]
        public async Task CreateTicketAsync_ValidData_CreatesTicketWithNewStatus()
        {
            // Act
            int id = await _service.CreateTicketAsync(1, 1, "Не печатает принтер", "Замятие бумаги", Priority.High);

            // Assert
            var created = await _ticketRepo.GetByIdAsync(id);
            Assert.NotNull(created);
            Assert.Equal(TicketStatus.New, created!.Status);
            Assert.Equal("Не печатает принтер", created.Title);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateTicketAsync_EmptyTitle_ThrowsDomainException(string badTitle)
        {
            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() =>
                _service.CreateTicketAsync(1, 1, badTitle, "Описание", Priority.Normal));
        }

        [Fact]
        public async Task CreateTicketAsync_DescriptionOver2000Chars_ThrowsDomainException()
        {
            // Arrange
            string longDesc = new string('A', 2001);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() =>
                _service.CreateTicketAsync(1, 1, "Заголовок", longDesc, Priority.Normal));
        }

        [Fact]
        public async Task CreateTicketAsync_DeletedAuthor_ThrowsNotFoundException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.CreateTicketAsync(2, 1, "Заголовок", "Описание", Priority.Normal));
        }

        [Fact]
        public async Task AssignExecutorAsync_DeletedEmployee_ThrowsDomainException()
        {
            // Arrange
            int ticketId = await _service.CreateTicketAsync(1, 1, "Тест", "Описание", Priority.Normal);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() =>
                _service.AssignExecutorAsync(ticketId, 2));
        }

        [Fact]
        public async Task ChangeStatusAsync_ToInProgressWithoutAssignee_ThrowsDomainException()
        {
            // Arrange
            int ticketId = await _service.CreateTicketAsync(1, 1, "Тест", "Описание", Priority.Normal);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() =>
                _service.ChangeStatusAsync(ticketId, TicketStatus.InProgress, 1));
        }

        [Fact]
        public async Task ChangeStatusAsync_FromClosedToInProgress_ThrowsDomainException()
        {
            // Arrange
            int ticketId = await _service.CreateTicketAsync(1, 1, "Тест", "Описание", Priority.Normal);
            await _service.AssignExecutorAsync(ticketId, 1);
            await _service.ChangeStatusAsync(ticketId, TicketStatus.InProgress, 1);
            await _service.ChangeStatusAsync(ticketId, TicketStatus.Resolved, 1);
            await _service.ChangeStatusAsync(ticketId, TicketStatus.Closed, 1);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() =>
                _service.ChangeStatusAsync(ticketId, TicketStatus.InProgress, 1));
        }

        [Fact]
        public async Task SoftDeleteEmployeeAsync_WithActiveTickets_ThrowsDomainException()
        {
            // Arrange
            int ticketId = await _service.CreateTicketAsync(1, 1, "Тест", "Описание", Priority.Normal);
            await _service.AssignExecutorAsync(ticketId, 1);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() =>
                _service.SoftDeleteEmployeeAsync(1));
        }

        [Fact]
        public async Task GetOverdueTicketsAsync_ReturnsOnlyExpiredActiveTickets()
        {
            // Arrange
            var baseTime = new DateTimeOffset(2026, 5, 1, 10, 0, 0, TimeSpan.Zero);
            _ticketRepo.Tickets.Add(new Ticket
            {
                Id = 1,
                Priority = Priority.Critical, // SLA 4 часа -> дедлайн 14:00
                Status = TicketStatus.InProgress,
                CreatedAt = baseTime
            });
            _ticketRepo.Tickets.Add(new Ticket
            {
                Id = 2,
                Priority = Priority.Normal, // SLA 24 часа -> дедлайн завтра 10:00
                Status = TicketStatus.New,
                CreatedAt = baseTime
            });

            // Act (проверяем в 15:00 — первая просрочена, вторая нет)
            var overdue = await _service.GetOverdueTicketsAsync(baseTime.AddHours(5));

            // Assert
            Assert.Single(overdue);
            Assert.Equal(1, overdue[0].Id);
        }
    }
}
