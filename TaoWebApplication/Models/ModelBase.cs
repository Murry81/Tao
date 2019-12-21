using System.Collections.Generic;
using TaoContracts.Contracts;

namespace TaoWebApplication.Models
{
    public class ModelBase : IFieldList
    {
        public int? SessionId { get; set; }
        public string CustomerId { get; set; }
        public CustomerDto Customer { get; set; }
        public List<PageDto> Pages { get; set; }
        public PageDto CurrentPage { get; set; }
        public List<PageDescriptorDto> PageDescriptors { get; set; }
        public List<FieldDescriptorDto> Fields { get; set; }
        
        // Need for the navigation menu
        public bool? IpaKapcsolt { get; set; }
    }
}