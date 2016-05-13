using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GRA.SRP.ControlRoom.Handlers
{
    /// <summary>
    /// Summary description for QRCode
    /// </summary>
    public class QRCode : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var code = context.Request.QueryString["code"];
            if (string.IsNullOrEmpty(code))
            {
                return;
            }

            var url = string.Format("{0}?SecretCode={1}",
                GRA.Tools.WebTools.GetBaseUrl(context.Request),
                code);

            int size = 200;
            var sizeString = context.Request.QueryString["size"];
            if (!string.IsNullOrEmpty(sizeString))
            {
                int.TryParse(sizeString, out size);
            }

            var barcodeWriter = new ZXing.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = size,
                    Width = size
                }
            };
            using (var bitmap = barcodeWriter.Write(url))
            {
                context.Response.ContentType = "image/png";
                bitmap.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}