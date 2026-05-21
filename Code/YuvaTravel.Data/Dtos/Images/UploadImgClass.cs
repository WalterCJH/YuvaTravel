using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Data.Dtos.Images
{
    public class UploadImg
    {
        public int uploaded { get; set; }
        public string fileName { get; set; }
        public string url { get; set; }
        public UploadImgMsg error { get; set; }
    }
}
