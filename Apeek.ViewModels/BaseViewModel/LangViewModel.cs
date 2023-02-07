using System;
using System.Collections.Generic;
using Apeek.Common;
using Apeek.Common.Controllers;
using Apeek.Entities.Interfaces;
namespace Apeek.ViewModels.BaseViewModel
{
    public abstract class LangViewModel<TEntity, TLang>
        where TEntity : IEntity, new()
        where TLang : IEntityLang, new()
    {
        public IDictionary<Language, TLang> EntityLang { get; set; }
        public TEntity Entity { get; set; }
        protected abstract void SetEntity(TLang entityLang);
        protected LangViewModel()
        {
            Entity = new TEntity();
            EntityLang = InitLangDictionary();
        }
        protected void SetEntities(TEntity entity, TLang entityLang)
        {
            SetEntity(entity);
            SetLang(entityLang);
        }
        protected void SetEntities(TEntity entity, IEnumerable<TLang> entityLangs)
        {
            Entity = entity;
            foreach (var entityLang in entityLangs)
            {
                SetEntity(entityLang);
                EntityLang[(Language) entityLang.Lang_Id] = entityLang;
            }
        }
        private void SetLang(TLang entityLang)
        {
            if (entityLang == null || entityLang.Lang_Id == 0)
            {
                throw new ArgumentException("entityLang should be initialized");
            }
            SetEntity(entityLang);
            EntityLang[(Language)entityLang.Lang_Id] = entityLang;
        }
        private void SetEntity(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("entity should be initialized");
            }
            Entity = entity;
            foreach (var entityLang in EntityLang.Values)
            {
                SetEntity(entityLang);
            }
        }
        private Dictionary<Language, TLang> InitLangDictionary()
        {
            var langDictionary = new Dictionary<Language, TLang>();
            foreach (var language in LanguageController.GetAllLanguages())
            {
                langDictionary.Add(language, new TLang() { Lang_Id = (int)language });
                SetEntity(langDictionary[language]);
            }
            return langDictionary;
        }
        public static Dictionary<Language, TLang> CreateLangDictionary()
        {
            var langDictionary = new Dictionary<Language, TLang>();
            foreach (var language in LanguageController.GetAllLanguages())
            {
                langDictionary.Add(language, new TLang() { Lang_Id = (int)language });
            }
            return langDictionary;
        }
        public static Dictionary<Language, TLang> CreateLangDictionary(Language lang, TLang langEntity)
        {
            var langDictionary = new Dictionary<Language, TLang>();
            langDictionary.Add(lang, langEntity);
            foreach (var language in LanguageController.GetAllLanguages())
            {
                if (language != lang)
                {
                    var copyLangEntity = langEntity.CreateNewBaseOnThis((int)language);
                    langDictionary.Add(language, (TLang) copyLangEntity);
                }
            }
            return langDictionary;
        }
        /// <summary>
        /// Initialize an instance with all languages
        /// </summary>
        /// <param name="langRecords"></param>
        /// <param name="profession"></param>
        /// <returns></returns>
        public static T Initialize<T>(TEntity enity, Dictionary<Language, TLang> langRecords)
            where T : LangViewModel<TEntity, TLang>, new()
        {
            var langViewModel = new T();
            langViewModel.SetEntities(enity, langRecords.Values);
            return langViewModel;
        }
        /// <summary>
        /// Initialize an instance with all languages
        /// </summary>
        /// <param name="langRecords"></param>
        /// <param name="profession"></param>
        /// <returns></returns>
        public static T Initialize<T>(TEntity profession, List<TLang> langRecords)
            where T : LangViewModel<TEntity, TLang>, new()
        {
            var langViewModel = new T();
            langViewModel.SetEntities(profession, langRecords);
            return langViewModel;
        }
    }
}