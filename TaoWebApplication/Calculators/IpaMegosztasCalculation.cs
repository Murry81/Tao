using Contracts.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class IpaMegosztasCalculation
    {
        public static void CalculateValues(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            var max = fields.Select(f => f.RowIndex).Distinct().Max();
            foreach (var field in fields.OrderBy(s => s.Id))
            {
                if (!field.IsCaculated || max == field.RowIndex)
                    continue;

                var f1800 = fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1800).StringValue;
                if (string.IsNullOrEmpty(f1800))
                {
                    field.DecimalValue = null;
                    field.StringValue = null;
                    continue;
                }

                switch (field.Id)
                {
                    case 1802: // Adó mértéke (%, két tizedes)
                        {
                            field.DecimalValue = Calculate1802(field, fields, service); //use 408
                            break;
                        }
                    case 1803: // Megosztás módja
                        {
                            field.StringValue = Calculate1803(field, fields, service, sessionId); //use 408
                            break;
                        }
                    case 1807: // Eszközérték
                        {
                            field.DecimalValue = Calculate1807(field, fields);
                            break;
                        }
                    case 1808: // Megosztott alap
                        {
                            field.DecimalValue = Calculate1808(field, fields, service, sessionId);
                            break;
                        }
                    case 1811: // Adóköteles adóalap
                        {
                            field.DecimalValue = Calculate1811(field, fields);
                            break;
                        }
                    case 1812: // Adó kedvezmények nélkül
                        {
                            field.DecimalValue = Calculate1812(field, fields);
                            break;
                        }
                    case 1816: // Belföldi e-útdíj településre jutó összege
                        {
                            field.DecimalValue = Calculate1816(field, fields, service, sessionId);
                            break;
                        }
                    case 1817: // Külföldi e-útdíj településre jutó összege
                        {
                            field.DecimalValue = Calculate1817(field, fields, service, sessionId);
                            break;
                        }
                    case 1818: // Belföldi matricás úthasználati díj településre jutó összege
                        {
                            field.DecimalValue = Calculate1818(field, fields, service, sessionId);
                            break;
                        }
                    case 1819: // Alapkutatás, alkalmazott kutatás, K+F önköltségének településre jutó összege
                        {
                            field.DecimalValue = Calculate1819(field, fields, service, sessionId);
                            break;
                        }
                    case 1821: // Kutatási kedvezmény
                        {
                            field.DecimalValue = Calculate1821(field, fields);
                            break;
                        }
                    case 1822: // Települési adó összege
                        {
                            field.DecimalValue = Calculate1822(field, fields);
                            break;
                        }
                    case 1826: // Feltöltési kötelezettség/Adókülönbözet
                        {
                            field.DecimalValue = Calculate1826(field, fields, service, sessionId);
                            break;
                        }
                    case 1827: // Települési adó összege
                        {
                            field.DecimalValue = Calculate1827(field, fields);
                            break;
                        }
                }
            }

            CalculateExtraFields(service, fields, sessionId);
        }

        private static void CalculateExtraFields(IDataService service, List<FieldDescriptorDto> pageFields, Guid sessionId)
        {
            // (1831) Total személyi jellegő ráfordítás sum(1804)
            // (1832) Total eszközérték sum(1807)
            // (1833) Külföldi telephely(1808 if kulfold)
            // (1834) Onkormányzat neve
            // (1835) true, ha 1803 = személyi jellegű ráfordítás
            // (1836) true, ha 1803 = eszközérték
            // (1837) true, ha 1803 = komplex módszer

            var fields = service.GetFieldValuesByFieldIdList(new List<int> { 1831, 1832, 1833, 1834, 1835, 1836, 1837 }, sessionId);

            var sum1804 = GenericCalculations.SumList(pageFields.Where(f => f.Id == 1804).ToList());
            var sum1807 = GenericCalculations.SumList(pageFields.Where(f => f.Id == 1807).ToList());

            var f54Value = GenericCalculations.GetValue(service.GetFieldsByFieldIdList(new List<int> { 54 }, sessionId).FirstOrDefault().DecimalValue);
            var f55Value = GenericCalculations.GetValue(service.GetFieldsByFieldIdList(new List<int> { 55 }, sessionId).FirstOrDefault().DecimalValue);


            var f1831 = fields.FirstOrDefault(f => f.FieldDescriptorId == 1831);
            if (f1831 != null)
            {
                f1831.DecimalValue = sum1804;
            }
            else
            {
                f1831 = new Contracts.Contracts.FieldValueDto
                {
                    DecimalValue = sum1804,
                    Id = Guid.NewGuid(),
                    FieldDescriptorId = 1831,
                    SessionId = sessionId
                };
            }

            var f1832 = fields.FirstOrDefault(f => f.FieldDescriptorId == 1832);
            if (f1832 != null)
            {
                f1832.DecimalValue = sum1807;
            }
            else
            {
                f1832 = new Contracts.Contracts.FieldValueDto
                {
                    DecimalValue = sum1807,
                    Id = Guid.NewGuid(),
                    FieldDescriptorId = 1832,
                    SessionId = sessionId
                };
            }

            // (1833) Külföldi telephely(1808 if kulfold)
            var kulfold = pageFields.FirstOrDefault(f => f.Id == 1800 && f.StringValue == "Külföld");
            var f1833 = fields.FirstOrDefault(f => f.FieldDescriptorId == 1833);
            if (kulfold != null)
            {
                if (f1833 != null)
                {
                    f1833.DecimalValue = pageFields.FirstOrDefault(f => f.Id == 1808 && f.RowIndex == kulfold.RowIndex).DecimalValue; 
                }
                else
                {
                    f1833 = new Contracts.Contracts.FieldValueDto
                    {
                        DecimalValue = pageFields.FirstOrDefault(f => f.Id == 1808 && f.RowIndex == kulfold.RowIndex).DecimalValue,
                        Id = Guid.NewGuid(),
                        FieldDescriptorId = 1833,
                        SessionId = sessionId
                    };
                }
            }

            // (1834) Onkormányzat neve
            var onkormanyzatList = new List<FieldValueDto>();
            foreach (var currentCity in pageFields.Where(f => f.Id == 1800))
            {
                var f1834 = fields.FirstOrDefault(f => f.FieldDescriptorId == 1834 && f.RowIndex == currentCity.RowIndex);
                if (string.IsNullOrEmpty(currentCity.StringValue))
                    continue;

                var onkormanyzat = service.GetOnkormanyzat(currentCity.StringValue);

                if (f1834 != null)
                {
                    f1834.StringValue = onkormanyzat;
                }
                else
                {
                    f1834 = new FieldValueDto
                    {
                        StringValue = onkormanyzat,
                        Id = Guid.NewGuid(),
                        FieldDescriptorId = 1834,
                        SessionId = sessionId,
                        RowIndex = currentCity.RowIndex
                    };
                }

                onkormanyzatList.Add(f1834);
            }

            // (1835) true, ha 1803 = személyi jellegű ráfordítás
            var type = pageFields.FirstOrDefault(f => f.Id == 1803).StringValue;

            var f1835 = fields.FirstOrDefault(f => f.FieldDescriptorId == 1835);
            if (f1835 != null)
            {
                f1835.BoolValue = type == "személyi jellegű ráfordítás";
            }
            else
            {
                f1835 = new FieldValueDto
                {
                    BoolValue = type == "személyi jellegű ráfordítás",
                    Id = Guid.NewGuid(),
                    FieldDescriptorId = 1835,
                    SessionId = sessionId
                };
            }

            // (1836) true, ha 1803 = eszközérték
            var f1836 = fields.FirstOrDefault(f => f.FieldDescriptorId == 1836);
            if (f1836 != null)
            {
                f1836.BoolValue = type == "eszközérték";
            }
            else
            {
                f1836 = new FieldValueDto
                {
                    BoolValue = type == "eszközérték",
                    Id = Guid.NewGuid(),
                    FieldDescriptorId = 1836,
                    SessionId = sessionId
                };
            }

            // (1837) true, ha 1803 = komplex módszer
            var f1837 = fields.FirstOrDefault(f => f.FieldDescriptorId == 1837);
            if (f1837 != null)
            {
                f1837.BoolValue = type == "komplex módszer";
            }
            else
            {
                f1837 = new FieldValueDto
                {
                    BoolValue = type == "komplex módszer",
                    Id = Guid.NewGuid(),
                    FieldDescriptorId = 1837,
                    SessionId = sessionId
                };
            }

            var resultList = new List<FieldValueDto> { f1831, f1832, f1833, f1835, f1836, f1837 };
            resultList.AddRange(onkormanyzatList);
            resultList.RemoveAll(f => f == null);
            service.UpdateFieldValues(resultList, sessionId);
        }

        public static void ReCalculateValues(IDataService service, Guid sessionId)
        {
            var fields = service.GetPageFields(18, sessionId);
            CalculateValues(fields, service, sessionId);
            service.UpdateFieldValues(fields, sessionId);
        }

        private static decimal? Calculate1827(FieldDescriptorDto field, List<FieldDescriptorDto> fields)
        {
            // Felöltési kötelezettség/Adókülönbözet + Be nem fizetett, korábban előírt előleg - Folyószámlán fennálló túlfizetés
            // f1826 + f1823 - f1825
            var f1826 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1826).DecimalValue);
            var f1823 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1823).DecimalValue);
            var f1825 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1825).DecimalValue);

            return f1826 + f1823 - f1825;
        }

        private static decimal? Calculate1826(FieldDescriptorDto field, List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Települési adó összege - Be nem fizetett, korábban előírt előleg - Befizetett előleg
            // Ha ez negatív és a kalkuláció jellege feltöltés, akkor az érték 0
            // f1822 - f1823 - f1824
            var f1822 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1822).DecimalValue);
            var f1823 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1823).DecimalValue);
            var f1824 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1824).DecimalValue);

            var ado = f1822 - f1823 - f1824;

            var f29 = service.GetFieldsByFieldIdList(new List<int> { 29 }, sessionId).FirstOrDefault().StringValue;
            if(f29 == "feltöltés")
            {
                return Math.Max(0, ado);
            }

            return ado;

        }

        private static decimal? Calculate1822(FieldDescriptorDto field, List<FieldDescriptorDto> fields)
        {
            // (Adóköteles adóalap * Adó mértéke) - Adókedvezmény 1 - Adókedvezmény 2 - Ideiglenes iparűzési adóátalány levonható része 
            // - Belföldi e-útdíj településre jutó összege - Külföldi e-útdíj településre jutó összege - Belföldi matricás úthasználati díj településre jutó összege 
            //    - Kutatási kedvezmény, ha negatív, akkor 0
            // ( f1811 * f1802) - f1813 - f1814 - f1815 - f1816 - f1817 - f1818 - f1821
            var f1811 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1811).DecimalValue);
            var f1802 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1802).DecimalValue) / 100;
            var f1813 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1813).DecimalValue);
            var f1814 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1814).DecimalValue);
            var f1815 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1815).DecimalValue);
            var f1816 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1816).DecimalValue);
            var f1817 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1817).DecimalValue);
            var f1818 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1818).DecimalValue);
            var f1821 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1821).DecimalValue);

            var ado = (f1811 * f1802) - f1813 - f1814 - f1815 - f1816 - f1817 - f1818 - f1821;
            return Math.Max(0, ado);
        }

        private static decimal? Calculate1821(FieldDescriptorDto field, List<FieldDescriptorDto> fields)
        {
            // Ha Kutatási kedvezmény alkalmazható = igaz, Alapkutatás, alkalmazott kutatás, K+F önköltségének településre jutó összege, egyébként 0
            //if !f1820 then 0
            //else f1819
            if (fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1820).StringValue == "igen")
            {
                return GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1819).DecimalValue);
            }
            return 0;
        }

        private static decimal? Calculate1819(FieldDescriptorDto field, List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // 2.1.Alapkutatás, alkalmazott kutatás, kísérleti fejlesztés önköltségének 10 % -a * Megosztott adóalap / 2.1.Iparűzési adó alapja
            // f403 * f1808 / adoalap

            var f403 = GenericCalculations.GetValue(service.GetFieldsByFieldIdList(new List<int> { 403 }, sessionId).FirstOrDefault()?.DecimalValue);
            var f1808 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1808).DecimalValue);
            var adoalap = CalculateAdoAlap(service, sessionId);

            if (adoalap == 0)
                return 0;

            return f403 * f1808 / adoalap;
        }

        private static decimal? Calculate1818(FieldDescriptorDto field, List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // 2.1.Megfizetett belföldi matricás úthasználati díj 7,5%-a * Megosztott alap / 2.1.Iparűzési adó alapja
            // f412 * f1808 / adoalap

            var f411 = GenericCalculations.GetValue(service.GetFieldsByFieldIdList(new List<int> { 411 }, sessionId).FirstOrDefault()?.DecimalValue);
            var f1808 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1808).DecimalValue);
            var adoalap = CalculateAdoAlap(service, sessionId);

            if (adoalap == 0)
                return 0;

            return f411 * f1808 / adoalap;
        }

        private static decimal? Calculate1817(FieldDescriptorDto field, List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // 2.1.Megfizetett külföldi távolságarányos e-útdíj 7,5%-a * Megosztott alap / 2.1.Iparűzési adó alapja
            // f411 * f1808 / adoalap

            var f410 = GenericCalculations.GetValue(service.GetFieldsByFieldIdList(new List<int> { 410 }, sessionId).FirstOrDefault()?.DecimalValue);
            var f1808 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1808).DecimalValue);
            var adoalap = CalculateAdoAlap(service, sessionId);

            if (adoalap == 0)
                return 0;

            return f410 * f1808 / adoalap;
        }

        private static decimal? Calculate1816(FieldDescriptorDto field, List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // 2.1.Megfizetett belföldi távolságarányos e - útdíj 7,5 % -a * Megosztott alap / 2.1.Iparűzési adó alapja 
            // f410 * f1808 / adoalap

            var f409 = GenericCalculations.GetValue(service.GetFieldsByFieldIdList(new List<int> { 409 }, sessionId).FirstOrDefault()?.DecimalValue);
            var f1808 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1808).DecimalValue);
            var adoalap = CalculateAdoAlap(service, sessionId);

            if (adoalap == 0)
                return 0;

            return f409 * f1808 / adoalap;
        }

        private static decimal? Calculate1812(FieldDescriptorDto field, List<FieldDescriptorDto> fields)
        {
            // Adóköteles adóalap *Adó mértéke
            // f1811 * f1802
            var f1802 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1802).DecimalValue) / 100;
            var f1811 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1811).DecimalValue);

            return f1811 * f1802;
        }

        private static decimal? Calculate1811(FieldDescriptorDto field, List<FieldDescriptorDto> fields)
        {
            // Megosztott alap - Adómentes alap 1 - Adómentes alap 2
            // f1808 - f1809 - f1810
            var f1808 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1808).DecimalValue);
            var f1809 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1809).DecimalValue);
            var f1810 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1810).DecimalValue);

            return f1808 - f1809 - f1810;
        }

        private static decimal? Calculate1808(FieldDescriptorDto field, List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            //• (f436)	2.1.Iparűzési adó alapja helyett a (f450) 2.2.Adóalanyra jutó adóalap mezőt kell használni, ha 0.IPA kapcsolt státusz = igaz (40)
            // f1803 Megosztás módja:
            var f1803 = fields.FirstOrDefault(f => f.Id == 1803).StringValue;
            switch (f1803)
            {
                case "személyi jellegű ráfordítás":
                    {   // Személyi jellegű ráfordítás / szum (Személyi jellegű ráfordítás) * 2.1.Iparűzési adó alapja
                        // f1804 / sum(1804) * (f436 or f450)
                        var f1804 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1804).DecimalValue);
                        var sumF1804 = GenericCalculations.SumList(fields.Where(f => f.Id == 1804).ToList());
                        decimal? adoalap = CalculateAdoAlap(service, sessionId);

                        if (sumF1804 == 0)
                            return 0;

                        return f1804 / sumF1804 * adoalap;
                    }
                case "eszközérték":
                    {
                        // Eszközérték / szum (Eszközérték) * 2.1.Iparűzési adó alapja
                        // f1807 / sum(1807) * (f436 or f450)

                        var f1807 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1807).DecimalValue);
                        var sumF1807 = GenericCalculations.SumList(fields.Where(f => f.Id == 1807).ToList());
                        var adoalap = CalculateAdoAlap(service, sessionId);

                        if (sumF1807 == 0)
                            return 0;

                        return f1807 / sumF1807 * adoalap;
                    }
                case "komplex módszer":
                    {
                        // Declare S as Szum (Személyi jellegű ráfordítás) / (szum (Eszközérték) + szum (Személyi jellegű ráfordítás))
                        // f1804 / (sum(f1807) + sum(1804) )
                        var f1804 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1804).DecimalValue);
                        var sumF1807 = GenericCalculations.SumList(fields.Where(f => f.Id == 1807).ToList());
                        var sumF1804 = GenericCalculations.SumList(fields.Where(f => f.Id == 1804).ToList());

                        decimal? s = 0;
                        if (sumF1807 + sumF1804 != 0)
                            s = sumF1804 / (sumF1807 + sumF1804);
                                                

                        // Declare E as Szum (Eszközérték) / (szum (Eszközérték) + szum (Személyi jellegű ráfordítás)
                        // f1807 / ( sum(f1807) + sum(1804))
                        var f1807 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1807).DecimalValue);

                        decimal? e = 0;
                        if (sumF1807 + sumF1804 != 0)
                            e = sumF1807 / (sumF1807 + sumF1804);

                        //	Személyi jellegű ráfordítás / szum(Személyi jellegű ráfordítás) * S * 2.1.Iparűzési adó alapja +Eszközérték / szum(Eszközérték) * E * 2.1.Iparűzési adó alapja
                        // f1804 / sum(1804) * s * adoalap + f1807 / sum(1807) * E * adoalap
                        var adoalap = CalculateAdoAlap(service, sessionId);

                        if (sumF1804 == 0 || sumF1807 == 0)
                            return 0;

                        return f1804 / sumF1804 * s * adoalap + f1807 / sumF1807 * e * adoalap;
                    }
                default:
                    return 0;
            }
        }

        private static decimal? CalculateAdoAlap(IDataService service, Guid sessionId)
        {
            return service.GetFieldsByFieldIdList(new List<int> { 40 }, sessionId).FirstOrDefault().BoolFieldValue ?
                                    service.GetFieldsByFieldIdList(new List<int> { 450 }, sessionId).FirstOrDefault()?.DecimalValue :
                                     service.GetFieldsByFieldIdList(new List<int> { 436 }, sessionId).FirstOrDefault()?.DecimalValue;
        }

        private static decimal Calculate1807(FieldDescriptorDto field, List<FieldDescriptorDto> fields)
        {
            // Értékcsökkenés + Bérleti díj
            // f1805 + f1806

            var f1805 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1805).DecimalValue);
            var f1806 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1806).DecimalValue);

            return f1805 + f1806;
        }

        private static string Calculate1803(FieldDescriptorDto field, List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
           var field408 = service.GetFieldsByFieldIdList(new List<int> { 408 }, sessionId).FirstOrDefault().DecimalValue;
            if (field408.HasValue && field408.Value > 100000000)
                return "komplex módszer";


            return field.StringValue;
        }

        private static decimal? Calculate1802(FieldDescriptorDto field, List<FieldDescriptorDto> fields, IDataService service)
        {
            // f1800 település
            var f1800 = fields.FirstOrDefault(f => f.RowIndex == field.RowIndex && f.Id == 1800).StringValue;
            if (string.IsNullOrEmpty(f1800))
                return null;

            var tax = service.GetCityTaxes().FirstOrDefault(c => c.City.ToLower() == f1800.ToLower());
            if (tax == null)
                return (decimal)2;

            return tax.IparuzesiAdo;
        }
    }
}