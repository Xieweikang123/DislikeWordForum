using Microsoft.AspNetCore.Builder;
using System.Linq.Expressions;
using System.Net;
using System.Speech;
using System.Speech.Synthesis;


public class Program
{



    public static void Main(string[] args)
    {



        //Console.WriteLine();
        ////  Invoke this sample with an arbitrary set of command line arguments.
        //string[] arguments = Environment.GetCommandLineArgs();

        //Console.WriteLine("GetCommandLineArgs: {0}", string.Join(", ", arguments));


        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        WebApplication app = builder.Build();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        // Add a single endpoint
        app.MapGet("/", () => "Hello World!");
        app.Run();


        Console.WriteLine("ok");
        Console.ReadKey();


    }

}






