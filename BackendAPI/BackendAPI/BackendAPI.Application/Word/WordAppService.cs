using BackendAPI.Core;
using BackendAPI.Core.Entities;
using Furion.DistributedIDGenerator;

namespace BackendAPI.Application
{
    /// <summary>
    /// 单词服务接口
    /// </summary>
    /// 
    public class WordAppService : IDynamicApiController
    {
        public WordAppService()
        {

        }

        public async Task<RetObj> Test()
        {
            return RetObj.Success(null, "测试");
        }

        public async Task<RetObj> GetMyWordList(UserDTO dto)
        {
            var db = DbContext.Instance;




            return RetObj.Success(null, "测试");
        }


    }
}