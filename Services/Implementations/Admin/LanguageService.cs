using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Repositories.Interfaces.Admin;
using Online_Learning.Services.Interfaces;
using Online_Learning.Services.Interfaces.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online_Learning.Services.Implementations.Admin
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;
        public LanguageService(ILanguageRepository languageRepository)
        {
            _languageRepository = languageRepository;
        }

        public async Task<IEnumerable<Language>> GetActiveLanguagesAsync()
        {
            return await _languageRepository.GetActiveLanguagesAsync();
        }
    }
} 