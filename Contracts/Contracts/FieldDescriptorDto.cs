using System;

namespace TaoContracts.Contracts
{
    public class FieldDescriptorDto : BaseFieldDescriptorDto
    {
       
        public Guid? FieldValueId { get; set; }
        public string StringValue { get; set; }
        public bool BoolFieldValue { get; set; }
        public DateTimeOffset? DateValue { get; set; }
        public decimal? DecimalValue { get; set; }
        public int? RowIndex { get; set; }
    }
}
