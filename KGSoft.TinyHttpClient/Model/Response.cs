using System.Net;

namespace KGSoft.TinyHttpClient
{
    public class Response<T> : Response
    {
        public T Result { get; set; }
    }

    public class Response
    {
        /// <summary>
        /// HTTP Status Code of the response
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Depicts whether or not the request was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// The response message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The byte[] content of the response
        /// </summary>
        public byte[] Content { get; set; }
    }
}
