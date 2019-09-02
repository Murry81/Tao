using TaoContracts.Contracts;

namespace TaoWebApplication.Models
{
    public interface ITableDescriptor
    {
        TableDescriptorDto TableDescriptor { get; set; }
    }
}