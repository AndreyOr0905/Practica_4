using System.Windows;
using HelpDesk.Core.Exceptions;

namespace WpfApp1;

public partial class AssignExecutorWindow : Window
{
    private readonly int _ticketId;

    public AssignExecutorWindow(int ticketId, string title)
    {
        InitializeComponent();
        _ticketId = ticketId;
        TicketTitleText.Text = $"Заявка #{ticketId}: {title}";

        Loaded += async (_, _) =>
        {
            ExecutorBox.ItemsSource = await App.EmployeeRepo.GetAllActiveAsync();
            if (ExecutorBox.Items.Count > 0) ExecutorBox.SelectedIndex = 0;
        };
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (ExecutorBox.SelectedValue is not int execId) return;
            await App.TicketService.AssignExecutorAsync(_ticketId, execId);
            DialogResult = true;
        }
        catch (DomainException ex)
        {
            MessageBox.Show(ex.Message, "Бизнес-правило", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
