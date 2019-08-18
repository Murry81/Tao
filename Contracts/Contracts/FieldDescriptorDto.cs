using System;

namespace TaoContracts.Contracts
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
        public string StringValue { get; set; }
        public bool BoolFieldValue { get; set; }
        public DateTimeOffset? DateValue { get; set; }
        public decimal? DecimalValue { get; set; }
        public string TypeName { get; set; }
        public string TypeOptions { get; set; }
        public string OrderCharacter { get; set; }
        public string Description { get; set; }
        public bool? IsSpecial { get; set; }
        public int Order { get; set; }
        public int? SectionGroup { get; set; }
    }
}
