using System.Collections.Generic;

namespace TaoContracts.Contracts
{
    public class TableDescriptorDto
    {
        public int TableId { get; set; }

        public List<int> FieldDescriptorIds { get; set; }

        // FieldDescriptorId and its captions
        public Dictionary<int, string> Captions { get; set; }

        //The data in the table
        public List<List<FieldDescriptorDto>> FieldValues { get; set; }
    }
}
