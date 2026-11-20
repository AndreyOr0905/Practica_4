using System.Text;
using System.Windows;
using HelpDesk.Core.Enums;

namespace WpfApp1;

public partial class ReportsWindow : Window
{
    public ReportsWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await BuildReportAsync();
    }

    private async Task BuildReportAsync()
    {
        var allTickets = await App.TicketRepo.GetAllAsync();
        var overdue = await App.TicketService.GetOverdueTicketsAsync();

        var sb = new StringBuilder();
        sb.AppendLine($"Всего заявок в системе: {allTickets.Count}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine("Распределение по статусам:");
        foreach (var status in Enum.GetValues<TicketStatus>())
        {
            int count = allTickets.Count(t => t.Status == status);
            sb.AppendLine($"  - {status,-12}: {count}");
        }

        sb.AppendLine("----------------------------------------");
        sb.AppendLine($"Просрочено активных заявок (SLA): {overdue.Count}");
        foreach (var t in overdue)
        {
            sb.AppendLine($"  * [#{t.Id}] {t.Title} (Приоритет: {t.Priority}, Статус: {t.Status})");
        }

        sb.AppendLine("----------------------------------------");
        sb.AppendLine("Время нахождения заявок в статусе InProgress:");
        foreach (var t in allTickets.Take(15))
        {
            var duration = await App.TicketService.GetTotalInProgressDurationAsync(t.Id);
            sb.AppendLine($"  * Заявка #{t.Id}: {duration.TotalHours:F1} ч.");
        }

        ReportTextBox.Text = sb.ToString();
    }
}
