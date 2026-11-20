using System.Windows;
using HelpDesk.Core.Enums;
using HelpDesk.Core.Exceptions;

namespace WpfApp1;

public partial class CreateTicketWindow : Window
{
    public CreateTicketWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            AuthorBox.ItemsSource = await App.EmployeeRepo.GetAllActiveAsync();
            if (AuthorBox.Items.Count > 0) AuthorBox.SelectedIndex = 0;

            EquipmentBox.ItemsSource = await App.EquipmentRepo.GetAllActiveAsync();
            if (EquipmentBox.Items.Count > 0) EquipmentBox.SelectedIndex = 0;

            PriorityBox.ItemsSource = Enum.GetValues<Priority>();
            PriorityBox.SelectedItem = Priority.Normal;
        };
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (AuthorBox.SelectedValue is not int authorId || EquipmentBox.SelectedValue is not int eqId)
            {
                MessageBox.Show("Выберите автора и оборудование.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var priority = (Priority)PriorityBox.SelectedItem;
            await App.TicketService.CreateTicketAsync(authorId, eqId, TitleBox.Text, DescriptionBox.Text, priority);
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
