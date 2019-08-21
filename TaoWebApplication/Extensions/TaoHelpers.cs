using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using TaoWebApplication.Models;

namespace TaoWebApplication.Extensions
{
    public static class TaoHelpers
    {
        public static MvcHtmlString CreateActualControl<TModel>(this HtmlHelper<TModel> helper, IFieldList model, int i)
        {
            var color = model.Fields[i].IsEditable ? "yellow" : "#e6f2ff";

            if (!model.Fields[i].IsEditable || model.Fields[i].IsCaculated)
            {
                switch (model.Fields[i].TypeName)
                {
                    case "bool":
                        return helper.Label(model.Fields[i].BoolFieldValue ? "Igen" : "Nem", new { @class = "TaoControl", @style = $"width:100%; background-color:{@color};" });

                    case "numeric":
                        return helper.Label(model.Fields[i].DecimalValue?.ToString(), new { @class = "TaoControl", @style = $"width:100%; background-color:{@color}; text-align:right;" });
                    case "date":
                        if (model.Fields[i].DateValue == null)
                        {
                            return helper.Label("", new { @class = "TaoControl", @style = $"width:100%; background-color:{@color};" });
                        }
                        return helper.Label(model.Fields[i].DateValue?.ToString("yyyy-MM-dd"), new { @class = "TaoControl", @style = $"width:100%; background-color:{@color};" });
                    default:
                        return helper.Label(model.Fields[i].StringValue, new { @class = "TaoControl", @style = $"width:100%; background-color:{@color};" });
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
                return helper.CheckBoxFor(m => model.Fields[i].BoolFieldValue, new { @class = "TaoControl", @style = "width:25px; height:25px;" });
            }
            else if (model.Fields[i].TypeName == "date")
            {
                if (model.Fields[i].DateValue == null)
                {
                    return helper.TextBoxFor(m => model.Fields[i].DateValue, new { @type = "date", @class = "datepicker TaoControl", @style = $"width:100%; background-color:{@color};" });
                }
                else
                {
                    return helper.TextBoxFor(m => model.Fields[i].DateValue, model.Fields[i].DateValue.Value.ToString("yyyy-MM-dd"), new { @type = "date", @class = "datepicker TaoControl", @style = $"width:100%; background-color:{@color};" });
                }
            }
            else if (model.Fields[i].TypeName == "numeric")
            {

                return helper.TextBoxFor(m => model.Fields[i].DecimalValue, model.Fields[i].DecimalValue?.ToString("G29", CultureInfo.InvariantCulture), new { @type = "number", step = "any", @class = "text-right form-control TaoControl", @style = $"width:100%; background-color:{@color}; height:25px" });
            }
            else
            {
                return helper.TextBoxFor(m => model.Fields[i].StringValue, new { @class = "TaoControl", @style = $"width:100%; background-color:{@color};" });
            }
        }
    }
}