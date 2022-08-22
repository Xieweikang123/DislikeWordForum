using BackendAPI.Core;
using BackendAPI.Core.Entities;
using Furion.DistributedIDGenerator;

namespace BackendAPI.Application
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public class UserAppService : IDynamicApiController
    {




        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public async Task<RetObj> Register(UserDTO dto)
        {
            var ret = new RetObj();
            //var guid = IDGen.NextID();

            //查询用户是否存在
            //如果存在，提示用户已存在
            //否则
            //往用户表添加一条记录，  返回token
            
            //两次密码是否一致
            if (!dto.Password.Equals(dto.PasswordAgain))
            {
                return RetObj.Error("两次密码不一致");
            }

            var db = DbContext.Instance;

            var queryUser = await DbContext.Instance.Queryable<CoreUser>().FirstAsync(x => x.UserName == dto.UserEmail);

            if (queryUser != null)
            {
                return RetObj.Error("该账号已注册");
            }

            //注册
            queryUser = new CoreUser()
            {
                Id = IDGen.NextID().ToString(),
                UserName = dto.UserName,
                Password = dto.Password,
                Createdate=DateTime.Now
            };



            //var db2 = DbContext.Instance;
            ////用户是否存在
            //var queryUser=DbContext.Instance.Queryable<User>().FirstAsync(x=>x.)

            //var res = DbContext.Instance.get


            //// 生成 token
            //var accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>()
            //{
            //    { "UserId", dto. },  // 存储Id
            //    { "Account",user.Account }, // 存储用户名
            //});


            return ret;

            //return _systemService.GetDescription();
        }

    }
}