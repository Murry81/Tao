using System.Collections.Generic;
using TaoContracts.Contracts;

namespace Contracts.Contracts
{
    public class ExportReportDto
    {
        public IEnumerable<ExportFieldDescriptorDto> Fields { get; set; }

        public int DocumentId { get; set; }

        public List<int?> RowIds { get; set; }
    }
}
