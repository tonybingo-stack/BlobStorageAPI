using BlobStorageAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace BlobStorageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureCRUDController : ControllerBase
    {
        private readonly IBlobStorage _storage;
        private readonly string _connectionString;
        private readonly IMapper _mapper;
        //private readonly string _container;

        public AzureCRUDController(IBlobStorage storage, IConfiguration iConfig)
        {
            _storage = storage;
            _connectionString = iConfig.GetValue<string>("MyConfig:StorageConnection");
            //_container = iConfig.GetValue<string>("MyConfig:ContainerName");
        }

        [Route("createContainer")]
        [HttpPost]
        public async Task<bool> CreateUserContainer(createContainerDto containerDto)
        {
            var name = containerDto.containerName;
            return await _storage.CreateContainerForUser(_connectionString, name);
        }

        [Route("deleteUserContainer")]
        [HttpDelete]
        public async Task<bool> deleteUserContainer(string containerName)
        {
            return await _storage.DeleteContainerForUser(_connectionString, containerName);
        }

        [HttpGet("ListFiles")]
        public async Task<List<string>> ListFiles(string containerName)
        {
            return await _storage.GetAllDocuments(_connectionString, containerName);
        }

        [HttpGet("GetFileByName")]
        public async Task<bool> GetFileByName(string fileName)
        {
            await _storage.GetDocument(_connectionString, "userprofilephoto", fileName);
            return true;
        }

        [Route("InsertFile")]
        [HttpPost]
        public async Task<Uri> InsertFile([FromForm(Name="file")]IFormFile asset)
        {

            //if (asset != null)
            //{
                Stream stream = asset.OpenReadStream();
                await _storage.UploadDocument(_connectionString, "userprofilephoto", asset.FileName, stream);
                var content = await _storage.GetDocument(_connectionString, "userprofilephoto", asset.FileName);
                return content;
            //}

        }

        //[HttpGet("DownloadFile")]
        //public async Task<IActionResult> DownloadFile(string fileName)
        //{
        //    var content = await _storage.GetDocument(_connectionString, "userprofilephoto", fileName);
        //    return File(content, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        //}

        [HttpGet("getFileUrl")]
        public async Task<Uri> GetURL(string fileName)
        {
            var content = await _storage.GetDocument(_connectionString, "userprofilephoto", fileName);
            return content;
        }

        [Route("DeleteFile")]
        [HttpDelete]
        public async Task<bool> DeleteFile(string fileName, string containerName)
        {
            return await _storage.DeleteDocument(_connectionString, containerName, fileName);
        }

    }
}
