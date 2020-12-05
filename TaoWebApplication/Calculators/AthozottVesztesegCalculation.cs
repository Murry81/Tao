using System;
using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;
using TaoDatabaseService;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class AthozottVesztesegCalculation
    {
        public static void ReCalculateValues(IDataService service, Guid sessionId)
        {
            var fields = service.GetAllPageFieldValues(19, sessionId);

            Dictionary<int, List<FieldValue>> table1 = CreateTable1(fields);
            Dictionary<int, List<FieldValue>> table2 = CreateTable2(fields);

            CalculateTargyeviVeszteseg(service, sessionId, fields);
            CalculateTable1(table1, table2, fields, sessionId);
            CalculateTable2(table1, table2, fields, sessionId);

            Calculate1912(fields, sessionId);
            Calculate1913(fields, sessionId);
            Calculate1914(fields, sessionId);
            Calculate1915(fields, sessionId);

            service.SaveValues(fields, sessionId);
        }

        private static void Calculate1915(List<FieldValue> fields, Guid sessionId)
        {
            // Szum (Felhasználás)
            // Szum (f1909 )
            var f1915 = fields.FirstOrDefault(f => f.FieldDescriptorId == 1915);
            var value = fields.Where(f => f.FieldDescriptorId == 1909).Select(f => GenericCalculations.GetValue(f.DecimalValue)).Sum();

            if (f1915 != null)
            {
                f1915.DecimalValue = value != 0 ? value : (decimal?)null;
            }

            if (f1915 == null && value != 0)
            {
                f1915 = new FieldValue
                {
                    FieldDescriptorId = 1915,
                    SessionId = sessionId,
                    Id = Guid.NewGuid(),
                    DecimalValue = value
                };

                fields.Add(f1915);
            }
        }

        private static void Calculate1914(List<FieldValue> fields, Guid sessionId)
        {
            // Korlátlanul felhasználható + 50%-ig felhasználható
            // f1912 + f1913
            var f1914 = fields.FirstOrDefault(f => f.FieldDescriptorId == 1914);
            var f1912 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.FieldDescriptorId == 1912)?.DecimalValue);
            var f1913 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.FieldDescriptorId == 1913)?.DecimalValue);

            var value = f1912 + f1913;
            if (f1914 != null)
            {
                f1914.DecimalValue = value != 0 ? value : (decimal?)null;
            }

            if (f1914 == null && value != 0)
            {
                f1914 = new FieldValue
                {
                    FieldDescriptorId = 1914,
                    SessionId = sessionId,
                    Id = Guid.NewGuid(),
                    DecimalValue = value
                };

                fields.Add(f1914);
            }
        }

        private static void Calculate1913(List<FieldValue> fields, Guid sessionId)
        {
            // (Szum (Nyitó) - Nyitó 2004) - (Szum (Elévülés) - Elévülés 2004)
            // azaz Szum (Nyitó) - Szum (Elévülés) ahol év nem 2004
            var f1913 = fields.FirstOrDefault(f => f.FieldDescriptorId == 1913);
            var rowIndex = fields.Where(f => f.FieldDescriptorId == 1905 && f.StringValue != "2004").Select(r=> r.RowIndex).ToList();

            //Nyitó
            var sumf1907 = fields.Where(f => f.FieldDescriptorId == 1907 && rowIndex.Contains( f.RowIndex )).Select(f=> GenericCalculations.GetValue(f.DecimalValue)).Sum();
            //Elévülés
            var sumf1908 = fields.Where(f => f.FieldDescriptorId == 1908 && rowIndex.Contains(f.RowIndex)).Select(f => GenericCalculations.GetValue(f.DecimalValue)).Sum();

            var value = sumf1907 - sumf1908;
            if (f1913 != null)
            {
                f1913.DecimalValue = value != 0 ? value : (decimal?)null;
            }

            if (f1913 == null && value != 0)
            {
                f1913 = new FieldValue
                {
                    FieldDescriptorId = 1913,
                    SessionId = sessionId,
                    Id = Guid.NewGuid(),
                    DecimalValue = value
                };

                fields.Add(f1913);
            }
        }

        private static void Calculate1912(List<FieldValue> fields, Guid sessionId)
        {
            // Nyitó 2004 - Elévülés 2004

            var f1912 = fields.FirstOrDefault(f => f.FieldDescriptorId == 1912);
            var rowIndex = fields.FirstOrDefault(f => f.FieldDescriptorId == 1905 && f.StringValue == "2004")?.RowIndex;

            if (rowIndex == null)
                return;

            //Nyitó
            var f1907 = GenericCalculations.GetValue( fields.FirstOrDefault(f => f.FieldDescriptorId == 1907 && f.RowIndex == rowIndex)?.DecimalValue);
            //Elévülés
            var f1908 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.FieldDescriptorId == 1908 && f.RowIndex == rowIndex)?.DecimalValue);

            var value = f1907 - f1908;
            if (f1912 != null)
            {
                f1912.DecimalValue = value != 0 ? value : (decimal?)null;
            }

            if (f1912 == null && value != 0)
            {
                f1912 = new FieldValue
                {
                    FieldDescriptorId = 1912,
                    SessionId = sessionId,
                    Id = Guid.NewGuid(),
                    DecimalValue = value
                };

                fields.Add(f1912);
            }
        }

        private static void CalculateTargyeviVeszteseg(IDataService service, Guid sessionId, List<FieldValue> fields)
        {
            // select rowindex
            var rowindex = fields.FirstOrDefault(f => f.FieldDescriptorId == 1901 && f.StringValue == "Tárgyévi veszteség")?.RowIndex;

            if (rowindex == null)
                return;

            //select f1902
            var f1902 = fields.FirstOrDefault(f => f.FieldDescriptorId == 1902 && f.RowIndex == rowindex);

            // -1 * 4.Adóalap, ha 4.Adóalap < 0, különben 0
            // -1 * f1411 ha f1411 < 0
            var f1411 = GenericCalculations.GetValue(service.GetFieldById(1411, sessionId).DecimalValue);

            decimal value = 0;
            if(f1411 < 0)
            {
                value = -1 * f1411;
            }

            if (f1902 != null)
            {
                f1902.DecimalValue = value != 0 ? value : (decimal?)null;
            }

            if (f1902 == null && value != 0)
            {
                f1902 = new FieldValue
                {
                    FieldDescriptorId = 1911,
                    RowIndex = rowindex,
                    SessionId = sessionId,
                    Id = Guid.NewGuid(),
                    DecimalValue = value
                };

                fields.Add(f1902);
            }
        }

        private static void CalculateTable1(Dictionary<int, List<FieldValue>> table1, Dictionary<int, List<FieldValue>> table2, List<FieldValue> fields, Guid sessionId)
        {
            foreach(var row in table1.Keys)
            {
                var list = table1[row];

                var field1905 = list.FirstOrDefault(f => f.FieldDescriptorId == 1905);
                // Nyitó
                var field1907 = list.FirstOrDefault(f => f.FieldDescriptorId == 1907);
                Calculate1907_1910(field1907, row, table2, field1905, fields, sessionId, "Nyitó", 1907);

                // Elévülés
                var field1908 = list.FirstOrDefault(f => f.FieldDescriptorId == 1908);
                Calculate1907_1910(field1908, row, table2, field1905, fields, sessionId, "Elévülés", 1908);

                // Felhasználás
                var field1909 = list.FirstOrDefault(f => f.FieldDescriptorId == 1909);
                Calculate1907_1910(field1909, row, table2, field1905, fields, sessionId, "Felhasználás", 1909);
                
                //Tárgyévi veszteség
                var field1910 = list.FirstOrDefault(f => f.FieldDescriptorId == 1910);
                Calculate1907_1910(field1910, row, table2, field1905, fields, sessionId, "Tárgyévi veszteség", 1910);

                //Egyenleg
                var field1911 = list.FirstOrDefault(f => f.FieldDescriptorId == 1911);
                Calculate1911(list, fields, sessionId, field1911, row);
            }
        }

        private static void Calculate1911(List<FieldValue> list, List<FieldValue> fields, Guid sessionId, FieldValue field1911, int row)
        {
            // Nyitó - Elévülés - Felhasználás + Tárgyévi veszteség
            // f1907 - f1908 - f1909 + f1910 
            var f1907 =  GenericCalculations.GetValue( list.FirstOrDefault(f => f.FieldDescriptorId == 1907)?.DecimalValue);
            var f1908 = GenericCalculations.GetValue(list.FirstOrDefault(f => f.FieldDescriptorId == 1908)?.DecimalValue);
            var f1909 = GenericCalculations.GetValue(list.FirstOrDefault(f => f.FieldDescriptorId == 1909)?.DecimalValue);
            var f1910 = GenericCalculations.GetValue(list.FirstOrDefault(f => f.FieldDescriptorId == 1910)?.DecimalValue);

            var result = f1907 - f1908 - f1909 + f1910;

            if (field1911 != null)
            {
                field1911.DecimalValue = result != 0 ? result : (decimal?)null;
            }

            if (field1911 == null && result != 0)
            {
                field1911 = new FieldValue
                {
                    FieldDescriptorId = 1911,
                    RowIndex = row,
                    SessionId = sessionId,
                    Id = Guid.NewGuid(),
                    DecimalValue = result
                };

                fields.Add(field1911);
            }
        }

        private static void Calculate1907_1910(FieldValue field1907, int row, Dictionary<int, List<FieldValue>> table2, FieldValue field1905, List<FieldValue> fields, Guid sessionId, string kepzes, int fieldId)
        {
            var rows = AffectedRowNumbers(fields, field1905.StringValue);
            decimal result = 0;
            foreach(int anotherRow in rows)
            {
                if (table2[anotherRow].Any(f => f.FieldDescriptorId == 1901 && f.StringValue == kepzes))
                {
                    result += GenericCalculations.GetValue(table2[anotherRow].FirstOrDefault(f => f.FieldDescriptorId == 1902)?.DecimalValue);
                }
            }

            if (field1907 != null)
            {
                field1907.DecimalValue = result > 0 ? result : (decimal?)null;
            }

            if (field1907 == null && result > 0)
            {
                field1907 = new FieldValue
                {
                    FieldDescriptorId = fieldId,
                    RowIndex = row,
                    SessionId = sessionId,
                    Id = Guid.NewGuid(),
                    DecimalValue = result
                };

                fields.Add(field1907);
            }            
        }


        private static void CalculateTable2(Dictionary<int, List<FieldValue>> table1, Dictionary<int, List<FieldValue>> table2, List<FieldValue> fields, Guid sessionId)
        {
            Dictionary<string, string> kepzesToEsedekesseg = new Dictionary<string, string>();

            foreach(var row in table1.Values)
            {
                kepzesToEsedekesseg.Add(row.FirstOrDefault(f => f.FieldDescriptorId == 1905).StringValue, row.FirstOrDefault(f => f.FieldDescriptorId == 1906).StringValue);
            }

            foreach (var row in table2.Keys)
            {
                var list = table2[row];

                //esedékesség
                var f1904 = list.FirstOrDefault(f => f.FieldDescriptorId == 1904);

                //Képzés éve
                var f1903 = list.FirstOrDefault(f => f.FieldDescriptorId == 1903).StringValue;

                var value = kepzesToEsedekesseg[f1903];

                if (f1904 != null)
                {
                    f1904.StringValue = value;
                }

                if (f1904 == null && !string.IsNullOrEmpty(value))
                {
                    f1904 = new FieldValue
                    {
                        FieldDescriptorId = 1904,
                        RowIndex = row,
                        SessionId = sessionId,
                        Id = Guid.NewGuid(),
                        StringValue = value
                    };

                    fields.Add(f1904);
                }
            }
        }

        private static Dictionary<int, List<FieldValue>> CreateTable1(List<FieldValue> fields)
        {
            var tableOneFields = new List<int> { 1905, 1906, 1907, 1908, 1909, 1910, 1911 };
            var result = new Dictionary<int, List<FieldValue>>();
            var rows = fields.Where(f => f.FieldDescriptorId == 1905).Select(f => f.RowIndex).ToList();
            foreach (var row in rows)
            {
                result.Add(row.Value, fields.Where(f => f.RowIndex == row && tableOneFields.Contains(f.FieldDescriptorId)).ToList());
            }

            return result;
        }

        private static Dictionary<int, List<FieldValue>> CreateTable2(List<FieldValue> fields)
        {
            var tableTwoFields = new List<int> { 1900, 1901, 1902, 1903, 1904 };
            var result = new Dictionary<int, List<FieldValue>>();
            var rows = fields.Where(f => f.FieldDescriptorId == 1900).Select(f => f.RowIndex).ToList();
            foreach (var row in rows)
            {
                result.Add(row.Value, fields.Where(f => f.RowIndex == row && tableTwoFields.Contains(f.FieldDescriptorId)).ToList());
            }

            return result;
        }

        private static List<int?> AffectedRowNumbers(List<FieldValue> fields, string dateString)
        {
            return fields.Where(f => f.FieldDescriptorId == 1903 && f.StringValue == dateString).Select(f => f.RowIndex).ToList();
        }
    }
}