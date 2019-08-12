using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaoDatabaseService.Contracts;

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