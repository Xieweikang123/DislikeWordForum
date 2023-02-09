using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using BackendAPI.Web.Core.Helper;
using Confluent.Kafka;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Completions;

namespace ChatGPT_API
{


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MyAttribute : Attribute
    {
        public string Value { get; set; }
    }

    [MyAttribute(Value = "Hello, 11!")]
    public class MyClass
    {
        [MyAttribute(Value = "Hello, World!")]
        public void MyMethod()
        {
            Console.WriteLine("MyMethod is executing");
        }
    }


    class Program
    {
        static void Main(string[] args)
        {

            using (StreamReader reader = new StreamReader("example.txt"))
            {
                string line;
                //while ((line = reader.ReadLine()) != null)
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }

            Console.WriteLine("OK");
            Console.ReadLine();
        }




    }
}
