using BackendAPI.Core;
using BackendAPI.Core.Entities;

using Furion.JsonSerialization;
namespace BackendAPI.Application
{
    public class WalkerService : IWalkerService, ITransient
    {
        public object GetDescription()
        {
        
            var res= DbContext.Instance.Queryable<User>().ToList();
            //return "让 .NET 开发更简单，更通用，更流行。";
            return res;
        }
    }
}