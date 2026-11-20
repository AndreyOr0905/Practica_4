using HelpDesk.Core.Interfaces;
using HelpDesk.Core.Services;
using HelpDesk.Data.EfModels;
using HelpDesk.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows;

namespace WpfApp1;

public partial class App : Application
{
    public static TicketService TicketService { get; private set; } = null!;
    public static TicketRepository TicketRepo { get; private set; } = null!;
    public static EmployeeRepository EmployeeRepo { get; private set; } = null!;
    public static EquipmentRepository EquipmentRepo { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
            .Build();

        string connectionString = config.GetConnectionString("HelpDeskDb")
            ?? throw new InvalidOperationException("Строка подключения HelpDeskDb не найдена.");

        var dbOptions = new DbContextOptionsBuilder<HelpDeskDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        TicketRepo = new TicketRepository(dbOptions);
        EmployeeRepo = new EmployeeRepository(dbOptions);
        EquipmentRepo = new EquipmentRepository(dbOptions);

        TicketService = new TicketService(TicketRepo, EmployeeRepo, EquipmentRepo);
    }
}
