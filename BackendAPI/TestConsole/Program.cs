using System;

class Program
{
    static void Main(string[] args)
    {
        // Print the command-line arguments
        Console.WriteLine("Command-line arguments:");
        foreach (string arg in args)
        {
            Console.WriteLine("  {0}", arg);
        }

        // Wait for user input
        Console.Write("Press any key to exit...");
        Console.ReadKey();
    }
}
