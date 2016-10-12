using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace WpfSettings.Utils
{
    internal static class BitmapUtils
    {
        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            if (bitmap == null)
                return null;
            IntPtr hbitmap = bitmap.GetHbitmap();
            BitmapSource source;
            try
            {
                source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hbitmap,
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(16, 16));
            }
            finally
            {
                DeleteObject(hbitmap);
            }
            return source;
        }

    }
}
