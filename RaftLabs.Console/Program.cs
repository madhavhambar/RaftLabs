using ConsoleTables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RaftLabUsers.Core.Configuration;
using RaftLabUsers.Core.Extensions;
using RaftLabUsers.Core.Services;

// service collections
var services = new ServiceCollection();
// configuration to read data from appsettings.json
var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

// add service to the collection
services.AddUserService(config.GetSection("ApiOptions").Get<ApiOptions>());

// build the service provider and get the user seervice
var serviceProvider = services.BuildServiceProvider();
var userService = serviceProvider.GetRequiredService<IExternalUserService>();

// start using the library service.
try
{
    // attepmt 1
    Console.WriteLine("...Fetching user with ID 1...");
    var user = await userService.GetUserByIdAsync(1);
    ConsoleTable
        .From([user])
        .Configure(o => o.NumberAlignment = Alignment.Right)
        .Write();

    // attepmt 2
    Console.WriteLine("\n...Fetching all users...");
    var allUsers = await userService.GetAllUsersAsync();
    ConsoleTable
        .From(allUsers)
        .Configure(o => o.NumberAlignment = Alignment.Right)
        .Write();

    // attepmt 3
    Console.WriteLine("\n...Fetching the same user with ID 1...");
    user = await userService.GetUserByIdAsync(1);
    ConsoleTable
        .From([user])
        .Configure(o => o.NumberAlignment = Alignment.Right)
        .Write();

    // attepmt 4
    Console.WriteLine("\n...Fetching the same user with ID 20...");
    user = await userService.GetUserByIdAsync(20);

    // attepmt 5
    // disconnect your internet or change the base url to see retry

}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Error: {ex.Message}");
    Console.ResetColor();
}