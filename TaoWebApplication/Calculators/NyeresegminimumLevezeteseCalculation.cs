using Contracts.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class NyeresegminimumLevezeteseCalculation
    {
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            foreach (var field in fields.OrderBy(s => s.Id))
            {
                if (!field.IsCaculated || field.Id == 1208)
                    continue;

                switch (field.Id)
                {
                    case 1204: // Összes bevétel
                        {
                            field.DecimalValue = Calculate1204(service, sessionId);
                            break;
                        }
                    case 1206: // Korrigált bevétel
                        {
                            field.DecimalValue = Calculate1206(fields);
                            break;
                        }
                    case 1207: // Nyereségminimum
                        {
                            field.DecimalValue = Calculate1207(fields);
                            break;
                        }
                    case 1209: // Az eredmény/ adóalap eléri nyereségminimumot
                        {
                            field.BoolFieldValue = Calculate1209(fields, service, sessionId);
                            break;
                        }
                    case 1210: // A minimum jövedelmet nem kell alkalmazni
                        {
                            field.BoolFieldValue = Calculate1210(fields);
                            break;
                        }
                    case 2105: // A magánszemély taggal szemben fennálló kötelezettség 
                        {
                            field.DecimalValue = Calculate2105(service, sessionId);
                            break;
                        }
                }
            }

            // Calculate the 1208 last
            // A vállalkozás a jövedelem (nyereség) -minimumot tekinti adóalapnak
            var f1208 = fields.FirstOrDefault(f => f.Id == 1208);
            f1208.BoolFieldValue = Calculate1208(fields);

            var f1205 = fields.FirstOrDefault(f => f.Id == 1205);
            f1205.DecimalValue = Calculate1205(fields);

            CalculateOthers(fields, service, sessionId);
        }

        public static void ReCalculateValues(IDataService service, Guid sessionId)
        {
            var fields = service.GetPageFields(12, sessionId);
            CalculateValues(fields, service, sessionId);
            service.UpdateFieldValues(fields, sessionId);
        }

        private static void CalculateOthers(List<FieldDescriptorDto> pageFields, IDataService service, Guid sessionId)
        {
            var fields = service.GetFieldValuesByFieldIdList(new List<int> { 2110, 2111 }, sessionId);

            var f2100 = GenericCalculations.GetValue(pageFields.FirstOrDefault(f => f.Id == 2100).DecimalValue);
            var f2101 = GenericCalculations.GetValue(pageFields.FirstOrDefault(f => f.Id == 2101).DecimalValue);
            var f2102 = GenericCalculations.GetValue(pageFields.FirstOrDefault(f => f.Id == 2102).DecimalValue);

            var f2103 = GenericCalculations.GetValue(pageFields.FirstOrDefault(f => f.Id == 2103).DecimalValue);
            var f2104 = GenericCalculations.GetValue(pageFields.FirstOrDefault(f => f.Id == 2104).DecimalValue);
            var f2105 = GenericCalculations.GetValue(pageFields.FirstOrDefault(f => f.Id == 2105).DecimalValue);

            // 2110  f2100 + f2101 + f2102
            var f2110 = fields.FirstOrDefault(f => f.FieldDescriptorId == 2110);
            if (f2110 != null)
            {
                f2110.DecimalValue = f2100 + f2101 + f2102;
            }
            else
            {
                f2110 = new FieldValueDto
                {
                    DecimalValue = f2100 + f2101 + f2102,
                    Id = Guid.NewGuid(),
                    FieldDescriptorId = 2110,
                    SessionId = sessionId
                };
            }


            // 2110  f2103 + f2104 + f2105
            var f2111 = fields.FirstOrDefault(f => f.FieldDescriptorId == 2111);
            if (f2111 != null)
            {
                f2111.DecimalValue = f2103 + f2104 + f2105;
            }
            else
            {
                f2111 = new FieldValueDto
                {
                    DecimalValue = f2103 + f2104 + f2105,
                    Id = Guid.NewGuid(),
                    FieldDescriptorId = 2111,
                    SessionId = sessionId
                };
            }

            service.UpdateFieldValues(new List<FieldValueDto> { f2110, f2111 }, sessionId);
        }

        private static decimal? Calculate2105(IDataService service, Guid sessionId)
        {
            // 3.3 Adóalap korrekció nyereségminimum esetén
            // f1006
            return service.GetFieldById(1006, sessionId)?.DecimalValue;
        }

        private static bool Calculate1210(List<FieldDescriptorDto> fields)
        {
            //Előtársasági, vagy azt követő adóév, illetve első adóév, ha nincs előtársaság = igaz
            //vagy
            //Az előző évi évesített árbevétel 15 % -át elérő elemi kár = igaz
            //vagy
            //Szervezeti forma miatti mentesség = igaz
            // f1201 || f1202 || f1203

            var f1201 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1201).BoolFieldValue);
            var f1202 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1202).BoolFieldValue);
            var f1203 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1203).BoolFieldValue);

            return f1201 || f1202 || f1203;
        }

        private static bool Calculate1209(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ha Max (4.Adózás előtti eredmény , 4.Adóalap) >= Nyereségminimum akkor = igaz
            // max(f1407, f1411) >= f1207

            var f1407 = GenericCalculations.GetValue(service.GetFieldById(1407, sessionId)?.DecimalValue);
            var f1411 = GenericCalculations.GetValue(service.GetFieldById(1407, sessionId)?.DecimalValue);
            var f1207 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1207).DecimalValue);

            return Math.Max(f1407, f1411) >= f1207;

        }

        private static bool Calculate1208(List<FieldDescriptorDto> fields)
        {
            //A minimum jövedelmet nem kell alkalmazni = igaz
            //Vagy
            //A társaság nyilatkozatot tesz = igaz
            //Vagy
            //Az eredmény / adóalap eléri nyereségminimumot = igaz
            // f1210 || f1200 ||  f1209

            var f1200 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1200).BoolFieldValue);
            var f1210 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1210).BoolFieldValue);
            var f1209 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1209).BoolFieldValue);

            return !(f1210 || f1200 || f1209);
        }

        private static decimal? Calculate1207(List<FieldDescriptorDto> fields)
        {
            // Korrigált bevétel * 0,02
            // f1206 * 0.02

            var f1206 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1206).DecimalValue);
            return f1206 * (decimal)0.02;
        }

        private static decimal? Calculate1206(List<FieldDescriptorDto> fields)
        {
            // Összes bevétel + összes bevételt módosító tényezők hatása
            // f1204 + f1205

            var f1204 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1204).DecimalValue);
            var f1205 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1205).DecimalValue);
            return f1204 + f1205;
        }

        private static decimal? Calculate1205(List<FieldDescriptorDto> fields)
        {
            // Szum(3.5.1.Növelő) - Szum(3.5.1.Csökkentő)
            // f2103 + f2104 + f2105 - f2100 - f2101 - 2102
            var f2103 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 2103).DecimalValue);
            var f2104 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 2104).DecimalValue);
            var f2105 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 2105).DecimalValue);

            var f2102 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 2102).DecimalValue);
            var f2101 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 2101).DecimalValue);
            var f2100 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 2100).DecimalValue);

            return f2103 + f2104 + f2105 - f2100 - f2101 - f2102;
        }

        private static decimal? Calculate1204(IDataService service, Guid sessionId)
        {
            // 3.1.Értékesítés nettó árbevétele + 3.1.Egyéb bevételek + 3.1.Pénzügyi bevételek
            // f803 + f804 + f805
            var list = new List<FieldDescriptorDto> { service.GetFieldById(803, sessionId), service.GetFieldById(804, sessionId), service.GetFieldById(805, sessionId) };

            return GenericCalculations.SumList(list);
        }
    }
}