using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KGSoft.TinyHttpClient.Test;

internal sealed class TestHttpServer
{
    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public TestHttpServer()
    {
        _listener = new TcpListener(IPAddress.Loopback, 0);
        _listener.Start();

        var endpoint = (IPEndPoint)_listener.LocalEndpoint;
        BaseUrl = $"http://127.0.0.1:{endpoint.Port}/";

        _ = Task.Run(AcceptRequests);
    }

    public string BaseUrl { get; }

    private async Task AcceptRequests()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync(_cancellationTokenSource.Token);
                _ = Task.Run(() => HandleRequest(client));
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                // Test server failures surface as failed HTTP tests.
            }
        }
    }

    private static async Task HandleRequest(TcpClient client)
    {
        using (client)
        {
            var stream = client.GetStream();
            var buffer = new byte[8192];
            var bytesRead = await stream.ReadAsync(buffer);
            var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            var requestLine = request.Split("\r\n", StringSplitOptions.None)[0];
            var parts = requestLine.Split(' ');
            var path = parts.Length > 1 ? parts[1] : "/";

            var response = BuildResponse(path);
            await stream.WriteAsync(response);
        }
    }

    private static byte[] BuildResponse(string path)
    {
        var pathWithoutQuery = path.Split('?')[0];

        return pathWithoutQuery switch
        {
            "/api/users/2" => BuildHttpResponse(200, UserJson),
            "/api/users" => BuildHttpResponse(200, UserJson),
            "/api/unknown/23" => BuildHttpResponse(404, "{}"),
            "/api/register" => BuildHttpResponse(400, "{\"error\":\"Missing password\"}"),
            "/unauthorized" => BuildHttpResponse(401, "{\"error\":\"Unauthorized\"}"),
            _ => BuildHttpResponse(404, "{}")
        };
    }

    private static byte[] BuildHttpResponse(int statusCode, string body)
    {
        var reasonPhrase = statusCode switch
        {
            200 => "OK",
            400 => "Bad Request",
            401 => "Unauthorized",
            404 => "Not Found",
            _ => "OK"
        };

        var bodyBytes = Encoding.UTF8.GetBytes(body);
        var headers = string.Join("\r\n",
            $"HTTP/1.1 {statusCode} {reasonPhrase}",
            "Content-Type: application/json; charset=utf-8",
            $"Content-Length: {bodyBytes.Length}",
            "Connection: close",
            string.Empty,
            string.Empty);

        var headerBytes = Encoding.ASCII.GetBytes(headers);
        var response = new byte[headerBytes.Length + bodyBytes.Length];
        Buffer.BlockCopy(headerBytes, 0, response, 0, headerBytes.Length);
        Buffer.BlockCopy(bodyBytes, 0, response, headerBytes.Length, bodyBytes.Length);

        return response;
    }

    private const string UserJson = "{\"data\":{\"id\":2,\"first_name\":\"Chewbacca\",\"last_name\":\"Solo\"}}";
}
