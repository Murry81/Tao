using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;
using TaoWebApplication.Calculators;
using TaoWebApplication.Models;

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

            model = ControllerHelper.FillModel(model, _service, currentpage, sessionId) as Models.TartalomjegyzekModel;
            
            Session["SessionId"] = sessionId.ToString();
            Session["CustomerId"] = Request.Form["SelectedCustomer.Id"];

            return View("~/Views/Tao/Tartalomjegyzek.cshtml", model);
        }

        public ActionResult Tartalomjegyzek()
        {
            var model = new Models.TartalomjegyzekModel();
            var currentpage = _service.GetPage("Tartalomjegyzek");
            model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString())) as Models.TartalomjegyzekModel;

            return View(model);
        }


        [HttpPost]
        public ActionResult Tartalomjegyzek(string buttonAction, TartalomjegyzekModel fc)
        {
            SaveValues(fc.Fields, TartalomCalculation.CalculateValues, 1);
       
            if(buttonAction == "Previous")
            {
                return RedirectToAction("Index", "Selector");
            }
            if (buttonAction == "Save")
            {
                var model = new Models.TartalomjegyzekModel();
                var currentpage = _service.GetPage("Tartalomjegyzek");
                model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString())) as Models.TartalomjegyzekModel;

                return View(model);
            }

            return RedirectToAction("Tenyadatok", "Tao");
        }

        public ActionResult Tenyadatok()
        {
            var model = new Models.TenyadatokModel();
            var currentpage = _service.GetPage("Tenyadatok");
            ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString()));
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
                model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString())) as Models.TenyadatokModel;
                return View(model);
            }
            return RedirectToAction("TenyadatKorrekcio", "Tao");
        }

        public ActionResult TenyadatKorrekcio()
        {
            var model = new Models.TenyadatKorrekcioModel();
            var currentpage = _service.GetPage("TenyadatKorrekcio");
            model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString())) as Models.TenyadatKorrekcioModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult TenyadatKorrekcio(string buttonAction, TenyadatokModel fc)
        {
            SaveValues(fc.Fields, TenyadatKorreckcioCalculation.CalculateValues, 3);

            if (buttonAction == "Previous")
            {
                return RedirectToAction("Tenyadatok", "Tao");
            }
            if (buttonAction == "Save")
            {
                var model = new Models.TenyadatKorrekcioModel();
                var currentpage = _service.GetPage("TenyadatKorrekcio");
                model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString())) as Models.TenyadatKorrekcioModel;
                return View(model);
            }
            return RedirectToAction("TenyadatKorrekcio", "Tao");
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