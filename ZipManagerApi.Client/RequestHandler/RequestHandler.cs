using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZipManagerApi.Client.Exceptions;

namespace ZipManagerApi.Client.RequestHandler;

public class RequestHandler(HttpClient _client, string _basePath) : IRequestHandler
{
    private async Task<HttpResponseMessage> MakeRequest(
        HttpMethod method,
        string path,
        object? body = null
    )
    {
        var message = new HttpRequestMessage(method, _basePath + path);
        if (body != null)
            message.Content = new StringContent(
                JsonSerializer.Serialize(body),
                System.Text.Encoding.UTF8,
                "application/json"
            );
        return await _client.SendAsync(message);
    }

    private async Task<string> GetContent(HttpResponseMessage? message)
    {
        if (message == null)
            throw new RequestException("No response from server");
        var content = await message.Content.ReadAsStringAsync();
        if (!message.IsSuccessStatusCode)
        {
            try
            {
                using var doc = JsonDocument.Parse(content);
                var detail = doc.RootElement.TryGetProperty("detail", out var value)
                    ? value.GetString() ?? "No detail"
                    : "No detail";
                throw new RequestException(detail);
            }
            catch (RequestException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new RequestException(message.ReasonPhrase ?? "Request failed");
            }
        }
        return content;
    }

    private static T Deserialize<T>(string content)
    {
        var res = JsonSerializer.Deserialize<T>(content);
        if (res == null)
            throw new RequestException("Serialization finished with failure");
        return res;
    }

    public async Task<List<string>> GetFiles()
    {
        var response = await MakeRequest(HttpMethod.Get, "/zip/files");
        var content = await GetContent(response);
        List<string> files = Deserialize<List<string>>(content);
        return files;
    }

    public async Task<string> GetStatus(Guid id)
    {
        var response = await MakeRequest(HttpMethod.Get, $"/zip/status?id={id}");
        var content = await GetContent(response);
        string status = Deserialize<string>(content);
        return status;
    }

    public async Task<Stream> DownloadArchive(Guid id)
    {
        var response = await MakeRequest(HttpMethod.Get, $"/zip?id={id}");
        return await response.Content.ReadAsStreamAsync();
    }

    public async Task<Guid> ZipFiles(List<string> fileNames)
    {
        var response = await MakeRequest(HttpMethod.Post, "/zip", fileNames);
        var content = await GetContent(response);
        var processId = Deserialize<Guid>(content);
        return processId;
    }
}
