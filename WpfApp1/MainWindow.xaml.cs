using System.Windows;
using System.Windows.Controls;
using HelpDesk.Core.Enums;
using HelpDesk.Core.Exceptions;
using HelpDesk.Core.Models;
using HelpDesk.Core.Services;

namespace WpfApp1;

public partial class MainWindow : Window
{
    private bool _isInitializing = true;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await InitializeFiltersAsync();
        await LoadTicketsAsync();
    }

    private async Task InitializeFiltersAsync()
    {
        _isInitializing = true;

        var statuses = new List<object> { "Все" };
        statuses.AddRange(Enum.GetValues<TicketStatus>().Cast<object>());
        StatusFilterBox.ItemsSource = statuses;
        StatusFilterBox.SelectedIndex = 0;

        var priorities = new List<object> { "Все" };
        priorities.AddRange(Enum.GetValues<Priority>().Cast<object>());
        PriorityFilterBox.ItemsSource = priorities;
        PriorityFilterBox.SelectedIndex = 0;

        var employees = await App.EmployeeRepo.GetAllActiveAsync();
        var assigneeList = new List<Employee> { new Employee { Id = 0, FullName = "Все исполнители" } };
        assigneeList.AddRange(employees);
        AssigneeFilterBox.ItemsSource = assigneeList;
        AssigneeFilterBox.SelectedIndex = 0;

        _isInitializing = false;
    }

    private async Task LoadTicketsAsync()
    {
        try
        {
            TicketStatus? statusFilter = StatusFilterBox.SelectedItem is TicketStatus st ? st : null;
            Priority? priorityFilter = PriorityFilterBox.SelectedItem is Priority pr ? pr : null;
            int? assigneeFilter = AssigneeFilterBox.SelectedValue is int empId && empId > 0 ? empId : null;

            DateTimeOffset? from = DateFromPicker.SelectedDate.HasValue
                ? new DateTimeOffset(DateFromPicker.SelectedDate.Value.Date, TimeSpan.Zero)
                : null;

            DateTimeOffset? to = DateToPicker.SelectedDate.HasValue
                ? new DateTimeOffset(DateToPicker.SelectedDate.Value.Date.AddDays(1).AddTicks(-1), TimeSpan.Zero)
                : null;

            var tickets = await App.TicketRepo.GetAllAsync(statusFilter, priorityFilter, assigneeFilter, from, to);
            var employees = (await App.EmployeeRepo.GetAllActiveAsync()).ToDictionary(x => x.Id, x => x.FullName);
            var equipments = (await App.EquipmentRepo.GetAllActiveAsync()).ToDictionary(x => x.Id, x => $"{x.InventoryNumber} ({x.Name})");

            var now = DateTimeOffset.UtcNow;

            var rows = tickets.Select(t =>
            {
                var deadline = SlaCalculator.CalculateSlaDeadline(t.CreatedAt, t.Priority);
                bool isOverdue = (t.Status is TicketStatus.New or TicketStatus.InProgress) && now > deadline;

                return new TicketRowViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Priority = t.Priority,
                    Status = t.Status,
                    AuthorName = employees.GetValueOrDefault(t.AuthorId, $"ID {t.AuthorId}"),
                    EquipmentName = equipments.GetValueOrDefault(t.EquipmentId, $"ID {t.EquipmentId}"),
                    AssigneeName = t.AssigneeId.HasValue
                        ? employees.GetValueOrDefault(t.AssigneeId.Value, $"ID {t.AssigneeId}")
                        : "Не назначен",
                    CreatedAtText = t.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                    SlaDeadlineText = deadline.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                    OverdueText = isOverdue ? "ДА (SLA!)" : "Нет"
                };
            }).ToList();

            TicketsGrid.ItemsSource = rows;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void Filter_Changed(object sender, EventArgs e)
    {
        if (!_isInitializing)
            await LoadTicketsAsync();
    }

    private async void ResetFilters_Click(object sender, RoutedEventArgs e)
    {
        _isInitializing = true;
        StatusFilterBox.SelectedIndex = 0;
        PriorityFilterBox.SelectedIndex = 0;
        AssigneeFilterBox.SelectedIndex = 0;
        DateFromPicker.SelectedDate = null;
        DateToPicker.SelectedDate = null;
        _isInitializing = false;

        await LoadTicketsAsync();
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadTicketsAsync();

    private async void CreateTicket_Click(object sender, RoutedEventArgs e)
    {
        var win = new CreateTicketWindow { Owner = this };
        if (win.ShowDialog() == true)
        {
            await LoadTicketsAsync();
        }
    }

    private async void AssignExecutor_Click(object sender, RoutedEventArgs e)
    {
        if (TicketsGrid.SelectedItem is not TicketRowViewModel selected)
        {
            MessageBox.Show("Выберите заявку в таблице.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var win = new AssignExecutorWindow(selected.Id, selected.Title) { Owner = this };
        if (win.ShowDialog() == true)
        {
            await LoadTicketsAsync();
        }
    }

    private async void ChangeStatus_Click(object sender, RoutedEventArgs e)
    {
        if (TicketsGrid.SelectedItem is not TicketRowViewModel selected)
        {
            MessageBox.Show("Выберите заявку в таблице.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var win = new ChangeStatusWindow(selected.Id, selected.Title, selected.Status) { Owner = this };
        if (win.ShowDialog() == true)
        {
            await LoadTicketsAsync();
        }
    }

    private void Reports_Click(object sender, RoutedEventArgs e)
    {
        var win = new ReportsWindow { Owner = this };
        win.ShowDialog();
    }

    private async void References_Click(object sender, RoutedEventArgs e)
    {
        var win = new ReferencesWindow { Owner = this };
        win.ShowDialog();
        await InitializeFiltersAsync();
        await LoadTicketsAsync();
    }
}

public class TicketRowViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string AssigneeName { get; set; } = string.Empty;
    public string CreatedAtText { get; set; } = string.Empty;
    public string SlaDeadlineText { get; set; } = string.Empty;
    public string OverdueText { get; set; } = string.Empty;
}
