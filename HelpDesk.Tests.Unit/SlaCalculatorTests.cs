using HelpDesk.Core.Enums;
using HelpDesk.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using HelpDesk.Core.Enums;
using HelpDesk.Core.Services;
using Xunit;

namespace HelpDesk.Tests.Unit
{
    public class SlaCalculatorTests
    {
        [Theory]
        [InlineData(Priority.Critical, 4)]
        [InlineData(Priority.High, 8)]
        [InlineData(Priority.Normal, 24)]
        [InlineData(Priority.Low, 72)]
        public void CalculateSlaDeadline_ValidPriority_ReturnsExpectedOffset(Priority priority, int expectedHours)
        {
            // Arrange
            var created = new DateTimeOffset(2026, 5, 1, 10, 0, 0, TimeSpan.Zero);

            // Act
            var deadline = SlaCalculator.CalculateSlaDeadline(created, priority);

            // Assert
            Assert.Equal(created.AddHours(expectedHours), deadline);
        }

        [Fact]
        public void CalculateSlaDeadline_InvalidPriority_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var created = DateTimeOffset.UtcNow;
            var invalidPriority = (Priority)999;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SlaCalculator.CalculateSlaDeadline(created, invalidPriority));
        }
    }
}
