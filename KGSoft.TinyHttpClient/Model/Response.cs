using System.Net;

namespace KGSoft.TinyHttpClient
{
    public class Response<T> : Response
    {
        public T Result { get; set; }
    }

    public class Response
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
