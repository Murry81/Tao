using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoDatabaseService.Contracts
{
    public class FieldDescriptorDto
    {
        public int Id { get; set; }
        public bool IsEditable { get; set; }
        public bool IsCaculated { get; set; }
        public bool IsMandatory { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
        public int FieldValueId { get; set; }
        public object FieldValue { get; set; }
        public string TypeName { get; set; }
        public string TypeOptions { get; set; }
    }
}
