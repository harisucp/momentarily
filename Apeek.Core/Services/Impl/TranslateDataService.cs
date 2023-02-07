using System.Collections.Generic;
using System.Linq;
using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Common;
namespace Apeek.Core.Services.Impl
{
    public interface ITranslateDataService : IDependency
    {
        List<TranslateCase> GetTranslateCases();
        List<Translate> GetTranslates();
    }
    public class TranslateDataService : ITranslateDataService
    {
        private readonly IRepository<Translate> _repTranslate;
        private readonly IRepository<TranslateCase> _repTranslateCase;
        public TranslateDataService(IRepository<Translate> repTranslate, IRepository<TranslateCase> repTranslateCase)
        {
            _repTranslate = repTranslate;
            _repTranslateCase = repTranslateCase;
        }
        public List<Translate> GetTranslates()
        {
            var translates = new List<Translate>();
            Uow.Wrap(u =>
            {
                translates = _repTranslate.Table.ToList();
            });
            return translates;
        }
        public List<TranslateCase> GetTranslateCases()
        {
            var translateCases = new List<TranslateCase>();
            Uow.Wrap(u =>
            {
                translateCases = _repTranslateCase.Table.ToList();
            });
            return translateCases;
        }
    }
}
