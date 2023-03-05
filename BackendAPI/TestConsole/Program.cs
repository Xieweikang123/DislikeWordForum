using Google.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

class MyClass
{

    public string MyProperty { get; set; }
}

class Program
{
    static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();


        

        app.UseRouting();


        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHealthChecks("/healthz").RequireAuthorization();
        app.MapGet("/", () => "Hello World!");


        app.Run();




        Console.WriteLine("ok");
    }



}