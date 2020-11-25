using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaoContracts.Contracts;

namespace TaoWebApplication.Models
{
    public class TaggalSzembeniKotelezettsegModel : ModelBase, ITableDescriptor
    {
        public List<TableDescriptorDto> TableDescriptors { get; set; }

        public List<FieldDescriptorDto> FillDeafultRows(DateTimeOffset endOfBusinessYear)
        {
            var result = new List<FieldDescriptorDto>();

            if (TableDescriptors[0].FieldValues.Count == 1)
            {
                //Előző év záró; Folyósítás; Törlesztés
                result.Add(new FieldDescriptorDto
                {
                    Id = 1000,
                    TypeName = "date",
                    DateValue = endOfBusinessYear.AddYears(-1),
                    RowIndex = 0
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 1001,
                    StringValue = "Előző év záró",
                    RowIndex = 0
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 1002,
                    DecimalValue = 0,
                    RowIndex = 0
                });
            }
            return result;
        }

        internal void MakeDefaultRowsReadonly()
        {
            foreach (var row in TableDescriptors[0].FieldValues)
            {
                foreach (var item in row.Where(r => r.RowIndex < 1))
                {
                    item.IsEditable = false;
                }
            }
        }

        internal static List<FieldDescriptorDto> RemoveDefaultFieldsBeforeSave(List<TableDescriptorDto> tables)
        {
            var result = new List<FieldDescriptorDto>();
            foreach (var table in tables)
            {
                var ignoreRows = 1;
                foreach (var list in table.FieldValues)
                {
                    if (list.Any(t => t.RowIndex < ignoreRows))
                    {
                        continue;
                    }
                    result.AddRange(list);
                }
            }

            return result;
        }
    }
}