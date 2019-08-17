using System;

namespace TaoContracts.Contracts
{
    public class SessionDto
    {
        public Guid Id { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public System.DateTime LastModifyDate { get; set; }
        public string Creator { get; set; }
        public string LastModifer { get; set; }
        public string Name { get; set; }
        public int CustomerId { get; set; }
        public DocumentDto DocumentType { get; set; }

        public string DocumentState { get; set; }
    }
}
