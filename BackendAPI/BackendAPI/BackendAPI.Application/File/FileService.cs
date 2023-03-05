using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendAPI.Application.File
{


    /// <summary>
    /// 文件上传服务
    /// </summary>
    public class FileService : IDynamicApiController
    {

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="suffix"></param>
        /// <param name="relativePath">相对路径</param>
        /// <returns></returns>
        public static string GetCurrentFilePathName(string suffix,out string relativePath)
        {
            var fileDir = App.WebHostEnvironment.WebRootPath + "/Files";
            //string suffix = Path.GetExtension(file.FileName);

            string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}{suffix}";
            relativePath = "Files/" + fileName;

            return fileDir + "/" + fileName;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<object> UploadImg(IFormFile file)
        {
            //var files = Request.Form.Files.FirstOrDefault();
            if (file == null)
            {
                throw new Exception("请上传文件");
            }

            #region  图片文件的条件判断
            //文件后缀
            var fileExtension = Path.GetExtension(file.FileName);

            //判断后缀是否是图片
            const string fileFilt = ".jpg|.jpeg|.png";
            if (fileExtension == null)
            {
                throw new Exception("上传图片格式不正确");

            }
            if (fileFilt.IndexOf(fileExtension.ToLower(), StringComparison.Ordinal) <= -1)
            {
                throw new Exception("请上传jpg、png格式的图片");
            }

            //判断文件大小    
            long length = file.Length;
            if (length > 1024 * 1024 * 5) //5M
            {
                throw new Exception("上传的文件不能大于5M");
            }

            #endregion
            //var fileDir = App.Configuration["OssConfig:UploadFileAddress"];
            var fileDir = App.WebHostEnvironment.WebRootPath + "/Files";
            string suffix = Path.GetExtension(file.FileName);

            string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}{suffix}";
            //string dir = Path.GetDirectoryName(ossConfig.UploadFileAddress);
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }
            using (FileStream fs = new FileStream(fileDir + "/" + fileName, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            string url = $"Files/{fileName}";
            var res = new
            {
                name = fileName,
                status = "done",
                thumbUrl = url,
                url = url
            };

            return res;
        }
    }
}
