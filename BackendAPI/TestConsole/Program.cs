using System;
using System.Net.Http;
using System.Text;
using BackendAPI.Web.Core.Helper;
using Confluent.Kafka;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Completions;

namespace ChatGPT_API
{
    class Program
    {
        async static Task Main(string[] args)
        {



            TimerHelper.Start();
            //Console.WriteLine(result);

            Console.WriteLine("OK");


            Console.ReadLine();
        }

     


    }
}
