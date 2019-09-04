using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using TaoContracts.Contracts;
using TaoWebApplication.Models;

namespace TaoWebApplication.Extensions
{
    public static class TaoHelpers
    {
        public static MvcHtmlString CreateActualControl<TModel>(this HtmlHelper<TModel> helper, IFieldList model, int i, ModelStateDictionary modelState = null)
        {
            var color = model.Fields[i].IsEditable ? "yellow" : "#e6f2ff";

            if (!model.Fields[i].IsEditable || model.Fields[i].IsCaculated)
            {
                switch (model.Fields[i].TypeName)
                {
                    case "bool":
                        return helper.Label(model.Fields[i].BoolFieldValue ? "Igen" : "Nem", new { @class = "TaoControl", @style = $"width:100%; background-color:{@color};  border:thick;" });

                    case "numeric":
                        return helper.Label(model.Fields[i].DecimalValue.HasValue ? model.Fields[i].DecimalValue.Value.ToString("#,#.00#;(#,#.00#)") : "", new { @class = "TaoControl", @style = $"width:100%; background-color:{@color}; text-align:right;  border:thick;" });
                    case "date":
                        if (model.Fields[i].DateValue == null)
                        {
                            return helper.Label("", new { @class = "TaoControl", @style = $"width:100%; background-color:{@color};  border:thick;" });
                        }
                        return helper.Label(model.Fields[i].DateValue?.ToString("yyyy-MM-dd"), new { @class = "TaoControl", @style = $"width:100%; background-color:{@color};  border:thick;" });
                    default:
                        return helper.Label(model.Fields[i].StringValue, new { @class = "TaoControl", @style = $"width:100%; background-color:{@color};  border:thick;" });
                }
            }

            else if (!string.IsNullOrEmpty(model.Fields[i].TypeOptions))
            {
                var comboItems = new List<SelectListItem>();
                foreach (var item in model.Fields[i].TypeOptions.Split(';'))
                {
                    comboItems.Add(new SelectListItem { Value = item, Text = item });
                }

                return helper.DropDownListFor(m => model.Fields[i].StringValue, new SelectList(comboItems, "Value", "Text", model.Fields[i].StringValue), new { @class = "TaoControl", style = $"width:100%; background-color:{@color};" });
            }
            else if (model.Fields[i].TypeName == "bool")
            {
                return helper.CheckBoxFor(m => model.Fields[i].BoolFieldValue, new { @class = "TaoControl", @style = "width:16px; height:16px;" });
            }
            else if (model.Fields[i].TypeName == "date")
            {
                if (model.Fields[i].DateValue == null)
                {
                    return helper.TextBoxFor(m => model.Fields[i].DateValue, new { @type = "date", @class = "datepicker TaoControl", @style = $"width:100%; background-color:{@color};" });
                }
                else
                {
                    var str = helper.TextBoxFor(m => model.Fields[i].DateValue, model.Fields[i].DateValue.Value.ToString("yyyy-MM-dd"), new { @type = "date", @class = "datepicker TaoControl", @style = $"width:100%; background-color:{@color};" });
                    if (modelState != null &&!modelState.IsValid && modelState.ContainsKey(model.Fields[i].Id.ToString()))
                    {
                       return MvcHtmlString.Create(str.ToString() + $"<span class=\"field-validation-error\">{modelState[model.Fields[i].Id.ToString()].Errors[0].ErrorMessage}</span>");
                     }
                    return str;
                }
            }
            else if (model.Fields[i].TypeName == "numeric")
            {

                return helper.TextBoxFor(m => model.Fields[i].DecimalValue, model.Fields[i].DecimalValue?.ToString("G29", CultureInfo.InvariantCulture), new { @type = "number", step = "any", @class = "text-right form-control TaoControl", @style = $"width:100%; background-color:{@color}; height:25px;" });
            }
            else
            {
                return helper.TextBoxFor(m => model.Fields[i].StringValue, new { @class = "TaoControl", @style = $"width:100%; background-color:{@color};" });
            }
        }


        public static MvcHtmlString CreateActualControlForTable<TModel>(this HtmlHelper<TModel> helper, ITableDescriptor model, int i, int j, int tableIndex, ModelStateDictionary modelState = null)
        {
            var color = model.TableDescriptors[tableIndex].FieldValues[i][j].IsEditable ? "yellow" : "#e6f2ff";

            if (!model.TableDescriptors[tableIndex].FieldValues[i][j].IsEditable || model.TableDescriptors[tableIndex].FieldValues[i][j].IsCaculated)
            {
                switch (model.TableDescriptors[tableIndex].FieldValues[i][j].TypeName)
                {
                    case "bool":
                        return helper.Label(model.TableDescriptors[tableIndex].FieldValues[i][j].BoolFieldValue ? "Igen" : "Nem", new { @class = "TaoControl", @style = $"width:100%; background-color:{@color};  border:thick;" });

                    case "numeric":
                        return helper.Label(model.TableDescriptors[tableIndex].FieldValues[i][j].DecimalValue.HasValue ? model.TableDescriptors[tableIndex].FieldValues[i][j].DecimalValue.Value.ToString("#,#.00#;(#,#.00#)") : "", new { @class = "TaoControl", @style = $"width:100%; background-color:{@color}; text-align:right;  border:thick;" });
                    case "date":
                        if (model.TableDescriptors[tableIndex].FieldValues[i][j].DateValue == null)
                        {
                            return helper.Label("", new { @class = "TaoControl", @style = $"width:100%; background-color:{@color};  border:thick;" });
                        }
                        return helper.Label(model.TableDescriptors[tableIndex].FieldValues[i][j].DateValue?.ToString("yyyy-MM-dd"), new { @class = "TaoControl", @style = $"width:100%; background-color:{@color};  border:thick;" });
                    default:
                        return helper.Label(model.TableDescriptors[tableIndex].FieldValues[i][j].StringValue, new { @class = "TaoControl", @style = $"width:100%; background-color:{@color};  border:thick;" });
                }
            }

            else if (!string.IsNullOrEmpty(model.TableDescriptors[tableIndex].FieldValues[i][j].TypeOptions))
            {
                var comboItems = new List<SelectListItem>();
                foreach (var item in model.TableDescriptors[tableIndex].FieldValues[i][j].TypeOptions.Split(';'))
                {
                    comboItems.Add(new SelectListItem { Value = item, Text = item });
                }

                return helper.DropDownListFor(m => model.TableDescriptors[tableIndex].FieldValues[i][j].StringValue, new SelectList(comboItems, "Value", "Text", model.TableDescriptors[tableIndex].FieldValues[i][j].StringValue), new { @class = "TaoControl", style = $"width:100%; background-color:{@color};" });
            }
            else if (model.TableDescriptors[tableIndex].FieldValues[i][j].TypeName == "bool")
            {
                return helper.CheckBoxFor(m => model.TableDescriptors[tableIndex].FieldValues[i][j].BoolFieldValue, new { @class = "TaoControl", @style = "width:16px; height:16px;" });
            }
            else if (model.TableDescriptors[tableIndex].FieldValues[i][j].TypeName == "date")
            {
                if (model.TableDescriptors[tableIndex].FieldValues[i][j].DateValue == null)
                {
                    return helper.TextBoxFor(m => model.TableDescriptors[tableIndex].FieldValues[i][j].DateValue, new { @type = "date", @class = "datepicker TaoControl", @style = $"width:100%; background-color:{@color};" });
                }
                else
                {
                    var str = helper.TextBoxFor(m => model.TableDescriptors[tableIndex].FieldValues[i][j].DateValue, model.TableDescriptors[tableIndex].FieldValues[i][j].DateValue.Value.ToString("yyyy-MM-dd"), new { @type = "date", @class = "datepicker TaoControl", @style = $"width:100%; background-color:{@color};" });
                    if (modelState != null && !modelState.IsValid && modelState.ContainsKey(model.TableDescriptors[tableIndex].FieldValues[i][j].Id.ToString()))
                    {
                        return MvcHtmlString.Create(str.ToString() + $"<span class=\"field-validation-error\">{modelState[model.TableDescriptors[tableIndex].FieldValues[i][j].Id.ToString()].Errors[0].ErrorMessage}</span>");
                    }
                    return str;
                }
            }
            else if (model.TableDescriptors[tableIndex].FieldValues[i][j].TypeName == "numeric")
            {

                return helper.TextBoxFor(m => model.TableDescriptors[tableIndex].FieldValues[i][j].DecimalValue, model.TableDescriptors[tableIndex].FieldValues[i][j].DecimalValue?.ToString("G29", CultureInfo.InvariantCulture), new { @type = "number", step = "any", @class = "text-right form-control TaoControl", @style = $"width:100%; background-color:{@color}; height:25px;" });
            }
            else
            {
                return helper.TextBoxFor(m => model.TableDescriptors[tableIndex].FieldValues[i][j].StringValue, new { @class = "TaoControl", @style = $"width:100%; background-color:{@color};" });
            }
        }
    }
}