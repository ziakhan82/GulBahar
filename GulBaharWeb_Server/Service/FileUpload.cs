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
			if(File.Exists(_webHostEnviroment.WebRootPath+filePath)) // if the path exist
			{
				File.Delete(_webHostEnviroment.WebRootPath+ filePath);
				return true;
			}
			return false;
		}

		public async Task<string> UploadFile(IBrowserFile file)
		{
			FileInfo fileInfo = new(file.Name); 
			var fileName =Guid.NewGuid().ToString() +fileInfo.Extension;// file name will be new name that we want to give to the uploaded file
			var folderDirectory = $"{_webHostEnviroment.WebRootPath}\\images\\Product";

			if (!Directory.Exists(folderDirectory)) // if folder directory doesnt exist.
			{
				Directory.CreateDirectory(folderDirectory); // create a directory
			}
			var filePath = Path.Combine(folderDirectory, fileName); // if exist upload the file
			await using FileStream fs = new FileStream(filePath, FileMode.Create);
			await file.OpenReadStream().CopyToAsync(fs);// file will be pasted in the location(Fs)

			var fullPath = $"/images/Product/{fileName}";
			return fullPath; // return the location


		}
	}
}
