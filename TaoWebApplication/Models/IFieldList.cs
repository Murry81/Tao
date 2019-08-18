using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaoContracts.Contracts;

namespace TaoWebApplication.Models
{
    public interface IFieldList
    {
        List<FieldDescriptorDto> Fields { get; set; }
    }
}
