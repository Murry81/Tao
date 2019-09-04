using System.Collections.Generic;
using TaoContracts.Contracts;

namespace TaoWebApplication.Models
{
    public class IpaKapcsoltStatusModel : ModelBase, ITableDescriptor
    {
        public List<TableDescriptorDto> TableDescriptors { get; set; } 
    }
}