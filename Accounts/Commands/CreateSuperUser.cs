using System.CommandLine;
using Microsoft.AspNetCore.Identity;

using Accounts.Models;

namespace Accounts.Commands;

public class CreateSuperUserCommand
{
    private readonly UserManager<User> _userManager;

    public CreateSuperUserCommand(UserManager<User> userManager) =>
        _userManager = userManager;

    public Command CreateCommand()
    {
        var command = new Command("createsuperuser", "Create a superuser for the application");

        command.SetAction(async parseResult =>
        {
            string firstName = ConsoleInput(
                "First Name: ", 
                value => string.IsNullOrEmpty(value) ? 
                throw new ArgumentException("first name is required.") : value
            );

            string lastName = ConsoleInput(
                "Last Name: ",
                value => string.IsNullOrWhiteSpace(value) ? 
                throw new ArgumentException("last name is required.") : value
            );

            string email = ConsoleInput(
                "Email: ",
                value => string.IsNullOrWhiteSpace(value) || !value.Contains("@") ? 
                throw new ArgumentException("a valid email is required.") : value
            );

            string password = ConsoleInput(
                "Password: ",
                value => string.IsNullOrWhiteSpace(value) || value.Length < 6 ? 
                throw new ArgumentException("Password must be at least 6 characters long.") : value,
                hideInput: true
            );

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                Console.WriteLine($"Superuser '{user.FullName}' created successfully.");
                await _userManager.AddToRoleAsync(user, "superuser");
            }
            else
            {
                Console.WriteLine("Failed to create superuser:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"- {error.Description}");
                }
            }
        });

        return command;
    }

    private static string ConsoleInput(
        string prompt,
        Func<string, string> validator,
        bool hideInput = false
    )
    {
        Console.Write(prompt);
        string? value;
        if (hideInput)
        {
            value = "";
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                if (key.Key == ConsoleKey.Backspace && value.Length > 0)
                {
                    value = value[0..^1];
                    Console.Write("\b \b");
                }
                else if (key.Key != ConsoleKey.Backspace)
                {
                    value += key.KeyChar;
                    Console.Write("*");
                }
            }
            Console.WriteLine();
        }
        else
        {
            value = Console.ReadLine();
        }

        return validator(value ?? string.Empty);
    }
}
