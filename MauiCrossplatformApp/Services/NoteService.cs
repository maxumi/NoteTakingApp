using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MauiCrossplatformApp.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
namespace MauiCrossplatformApp.Services
{

    public class NoteService : INoteService
    {
        private readonly HttpClient _client;
        JsonSerializerOptions _serializerOptions;
        private string _base_Uri = "http://10.0.2.2:5083/api/Note/";

        public NoteService()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        private Uri BuildUri(string relativePath = "")
        {
            var path = string.IsNullOrWhiteSpace(relativePath)
                ? _base_Uri
                : $"{_base_Uri.TrimEnd('/')}/{relativePath.TrimStart('/')}";
            return new Uri(path);
        }
        public async Task<List<FileSystemEntryDto>> GetTreeAsync()
        {
            var Items = new List<FileSystemEntryDto>();

            Uri uri = new Uri(_base_Uri+"tree");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Items = JsonSerializer.Deserialize<List<FileSystemEntryDto>>(content, _serializerOptions);
                }
                else
                {
                    var errorText = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"[LoadItems] ERROR BODY: {errorText}");
                    throw new HttpRequestException(
                        $"Server returned {(int)response.StatusCode} {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(uri);
                Debug.WriteLine($"\tERROR {0} {ex.Message}");
            }
            return Items;
        }

        public async Task<List<NoteDto>> GetAllNotesAsync()
        {
            var uri = BuildUri();
            Debug.WriteLine($"[NoteService] GET {uri}");

            try
            {
                using var response = await _client.GetAsync(uri).ConfigureAwait(false);
                Debug.WriteLine($"[NoteService] Response {(int)response.StatusCode} {response.ReasonPhrase}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return JsonSerializer.Deserialize<List<NoteDto>>(content, _serializerOptions)
                           ?? new List<NoteDto>();
                }
                else
                {
                    var errorText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Debug.WriteLine($"[NoteService] ERROR BODY: {errorText}");
                    throw new HttpRequestException(
                        $"Server returned {(int)response.StatusCode} {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NoteService] Exception fetching notes: {ex.Message}");
                return new List<NoteDto>();
            }
        }

        public async Task<NoteDto> GetNoteAsync(int id)
        {
            var uri = BuildUri(id.ToString());
            Debug.WriteLine($"[NoteService] GET {uri}");

            try
            {
                using var response = await _client.GetAsync(uri).ConfigureAwait(false);
                Debug.WriteLine($"[NoteService] Response {(int)response.StatusCode} {response.ReasonPhrase}");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonSerializer.Deserialize<NoteDto>(content, _serializerOptions)
                       ?? throw new JsonException("Failed to deserialize NoteDto");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NoteService] Exception fetching note {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<NoteDto> CreateNoteAsync(NoteDto dto)
        {
            var uri = BuildUri();
            Debug.WriteLine($"[NoteService] POST {uri}");

            try
            {
                using var response = await _client.PostAsJsonAsync(uri, dto, _serializerOptions).ConfigureAwait(false);
                Debug.WriteLine($"[NoteService] Response {(int)response.StatusCode} {response.ReasonPhrase}");

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonSerializer.Deserialize<NoteDto>(content, _serializerOptions)
                       ?? throw new JsonException("Failed to deserialize created NoteDto");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NoteService] Exception creating note: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateNoteAsync(int id, NoteDto dto)
        {
            var uri = BuildUri(id.ToString());
            Debug.WriteLine($"[NoteService] PUT {uri}");

            try
            {
                using var response = await _client.PutAsJsonAsync(uri, dto, _serializerOptions).ConfigureAwait(false);
                Debug.WriteLine($"[NoteService] Response {(int)response.StatusCode} {response.ReasonPhrase}");

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NoteService] Exception updating note {id}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteNoteAsync(int id)
        {
            var uri = BuildUri(id.ToString());
            Debug.WriteLine($"[NoteService] DELETE {uri}");

            try
            {
                using var response = await _client.DeleteAsync(uri).ConfigureAwait(false);
                Debug.WriteLine($"[NoteService] Response {(int)response.StatusCode} {response.ReasonPhrase}");

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NoteService] Exception deleting note {id}: {ex.Message}");
                throw;
            }
        }
    }
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Console.WriteLine($"→ Request: {request.Method} {request.RequestUri}");

            var response = await base.SendAsync(request, cancellationToken);

            Console.WriteLine($"← Response: {(int)response.StatusCode} {response.RequestMessage.RequestUri}");
            return response;
        }
    }
}
