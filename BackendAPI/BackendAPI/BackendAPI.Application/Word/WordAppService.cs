using BackendAPI.Application.User;
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

        [HttpPost]
        public async Task<object> GetMyWordList(PageInfo dto)
        {
            var userId = CurrentUserInfo.UserId;
            var db = DbContext.Instance;

            RefAsync<int> allCount = 0;
            var pageList = await db.Queryable<EnglishWord>().Where(x => x.BelongUserId == userId).ToPageListAsync(dto.pageNumber, dto.pageSize, allCount);

            return new { pageList, allCount };
        }


    }
}