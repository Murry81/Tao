using System;
using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;
using TaoWebApplication.Calculators;

namespace TaoWebApplication.Validators
{
    public class IpaMegosztasValidator : IValidator
    {
        public static Dictionary<string, string> Validate(List<FieldDescriptorDto> fields)
        {
            var result = new Dictionary<string, string>();

            var rows = fields.Select(f => f.RowIndex).Distinct();

            // Check only one Külföld
            var kulfolds = CheckOnlyOneKulfold(fields, result);
            CheckSzekhelys(fields, result, kulfolds);
            CheckF1830(result, fields);
            CheckMegosztasModja(result, fields);
            return result;
        }

        private static void CheckMegosztasModja(Dictionary<string, string> result, List<FieldDescriptorDto> fields)
        {
            //Az eszközérték nem választható ki, ha székhelynek jelölt település rekordjában Eszközérték = 0.
            // szekhyely => f1801, eszkozérték => f1807

            var szekhely = fields.Where(f => f.Id == 1801 && f.StringValue == "igen").FirstOrDefault();
            if (szekhely == null)
                return;

            var eszkozertekSzekhely = fields.Where(f => f.Id == 1807 && f.RowIndex == szekhely.RowIndex).FirstOrDefault().DecimalValue;
            if (eszkozertekSzekhely > 0)
                return;

            var rowindexes = fields.Select(f => f.RowIndex).Distinct().ToList();
            rowindexes.Sort();

            foreach (var megosztas in fields.Where(f => f.Id == 1803))
            {
                if(megosztas.StringValue == "eszközérték")
                {
                    if (result.ContainsKey($"0;{rowindexes.IndexOf(megosztas.RowIndex.Value)};3"))
                        result[$"0;{rowindexes.IndexOf(megosztas.RowIndex.Value)};3"] += "Az eszközérték nem választható ki, ha székhelynek jelölt település rekordjában Eszközérték = 0."; 
                    else
                       result.Add($"0;{rowindexes.IndexOf(megosztas.RowIndex.Value)};3", "Az eszközérték nem választható ki, ha székhelynek jelölt település rekordjában Eszközérték = 0.");
                }
            }
        }

        private static void CheckF1830(Dictionary<string, string> result, List<FieldDescriptorDto> fields)
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
                        result.Add($"0;{rowindexes.IndexOf(f.RowIndex)};15", "Ideiglenes iparűzési adóátalány teljes összeg nem lehet kisebb mint az Ideiglenes iparűzési adóátalány levonható része.");
                }
            }

        }


        private static void CheckSzekhelys(List<FieldDescriptorDto> fields, Dictionary<string, string> result, List<FieldDescriptorDto> kulfolds)
        {
            var rowindexes = fields.Select(f => f.RowIndex).Distinct().ToList();
            rowindexes.Sort();
            var szekhelys = fields.Where(f => f.Id == 1801 && f.StringValue == "igen").ToList();

            if (szekhelys.Count() > 1)
            {
                for (int i = 1; i < szekhelys.Count(); i++)
                {
                    if (result.ContainsKey($"0;{rowindexes.IndexOf(szekhelys[i].RowIndex.Value)};1"))
                        result[$"0;{rowindexes.IndexOf(szekhelys[i].RowIndex.Value)};1"] += "; Csak egy város lehet székhely.";
                    else
                        result.Add($"0;{rowindexes.IndexOf(szekhelys[i].RowIndex.Value)};1", "Csak egy város lehet székhely.");
                }
            }

            // select külföldi székhely
            foreach (var szekhely in szekhelys)
            {
                if (kulfolds.FirstOrDefault(k => k.RowIndex.Value == szekhely.RowIndex.Value) != null)
                {
                    if (result.ContainsKey($"0;{rowindexes.IndexOf(szekhely.RowIndex.Value)};1"))
                        result[$"0;{rowindexes.IndexOf(szekhely.RowIndex.Value)};1"] += "; Külföld nem lehet székhely.";
                    else
                        result.Add($"0;{rowindexes.IndexOf(szekhely.RowIndex.Value)};1", "Külföld nem lehet székhely.");
                }
            }
        }

        private static List<FieldDescriptorDto> CheckOnlyOneKulfold(List<FieldDescriptorDto> fields, Dictionary<string, string> result)
        {
            var rowindexes = fields.Select(f => f.RowIndex).Distinct().ToList();
            rowindexes.Sort();
            var kulfolds = fields.Where(f => f.Id == 1800 && f.StringValue == "Külföld").ToList();
            if (kulfolds.Count() > 1)
            {
                for (int i = 1; i < kulfolds.Count(); i++)
                {
                    if (result.ContainsKey($"0;{rowindexes.IndexOf(kulfolds[i].RowIndex.Value)};0"))
                        result[$"0;{rowindexes.IndexOf(kulfolds[i].RowIndex.Value)};0"] += "; Csak egy külföld vehető fel.";
                    else
                        result.Add($"0;{rowindexes.IndexOf(kulfolds[i].RowIndex.Value)};0", "Csak egy külföld vehető fel.");
                }
            }

            return kulfolds;
        }
    }
}