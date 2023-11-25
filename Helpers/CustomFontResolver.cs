using System;
using System.IO;
using System.Reflection;
using PdfSharp.Fonts;

namespace az_function
{
    public class CustomFontResolver : IFontResolver
    {
        public byte[] GetFont(string faceName)
        {
            // Load the font file based on faceName
            // For simplicity, this example uses a basic implementation for Arial

            if (faceName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
            {
                string resourceName = "az_function.Fonts.Arial.ttf";
                byte[] fontBytes = LoadFontResource(resourceName);
                // Load the Arial font file
                return fontBytes;
            }

            // If the requested font is not found, return null or throw an exception
            return null;
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            // Resolve the typeface based on familyName, isBold, and isItalic
            // For simplicity, this example assumes all fonts are regular (not bold or italic)
            return new FontResolverInfo("Arial");
        }

        private byte[] LoadFontResource(string resourceName)
        {
            // Load the font resource from the assembly
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}