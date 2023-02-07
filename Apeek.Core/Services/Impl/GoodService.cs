using System;
using System.Collections.Generic;
using System.Linq;
using Apeek.Common;
using Apeek.Common.Definitions;
using Apeek.Common.Extensions;
using Apeek.Common.Interfaces;
using Apeek.Common.Logger;
using Apeek.Common.Models;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Common;
using Apeek.NH.Repository.Repositories;
using Apeek.ViewModels.Models;
namespace Apeek.Core.Services.Impl
{
    public class GoodService<T, U> : IGoodService<T, U> where T : Good where U : class
    {
        protected readonly IRepositoryGood<T, U> _repGood;
        protected readonly IRepositoryGoodPropertyValue _repGoodPropertyValue;
        protected List<GoodProperty> _goodProperty;
        protected readonly IRepositoryUserGood _repUserGood;
        protected readonly IRepositoryGoodCategory _repGoodCategory;
        protected readonly IRepositoryCategory _repositoryCategory;
        protected readonly IImageDataService _imageDataService;
        protected readonly IRepositoryGoodLocation _repGoodLocation;
        protected readonly IRepositoryAudit<GoodStartEndDate> _repStartEndDate;
        protected readonly IGoodShareDateRepository _goodShareDateRepository;
        protected readonly IRepositoryGoodStartDateEndDate _repositoryGoodStartDateEndDate;
        protected readonly IRepositoryGoodImg _repGoodImg;
        public GoodService(IRepositoryGood<T, U> repGood, IRepositoryGoodPropertyValue repGoodPropertyValue,
            IRepositoryUserGood repUserGood, IRepositoryGoodCategory repGoodCategory, IRepositoryCategory repositoryCategory, IImageDataService imageDataService,
            IRepositoryGoodLocation repGoodLocation, IRepositoryAudit<GoodStartEndDate> repStartEndDate,
            IGoodShareDateRepository goodShareDateRepository,
              IRepositoryGoodStartDateEndDate repositoryGoodStartDateEndDate,
                 IRepositoryGoodImg repGoodImg
            )
        {
            if (!RepCache<GoodProperty>.Loaded)
            {
                _goodProperty = new List<GoodProperty>();
                Uow.Wrap(u =>
                {
                    _goodProperty = new RepCache<GoodProperty>().List();
                });
            }
            else
            {
                _goodProperty = new RepCache<GoodProperty>().List();
            }
            _repGood = repGood;
            _repGoodPropertyValue = repGoodPropertyValue;
            _repUserGood = repUserGood;
            _repGoodCategory = repGoodCategory;
            _repositoryCategory = repositoryCategory;
            _imageDataService = imageDataService;
            _repGoodLocation = repGoodLocation;
            _repStartEndDate = repStartEndDate;
            _goodShareDateRepository = goodShareDateRepository;
            _repositoryGoodStartDateEndDate = repositoryGoodStartDateEndDate;
            _repGoodImg = repGoodImg;
        }
        public Result<T> GetMyGood(int userId, int goodId)
        {
            var result = new Result<T>(CreateResult.Error, null);
            Uow.Wrap(r =>
            {
                var userGood = _repUserGood.Table.FirstOrDefault(p => p.GoodId == goodId);
                if (userGood != null && userGood.UserId == userId)
                {
                    var good = _repGood.Get(goodId);
                    good.GoodPropertyValues = _repGoodPropertyValue.GetGoodProperties(goodId);
                    good.Images = good.GoodImages.Where(p => p.Type == (int)ImageType.Original).Select(p => new UserImageModel
                    {
                        Id = p.Id,
                        UserId = userId,
                        FileName = p.FileName,
                        ImgFolder = ImageFolder.Good,
                        GoodId = p.GoodId,
                        Url = p.ImageUrl(ImageFolder.Good.ToString()),
                        Sequence = p.Sequence
                    })
                    .ToList();
                    good.GoodShareDates = _goodShareDateRepository.Table.Where(x => x.GoodId == good.Id)
                                            .Select(x => x.ShareDate.ToJavaScriptMilliseconds())
                                            .ToList();

                    var startTime = _goodShareDateRepository.Table.Where(x => x.GoodId == good.Id)                                            .Select(x => x.StartTime).FirstOrDefault();                    var endTime = _goodShareDateRepository.Table.Where(x => x.GoodId == good.Id)                                .Select(x => x.EndTime).FirstOrDefault();                    if (startTime != null)                    {                        good.StartTime = startTime;                    }                    else                    {                        good.StartTime = "09:00 AM";                    }                    if (endTime != null)                    {                        good.EndTime = endTime;                    }                    else                    {                        good.EndTime = "06:00 AM";                    }

                    good.CategorList = _repGoodCategory.GetGoodCategorylist(goodId);
                    good.CategoryId = _repGoodCategory.GetGoodCategoryId(goodId);

                    result.Obj = good;
                    result.CreateResult = CreateResult.Success;
                }
            },
            null,
            LogSource.GoodService);
            return result;
        }
        private void SaveOrUpdateGood(T good, int userId)
        {
            _repGood.SaveOrUpdateAudit(good, userId);
        }
        private void SaveOrUpdateGoodPropertyValues(T good, int userId)
        {
            string errorMessage = "";
            foreach (var gpv in good.GoodPropertyValues)
            {
                var propertyValue = gpv.Value;
                propertyValue.GoodId = good.Id;
                try
                {
                    if (propertyValue.GoodPropertyId == 0)
                    {
                        propertyValue.GoodPropertyId = _goodProperty.Single(p => p.Name == gpv.Key).Id;
                    }
                }
                catch (InvalidOperationException)
                {
                    errorMessage += string.Format("Property {0} was not found or has duplicate ", gpv.Key) + Environment.NewLine;
                }
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new Exception(errorMessage);
            }
            _repGoodPropertyValue.SaveOrUpdateAudit(good.GoodPropertyValues.Values.ToList(), userId);
        }
        private void CreateUserGood(T good, int userId)
        {
            _repUserGood.SaveOrUpdateAudit(new UserGood
            {
                UserId = userId,
                GoodId = good.Id
            }, userId);

            string[] values = good.CategorList.Split(',').Select(sValue => sValue.Trim()).ToArray();
            foreach (var item in values)
            {
                int categoryId = 0;
                int.TryParse(item, out categoryId);
                _repGoodCategory.SaveOrUpdateAudit(new GoodCategory
                {
                    GoodId = good.Id,
                    CategoryId = categoryId
                }, userId);
            }

        }
        private void UpdateGoodCategory(T good, int userId)
        {

            var currentGoodCategory = _repGoodCategory.Table.ToList().Where(x => x.GoodId == good.Id);
            foreach (var item in currentGoodCategory)
            {
                _repGoodCategory.Delete(item);
            }
            string[] values = good.CategorList.Split(',').Select(sValue => sValue.Trim()).ToArray();
            foreach (var item in values)
            {
                int categoryId = 0;
                int.TryParse(item, out categoryId);
                _repGoodCategory.SaveOrUpdateAudit(new GoodCategory
                {
                    GoodId = good.Id,
                    CategoryId = categoryId
                }, userId);
            }
            //var newGoodCategory = good.CategoryId;
            //_repGoodCategory.Update(currentGoodCategory);
            //if (currentGoodCategory.CategoryId != newGoodCategory)
            //{
            //    currentGoodCategory.CategoryId = newGoodCategory;
            //    _repGoodCategory.Update(currentGoodCategory);
            //}

        }
        private void SaveUserGood(T good, int userId)
        {
            var userGoodExists = _repUserGood.Table.Any(x => x.GoodId == good.Id);
            if (!userGoodExists)
            {
                CreateUserGood(good, userId);
            }
            else
            {
                UpdateGoodCategory(good, userId);
            }
        }
        public Result<T> SaveGood(T good, int userId)
        {
            var result = new Result<T>(CreateResult.Error, good);
            Uow.WrapWithResult(u =>
            {

                SaveOrUpdateGood(good, userId);
                SaveOrUpdateGoodPropertyValues(good, userId);
                SaveUserGood(good, userId);
                if (good.GoodShareDates != null)
                {
                    _goodShareDateRepository.Table.Where(x => x.GoodId == good.Id).ToList().ForEach(x => _goodShareDateRepository.Delete(x));
                    good.GoodShareDates.ToList().ForEach(x => _goodShareDateRepository.Save(new GoodShareDate()
                    {
                        GoodId = good.Id,
                        ShareDate = x.ToDateTimeFromJavaScriptMilliseconds(),
                        StartTime = good.StartTime,
                        EndTime = good.EndTime
                    }));
                }
                _repGoodLocation.SaveOrUpdateAudit(new GoodLocation
                {
                    GoodId = good.Id,
                    Latitude = good.GoodLocation.Latitude,
                    Longitude = good.GoodLocation.Longitude
                }, userId);
                _repStartEndDate.SaveOrUpdateAudit(new GoodStartEndDate
                {
                    GoodId = good.Id
                }, userId);
                return true;
            }, null, LogSource.GoodService)
            .OnSuccess(() =>
            {
                //save images
                if (good.Images != null && good.Images.Any())
                {
                    SaveGoodImages(good, userId);
                }
                result.CreateResult = CreateResult.Success;
            }).Run();
            return result;
        }
        private void SaveGoodImages(T good, int userId)
        {
            var sequence = 0;
            var imageSetting = Ioc.Get<IImageSettings>().GoodImageSizes;
            var resultDeleteItems = DeleteAlreadyExsistGoodImages(good.Id);
            foreach (var img in good.Images)
            {
                if (img.FileName.TypeImage() == ImageType.Original.ToString())
                {
                    var refreshUserImageModel = new RefreshUserImageModel
                    {
                        Id = img.Id,
                        UserId = userId,
                        FileName = img.FileName,
                        ImgSettings = imageSetting,
                        GoodId = good.Id,
                        ImgFolder = ImageFolder.Good,
                        Sequence = sequence
                    };
                    _imageDataService.RefreshGoodImage(refreshUserImageModel);
                }
                else
                {
                    var imageModel = new UserImageModel
                    {
                        Id = img.Id,
                        UserId = userId,
                        GoodId = good.Id,
                        Sequence = sequence
                    };
                    _imageDataService.RefreshGoodImageSequence(imageModel);
                }
                sequence++;
            }
        }
        public bool DeleteAlreadyExsistGoodImages(int goodId)        {            var result = Uow.Wrap(u =>            {                var deleteExsistImages = _repGoodImg.Table.Where(x => x.GoodId == goodId).ToList();                foreach (var author in deleteExsistImages)                {                    var imageWithoutGoodId = _repGoodImg.Table.Where(x => x.FileName == author.FileName).FirstOrDefault();                    if (imageWithoutGoodId != null)                    {                        _repGoodImg.Delete(imageWithoutGoodId);                    }                    _repGoodImg.Delete(author);                }            }, null, LogSource.GoodService);            return result;        }
        public bool DeleteGood(int goodId)
        {
            var result = Uow.Wrap(u =>
            {
                var goodProperties = _repGoodPropertyValue.GetGoodPropertiesValue(goodId).ToList();
                _repGoodPropertyValue.Delete(goodProperties);
                var userGood = _repUserGood.Table.FirstOrDefault(p => p.GoodId == goodId);
                _repUserGood.Delete(userGood);
                var userCategoryGood = _repGoodCategory.Table.FirstOrDefault(p => p.GoodId == goodId);
                _repGoodCategory.Delete(userCategoryGood);
                var goodLocation = _repGoodLocation.Get(goodId);
                _repGoodLocation.Delete(goodLocation);
                var goodStartEndDate = _repStartEndDate.Get(goodId);
                _repStartEndDate.Delete(goodStartEndDate);
                var good = _repGood.Get(goodId);
                _repGood.Delete(good);
            }, null, LogSource.GoodService);
            return result;
        }
        public bool DeleteGood(T good)
        {
            var result = Uow.Wrap(u =>
            {
                var goodProperties = _repGoodPropertyValue.GetGoodPropertiesValue(good.Id).ToList();
                _repGoodPropertyValue.Delete(goodProperties);
                var userGood = _repUserGood.Table.FirstOrDefault(p => p.GoodId == good.Id);
                _repUserGood.Delete(userGood);
                var userCategoryGood = _repGoodCategory.Table.FirstOrDefault(p => p.GoodId == good.Id);
                _repGoodCategory.Delete(userCategoryGood);
                var goodLocation = _repGoodLocation.Get(good.Id);
                _repGoodLocation.Delete(goodLocation);
                var goodStartEndDate = _repStartEndDate.Get(good.Id);
                _repStartEndDate.Delete(goodStartEndDate);
                _repGood.Delete(good);
            }, null, LogSource.GoodService);
            return result;
        }

       
    }
}