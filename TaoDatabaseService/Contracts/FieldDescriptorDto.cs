using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public Guid? FieldValueId { get; set; }

        [RegularExpression("^[1-9][0-9]+", ErrorMessage = "Számot kell megadni.")]
        public string FieldValue { get; set; }
        public string TypeName { get; set; }
        public string TypeOptions { get; set; }
        public string OrderCharacter { get; set; }
        public string Description { get; set; }
        public bool? IsSpecial { get; set; }
    }
}
