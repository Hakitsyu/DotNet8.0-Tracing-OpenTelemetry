using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Question.Application.Services.Abstractions
{
    public interface ITranslatorService
    {
        Task<string> TranslateAsync(string question, string language, CancellationToken cancellationToken = default);
    }
}
