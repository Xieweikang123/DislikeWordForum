using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace BackendAPI.Core
{
    public class ImageHelper
    {


        //base64编码的字符串转为图片 
        public static Image Base64StringToImage(string strbase64)
        {

            //byte[] arr = Convert.FromBase64String(strbase64);
            //MemoryStream ms = new MemoryStream(arr);
            //Bitmap bmp = new Bitmap(ms);
            strbase64 = strbase64.Replace("data:image/png;base64,", "");

            byte[] imageBytes = Convert.FromBase64String(strbase64);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            return image;


            //var base64 = "";
            //base64 = base64.Replace("data:image/png;base64,", "").Replace("data:image/jgp;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");//将base64头部信息替换
            //byte[] bytes = Convert.FromBase64String(base64);
            //MemoryStream memStream = new MemoryStream(bytes);
            //Image mImage = Image.FromStream(memStream);

            //return mImage;


            //byte[] bytes = Convert.FromBase64String(strbase64);

            //using (MemoryStream ms = new MemoryStream(bytes))
            //{
            //    Image pic = Image.FromStream(ms);

            //    pic.Save(DefaultImagePath);
            //}

            //return bmp;

        }

    }
}
