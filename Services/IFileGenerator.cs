using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace az_function
{
    public interface IFileGenerator
    {
        Task<byte[]> GenerateFileStream(string text, ILogger log);
    }
}