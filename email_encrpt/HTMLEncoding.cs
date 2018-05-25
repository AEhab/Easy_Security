using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
namespace email_encrpt
{
    class HTMLEncoding
    {
        public static string TextToHTML(string text)
        {
            string ret = HttpUtility.HtmlEncode(text);
            ret = ret.Replace("\r\n", "\r");
            ret = ret.Replace("\n", "\r");
            ret = ret.Replace("\r", "<br>\r\n");
            ret = ret.Replace(" ", " &nbsp;");
            return ret;
        }

    }
}
