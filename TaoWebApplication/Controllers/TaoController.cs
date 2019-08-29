﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;
using TaoWebApplication.Calculators;
using TaoWebApplication.Models;
using TaoWebApplication.Validators;

namespace TaoWebApplication.Controllers
{
    public class TaoController : Controller
    {

        private readonly IDataService _service;
       
        public TaoController(IDataService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult TartalomjegyzekStart(string id)
        {
            var model = new Models.TartalomjegyzekModel();
            var currentpage = _service.GetPage("Tartalomjegyzek");
            Guid sessionId = Guid.Empty;

            if (Request.Form.AllKeys.Contains("StartNew"))
            {
                var result = _service.CreateSession(new SessionDto
                {
                    CustomerId = int.Parse(Request.Form["SelectedCustomer.Id"]),
                    DocumentType = new DocumentDto { Id = int.Parse(Request.Form["SelectedDocumentType"]) }
                });

                sessionId = result.Id;
            }
            else
            {
                sessionId = Guid.Parse(Request.Form["Continue"]);
            }

            var customerId = int.Parse(Request.Form["SelectedCustomer.Id"]);
            model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as TartalomjegyzekModel;
            
            Session["SessionId"] = sessionId.ToString();
            Session["CustomerId"] = customerId.ToString();

            return View("~/Views/Tao/Tartalomjegyzek.cshtml", model);
        }

        public ActionResult Tartalomjegyzek()
        {
            var model = new Models.TartalomjegyzekModel();
            var currentpage = _service.GetPage("Tartalomjegyzek");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString()), customerId) as TartalomjegyzekModel;

            return View(model);
        }


        [HttpPost]
        public ActionResult Tartalomjegyzek(string buttonAction, TartalomjegyzekModel fc)
        {
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());

            var validationResult = TartalomValidator.Validate(fc.Fields);
            if(validationResult.Keys.Count > 0)
            {
                foreach (var error in validationResult.Keys)
                {
                    ModelState.AddModelError(error.ToString(), validationResult[error]);
                }

                var model = new TartalomjegyzekModel();
                var currentpage = _service.GetPage("Tartalomjegyzek");
                model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as TartalomjegyzekModel;
                foreach (var field in model.Fields)
                {
                    var newValueField = fc.Fields.FirstOrDefault(f => f.Id == field.Id);
                    if (newValueField != null)
                    {
                        field.BoolFieldValue = newValueField.BoolFieldValue;
                        field.DateValue = newValueField.DateValue;
                        field.DecimalValue = newValueField.DecimalValue;
                        field.StringValue = newValueField.StringValue;
                    }
                }
                return View(model);
            }
            
            SaveValues(fc.Fields, TartalomCalculation.CalculateValues, 1);
       
            if(buttonAction == "Previous")
            {
                return RedirectToAction("Index", "Selector");
            }
            if (buttonAction == "Save")
            {
                var model = new TartalomjegyzekModel();
                var currentpage = _service.GetPage("Tartalomjegyzek");            
                model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as TartalomjegyzekModel;

                return View(model);
            }
            
            return RedirectToAction("Tenyadatok", "Tao");
        }

        public ActionResult Tenyadatok()
        {
            var model = new Models.TenyadatokModel();
            var currentpage = _service.GetPage("Tenyadatok");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString()), customerId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Tenyadatok(string buttonAction, TenyadatokModel fc)
        {
            SaveValues(fc.Fields, TenyadatokCalculation.CalculateValues, 2);

            if (buttonAction == "Previous")
            {
                return RedirectToAction("Tartalomjegyzek", "Tao");
            }
            if (buttonAction == "Save")
            {
                var model = new Models.TenyadatokModel();
                var currentpage = _service.GetPage("Tenyadatok");
                var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
                model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString()), customerId) as TenyadatokModel;
                return View(model);
            }
            return RedirectToAction("TenyadatKorrekcio", "Tao");
        }

        public ActionResult TenyadatKorrekcio()
        {
            var model = new Models.TenyadatKorrekcioModel();
            var currentpage = _service.GetPage("TenyadatKorrekcio");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as TenyadatKorrekcioModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult TenyadatKorrekcio(string buttonAction, TenyadatokModel fc)
        {
            SaveValues(fc.Fields, TenyadatKorreckcioCalculation.CalculateValues, 3);
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());

            if (buttonAction == "Previous")
            {
                return RedirectToAction("Tenyadatok", "Tao");
            }
            if (buttonAction == "Save")
            {
                var model = new Models.TenyadatKorrekcioModel();
                var currentpage = _service.GetPage("TenyadatKorrekcio");
                var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());     
                model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as TenyadatKorrekcioModel;
                
                return View(model);
            }

            var isKapcsolt =_service.GetFieldsByFieldIdList(new List<int> { 40 }, sessionId).FirstOrDefault()?.BoolFieldValue;
            if (isKapcsolt == null || !isKapcsolt.Value)
            {
                return RedirectToAction("IpaNemKapcsolt", "Tao");
            }
            else
            {
                return RedirectToAction("IpaKapcsolt", "Tao");
            }
        }

        public ActionResult IpaNemKapcsolt()
        {
            var model = new IpaNemKapcsoltModel();
            var currentpage = _service.GetPage("IpaNemKapcsolt");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString()), customerId) as IpaNemKapcsoltModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult IpaNemKapcsolt(string buttonAction, IpaNemKapcsoltModel fc)
        {
            SaveValues(fc.Fields, IpaNemKapcsoltCalculation.CalculateValues, pageId:4);

            if (buttonAction == "Previous")
            {
                return RedirectToAction("TenyadatKorrekcio", "Tao");
            }
            if (buttonAction == "Save")
            {
                return RedirectToAction("IpaNemKapcsolt", "Tao");
            }
            return RedirectToAction("IpaNemKapcsolt", "Tao");
        }

        public ActionResult IpaKapcsolt()
        {
            var model = new IpaKapcsoltModel();
            var currentpage = _service.GetPage("IpaKapcsolt");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString()), customerId) as IpaKapcsoltModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult IpaKapcsolt(string buttonAction, IpaKapcsoltModel fc)
        {
            SaveValues(fc.Fields, IpaKapcsoltCalculation.CalculateValues, pageId: 5);

            if (buttonAction == "Previous")
            {
                return RedirectToAction("TenyadatKorrekcio", "Tao");
            }
            if (buttonAction == "Save")
            {
                return RedirectToAction("IpaKapcsolt", "Tao");
            }
            return RedirectToAction("IpaKapcsolt", "Tao");
        }

        private void SaveValues(List<FieldDescriptorDto> fieldValues, Action<List<FieldDescriptorDto>, IDataService, Guid> calulator, int pageId)
        {
            if (Guid.TryParse(System.Web.HttpContext.Current.Session["SessionId"]?.ToString(), out var sessionId))
            {
                var fields = _service.GetPageFields(pageId, sessionId);

                foreach (var field in fields)
                {
                    if (!field.IsCaculated && field.IsEditable)
                    {
                        DataConverter.GetTypedValue(field, fieldValues);
                    }
                }

                if (calulator != null)
                {
                    calulator(fields, _service, sessionId);
                }

                _service.UpdateFieldValues(fields, sessionId);
            }
        }
    }
}