using System.Collections.Generic;
using TaoContracts.Contracts;

namespace TaoWebApplication.Models
{
    public interface ITableDescriptor
    {
        List<TableDescriptorDto> TableDescriptors { get; set; }
    }
}