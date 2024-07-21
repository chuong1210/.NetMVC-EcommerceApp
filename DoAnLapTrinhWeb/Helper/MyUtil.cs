using System.Text;

namespace DoAnLapTrinhWeb.Helper
{
	public class MyUtil 

	{
		public static string GenerateRandomKey(int length=5) 
		{
			var pattern = @"sasafaaruyuidah73y49jkadkahopaicnlnlvoah390@(!3q!@";
			var sb = new StringBuilder();
			var rd = new Random();
			for (int i = 0; i < length; i++)
			{
				sb.Append(pattern[rd.Next(0, pattern.Length)]);
			}
			return sb.ToString(); // Add parentheses to call the ToString method
		
		}

		public static string UploadImage(IFormFile urlImage, string folder)
		{
			try
			{
				var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, urlImage.FileName);
				using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
				{
					urlImage.CopyTo(myfile);
				}
				return urlImage.FileName;
			}
			catch (Exception ex)
			{
				return string.Empty;
			}
		}
	}
}
