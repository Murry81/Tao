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

        [HttpPost]
        public ActionResult Tartalomjegyzek(string buttonAction, TartalomjegyzekModel fc)
        {
            SaveValues(fc.Fields, TenyadatokCalculation.CalculateValues, 1);
       
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
        public ActionResult Tenyadatok(string id)
        {
            //SaveValues(null, 2);

            var model = new Models.TenyadatokModel();
            var currentpage = _service.GetPage("Tenyadatok");
            model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString())) as Models.TenyadatokModel;
            return View(model);
        }

        private void SaveValues(List<FieldDescriptorDto> fieldValues, Action<List<FieldDescriptorDto>, IDataService> calulator, int pageId)
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
                    calulator(fields, _service);
                }

                _service.UpdateFieldValues(fields, sessionId);
            }
        }
    }
}