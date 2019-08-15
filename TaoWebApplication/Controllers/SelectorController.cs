using System.Linq;
using System.Web.Mvc;
using TaoDatabaseService.Interfaces;
using TaoWebApplication.Models;

namespace TaoWebApplication.Controllers
{
    public class SelectorController : Controller
    {
        private readonly IDataService _service;

        public SelectorController(IDataService service)
        {
            _service = service;
        }

        // GET: Selector
        public ActionResult Index()
        {
            var customers = _service.GetCustomers();
            var selectedCustomer = customers.First();
            return View("~/Views/Tao/Selector.cshtml", new SelectorModel
            {
                Customers = customers,
                SelectedCustomerId = selectedCustomer.Id.ToString(),
                SelectedCustomer = selectedCustomer,
                CustomerSessions = _service.GetCustomerSessions(selectedCustomer.Id),
                DocumentTypes = _service.GetAllDocumentType()
            });
        }

        [HttpPost]
        public ActionResult SelectCustomer(string customerId)
        {
            var selectedCustomerId = Request.Form["SelectedCustomerId"];
            var customers = _service.GetCustomers();
            var sessions = _service.GetCustomerSessions(int.Parse(selectedCustomerId));
            return View("~/Views/Tao/Selector.cshtml", new SelectorModel
            {
                Customers = customers,
                SelectedCustomerId = selectedCustomerId,
                SelectedCustomer = customers.FirstOrDefault(c => c.Id.ToString() == selectedCustomerId),
                CustomerSessions = sessions,
                DocumentTypes = _service.GetAllDocumentType()
            });
        }

        [HttpPost]
        public ActionResult SelectCustomerDocument(string customerId)
        {
            var selectedCustomerId = Request.Form["SelectedCustomer.Id"];
            var customers = _service.GetCustomers();
            var sessions = _service.GetCustomerSessions(int.Parse(selectedCustomerId));

            var selectedSessionId = string.Empty;
            if (Request.Form.AllKeys.Contains("Continue"))
            {
               selectedSessionId = Request.Form["Continue"];
            }
            

            return View("~/Views/Tao/Selector.cshtml", new SelectorModel
            {
                Customers = customers,
                SelectedCustomerId = selectedCustomerId,
                SelectedCustomer = customers.FirstOrDefault(c => c.Id.ToString() == selectedCustomerId),
                CustomerSessions = sessions,
                DocumentTypes = _service.GetAllDocumentType()
            });
        }
    }
}