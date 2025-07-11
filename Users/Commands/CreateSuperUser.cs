using System.CommandLine;
using Microsoft.AspNetCore.Identity;

using Users.Models;

namespace Users.Commands;

public class CreateSuperUserCommand
{
    private readonly UserManager<User> _userManager;

    public CreateSuperUserCommand(UserManager<User> userManager) =>
        _userManager = userManager;

    public Command CreateCommand()
    {
        var firstNameArg = new Argument<string>(
            name: "firstname",
            description: "First name of the superuser",
            getDefaultValue: () => string.Empty
        );

        var lastNameArg = new Argument<string>(
            name: "lastname",
            description: "Last name of the superuser",
            getDefaultValue: () => string.Empty
        );

        var emailArg = new Argument<string>(
            name: "email",
            description: "Email address of the superuser",
            getDefaultValue: () => string.Empty
        );

        var passwordArg = new Argument<string>(
            name: "password",
            description: "Password of the superuser",
            getDefaultValue: () => string.Empty
        );

        var command = new Command("createsuperuser", "Create a superuser for the application")
        {
            firstNameArg,
            lastNameArg,
            emailArg,
            passwordArg
        };

        command.SetHandler(async (firstName, lastName, email, password) =>
        {
            firstName = GetConsoleInputIfEmpty(firstName, "Enter first name: ", value =>
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("A first name is required.");
                return value;
            });

            lastName = GetConsoleInputIfEmpty(lastName, "Enter last name: ", value =>
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("A last name is required.");
                return value;
            });

            email = GetConsoleInputIfEmpty(email, "Enter email address: ", value =>
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
                    throw new ArgumentException("A valid email address is required.");
                return value;
            });

            password = GetConsoleInputIfEmpty(password, "Enter password: ", value =>
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length < 6)
                    throw new ArgumentException("Password must be at least 6 characters long.");
                return value;
            }, hideInput: true);

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                Console.WriteLine($"Superuser '{user.GetFullName()}' created successfully.");
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
        }, firstNameArg, lastNameArg, emailArg, passwordArg);

        return command;
    }

    private static string GetConsoleInputIfEmpty(
        string input,
        string prompt,
        Func<string, string> validator,
        bool hideInput = false
    )
    {
        if (!string.IsNullOrWhiteSpace(input))
            return input;

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
