using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaoWebApplication.Models
{
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var cultureCookie = controllerContext.HttpContext.Request.Cookies["language"];

            var culture = "en-US";

            if (cultureCookie != null)
                culture = cultureCookie.Value;

            decimal value;

            var valueProvider = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProvider == null)
                return null;

            if (String.IsNullOrEmpty(valueProvider.AttemptedValue))
                return null;

            if (Decimal.TryParse(valueProvider.AttemptedValue, NumberStyles.Currency, new CultureInfo(culture), out value))
            {
                return value;
            }

            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Invalid decimal");

            return null;
        }
    }
}