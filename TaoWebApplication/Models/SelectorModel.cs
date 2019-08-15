using System.Collections.Generic;
using TaoContracts.Contracts;

namespace TaoWebApplication.Models
{
    public class SelectorModel
    {
        public int? SessionId { get; set; }

        public string SelectedCustomerId { get; set; }

        public CustomerDto SelectedCustomer { get; set; }

        public List<CustomerDto> Customers { get; set; }

        public List<SessionDto> CustomerSessions { get; set; }

        public int SelectedCustomerSessionId { get; set; }

        public List<DocumentDto> DocumentTypes { get; set; }

        public int SelectedDocumentType { get; set; }
    }
}