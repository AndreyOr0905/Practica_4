using System.Windows;
using HelpDesk.Core.Enums;
using HelpDesk.Core.Exceptions;

namespace WpfApp1;

public partial class ChangeStatusWindow : Window
{
    private readonly int _ticketId;

    public ChangeStatusWindow(int ticketId, string title, TicketStatus currentStatus)
    {
        InitializeComponent();
        _ticketId = ticketId;
        TicketInfoText.Text = $"Заявка #{ticketId}: {title} (Текущий: {currentStatus})";

        Loaded += async (_, _) =>
        {
            ChangedByBox.ItemsSource = await App.EmployeeRepo.GetAllActiveAsync();
            if (ChangedByBox.Items.Count > 0) ChangedByBox.SelectedIndex = 0;

            NewStatusBox.ItemsSource = Enum.GetValues<TicketStatus>();
            NewStatusBox.SelectedItem = currentStatus;
        };
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (ChangedByBox.SelectedValue is not int userId) return;
            var newStatus = (TicketStatus)NewStatusBox.SelectedItem;

            await App.TicketService.ChangeStatusAsync(_ticketId, newStatus, userId);
            DialogResult = true;
        }
        catch (DomainException ex)
        {
            MessageBox.Show(ex.Message, "Запрещённый переход", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
