using Microsoft.Net.Http.Headers;

namespace AutoDocApi.Endpoints;
public static class MultipartRequestHelper
	{
    	// get the boundary information, for above exmaple would be: --------------------------156313382635509050530525
		public static string GetBoundary(MediaTypeHeaderValue contentType)
		{
			var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

			if (string.IsNullOrWhiteSpace(boundary))
			{
				throw new InvalidDataException("Missing content-type boundary.");
			}
			return boundary;
		}
    
    	// validate if it was multipart form data
		public static bool IsMultipartContentType(string contentType)
		{
			return !string.IsNullOrEmpty(contentType)
				   && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
		}
	}