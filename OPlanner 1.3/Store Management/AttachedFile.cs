using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class FileAttachment
    {
        public FileAttachment(ProductStudio.File psFile)
        {
            PSFile = psFile;
        }

        private ProductStudio.File PSFile { get; set; }
        public string FileName { get { return PSFile.FileName; } }
        public string FileType { get { return "Attachment"; } }
        public DateTime DateAdded { get { return PSFile.DateAdded; } }
    }
}
