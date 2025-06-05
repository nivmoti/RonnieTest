using RonnieTest.APIHandler;

class Program
{
    static async Task Main(string[] args)
    {
        Console.Write("Enter folder path to save the file: ");
        string? folderPath = Console.ReadLine();

        Console.Write("Enter desired format (JSON or CSV): ");
        string? formatInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(formatInput))
        {
            Console.WriteLine("Invalid input.");
            return;
        }

        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine("Folder does not exist. Creating...");
            Directory.CreateDirectory(folderPath);
        }

        APIHandler aPIHandler = new APIHandler();
        aPIHandler.AddAPI("https://randomuser.me/api/?results=500");
        aPIHandler.AddAPI("https://jsonplaceholder.typicode.com/users");
        aPIHandler.AddAPI("https://dummyjson.com/users");
        aPIHandler.AddAPI("https://reqres.in/api/users");

        DataHandler dataHandler = new DataHandler(aPIHandler);

        // Fetch users from all APIs
        await dataHandler.LoadUsersAsync();

        // Save the result to file
        await dataHandler.SaveToFileAsync(folderPath, formatInput);

        Console.WriteLine("Done.");
    }
}
