using System.Globalization;
using Castle.Core.Resource;

namespace az_function
{
    public static class StringHelper
    {
        public static string CapitalizeFirstLetter(string input)
        {
            // Create a TextInfo object to apply the ToTitleCase method
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            // Use ToTitleCase to capitalize the first letter and lowercase the rest
            return textInfo.ToTitleCase(input.ToLower());
        }
    }
}