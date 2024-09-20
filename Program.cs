using Bogus;
using Lesson4CriticalSection.Models;
using System.ComponentModel.Design;
using System.Text.Json;

internal class Program
{
    private static List<User> _users = [];
    private static object _obj = new object();

    private static void FillUsersFromFile(string filePath)
    {

        var json = File.ReadAllText(filePath);
        var users = JsonSerializer.Deserialize<List<User>>(json);

        lock (_obj)
        {
            _users.AddRange(users!);
        }
    }
    private static void FillByMultipleThreads()
    {   

        ThreadPool.QueueUserWorkItem(o => FillUsersFromFile("1.json"));
        ThreadPool.QueueUserWorkItem(o => FillUsersFromFile("2.json"));
        ThreadPool.QueueUserWorkItem(o => FillUsersFromFile("3.json"));
        ThreadPool.QueueUserWorkItem(o => FillUsersFromFile("4.json"));
        ThreadPool.QueueUserWorkItem(o => FillUsersFromFile("5.json"));
    }

    private static void FillBySingleThread()
    {
        FillUsersFromFile("1.json");
        FillUsersFromFile("2.json");
        FillUsersFromFile("3.json");
        FillUsersFromFile("4.json");
        FillUsersFromFile("5.json");
    }
    private static void Main(string[] args)
    {
        var faker = new Faker<User>();

        var users = faker.RuleFor(u => u.FirstName, f => f.Person.FirstName)
             .RuleFor(u => u.LastName, f => f.Person.LastName)
             .RuleFor(u => u.Email, f => f.Internet.Email())
             .Generate(20);

        var json = JsonSerializer.Serialize(users);
        File.WriteAllText("1.json", json);
        File.WriteAllText("2.json", json);
        File.WriteAllText("3.json", json);
        File.WriteAllText("4.json", json);
        File.WriteAllText("5.json", json);

        Console.Clear();
    Menu:
        int choice;
        Console.WriteLine("1.Single");
        Console.WriteLine("2.Multiple");
        Console.Write("Enter your choice : ");
        choice = int.Parse(Console.ReadLine()!);

        if (choice == 1)
        {
            FillBySingleThread();
        }

        else if (choice == 2)
        {
            FillByMultipleThreads();
        }
        else
        {
            Console.WriteLine("Wrong choice");
            Thread.Sleep(2000);
            goto Menu;
        }

        Console.Clear();
        int count = 0;
        foreach (User user in _users)
        {
            count++;
            Console.WriteLine($"{count} {user.FirstName} {user.LastName}");
        }

        Console.ReadKey();
    }
}