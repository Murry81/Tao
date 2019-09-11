using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoContracts.Contracts
{
    public class BaseFieldDescriptorDto
    {
        public int Id { get; set; }
        public bool IsEditable { get; set; }
        public bool IsCaculated { get; set; }
        public bool IsMandatory { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
        public string TypeName { get; set; }
        public string TypeOptions { get; set; }
        public string OrderCharacter { get; set; }
        public string Description { get; set; }
        public bool? IsSpecial { get; set; }
        public int? Order { get; set; }
        public int? SectionGroup { get; set; }
        public string HtmlClass { get; set; }
    }
}
