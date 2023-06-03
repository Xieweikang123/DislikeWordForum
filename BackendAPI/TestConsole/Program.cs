using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel.DataAnnotations;


// Define a service interface
public interface IMyService
{
    void DoSomething();
}

// Define a class that implements the service interface
public class MyService : IMyService
{
    public void DoSomething()
    {
        Console.WriteLine("MyService is doing something.");
    }
}

public class MyService2 : IMyService
{

    private int num = 0;
    public void DoSomething()
    {

        Console.WriteLine($"num: {num++}");
    }
}



// Define a class that uses the service
public class MyClass
{
    private readonly IMyService? _myService;

    public MyClass(IServiceProvider serviceProvider)
    {
        var tpf = typeof(IMyService);
        // Request an instance of the service from the service provider
        _myService = serviceProvider.GetService(typeof(IMyService)) as IMyService;
    }

    public void DoSomethingWithService()
    {
        // Use the service object
        _myService?.DoSomething();
    }
}



class Program
{
    static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
         .AddTransient<IMyService, MyService2>()
         .BuildServiceProvider();



        //serviceProvider.
        // Create an instance of the class that uses the service
        var myClass = new MyClass(serviceProvider);

        // Use the service object
        myClass.DoSomethingWithService();
        myClass.DoSomethingWithService();


        // Wait for user input
        Console.Write("Press any key to exit...");
        //Console.ReadKey();
    }
}
