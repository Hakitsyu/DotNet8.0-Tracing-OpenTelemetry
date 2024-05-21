using Question.Application.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Question.Application.Services
{
    public sealed class TranslatorService(HttpClient client) : ITranslatorService
    {
        private const string Resource = "translate";

        public async Task<string> TranslateAsync(string question, string language, 
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(question))
                throw new ArgumentNullException(nameof(question));
            if (string.IsNullOrWhiteSpace(language)) 
                throw new ArgumentNullException(nameof(language));

            TranslateRequest request = new(question, language);
            var response = await client.PostAsJsonAsync(Resource, request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync(cancellationToken));

            return (await response.Content.ReadFromJsonAsync<TranslatedResponse>(cancellationToken))?.Question ?? question;
        }
    }

    internal record TranslateRequest(string Question, string Language);

    internal record TranslatedResponse(string Question);
}
