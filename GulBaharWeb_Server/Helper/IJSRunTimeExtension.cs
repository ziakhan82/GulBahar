using Microsoft.JSInterop;

namespace GulBaharWeb_Server.Helper
{
	public static class IJSRunTimeExtension
	{
		public static async ValueTask ToasterSuccess(this IJSRuntime jSRuntime, string message)
		{
			await jSRuntime.InvokeVoidAsync("ShowToastr", "success", message);
		}

		public static async ValueTask ToasterError(this IJSRuntime jSRuntime, string message)
		{
			await jSRuntime.InvokeVoidAsync("ShowToastr", "error", message);
		}
	}
}
