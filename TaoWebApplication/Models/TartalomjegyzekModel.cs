using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaoDatabaseService.Contracts;

namespace TaoWebApplication.Models
{
    public class TartalomjegyzekModel
    {
        public List<PageDto> Pages { get; set; }
        public List<PageDescriptorDto> PageDescriptors { get; set; }
        public List<FieldDescriptorDto> Fields { get; set; }
    }
}