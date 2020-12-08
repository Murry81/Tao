using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class InnovaciosJarulekCalculation
    {
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            var customer = service.GetCustomer(int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString()));

            foreach (var field in fields.OrderBy(s => s.Id))
            {
                if (!field.IsCaculated)
                    continue;

                switch (field.Id)
                {
                    case 704: // Innovációs járulék adóalap
                        {
                            field.DecimalValue = Calculate704(fields, service, sessionId);
                            break;
                        }
                    case 705: // Innovációs járulék
                        {
                            field.DecimalValue = Calculate705(fields);
                            break;
                        }
                    case 706: // 12.20-i feltöltési kötelezettség/Adókülönbözet
                        {
                            field.DecimalValue = Calculate706(fields, service, sessionId);
                            break;
                        }
                    case 707: // Pénzügyileg rendezendő
                        {
                            field.DecimalValue = Calculate707(fields);
                            break;
                        }
                }
            }
        }

        public static void ReCalculateValues(IDataService service, Guid sessionId)
        {
            var fields = service.GetPageFields(7, sessionId);
            CalculateValues(fields, service, sessionId);
            service.UpdateFieldValues(fields, sessionId);
        }

        private static decimal? Calculate707(List<FieldDescriptorDto> fields)
        {
            // 12.20 - i feltöltési kötelezettség/ Adókülönbözet + 12.20 előírt előleg +Be nem fizetett, korábban előírt előleg -folyószámlán fennálló túlfizetés
            // f706 + f700 + f701 - f703

            var f706 = GenericCalculations.GetValue( fields.FirstOrDefault(f => f.Id == 706)?.DecimalValue);
            var f700 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 700)?.DecimalValue);
            var f701 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 701)?.DecimalValue);
            var f703 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 703)?.DecimalValue);

            return f706 + f700 + f701 - f703;
        }

        private static decimal? Calculate706(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Innovációs járulék - 12.20 előírt előleg - Be nem fizetett, korábban előírt előleg - Folyószámlán fennálló túlfizetés
            // Ha ez negatív és a kalkuláció jellege feltöltés, akkor 0
            // f705 - f700 - f701 - 703;

            var f705 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 705)?.DecimalValue);
            var f700 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 700)?.DecimalValue);
            var f701 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 701)?.DecimalValue);
            var f702 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 702)?.DecimalValue);

            //f29 kalkuláció jellege
            var f29 = service.GetFieldsByFieldIdList(new List<int> { 29 }, sessionId).FirstOrDefault()?.StringValue;

            var sum = f705 - f700 - f701 - f702;
            if (f29 == "Feltöltés" && sum < 0)
                return 0;

            return sum;
        }

        private static decimal? Calculate705(List<FieldDescriptorDto> fields)
        {
            //	Innovációs járulék adóalap * 0,003
            // f704 * 0.003

            var f704 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 704)?.DecimalValue);
            var result = f704 * (decimal)0.003;

            return decimal.Round(result / 1000) * 1000;
        }

        private static decimal? Calculate704(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {          
            // 2.1 Iparúzési adó alapja f436
            // 2.2.Adóalanyra jutó adóalap f450

            // Ipa kapcsolt státusz f40
            var f29 = service.GetFieldsByFieldIdList(new List<int> { 40 }, sessionId).FirstOrDefault()?.BoolFieldValue;
            if(f29.Value)
            {
                // Ha 0.IPA kapcsolt státusz = igaz, akkor 2.2.Adóalanyra jutó adóalap
                return service.GetFieldsByFieldIdList(new List<int> { 450 }, sessionId).FirstOrDefault()?.DecimalValue;
            }

            // Ha 0.IPA kapcsolt státusz = hamis, akkor 2.1.Iparűzési adó alapja
            return service.GetFieldsByFieldIdList(new List<int> { 436 }, sessionId).FirstOrDefault()?.DecimalValue;

        }
    }
}