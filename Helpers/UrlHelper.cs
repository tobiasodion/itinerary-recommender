using System;

namespace az_function
{
    public static class UrlHelper
    {
        public static string GetFullUrl(string baseUrl, string path)
        {
            Uri baseUri = new Uri(baseUrl);
            Uri fullUri = new Uri(baseUri, path);

            // Get the full URL as a string
            return fullUri.ToString();
        }
    }
}