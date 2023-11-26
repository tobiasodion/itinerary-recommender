using System.IO;
using Newtonsoft.Json;

namespace az_function
{
    public record PdfContent
    (
        byte[] Content
    );
}