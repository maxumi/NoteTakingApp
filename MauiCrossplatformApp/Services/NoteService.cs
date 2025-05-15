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
            }
            catch (Exception ex)
            {
                Debug.WriteLine(uri);
                Debug.WriteLine($"\tERROR {0} {ex.Message}");
            }
            return Items;
            // GET https://…/api/note/tree
        }

        public async Task<List<NoteDto>> GetAllNotesAsync()
        {
            // GET https://…/api/note
            return await _client
                .GetFromJsonAsync<List<NoteDto>>("")
                .ConfigureAwait(false);
        }

        public async Task<NoteDto> GetNoteAsync(int id)
        {
            // GET https://…/api/note/{id}
            return await _client
                .GetFromJsonAsync<NoteDto>($"{id}")
                .ConfigureAwait(false);
        }

        public async Task<NoteDto> CreateNoteAsync(NoteDto dto)
        {
            var response = await _client
                .PostAsJsonAsync("", dto)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            return await response.Content
                                 .ReadFromJsonAsync<NoteDto>()
                                 .ConfigureAwait(false);
        }

        public async Task UpdateNoteAsync(int id, NoteDto dto)
        {
            // PUT https://…/api/note/{id}
            var response = await _client
                .PutAsJsonAsync($"{id}", dto)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteNoteAsync(int id)
        {
            var response = await _client
                .DeleteAsync($"{id}")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
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
