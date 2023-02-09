using BackendAPI.Web.Core.Helper;
using Microsoft.Extensions.Configuration;

namespace ChatGPT_API
{
    class Program
    {
        async static Task Main(string[] args)
        {

            TimerHelper.configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")
              .Build();

            //var value1 = configuration.GetSection("Settings")["Key1"];
            //var value2 = configuration.GetSection("Settings")["Key2"];



            TimerHelper.Start();
            //Console.WriteLine(result);

            Console.WriteLine("OK");


            Console.ReadLine();
        }




    }
}
