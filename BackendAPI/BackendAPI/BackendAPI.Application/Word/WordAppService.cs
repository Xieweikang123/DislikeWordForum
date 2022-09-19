using BackendAPI.Application.User;
using BackendAPI.Core;
using BackendAPI.Core.Entities;
using Furion.DistributedIDGenerator;
using System.Linq.Expressions;

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

            var orderProp = "RecordTimes";
            var orderType = "desc";

            //指定了排序列
            if (!string.IsNullOrEmpty(dto.prop))
            {
                orderProp = dto.prop;
            }

            orderType = dto.order switch
            {
                "descending" => "desc",
                "ascending" => "asc",
                _ => "desc"
            };

            

            RefAsync<int> allCount = 0;
            var pageList = await db.Queryable<EnglishWord>().Where(x => x.BelongUserId == userId).OrderBy($"{orderProp}  {orderType}").ToPageListAsync(dto.pageNumber, dto.pageSize, allCount);

            return new { pageList, allCount };
        }


    }
}