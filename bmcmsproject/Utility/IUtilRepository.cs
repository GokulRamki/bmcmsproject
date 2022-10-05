using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace bmcmsproject.Utility
{
    public interface IUtilRepository : IDisposable
    {

        Bitmap ResizeImage(Stream streamImage, int resizeWidth, int resizeHeight);
    }


}