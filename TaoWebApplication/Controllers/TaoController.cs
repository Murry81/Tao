using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;
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
            else if (Request.Form.AllKeys.Contains("ExportIpa"))
            {
                sessionId = Guid.Parse(Request.Form["ExportIpa"]);
                return GenerateXml(1, sessionId);
            }
            else if (Request.Form.AllKeys.Contains("ExportTarsasagi"))
            {
                sessionId = Guid.Parse(Request.Form["ExportTarsasagi"]);
                return GenerateXml(2, sessionId);
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
            
            SaveValues(fc.Fields, TartalomCalculation.CalculateValues, 1);
            
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
       
            if(buttonAction == "Previous")
            {
                return RedirectToAction("Index", "Selector");
            }
            if (buttonAction == "Next")
            {
                return RedirectToAction("Tenyadatok", "Tao");
            }

            var model1 = new TartalomjegyzekModel();
            var currentpage1 = _service.GetPage("Tartalomjegyzek");
            model1 = ControllerHelper.FillModel(model1, _service, currentpage1, sessionId, customerId) as TartalomjegyzekModel;

            return View(model1);
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
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(2);
            }
            if (buttonAction == "Previous")
            {
                return RedirectToAction("Tartalomjegyzek", "Tao");
            }
            if (buttonAction == "Next")
            {
                var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());
                
                var isKapcsolt = _service.GetFieldsByFieldIdList(new List<int> { 40 }, sessionId).FirstOrDefault()?.BoolFieldValue;
                if (isKapcsolt == null || !isKapcsolt.Value)
                {
                    return RedirectToAction("IpaNemKapcsolt", "Tao");
                }
                else
                {
                    return RedirectToAction("IpaKapcsolt", "Tao");
                }
            }

            var model = new Models.TenyadatokModel();
            var currentpage = _service.GetPage("Tenyadatok");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString()), customerId) as TenyadatokModel;
            return View(model);
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
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(3);
            }
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());

            if (buttonAction == "Previous")
            {
                return RedirectToAction("Tenyadatok", "Tao");
            }
            if (buttonAction == "Next")
            {
                var isKapcsolt = _service.GetFieldsByFieldIdList(new List<int> { 40 }, sessionId).FirstOrDefault()?.BoolFieldValue;
                if (isKapcsolt == null || !isKapcsolt.Value)
                {
                    return RedirectToAction("IpaNemKapcsolt", "Tao");
                }
                else
                {
                    return RedirectToAction("IpaKapcsolt", "Tao");
                }
            }

            var model = new Models.TenyadatKorrekcioModel();
            var currentpage = _service.GetPage("TenyadatKorrekcio");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as TenyadatKorrekcioModel;

            return View(model);
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
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(4);
            }
            if (buttonAction == "Previous")
            {
                return RedirectToAction("Tenyadatok", "Tao");
            }
            if (buttonAction == "Next")
            {
                return RedirectToAction("InnovaciosJarulek", "Tao");
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
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(5);
            }
            if (buttonAction == "Previous")
            {
                var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());
                var isVegleges = _service.GetFieldById(29, sessionId).StringValue == "Végleges kalkuláció";
                if (!isVegleges)
                {
                    return RedirectToAction("TenyadatKorrekcio", "Tao");
                }
                else
                {
                    return RedirectToAction("Tenyadatok", "Tao");
                }
            }
            if (buttonAction == "Next")
            {
                return RedirectToAction("IpaKapcsoltStatus", "Tao"); 
            }
            return RedirectToAction("IpaKapcsolt", "Tao");
        }


        public ActionResult IpaKapcsoltStatus()
        {
            var model = new IpaKapcsoltStatusModel();
            var currentpage = _service.GetPage("IpaKapcsoltStatusz");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as IpaKapcsoltStatusModel;
            model.TableDescriptors = _service.GetTableData(6, sessionId);
            return View(model);
        }

        [HttpPost]
        public ActionResult IpaKapcsoltStatus(string buttonAction, IpaKapcsoltStatusModel fc)
        {
            var field = new List<FieldDescriptorDto>();
            foreach(var list in fc.TableDescriptors[0].FieldValues)
            {
                field.AddRange(list);
            }

            field = FillFieldValuesForTable(field, 1);
            SaveValues(field, IpaKapcsoltStatusCalculation.CalculateValues);
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(6);
            }
            if (buttonAction == "Previous")
            {
                return RedirectToAction("IpaKapcsolt", "Tao");
            }
            if (buttonAction == "Next")
            {
                return RedirectToAction("InnovaciosJarulek", "Tao"); 
            }
            return RedirectToAction("IpaKapcsoltStatus", "Tao");
        }


        public ActionResult InnovaciosJarulek()
        {
            var model = new InnovaciosJarulekModel();
            var currentpage = _service.GetPage("InnovJar");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as InnovaciosJarulekModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult InnovaciosJarulek(string buttonAction, IpaKapcsoltStatusModel fc)
        {
            SaveValues(fc.Fields, InnovaciosJarulekCalculation.CalculateValues, pageId: 7);
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(7);
            }
            if (buttonAction == "Previous")
            {
                return RedirectToAction("IpaKapcsoltStatus", "Tao");
            }
            if (buttonAction == "Next")
            {
                return RedirectToAction("IpaMegosztas", "Tao"); 
            }
            return RedirectToAction("InnovaciosJarulek", "Tao");
        }


        public ActionResult IpaMegosztas()
        {
            var model = new IpaMegosztasModel();
            var currentpage = _service.GetPage("IpaMegosztas");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as IpaMegosztasModel;
            model.TableDescriptors = _service.GetTableData(18, sessionId);

            var allFields = new List<FieldDescriptorDto>();

            foreach (var list in model.TableDescriptors[0].FieldValues)
            {
                allFields.AddRange(list);
            }
            var validationResult = IpaMegosztasValidator.Validate(allFields);
            if (validationResult.Keys.Count > 0)
            {
                foreach (var error in validationResult.Keys)
                {
                    ModelState.AddModelError(error.ToString(), validationResult[error]);
                }
            }
            
            return View(model);
        }

        [HttpPost]
        public ActionResult IpaMegosztas(string buttonAction, IpaMegosztasModel fc)
        {
            var field = new List<FieldDescriptorDto>();
            foreach (var list in fc.TableDescriptors[0].FieldValues)
            {
                field.AddRange(list);
            }

            field = FillFieldValuesForTable(field, 2);

            foreach (var item in fc.Fields)
            {
                field.Add(item);
            }

            SaveValues(field, IpaMegosztasCalculation.CalculateValues);
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(18);
            }
            if (buttonAction == "Previous")
            {
                return RedirectToAction("InnovaciosJarulek", "Tao");
            }
            if (buttonAction == "Next")
            {
                return RedirectToAction("AEEVarhatoAdatok", "Tao"); 
            }
            return RedirectToAction("IpaMegosztas", "Tao");
        }

        [HttpPost]
        public ActionResult GenerateIpaMegosztasExcel()
        {
            var model = new IpaMegosztasModel();
            var currentpage = _service.GetPage("IpaMegosztas");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as IpaMegosztasModel;
            model.TableDescriptors = _service.GetTableData(18, sessionId);
            var dataSet = new DataSet();
            dataSet.Tables.Add("IpaMegosztas");
            foreach (var r in model.TableDescriptors[0].FieldValues[0])
            {
                dataSet.Tables[0].Columns.Add(r.Title, r.TypeName == "numeric" ? typeof(decimal) : r.TypeName == "bool" ? typeof(bool) : r.TypeName == "date" ? typeof(DateTime) : typeof(string));
            }

            for(int row = 0; row < model.TableDescriptors[0].FieldValues.Count; row++)
            {
                var rowValues = new List<object>();
                for(int column = 0; column < model.TableDescriptors[0].FieldValues[row].Count; column++)
                {
                    switch (model.TableDescriptors[0].FieldValues[row][column].TypeName)
                    {
                        case "numeric":
                            rowValues.Add(model.TableDescriptors[0].FieldValues[row][column].DecimalValue);
                            break;
                        case "bool":
                            rowValues.Add(model.TableDescriptors[0].FieldValues[row][column].BoolFieldValue);
                            break;
                        case "date":
                            rowValues.Add(model.TableDescriptors[0].FieldValues[row][column].DateValue);
                            break;
                        default:
                            rowValues.Add(model.TableDescriptors[0].FieldValues[row][column].StringValue);
                            break;
                    }
                   
                }
                dataSet.Tables[0].Rows.Add(rowValues.ToArray());
            }                

            var file = ExcelExport.ExcelExport.GenerateExcelFile(dataSet, "IpaMegosztas");
            return File(file, "application/vnd.ms-excel", "IpamMegosztas.xlsx");
        }

        public ActionResult AEEVarhatoAdatok()
        {
            var model = new AEEVarhatoAdatokModel();
           
            var currentpage = _service.GetPage("AeeVarhatoAdatok");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString()), customerId) as AEEVarhatoAdatokModel;
            SaveValues(model.Fields, AEEVarhatoAdatokCalculation.CalculateValues);

            foreach (var field in model.Fields)
            {
                field.IsEditable = false;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult AeeVarhatoAdatok(string buttonAction, IpaKapcsoltModel fc)
        {
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(8);
            }
            if (buttonAction == "Previous")
            {
                return RedirectToAction("IpaMegosztas", "Tao");
            }
            if (buttonAction == "Next")
            {
                return RedirectToAction("Alultokesites", "Tao");
            }
            return RedirectToAction("AEEVarhatoAdatok", "Tao");
        }

        public ActionResult Alultokesites()
        {
            var model = new AlultokesitesModel();

            var currentpage = _service.GetPage("TaoAlultokesites");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, sessionId , customerId) as AlultokesitesModel;
            model.TableDescriptors = _service.GetTableData(9, sessionId);

            var values = model.FillDeafultRows(_service.GetFieldById(32, sessionId).DateValue.Value);
            if(values.Count > 0)
            {
                SaveValues(values, null);
                model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as AlultokesitesModel;
                model.TableDescriptors = _service.GetTableData(9, sessionId);
            }

            model.MakeDefaultRowsReadonly();
            return View(model);
        }

        [HttpPost]
        public ActionResult Alultokesites(string buttonAction, AlultokesitesModel fc)
        {
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());
            var fields = AlultokesitesModel.RemoveDefaultFieldsBeforeSave(fc.TableDescriptors);
            fields = FillFieldValuesForTable(fields, 3, 4);

            AlultokesitesCalculation.CalculateValues(fields, _service, sessionId, fc.Fields);
            //Save table fields
            SaveValues(fields, null);
            // Save other fields
            SaveValues(fc.Fields, null, 9);
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(9);
            }
            if (buttonAction == "Previous")
            {
                return RedirectToAction("AEEVarhatoAdatok", "Tao");
            }
            if (buttonAction == "Next")
            {
                return RedirectToAction("TaggalSzembeniKotelezettseg", "Tao"); 
            }
            return RedirectToAction("Alultokesites", "Tao");
        }

        public ActionResult TaggalSzembeniKotelezettseg()
        {
            var model = new TaggalSzembeniKotelezettsegModel();

            var currentpage = _service.GetPage("TaggalSzembeniKot");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as TaggalSzembeniKotelezettsegModel;
            model.TableDescriptors = _service.GetTableData(10, sessionId);

            var values = model.FillDeafultRows(_service.GetFieldById(32, sessionId).DateValue.Value);
            if (values.Count > 0)
            {
                SaveValues(values, null);
                model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as TaggalSzembeniKotelezettsegModel;
                model.TableDescriptors = _service.GetTableData(10, sessionId);
            }

            model.MakeDefaultRowsReadonly();
            return View(model);
        }

        [HttpPost]
        public ActionResult TaggalSzembeniKotelezettseg(string buttonAction, TaggalSzembeniKotelezettsegModel fc)
        {
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());
            var fields = TaggalSzembeniKotelezettsegModel.RemoveDefaultFieldsBeforeSave(fc.TableDescriptors);
            fields = FillFieldValuesForTable(fields, 5);

            TaggalSzembeniKotelezettsegCalculation.CalculateValues(fields, _service, sessionId, fc.Fields);
            //Save table fields
            SaveValues(fields, null);
            // Save other fields
            SaveValues(fc.Fields, null, 10);
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(10);
            }
            if (buttonAction == "Previous")
            {
                return RedirectToAction("Alultokesites", "Tao");
            }
            if (buttonAction == "Next")
            {
                return RedirectToAction("TaoAdoalapKorrekcio", "Tao"); 
            }
            return RedirectToAction("TaggalSzembeniKotelezettseg", "Tao");
        }

        public ActionResult TaoAdoalapKorrekcio()
        {
            var model = new TaoAdoalapKorrekcioModel();
            var currentpage = _service.GetPage("TaoAdoalapKorr");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as TaoAdoalapKorrekcioModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult TaoAdoalapKorrekcio(string buttonAction, TaoAdoalapKorrekcioModel fc)
        {
            SaveValues(fc.Fields, TaoAdoalapKorrekcioCalculation.CalculateValues, pageId: 11);
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(11);
            }
            if (buttonAction == "Previous")
            {
                return RedirectToAction("TaggalSzembeniKotelezettseg", "Tao");
            }
            if (buttonAction == "Next")
            {
                return RedirectToAction("AthozottVeszteseg", "Tao");         
            }
            return RedirectToAction("TaoAdoalapKorrekcio", "Tao");
        }

        public ActionResult AthozottVeszteseg()
        {
            var model = new AthozottVesztesegModel();

            var currentpage = _service.GetPage("AthozottVeszteseg");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as AthozottVesztesegModel;
            model.TableDescriptors = _service.GetTableData(19, sessionId);

            var values = model.FillDeafultRows(_service.GetFieldById(32, sessionId).DateValue.Value, sessionId, _service);
            model.SetUpKepzesEveCombo(_service, sessionId);
            if (values.Count > 0)
            {
                SaveValues(values, null);
                model = ControllerHelper.FillModel(model, _service, currentpage, sessionId, customerId) as AthozottVesztesegModel;
                model.TableDescriptors = _service.GetTableData(19, sessionId);
            }

            model.MakeDefaultRowsReadonly();
            return View(model);
        }

        [HttpPost]
        public ActionResult AthozottVeszteseg(string buttonAction, AthozottVesztesegModel fc)
        {
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());
            var fields = new List<FieldDescriptorDto>();
            fields = fc.RemoveDefaultFieldsBeforeSave(fc.TableDescriptors);
            fields = FillFieldValuesForTable(fields, 6);
                       
            //Save table fields
            SaveValues(fields, null);

            AthozottVesztesegCalculation.ReCalculateValues(_service, sessionId);
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(19);
            }
            if (buttonAction == "Previous")
            {
                return RedirectToAction("TaoAdoalapKorrekcio", "Tao");
            }
            if (buttonAction == "Next")
            {
                return RedirectToAction("NyeresegminimumLevezetese", "Tao");
               
            }
            return RedirectToAction("AthozottVeszteseg", "Tao");
        }

        public ActionResult NyeresegminimumLevezetese()
        {
            var model = new NyeresegminimumLevezeteseModel();
            var currentpage = _service.GetPage("TaoNyeresegminimum");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString()), customerId) as NyeresegminimumLevezeteseModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult NyeresegminimumLevezetese(string buttonAction, IpaKapcsoltModel fc)
        {
            SaveValues(fc.Fields, NyeresegminimumLevezeteseCalculation.CalculateValues, pageId: 12);
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(12);
            }
            if (buttonAction == "Previous")
            {
                return RedirectToAction("AthozottVeszteseg", "Tao");
            }
            if (buttonAction == "Next")
            {
                return RedirectToAction("Adokedvezmeny", "Tao");
            }
            return RedirectToAction("NyeresegminimumLevezetese", "Tao");
        }

        public ActionResult Adokedvezmeny()
        {
            var model = new AdokedvezmenyModel();
            var currentpage = _service.GetPage("TaoAdokedvezmeny");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString()), customerId) as AdokedvezmenyModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult Adokedvezmeny(string buttonAction, AdokedvezmenyModel fc)
        {
            SaveValues(fc.Fields, AdokedvezmenyCalculation.CalculateValues, pageId: 13);
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(13);
            }
            if (buttonAction == "Previous")
            {
                return RedirectToAction("NyeresegminimumLevezetese", "Tao");
            }
            if (buttonAction == "Next")
            {
                return RedirectToAction("TarsasagiAdo", "Tao");
            }
            return RedirectToAction("Adokedvezmeny", "Tao");
        }


        public ActionResult TarsasagiAdo()
        {
            var model = new TarsasagiAdoModel();
            var currentpage = _service.GetPage("Tao");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString()), customerId) as TarsasagiAdoModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult TarsasagiAdo(string buttonAction, TarsasagiAdoModel fc)
        {
            SaveValues(fc.Fields, TarsasagiAdoCalculations.CalculateValues, pageId: 14);
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(14);
            }
            if (buttonAction == "Previous")
            {
                return RedirectToAction("Adokedvezmeny", "Tao");
            }
            if (buttonAction == "Next")
            {
                return RedirectToAction("AdozottEredmeny", "Tao");
            }
            return RedirectToAction("TarsasagiAdo", "Tao");
        }

        public ActionResult AdozottEredmeny()
        {
            var model = new AdozottEredmenyModel();
            var currentpage = _service.GetPage("AdozottEredmeny");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString()), customerId) as AdozottEredmenyModel;
           
            return View(model);
        }

        [HttpPost]
        public ActionResult AdozottEredmeny(string buttonAction, AdozottEredmenyModel fc)
        {
            SaveValues(fc.Fields, AdozottEredmenyCalculations.CalculateValues, pageId: 22);
            if (buttonAction == "SaveAll")
            {
                RecalculateAll(22);
            }
            if (buttonAction == "Previous")
            {
                return RedirectToAction("TarsasagiAdo", "Tao");
            }
            if (buttonAction == "Next")
            {
                var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());
                if (_service.GetFieldById(61, sessionId).BoolFieldValue)
                {
                    return RedirectToAction("EnergiaEllatok", "Tao");
                }
            }
            return RedirectToAction("AdozottEredmeny", "Tao");
        }

        public ActionResult EnergiaEllatok()
        {
            var model = new EnergiaEllatokModel();
            var currentpage = _service.GetPage("EnergiaEllatok");
            var customerId = int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString());
            model = ControllerHelper.FillModel(model, _service, currentpage, Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString()), customerId) as EnergiaEllatokModel;

            return View(model);
        }

        [HttpPost]
        public ActionResult EnergiaEllatok(string buttonAction, EnergiaEllatokModel fc)
        {
            SaveValues(fc.Fields, EnergiaEllatokCalculations.CalculateValues, pageId: 15);
            if(buttonAction == "SaveAll")
            {
                RecalculateAll(15);
            }
            if (buttonAction == "Previous")
            {
                return RedirectToAction("AdozottEredmeny", "Tao");
            }
            if (buttonAction == "Next")
            {
                return RedirectToAction("EnergiaEllatok", "Tao");
            }
            return RedirectToAction("EnergiaEllatok", "Tao");
        }

        private void SaveValues(List<FieldDescriptorDto> fieldValues, Action<List<FieldDescriptorDto>, IDataService, Guid> calulator, int pageId)
        {
            if (Guid.TryParse(System.Web.HttpContext.Current.Session["SessionId"]?.ToString(), out var sessionId))
            {
                var fields = _service.GetPageFields(pageId, sessionId);

                foreach (var field in fields)
                {
                    if ((!field.IsCaculated && field.IsEditable) || calulator == null)
                    {
                        DataConverter.GetTypedValue(field, fieldValues);
                    }
                }

                calulator?.Invoke(fields, _service, sessionId);
                _service.UpdateFieldValues(fields, sessionId);
            }
        }

        private void SaveValues(List<FieldDescriptorDto> fieldValues, Action<List<FieldDescriptorDto>, IDataService, Guid> calulator)
        {
            if (Guid.TryParse(System.Web.HttpContext.Current.Session["SessionId"]?.ToString(), out var sessionId))
            {
                calulator?.Invoke(fieldValues, _service, sessionId);
                _service.UpdateFieldValues(fieldValues, sessionId);
            }
        }

        private List<FieldDescriptorDto> FillFieldValuesForTable(List<FieldDescriptorDto> fieldValues, params int[] tableId)
        {
            foreach (var id in tableId)
            {
                var tableFields = _service.GetTableFields(id).ToList();

                foreach (var field in fieldValues)
                {
                    var baseData = tableFields.FirstOrDefault(t => t.Id == field.Id);
                    if (baseData != null)
                    {
                        field.IsCaculated = baseData.IsCaculated;
                        field.IsEditable = baseData.IsEditable;
                        field.IsMandatory = baseData.IsMandatory;
                        field.TypeName = baseData.TypeName;
                        field.IsSpecial = baseData.IsSpecial;
                        field.TypeOptions = baseData.TypeOptions;
                    }
                }
            }
            return fieldValues;
        }

        public FileContentResult GenerateXml(int documentType, Guid sessionId)
        {
            string file = XmlExport.XmlExport.GenerateDocument(documentType, sessionId, _service);
            return File(Encoding.UTF8.GetBytes(file), MediaTypeNames.Text.Xml,  $"{DateTime.Now.ToString("yyyyMMdd")}_{_service.GetDocumentIdentifier(documentType)}.xml");
        }

        private static Dictionary<int, Action<IDataService, Guid>> calculatorPageIdMap = new Dictionary<int, Action<IDataService, Guid>>
        {
            { 1, TartalomCalculation.ReCalculateValues },
             { 2, TenyadatokCalculation.ReCalculateValues },
             { 3, TenyadatKorreckcioCalculation.ReCalculateValues },
             { 4, IpaNemKapcsoltCalculation.ReCalculateValues },
             { 5, IpaKapcsoltCalculation.ReCalculateValues },
             { 6, IpaKapcsoltStatusCalculation.ReCalculateValues },
             { 7, InnovaciosJarulekCalculation.ReCalculateValues },
             { 8, AEEVarhatoAdatokCalculation.ReCalculateValues },
             { 9, AlultokesitesCalculation.ReCalculateValues },
             { 10, TaggalSzembeniKotelezettsegCalculation.ReCalculateValues },
             { 11, TaoAdoalapKorrekcioCalculation.ReCalculateValues },
             { 12, NyeresegminimumLevezeteseCalculation.ReCalculateValues },
             { 13, AdokedvezmenyCalculation.ReCalculateValues },
             { 14, TarsasagiAdoCalculations.ReCalculateValues },
             { 15, EnergiaEllatokCalculations.ReCalculateValues },
             { 18, IpaMegosztasCalculation.ReCalculateValues },
             { 19, AthozottVesztesegCalculation.ReCalculateValues },
             { 22, AdozottEredmenyCalculations.ReCalculateValues }
        };

        private void RecalculateAll(int pageId)
        {
            var sessionId = Guid.Parse(System.Web.HttpContext.Current.Session["SessionId"].ToString());

            var calculators = new List<Action<IDataService, Guid>> {
                TartalomCalculation.ReCalculateValues,
                TenyadatokCalculation.ReCalculateValues,
                };

            var isVegleges = _service.GetFieldById(29, sessionId).StringValue == "Végleges kalkuláció";
            if (!isVegleges)
            {
                calculators.Add(TenyadatKorreckcioCalculation.ReCalculateValues);

            }

            var isKapcsolt = _service.GetFieldsByFieldIdList(new List<int> { 40 }, sessionId).FirstOrDefault()?.BoolFieldValue;
            if (isKapcsolt.Value)
            {
                calculators.Add(IpaKapcsoltCalculation.ReCalculateValues);
                calculators.Add(IpaKapcsoltStatusCalculation.ReCalculateValues);
            }
            else
            {
                calculators.Add(IpaNemKapcsoltCalculation.ReCalculateValues);
            }

            calculators.AddRange(new List<Action<IDataService, Guid>>
            {
                 InnovaciosJarulekCalculation.ReCalculateValues,
                 AEEVarhatoAdatokCalculation.ReCalculateValues,
                 AlultokesitesCalculation.ReCalculateValues,
                 TaggalSzembeniKotelezettsegCalculation.ReCalculateValues,
                 TaoAdoalapKorrekcioCalculation.ReCalculateValues,
                 NyeresegminimumLevezeteseCalculation.ReCalculateValues,
                 AdokedvezmenyCalculation.ReCalculateValues,
                 TarsasagiAdoCalculations.ReCalculateValues,
                 EnergiaEllatokCalculations.ReCalculateValues,
                 IpaMegosztasCalculation.ReCalculateValues,
                 AthozottVesztesegCalculation.ReCalculateValues,
                 AdozottEredmenyCalculations.ReCalculateValues
            });

            if (isKapcsolt.Value)
            {
                calculators.Add(IpaKapcsoltCalculation.ReCalculateValues);
                calculators.Add(IpaKapcsoltStatusCalculation.ReCalculateValues);
            }
            else
            {
                calculators.Add(IpaNemKapcsoltCalculation.ReCalculateValues);
            }

            var currentCalculator = calculatorPageIdMap[pageId];

            var index = calculators.IndexOf(currentCalculator);
            for(int i = index; i< calculators.Count; i++)
            {
                calculators[i].Invoke(_service, sessionId);
            }
        }

        public ActionResult DeleteRow(int id)
        {

            return RedirectToAction("AthozottVeszteseg", "Tao");
        }
    }
}