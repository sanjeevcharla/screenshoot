using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static ScreenShoot.Util.Constants;

namespace ScreenShoot.Util
{
    public static class Extensions
    {
        public static ListViewItem FirstSelectedItem(this ListView listView)
        {
            return listView.SelectedItems != null
                ? listView.SelectedItems[0]
                : null;
        }
        public static bool HasValidFileName(this ListViewItem listViewItem)
        {
            return listViewItem.Tag != null
                && !String.IsNullOrEmpty(listViewItem.Tag.ToString())
                && File.Exists(listViewItem.Tag.ToString());
        }
        public static string FilePath(this ListViewItem listViewItem)
        {
            return listViewItem.Tag.ToString();
        }

        public static Image PathToImage(this string imageFilePath)
        {
            if (!string.IsNullOrEmpty(imageFilePath) && File.Exists(imageFilePath))
            {
                using (FileStream stream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
                    return (Image)(new Bitmap(Image.FromStream(stream), IMAGE_SIZE, IMAGE_SIZE));
            }
            return null;
        }

        public static string NameFormat(this DateTime dateTime)
        {
            return dateTime.ToString("dd_MM_yyyy_hh_mm_ss_tt_FFFFF");
        }
    }
}