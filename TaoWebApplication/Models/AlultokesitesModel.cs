using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaoContracts.Contracts;

namespace TaoWebApplication.Models
{
    public class AlultokesitesModel : ModelBase, ITableDescriptor
    {
        public List<TableDescriptorDto> TableDescriptors { get; set; }

        public List<FieldDescriptorDto> FillDeafultRows(DateTimeOffset endOfBusinessYear)
        {
            var result = new List<FieldDescriptorDto>();

            if (TableDescriptors[0].FieldValues.Count == 1)
            {
                //Add the values
                //jegyzett tőke
                //tőketartalék
                //eredménytartalék
                //lekötött tartalék

                result.Add(new FieldDescriptorDto
                {
                    Id = 903,
                    TypeName = "date",
                    DateValue = endOfBusinessYear.AddYears(-1),
                    RowIndex = 0
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 904,
                    StringValue = "Jegyzett tőke",
                    RowIndex = 0
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 905,
                    DecimalValue = 0,
                    RowIndex = 0
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 903,
                    TypeName = "date",
                    DateValue = endOfBusinessYear.AddYears(-1),
                    RowIndex = 1
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 904,
                    StringValue = "Tőketartalék",
                    RowIndex = 1
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 905,
                    DecimalValue = 0,
                    RowIndex = 1
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 903,
                    TypeName = "date",
                    DateValue = endOfBusinessYear.AddYears(-1),
                    RowIndex = 2
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 904,
                    StringValue = "Eredménytartalék",
                    RowIndex = 2
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 905,
                    DecimalValue = 0,
                    RowIndex = 2
                });
                result.Add(new FieldDescriptorDto
                  {
                      Id = 903,
                      TypeName = "date",
                      DateValue = endOfBusinessYear.AddYears(-1),
                      RowIndex = 3
                  });
                result.Add(new FieldDescriptorDto
                {
                    Id = 904,
                    StringValue = "Lekötött tartalék",
                    RowIndex = 3
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 905,
                    DecimalValue = 0,
                    RowIndex = 3
                });
            }


            if (TableDescriptors[0].FieldValues.Count == 1)
            {
                //Előző év záró; Folyósítás; Törlesztés
                result.Add(new FieldDescriptorDto
                {
                    Id = 906,
                    TypeName = "date",
                    DateValue = endOfBusinessYear.AddYears(-1),
                    RowIndex = 0
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 907,
                    StringValue = "Előző év záró",
                    RowIndex = 0
                });
                result.Add(new FieldDescriptorDto
                {
                    Id = 908,
                    DecimalValue = 0,
                    RowIndex = 0
                });
            }
            return result;
        }

        internal void MakeDefaultRowsReadonly()
        {
            foreach(var row in this.TableDescriptors[0].FieldValues)
            {
               foreach(var item in row.Where(r => r.RowIndex < 4))
                {
                    item.IsEditable = false;
                }
            }
            foreach (var row in this.TableDescriptors[1].FieldValues)
            {
                foreach (var item in row.Where(r => r.RowIndex < 1))
                {
                    item.IsEditable = false;
                }
            }
        }

        internal List<FieldDescriptorDto> RemoveDefaultFieldsBeforeSave(List<TableDescriptorDto> tables)
        {
            var result = new List<FieldDescriptorDto>();
            foreach (var table in tables)
            {
                var ignoreRows = table.FieldValues.FirstOrDefault().Any(t => t.Id == 903) ? 4 : 1;
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