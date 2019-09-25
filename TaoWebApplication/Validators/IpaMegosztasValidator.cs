using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;
using TaoWebApplication.Calculators;

namespace TaoWebApplication.Validators
{
    public class IpaMegosztasValidator : IValidator
    {
        public static Dictionary<ValidatorElement, string> Validate(List<FieldDescriptorDto> fields)
        {
            var result = new Dictionary<ValidatorElement, string>();

            var rows = fields.Select(f => f.RowIndex).Distinct();

            // Check only one Külföld
            var kulfolds = CheckOnlyOneKulfold(fields, result);
            CheckSzekhelys(fields, result, kulfolds);
            CheckF1830(result, fields);
            return result;
        }

        private static void CheckF1830(Dictionary<ValidatorElement, string> result, List<FieldDescriptorDto> fields)
        {
            //	Ha Ideiglenes iparűzési adóátalány levonható része > 0, akkor kötelező és Ideiglenes iparűzési adóátalány teljes összeg >= Ideiglenes iparűzési adóátalány levonható része
            //if 1830 > 0
            //        f1830 >= f1815

            var rowindexes = fields.Select(f => f.RowIndex).Distinct().ToList();
            rowindexes.Sort();

            var fieldsToCheck = fields.Where(f => f.Id == 1830);
            foreach(var f in fieldsToCheck)
            {
                var f1830 = GenericCalculations.GetValue(f.DecimalValue);
                if ( f1830 > 0)
                {
                    var f1815 = GenericCalculations.GetValue(fields.FirstOrDefault(field => field.Id == 1815 && f.RowIndex == field.RowIndex).DecimalValue);
                    if (f1830 < f1815)
                        result.Add(new ValidatorElement { ColumnId = 31, RowIndex = rowindexes.IndexOf(f.RowIndex) }, "Ideiglenes iparűzési adóátalány teljes összeg nem lehet kisebb mint az Ideiglenes iparűzési adóátalány levonható része.");
                }
            }

        }


        private static void CheckSzekhelys(List<FieldDescriptorDto> fields, Dictionary<ValidatorElement, string> result, List<FieldDescriptorDto> kulfolds)
        {
            var rowindexes = fields.Select(f => f.RowIndex).Distinct().ToList();
            rowindexes.Sort();
            var szekhelys = fields.Where(f => f.Id == 1801 && f.StringValue == "Igen").ToList();

            if (szekhelys.Count() > 1)
            {
                for (int i = 1; i < szekhelys.Count(); i++)
                {
                    result.Add(new ValidatorElement { RowIndex = rowindexes.IndexOf(szekhelys[i].RowIndex.Value), ColumnId = 2 }, "Csak egy város lehet székhely.");
                }
            }

            // select külföldi székhely
            foreach (var szekhely in szekhelys)
            {
                if (kulfolds.FirstOrDefault(k => k.RowIndex.Value == szekhely.RowIndex.Value) != null)
                {
                    result.Add(new ValidatorElement { RowIndex = rowindexes.IndexOf(szekhely.RowIndex.Value), ColumnId = 2 }, "Külföld nem lehet székhely.");
                }
            }
        }

        private static List<FieldDescriptorDto> CheckOnlyOneKulfold(List<FieldDescriptorDto> fields, Dictionary<ValidatorElement, string> result)
        {
            var rowindexes = fields.Select(f => f.RowIndex).Distinct().ToList();
            rowindexes.Sort();
            var kulfolds = fields.Where(f => f.Id == 1800 && f.StringValue == "Külföld").ToList();
            if (kulfolds.Count() > 1)
            {
                for (int i = 1; i < kulfolds.Count(); i++)
                {
                    result.Add(new ValidatorElement { RowIndex = rowindexes.IndexOf(kulfolds[i].RowIndex.Value), ColumnId = 1 }, "Csak egy külföld vehető fel.");
                }
            }

            return kulfolds;
        }
    }

    public class ValidatorElement
    {
        public int RowIndex { get; set; }
        public int ColumnId { get; set; }
    }
}