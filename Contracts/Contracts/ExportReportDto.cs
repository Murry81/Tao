using System.Collections.Generic;
using TaoContracts.Contracts;

namespace Contracts.Contracts
{
    public class ExportReportDto
    {
        public IEnumerable<FieldDescriptorDto> Fields { get; set; }
        public int DocumentId { get; set; }
    }
}
