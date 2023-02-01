using Microsoft.AspNetCore.Builder;
using System.Linq.Expressions;
using System.Net;
using Wechaty;
using Wechaty.Module.Puppet.Schemas;
//using Wechaty.Module;



public class Program
{



    public static void Main(string[] args)
    {


        var options = new PuppetOptions();
        //WechatyScanEventListener wechatyScanEventListener = (qrcode, status, data) =>
        //{
        //    Console.WriteLine($"Scan QR Code to login: {status} https://wechaty.js.org/qrcode/{(qrcode)}`");
        //};
        var wechaty = new Wechaty.Wechaty(options).OnScan((qrcode, status, data) =>
        {
            Console.WriteLine($"Scan QR Code to login: {status} https://wechaty.js.org/qrcode/{(qrcode)}`");
        }).OnLogin(user =>
        {
            Console.WriteLine("User {user} logined");
        }).OnMessage(message =>
        {
            Console.WriteLine($"Message: {message}");
        }).Start();



        //var options = new PuppetOptions();

        //var wechaty = new Wechaty.Wechaty(options).OnScan((qrcode, status, null) => {
        //    Console.WriteLine($"Scan QR Code to login: {status} https://wechaty.js.org/qrcode/{(qrcode)}`");
        //}).OnLogin(user =>
        //{
        //    Console.WriteLine("User {user} logined");
        //}).OnMessage(message =>
        //{
        //    Console.WriteLine($"Message: {message}");
        //}).Start();


        Console.WriteLine("ok");
        Console.ReadKey();


    }

}






