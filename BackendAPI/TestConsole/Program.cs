using System;
using System.Reflection;

class MyClass
{

    public string MyProperty { get; set; }
}

class Program
{
    static void Main()
    {
        //MyClass obj = new MyClass { MyProperty = "hello world" };
        //string propertyName = "MyProperty";


        //var ss1 = obj.GetPropertyValue(propertyName);
        //var propertyAccessor = new PropertyAccessor(obj);

        //var name = propertyAccessor[propertyName];



        var res = -8 % 7;

        Console.WriteLine("ok");
        //// 使用反射获取对象的属性值
        //PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);
        //if (propertyInfo != null)
        //{
        //    object propertyValue = propertyInfo.GetValue(obj);
        //    Console.WriteLine(propertyValue);
        //}
    }



    static void Method1()
    {
        throw new Exception("我错了1");
    }
    static Task Method2()
    {
        Console.WriteLine("m2 执行");
        return Task.Run(() =>
          {
              Console.WriteLine("m2 task");
               
              throw new Exception("我错了2");

          });

    }



}

static class PropertyAccessor
{
    //private object obj;

    public static object GetPropertyValue(this object obj, string propertyName)
    {
        var propertyInfo = obj.GetType().GetProperty(propertyName);
        if (propertyInfo != null)
        {
            return propertyInfo.GetValue(obj);
        }
        return null;
    }


    //public PropertyAccessor(object obj)
    //{
    //    this.obj = obj;
    //}

    //public object this[string key]
    //{
    //    get
    //    {
    //        var propertyInfo = obj.GetType().GetProperty(key);
    //        if (propertyInfo != null)
    //        {
    //            return propertyInfo.GetValue(obj);
    //        }
    //        return null;
    //    }
    //}


}
