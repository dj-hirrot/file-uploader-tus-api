using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TusDotNetClient;

namespace FileUploader.Controllers
{
    [ApiController]
    public class BridgeFileToServerController : ControllerBase
    {
        // POST upload
        [Route("[action]")]
        [HttpPost]
        public async Task Upload(IFormFile file)
        {
            var filePath = SaveAndGetFilePath(file);

            var _file = new FileInfo(@filePath);
            var client = new TusClient();
            var fileUrl = await client.CreateAsync("http://localhost:3000/upload", _file.Length);
            var uploadOperation = client.UploadAsync(fileUrl, _file, 5D);

            uploadOperation.Progressed += (transferred, total) =>
                Console.WriteLine($"{((float)transferred / (float)total) * 100.00}%");

            await uploadOperation;
        }

        private string SaveAndGetFilePath(IFormFile file)
        {
            string filePath = Path.GetTempFileName();

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return filePath;
            }
            else
            {
                return null;
            }
        }
    }
}
