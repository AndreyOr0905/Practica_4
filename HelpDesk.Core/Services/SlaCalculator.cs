using System;
using System.Collections.Generic;
using System.Text;
using HelpDesk.Core.Enums;

namespace HelpDesk.Core.Services
{
    public static class SlaCalculator
    {
        public static DateTimeOffset CalculateSlaDeadline(DateTimeOffset createdAt, Priority priority)
        {
            return priority switch
            {
                Priority.Critical => createdAt.AddHours(4),
                Priority.High => createdAt.AddHours(8),
                Priority.Normal => createdAt.AddHours(24),
                Priority.Low => createdAt.AddHours(72),
                _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, "Неизвестный приоритет заявки.")
            };
        }
    }
}
