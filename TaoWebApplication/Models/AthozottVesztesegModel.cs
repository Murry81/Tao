using System;
using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Models
{
    public class AthozottVesztesegModel : ModelBase, ITableDescriptor
    {
        public List<TableDescriptorDto> TableDescriptors { get; set; }

        public List<FieldDescriptorDto> FillDeafultRows(DateTimeOffset endOfBusinessYear, Guid sessionId, IDataService service)
        {
            var result = new List<FieldDescriptorDto>();
            var uzletiEvVege = service.GetFieldById(32, sessionId).DateValue.Value;

            if (TableDescriptors[1].FieldValues.Count == 1)
            {
                //Előző év záró; Folyósítás; Törlesztés
                result.Add(new FieldDescriptorDto
                {
                    Id = 1905,
                    StringValue = "2004",
                    RowIndex = 0
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 1906,
                    StringValue = "korlátlan",
                    RowIndex = 0
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 1905,
                    StringValue = "2005-2016",
                    RowIndex = 1
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 1906,
                    StringValue = "2026-12-31",
                    RowIndex = 1
                });
                var today = new DateTime(2017, 12, 31);
                var rowindex = 2;
                while (today.Year <= uzletiEvVege.Year)
                {
                    result.Add(new FieldDescriptorDto
                    {
                        Id = 1905,
                        StringValue = today.Year.ToString(),
                        RowIndex = rowindex
                    });
                    result.Add(new FieldDescriptorDto
                    {
                        Id = 1906,
                        StringValue = today.AddYears(5).ToString("yyyy-MM-dd"),
                        RowIndex = rowindex
                    });
                    rowindex++;
                    today = today.AddYears(1);
                }
            }

            // Üzleti év vége f32
            
            if (TableDescriptors[0].FieldValues.Count == 1)
            {
                //Előző év záró; Folyósítás; Törlesztés
                result.Add(new FieldDescriptorDto
                {
                    Id = 1900,
                    DateValue = new DateTime(2004, uzletiEvVege.Month, uzletiEvVege.Day),
                    TypeName = "date",
                    RowIndex = 0
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 1901,
                    StringValue = "Nyitó",
                    RowIndex = 0
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 1903,
                    StringValue = "2004",
                    RowIndex = 0
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 1904,
                    StringValue = "Korlátlan",
                    RowIndex = 0
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 1900,
                    DateValue = new DateTime(2016, uzletiEvVege.Month, uzletiEvVege.Day),
                    TypeName = "date",
                    RowIndex = 1
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 1901,
                    StringValue = "Nyitó",
                    RowIndex = 1
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 1903,
                    StringValue = "2005-2016",
                    RowIndex = 1
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 1904,
                    StringValue = "2026-12-31",
                    RowIndex = 1
                });

                int rowindex = 2;
                var today = new DateTime(2017, uzletiEvVege.Month, uzletiEvVege.Day);
                while (today.Year <= uzletiEvVege.Year)
                {
                    result.Add(new FieldDescriptorDto
                    {
                        Id = 1900,
                        DateValue = today,
                        TypeName = "date",
                        RowIndex = rowindex
                    });
                    result.Add(new FieldDescriptorDto
                    {
                        Id = 1901,
                        StringValue = "Nyitó",
                        RowIndex = rowindex
                    });
                    result.Add(new FieldDescriptorDto
                    {
                        Id = 1903,
                        StringValue = today.Year.ToString(),
                        RowIndex = rowindex
                    });
                    result.Add(new FieldDescriptorDto
                    {
                        Id = 1904,
                        StringValue = new DateTime(today.AddYears(5).Year, 12, 31).ToString("yyyy-MM-dd"),
                        RowIndex = rowindex
                    });
                    rowindex++;
                    today = today.AddYears(1);
                }

                result.FirstOrDefault(f => f.Id == 1901 && f.RowIndex == rowindex - 1).StringValue = "Tárgyévi veszteség";
            }
            return result;
        }

        internal void MakeDefaultRowsReadonly()
        {
            foreach (var row in TableDescriptors[1].FieldValues)
            {
                foreach (var item in row)
                {
                    item.IsEditable = false;
                }
            }

            var rowindex = 2 + DateTime.UtcNow.Year - 2017;
            List<int> rowindexesToFreeze = new List<int> { 1900, 1901, 1903, 1904 };
            foreach (var row in TableDescriptors[0].FieldValues)
            {
                foreach (var item in row.Where(f => f.RowIndex <= rowindex &&  rowindexesToFreeze.Contains(f.Id)))
                {
                    item.IsEditable = false;
                }

                if(row.FirstOrDefault(f=>f.Id == 1901).StringValue == "Tárgyévi veszteség")
                {
                    row.FirstOrDefault(f => f.Id == 1902).IsEditable = false;
                }
            }
        }

        internal void SetUpKepzesEveCombo(IDataService service, Guid sessionId)
        {
            var fields = string.Join(";",  service.GetFieldValuesById(1905, sessionId).OrderBy(f=> f.RowIndex).Select(f => f.StringValue).Distinct().ToList());
            fields = $";{fields}";

            foreach (var row in TableDescriptors[0].FieldValues)
            {
                foreach (var item in row.Where(f => f.Id == 1903))
                {
                    item.TypeOptions = fields;
                }
            }
        }

        internal List<FieldDescriptorDto> RemoveDefaultFieldsBeforeSave(List<TableDescriptorDto> tables)
        {
            var result = new List<FieldDescriptorDto>();
            foreach (var table in tables)
            {
                foreach (var list in table.FieldValues)
                    foreach(var item in list.Where(f => f.IsEditable))
                    {
                        result.Add(item);
                    }
            }

            return result;
        }
    }
}