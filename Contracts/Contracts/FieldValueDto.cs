using System;

namespace Contracts.Contracts
{
    public class FieldValueDto
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public int? RowIndex { get; set; }
        public int FieldDescriptorId { get; set; }
        public bool? BoolValue { get; set; }
        public DateTimeOffset? DateValue { get; set; }
        public decimal? DecimalValue { get; set; }
        public string StringValue { get; set; }
    }
}
