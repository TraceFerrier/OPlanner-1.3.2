using System.IO;
using System.Windows.Media.Imaging;

namespace PlannerNameSpace
{
    public class ImageAttachment : Attachment
    {
        public BitmapSource BitmapSource { get; set; }

        public ImageAttachment()
        {
        }
    }

}
