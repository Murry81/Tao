﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CustomerNameTaoEntities : DbContext
    {
        public CustomerNameTaoEntities()
            : base("name=CustomerNameTaoEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<ContactDetail> ContactDetail { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<DocumentExportType> DocumentExportType { get; set; }
        public virtual DbSet<DocumentState> DocumentState { get; set; }
        public virtual DbSet<DocumentType> DocumentType { get; set; }
        public virtual DbSet<FieldDescriptor> FieldDescriptor { get; set; }
        public virtual DbSet<FieldValue> FieldValue { get; set; }
        public virtual DbSet<Page> Page { get; set; }
        public virtual DbSet<PageDescriptor> PageDescriptor { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<DocumentExport> DocumentExport { get; set; }
        public virtual DbSet<TableDescriptor> TableDescriptor { get; set; }
        public virtual DbSet<TaxesByCities> TaxesByCities { get; set; }
    }
}
