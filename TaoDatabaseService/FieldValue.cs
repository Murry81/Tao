//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TaoDatabaseService
{
    using System;
    using System.Collections.Generic;
    
    public partial class FieldValue
    {
        public System.Guid Id { get; set; }
        public int FieldDescriptorId { get; set; }
        public Nullable<decimal> DecimalValue { get; set; }
        public string StringValue { get; set; }
        public Nullable<System.DateTimeOffset> DateValue { get; set; }
        public Nullable<bool> BoolValue { get; set; }
        public System.Guid SessionId { get; set; }
        public Nullable<int> RowIndex { get; set; }
    
        public virtual FieldDescriptor FieldDescriptor { get; set; }
        public virtual Session Session { get; set; }
    }
}
