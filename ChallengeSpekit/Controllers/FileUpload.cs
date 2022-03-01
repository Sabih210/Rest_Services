using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChallengeSpekit.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileUpload : ControllerBase
    {
        [HttpPost("upload-file")]
        public string  UploadFile([FromQuery] IFormFile file)
        {
            string _reslt = string.Empty;
            if (file.Length > 0)
            {
                // Do whatever you want with your file here
                // e.g.: upload it to somewhere like Azure blob or AWS S3
            }
            return _reslt;
            //TODO: Save file description and image URL etc to database.
        }
    }
}
