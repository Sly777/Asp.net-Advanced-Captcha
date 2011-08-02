using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Drawing.Imaging;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class CaptchaImg : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int captchaWidth = (!int.TryParse(Request.QueryString["w"], out captchaWidth)) ? 175 : int.Parse(Request.QueryString["w"]);
        int captchaHeight = (!int.TryParse(Request.QueryString["h"], out captchaHeight)) ? 35 : int.Parse(Request.QueryString["h"]);

        Captcha.caseSensitive = false;
        Captcha ci = new Captcha(captchaWidth, captchaHeight);

        Response.Clear();
        Response.ContentType = "image/jpeg";

        ci.Image.Save(Response.OutputStream, ImageFormat.Jpeg);
        ci.Dispose();
    }
}
