using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZeroTouchHR.Services.Interfaces
{
    public interface IS3Service
    {
         Task UploadFile(System.IO.Stream inputStream, string fileName, string folderName);
    }
}
