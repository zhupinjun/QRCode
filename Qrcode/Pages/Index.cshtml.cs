using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ThoughtWorks.QRCode.Codec;

namespace Qrcode.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public string aaa = "12333";

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(string text,string imgurl)
        {
            #region 参数获取
            //生成二维码的路径
            String data = text;
            //二维码中间 的logo
            //string imgurl = imgurl;
            #endregion

            #region
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            String encoding = "Byte";
            if (encoding == "Byte")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            }
            else if (encoding == "AlphaNumeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
            }
            else if (encoding == "Numeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
            }
            try
            {
                int scale = 8;
                qrCodeEncoder.QRCodeScale = scale;
            }
            catch (Exception ex)
            {
               
            }
            try
            {
                int version = Convert.ToInt16(0);
                qrCodeEncoder.QRCodeVersion = version;
            }
            catch (Exception ex)
            {
            }

            string errorCorrect = "H";// cboCorrectionLevel.Text;
            if (errorCorrect == "L")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            else if (errorCorrect == "M")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            else if (errorCorrect == "Q")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            else if (errorCorrect == "H")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
            #endregion
            #region 输出图片 new
            MemoryStream ms = new MemoryStream();
            //System.Drawing.Image test = CombinImage(qrCodeEncoder.Encode(data), imgurl);
            qrCodeEncoder.Encode(data).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] bytes = ms.ToArray();

            Response.Clear();
            Response.ContentType = "image/jpeg";
            bytes = Hy.Common.ImageOp.SmallPic(bytes, 200, 200, "jpg");

            ms = new MemoryStream(bytes);
            Bitmap bm = (Bitmap)System.Drawing.Image.FromStream(ms);
            System.Drawing.Image test = CombinImage(bm, imgurl);
            ms = new MemoryStream();
            test.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            bytes = ms.ToArray();
            ms.Close();


            //SetClientCaching();
            //Response.BinaryWrite(bytes);
            #endregion

        }
        //private void SetClientCaching()
        //{
        //    DateTime lastModified = DateTime.Now;

        //    Response.Cache.SetETag(lastModified.Ticks.ToString());
        //    Response.Cache.SetLastModified(lastModified);
        //    //public 以指定响应能由客户端和共享（代理）缓存进行缓存。
        //    Response.Cache.SetCacheability(HttpCacheability.Public);
        //    //是允许文档在被视为陈旧之前存在的最长绝对时间。
        //    Response.Cache.SetMaxAge(new TimeSpan(0, 24, 0, 0));
        //    //将缓存过期从绝对时间设置为可调时间
        //    Response.Cache.SetSlidingExpiration(true);
        //}
        /// <summary>
        /// 拼接二维码，二维码中间有logo
        /// </summary>
        /// <param name="imgBack"></param>
        /// <param name="destImg">logo 图片</param>
        /// <returns></returns>
        public static System.Drawing.Image CombinImage(System.Drawing.Image imgBack, string destImg)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destImg);
            WebResponse response = request.GetResponse();//获得响应
            Stream stram = response.GetResponseStream();
            System.Drawing.Image img = System.Drawing.Image.FromStream(response.GetResponseStream());

            //System.Drawing.Image img = System.Drawing.Image.FromFile(destImg); //照片图片 本地
            if (img.Height != 80 || img.Width != 80)
            {
                img = KiResizeImage(img, 40, 40, 0);
            }
            Graphics g = Graphics.FromImage(imgBack);
            g.DrawImage(imgBack, 0, 0, imgBack.Width, imgBack.Height); //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);
            //g.FillRectangle(System.Drawing.Brushes.White, imgBack.Width / 2 - img.Width / 2 - 1, imgBack.Width / 2 - img.Width / 2 - 1,1,1);//相片四周刷一层黑色边框
            //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);
            g.DrawImage(img, imgBack.Width / 2 - img.Width / 2, imgBack.Width / 2 - img.Width / 2, img.Width, img.Height);
            GC.Collect();

            return imgBack;
        }

        public static System.Drawing.Image KiResizeImage(System.Drawing.Image bmp, int newW, int newH, int Mode)
        {
            try
            {
                System.Drawing.Image b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }

        public static System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn == null)
                return null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArrayIn))
            {
                System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
                ms.Flush();
                return returnImage;
            }
        }
    }
}
