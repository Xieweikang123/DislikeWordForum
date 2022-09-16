using BackendAPI.Application.User;
using BackendAPI.Core;
using BackendAPI.Core.Entities;
using Furion.DistributedIDGenerator;

namespace BackendAPI.Application
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    /// 
    public class UserAppService : IDynamicApiController
    {


        public async Task<RetObj> Test()
        {
            return RetObj.Success(null, "测试");
        }


        /// <summary>
        /// 获取个人信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<RetObj> GetUserInfo()
        {
             
            var userId = CurrentUserInfo.UserId;
            


            var db = DbContext.Instance;
            //var queryUser = await DbContext.Instance.Queryable<CoreUser>().FirstAsync(x => x.UserName == dto.UserName && x.Password == dto.Password);
            //if (queryUser == null)
            //{
            //    throw new Exception("用户不存在或者密码错误");

            //}
            //// 生成 token
            //queryUser.Token = JWTEncryption.Encrypt(new Dictionary<string, object>()
            //{
            //    { "UserId", queryUser.Id },  // 存储Id
            //    { "Account",queryUser.UserName }, // 存储用户名
            //});

            //await db.Updateable(queryUser).ExecuteCommandAsync();

            return RetObj.Success("登录成功");
        }

        /// <summary>
        /// 登录成功
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<RetObj> Login(UserDTO dto)
        {

            var db = DbContext.Instance;
            var queryUser = await DbContext.Instance.Queryable<CoreUser>().FirstAsync(x => x.UserName == dto.UserName && x.Password == dto.Password);
            if (queryUser == null)
            {
                throw new Exception("用户不存在或者密码错误");

            }
            // 生成 token
            queryUser.Token = JWTEncryption.Encrypt(new Dictionary<string, object>()
            {
                { "UserId", queryUser.Id },  // 存储Id
                { "Account",queryUser.UserName }, // 存储用户名
            });

            await db.Updateable(queryUser).ExecuteCommandAsync();

            return RetObj.Success(queryUser.Token, "登录成功");
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
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
                throw new Exception("两次密码不一致");
            }

            var db = DbContext.Instance;

            var queryUser = await DbContext.Instance.Queryable<CoreUser>().FirstAsync(x => x.UserName == dto.UserName);

            if (queryUser != null)
            {
                throw new Exception("该账号已注册");
            }

            //注册
            queryUser = new CoreUser()
            {
                Id = IDGen.NextID().ToString(),
                UserName = dto.UserName,
                Password = dto.Password,
                Createdate = DateTime.Now
            };



            // 生成 token
            queryUser.Token = JWTEncryption.Encrypt(new Dictionary<string, object>()
            {
                { "UserId", queryUser.Id },  // 存储Id
                { "Account",queryUser.UserName }, // 存储用户名
            });

            //插入数据库
            var icount = await db.Insertable(queryUser).ExecuteCommandAsync();
            return RetObj.Success(queryUser.Token, "注册成功");
        }

    }
}