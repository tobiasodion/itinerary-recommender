using System.Net;

namespace az_function{
    record RequestErrorModel(
    HttpStatusCode StatusCode,
    string ErrorMessage
);
}