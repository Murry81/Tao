using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoDatabaseService.Contracts
{
   public class PageDescriptorDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string OrderCharacter { get; set; }
        public string Description { get; set; }
        public int PageId { get; set; }
        public int? SectionGroup { get; set; }
        public int? FieldId { get; set; }

    }
}
