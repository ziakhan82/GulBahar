using GulBaharWeb_Server.Service.IService;
using Microsoft.AspNetCore.Components.Forms;


namespace GulBaharWeb_Server.Service
{
	public class FileUpload : IFileUpload
	{
		private readonly IWebHostEnvironment _webHostEnviroment;
        public FileUpload(IWebHostEnvironment webHostEnviroment)
		{
			_webHostEnviroment = webHostEnviroment;
		}

		public bool DeleteFile(string filePath)
		{
			if(File.Exists(_webHostEnviroment.WebRootPath+filePath))
			{
				File.Delete(_webHostEnviroment.WebRootPath+ filePath);
				return true;
			}
			return false;
		}

		public async Task<string> UploadFile(IBrowserFile file)
		{
			FileInfo fileInfo = new(file.Name);
			var fileName =Guid.NewGuid().ToString() +fileInfo.Extension;
			var folderDirectory = $"{_webHostEnviroment.WebRootPath}\\images\\Product";

			if (!Directory.Exists(folderDirectory))
			{
				Directory.CreateDirectory(folderDirectory);
			}
			var filePath = Path.Combine(folderDirectory, fileName);
			await using FileStream fs = new FileStream(filePath, FileMode.Create);
			await file.OpenReadStream().CopyToAsync(fs);

			var fullPath = $"/images/Product/{fileName}";
			return fullPath;


		}
	}
}
