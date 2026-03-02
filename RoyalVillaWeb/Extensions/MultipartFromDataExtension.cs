namespace RoyalVillaWeb.Extensions;

public static class MultipartFromDataExtension
{
	public static MultipartFormDataContent ToMultipartFormData(this object obj)
	{
		var formData = new MultipartFormDataContent();
		var properties = obj.GetType().GetProperties();

		foreach(var propertiy in properties)
		{
			var value = propertiy.GetValue(obj);
			if(value == null)
			{
				continue;
			}
			var properityName = propertiy.Name;

			if(value is IFormFile file && file.Length>0)
			{
				var streamContent = new StreamContent(file.OpenReadStream());
				streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
				formData.Add(streamContent, properityName, file.FileName);
			}
            else if (value is not IFormFile)
            {
                 formData.Add(new StringContent(value.ToString()!), properityName);
            }
        }
			return formData;
	}
}
