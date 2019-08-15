using System.Collections.Generic;
using TaoContracts.Contracts;

namespace TaoWebApplication.Models
{
    public class ModelBase
    {
        public int SessionId { get; set; }
        public string CustomerId { get; set; }
        public List<PageDto> Pages { get; set; }
        public PageDto CurrentPage { get; set; }
        public List<PageDescriptorDto> PageDescriptors { get; set; }
        public List<FieldDescriptorDto> Fields { get; set; }
    }
}