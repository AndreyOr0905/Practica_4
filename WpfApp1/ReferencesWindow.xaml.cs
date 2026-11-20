using System.Windows;
using HelpDesk.Core.Exceptions;
using HelpDesk.Core.Models;

namespace WpfApp1;

public partial class ReferencesWindow : Window
{
    public ReferencesWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        EmployeesGrid.ItemsSource = await App.EmployeeRepo.GetAllActiveAsync();
        EquipmentGrid.ItemsSource = await App.EquipmentRepo.GetAllActiveAsync();
    }

    private async void AddEmployee_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmpNameBox.Text)) return;
        await App.EmployeeRepo.CreateAsync(new Employee
        {
            FullName = EmpNameBox.Text.Trim(),
            Position = EmpPosBox.Text.Trim(),
            Department = EmpDeptBox.Text.Trim()
        });
        EmpNameBox.Clear(); EmpPosBox.Clear(); EmpDeptBox.Clear();
        await LoadDataAsync();
    }

    private async void DeleteEmployee_Click(object sender, RoutedEventArgs e)
    {
        if (EmployeesGrid.SelectedItem is not Employee emp) return;
        try
        {
            await App.TicketService.SoftDeleteEmployeeAsync(emp.Id);
            await LoadDataAsync();
        }
        catch (DomainException ex)
        {
            MessageBox.Show(ex.Message, "Нельзя удалить", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void AddEquipment_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EqInvBox.Text) || string.IsNullOrWhiteSpace(EqNameBox.Text)) return;
        await App.EquipmentRepo.CreateAsync(new Equipment
        {
            InventoryNumber = EqInvBox.Text.Trim(),
            Name = EqNameBox.Text.Trim(),
            Department = EqDeptBox.Text.Trim()
        });
        EqInvBox.Clear(); EqNameBox.Clear(); EqDeptBox.Clear();
        await LoadDataAsync();
    }

    private async void DeleteEquipment_Click(object sender, RoutedEventArgs e)
    {
        if (EquipmentGrid.SelectedItem is not Equipment eq) return;
        await App.EquipmentRepo.SoftDeleteAsync(eq.Id);
        await LoadDataAsync();
    }
}
