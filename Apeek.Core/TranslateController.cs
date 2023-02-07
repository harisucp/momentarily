using System;
using System.Collections.Generic;
using Apeek.Core.Interfaces;
using Apeek.Core.Services.Impl;
using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.Entities.Entities;
using Apeek.Entities.Validators;
namespace Apeek.Core
{
    public class TranslateController : ITranslateController
    {
        private readonly ITranslateDataService _translateDataService;
        private Dictionary<Language, Dictionary<string, string>> _dictionary;
        private Dictionary<Language, Dictionary<string, Dictionary<Cases,string>>> _dictionaryCases;
        public TranslateController(ITranslateDataService translateDataService)
        {
            _translateDataService = translateDataService;
            Refresh();
        }
        public void Refresh()
        {
            _dictionary = new Dictionary<Language, Dictionary<string, string>>();
            _dictionaryCases = new Dictionary<Language, Dictionary<string, Dictionary<Cases, string>>>();
            foreach (var lang in LanguageController.GetAllLanguages())
            {
                _dictionary.Add(lang, new Dictionary<string, string>());
                _dictionaryCases.Add(lang, new Dictionary<string, Dictionary<Cases, string>>());
            }
            InitializeCases(_translateDataService.GetTranslateCases());
            InitializeContent(_translateDataService.GetTranslates());
            InitializeValidationMessages();
        }
        #region Private Methods
        private void AddCase(Language language, string valueOriginal, Cases c, string valueCase)
        {
            if (!_dictionaryCases[language].ContainsKey(valueOriginal))
            {
                _dictionaryCases[language].Add(valueOriginal, Case(c, valueCase));
            }
            else
            {
                _dictionaryCases[language][valueOriginal].Add(c, valueCase);
            }
        }
        private Dictionary<Cases, string> Case(Cases c, string valueCase)
        {
            return new Dictionary<Cases, string> { { c, valueCase } };
        }
        #endregion Private Methods
        #region TranslateMethods
        public string TranslateCase(string originalValue, Cases c)
        {
            if (_dictionaryCases[LanguageController.CurLang].ContainsKey(originalValue))
            {
                if (_dictionaryCases[LanguageController.CurLang][originalValue].ContainsKey(c))
                    return _dictionaryCases[LanguageController.CurLang][originalValue][c];
            }
            return originalValue;
        }
        public string this[string key]
        {
            get
            {
                if (_dictionary[LanguageController.CurLang].ContainsKey(key))
                    return _dictionary[LanguageController.CurLang][key];
                return key;
            }
        }
        #endregion TranslateMethods
        #region Translation
        private void InitializeCases(List<TranslateCase> translateCases)
        {
            foreach (var translateCase in translateCases)
            {
                AddCase((Language)translateCase.LangId, translateCase.Key, (Cases)Enum.Parse(typeof(Cases), translateCase.Case, true), translateCase.Value);
            }
        }
        private void InitializeValidationMessages()
        {
            _dictionary[Language.uk].Add(ValidationErrors.FillInAge, "Введіть вік");
            _dictionary[Language.uk].Add(ValidationErrors.SelectCity, "Виберіть місто");
            _dictionary[Language.uk].Add(ValidationErrors.FillInFullName, "Введіть Ваше повне ім\'я");
        }
        private void InitializeContent(List<Translate> translates)
        {
            foreach (var translate in translates)
            {
                _dictionary[(Language)translate.LangId].Add(translate.Key, translate.Value);
            }
        }
        #endregion Translation
    }
}
