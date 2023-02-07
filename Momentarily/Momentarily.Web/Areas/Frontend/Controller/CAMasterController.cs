using Apeek.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using Apeek.ViewModels.Models.Impl;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.Web.Framework.ControllerHelpers;
using Momentarily.ViewModels.Models;

namespace Momentarily.Web.Areas.Frontend.Controller
{
    public class CAMasterController : FrontendController
    {
        private readonly AccountControllerHelper<MomentarilyRegisterModel> _helperNew;
        private readonly ICAMasterBulkEmailService _cAMasterBulkEmailService;
        private string SubscriberListId = "hMFdFOy89j4lo14A6v5zaw";
        private string SubscriberCampaignListId = "tFxUl5D7snUveY8khwEGkg";
        // GET: Frontend/CAMaster
        public CAMasterController(CAMasterBulkEmailService cAMasterBulkEmailService)
        {
            _cAMasterBulkEmailService = cAMasterBulkEmailService;
            _helperNew = new AccountControllerHelper<MomentarilyRegisterModel>();
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Import()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase FileUpload)
        {
            if (Request.Form["Import"] != null)
            {
                if (FileUpload == null || FileUpload.ContentLength == 0)
                {
                    ViewBag.error = "Please select a excel file.";
                    return View("Import");
                }
                else
                {
                    if (FileUpload.FileName.EndsWith("xls") || FileUpload.FileName.EndsWith("xlsx") || FileUpload.FileName.EndsWith("csv"))
                    {
                        string path = Server.MapPath("~/Content/ImportFies/" + FileUpload.FileName);
                        if (System.IO.File.Exists(path))
                        {
                            try
                            {
                                System.IO.File.Delete(path);
                            }
                            catch (Exception ex)
                            {
                                ViewBag.error = "This file is being used by another process. Please close from there!";
                                return View();
                            }
                        }
                        FileUpload.SaveAs(path);

                        //Read data from excel file
                        Excel.Application application = new Excel.Application();
                        Excel.Workbook workbook = application.Workbooks.Open(path);
                        Excel.Worksheet worksheet = workbook.ActiveSheet;
                        Excel.Range range = worksheet.UsedRange;
                        List<CAMasterViewModel> cAMasterViewModelsList = new List<CAMasterViewModel>();
                        for (int row = 2; row <= range.Rows.Count; row++)
                        {
                            CAMasterViewModel cAMasterViewModel = new CAMasterViewModel();
                            cAMasterViewModel.Email = range.Cells[row, 1].Text.Trim();
                            cAMasterViewModel.AddedToSendy = false;
                            //cAMasterViewModelsList.Add(cAMasterViewModel);
                            _cAMasterBulkEmailService.GetAllCAMasterRecord(cAMasterViewModel);
                        }
                        //ViewBag.listCAMaster = cAMasterViewModelsList;

                        ViewBag.error = "File successfully imported.";
                    }
                    else
                    {
                        ViewBag.error = "File type is incorrect.";
                        return View("Import");
                    }
                }
            }
            else if (Request.Form["Process"] != null)
            {
                var checkAlreadyUpdated = _cAMasterBulkEmailService.GetCheckAlreadyUpdateForTheDay();
                if(!checkAlreadyUpdated)
                {
                    CAMasterViewModel result = _cAMasterBulkEmailService.SaveLimitedEmailRecord();
                    if (result != null && result.ReturnStatus)
                    {
                        foreach (var item in result.cAMasterViewModelsList)
                        {
                            _helperNew.subscribeEmail(SubscriberListId, item.Email, "", true);
                            _helperNew.subscribeEmail(SubscriberCampaignListId, item.Email, "", true);
                        }
                        ViewBag.errorsendy = result.AddedCount + " email has been updated on sendy";
                    }
                    else
                    {
                        ViewBag.errorsendy = "Something went worng!";
                    }
                }
                else
                {
                    ViewBag.errorsendy = "Today's process has been expired.";
                }
                
            }
            return View();
        }


    }
}