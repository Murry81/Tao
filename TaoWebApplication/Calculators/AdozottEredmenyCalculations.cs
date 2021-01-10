using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class AdozottEredmenyCalculations
    {
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            var calculationList = new List<FieldDescriptorDto>
            {
                fields.FirstOrDefault(f => f.Id == 2202),
                fields.FirstOrDefault(f => f.Id == 2203),
                fields.FirstOrDefault(f => f.Id == 2204),
                fields.FirstOrDefault(f => f.Id == 2201),
                fields.FirstOrDefault(f => f.Id == 2205),
                fields.FirstOrDefault(f => f.Id == 2206),
                fields.FirstOrDefault(f => f.Id == 2207)
            };

            foreach (var field in calculationList)
            {
                if (!field.IsCaculated)
                    continue;

                switch (field.Id)
                {
                    case 2201: // Adózás előtti eredmény
                        {
                            field.DecimalValue = Calculate2201(service, sessionId, fields);
                            break;
                        }
                    case 2204: // Ráfordításként elszámolandó adók
                        {
                            field.DecimalValue = Calculate2204(fields);
                            break;
                        }
                    case 2202: // Iparűzési adó
                        {
                            field.DecimalValue = Calculate2202(service, sessionId);
                            break;
                        }
                    case 2203: // Innovációs járulék
                        {
                            field.DecimalValue = Calculate2203(service, sessionId);
                            break;
                        }
                    case 2205: // Korrigált adózás előtti eredmény
                        {
                            field.DecimalValue = Calculate2205(fields);
                            break;
                        }
                    case 2206: // Nyereséget terhelő adók
                        {
                            field.DecimalValue = Calculate2206(service, sessionId, fields);
                            break;
                        }
                    case 2207: // Adózott eredmény
                        {
                            field.DecimalValue = Calculate2207(fields);
                            break;
                        }
                }
            }
        }

        public static void ReCalculateValues(IDataService service, Guid sessionId)
        {
            var fields = service.GetPageFields(22, sessionId);
            CalculateValues(fields, service, sessionId);
            service.UpdateFieldValues(fields, sessionId);
        }

        private static decimal? Calculate2207(List<FieldDescriptorDto> fields)
        {
            // Korrigált adózás előtti eredmény - Nyereséget terhelő adók
            // f2205 - f2206

            var f2205 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 2205).DecimalValue);
            var f2206 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 2206).DecimalValue);

            return f2205 - f2206;
        }

        private static decimal? Calculate2206(IDataService service, Guid sessionId, List<FieldDescriptorDto> fields)
        {
            // 4.Tárgyévi társasági adó + 5.Energiaellátók jövedelemadója
            // f1414 + f1526 + f3110
            //Ha 1. Tényadatok/Energiaellátó (61) nincs jelölve, akkor 5. Energiellátók jövedelemadója nem tölthető, az ottani értékekkel nem kell számolni 
            
            var f1414 = GenericCalculations.GetValue(service.GetFieldById(1414, sessionId).DecimalValue);
            var f61 = GenericCalculations.GetValue(service.GetFieldById(1414, sessionId).BoolFieldValue);
            var f3110 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 3110).DecimalValue);
                
            if(f61)
            {
                var f1526 = GenericCalculations.GetValue(service.GetFieldById(1526, sessionId)?.DecimalValue);
                return f1414 + f1526 + f3110;
            }
            return f1414 + f3110;
        }

        private static decimal? Calculate2205(List<FieldDescriptorDto> fields)
        {
            // Adózás előtti eredmény - Ráfordításként elszámolandó adók
            // f2201 - f2204
            var f2201 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 2201).DecimalValue);
            var f2204 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 2204).DecimalValue);

            return f2201 - f2204;
        }

        private static decimal? Calculate2203(IDataService service, Guid sessionId)
        {
            // 2.3.Innovációs járulék
            // f705

            return GenericCalculations.GetValue(service.GetFieldById(705, sessionId).DecimalValue);
        }

        private static decimal? Calculate2202(IDataService service, Guid sessionId)
        {
            // 2.x.Iparűzési adó csökkentett összege
            // f440

            return GenericCalculations.GetValue(service.GetFieldById(440, sessionId).DecimalValue);
        }

        private static decimal? Calculate2204(List<FieldDescriptorDto> fields)
        {
            // Iparűzési adó + Innovációs járulék
            // f2202 + f2203

            var f2202 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 2202).DecimalValue);
            var f2203 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 2203).DecimalValue);

            return f2202 + f2203;
        }

        private static decimal? Calculate2201(IDataService service, Guid sessionId, List<FieldDescriptorDto> fields)
        {
            // 4.Évesre átszámított nyereség + 4.Várható korrekció + 4.1.Ráfordításként elszámolandó adók (2204)
            // f1408 + f1409 + f2204

            var f1408 = GenericCalculations.GetValue(service.GetFieldById(1408, sessionId).DecimalValue);
            var f1409 = GenericCalculations.GetValue(service.GetFieldById(1409, sessionId).DecimalValue);
            var f2204 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 2204).DecimalValue);

            return f1408 + f1409 + f2204;
        }
    }
}