using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TusDotNetClient;

namespace FileUploader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BridgeFileToServerController : ControllerBase
    {
        // POST api/upload
        [HttpPost("[action]")]
        public async Task UploadAsync([FromBody] IFormFile file)
        {
            var filePath = SaveAndGetFilePath(file);

            var _file = new FileInfo(@filePath);
            var client = new TusClient();
            var fileUrl = await client.CreateAsync("http://localhost:3000/upload", _file.Length);
            var uploadOperation = client.UploadAsync(fileUrl, _file, 5D);

            uploadOperation.Progressed += (transferred, total) =>
                Console.WriteLine($"Progress: {transferred}/{total}");

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
