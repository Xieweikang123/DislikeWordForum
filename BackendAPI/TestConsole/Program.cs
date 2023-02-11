using System;
using Castle.DynamicProxy;

public class LogInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        Console.WriteLine("Entering method: " + invocation.Method.Name);
        invocation.Proceed();
        Console.WriteLine("Exiting method: " + invocation.Method.Name);
    }
}

public class MyClass
{
    public virtual void MyMethod(string a)
    {
        Console.WriteLine("MyMethod is executing " + a);
    }
}

class Program
{
    static void Main(string[] args)
    {
        var proxyGenerator = new ProxyGenerator();
        var myObj = proxyGenerator.CreateClassProxy<MyClass>(new LogInterceptor());

        myObj.MyMethod("zz");
    }
}
