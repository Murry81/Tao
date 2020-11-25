using System;
using System.Collections.Generic;
using System.Linq;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;
using TaoWebApplication.Models;

namespace TaoWebApplication.Calculators
{
    public class AlultokesitesCalculation
    {
        public static void CalculateValues(List<FieldDescriptorDto> tableFields, IDataService service, Guid sessionId, List<FieldDescriptorDto> otherFields)
        {
            var endOfBusinessYear = service.GetFieldById(32, sessionId).DateValue.Value;

            var valueFieldsForTable = service.GetTableFieldsByFieldIdList(905, sessionId).Where(f => f.RowIndex < 4).ToList();
            var previousYearValue = GenericCalculations.SumList(valueFieldsForTable);
            valueFieldsForTable = service.GetTableFieldsByFieldIdList(908, sessionId).Where(f => f.RowIndex < 1).ToList();
            var previousYearValueTableTwo = GenericCalculations.SumList(valueFieldsForTable);

            var customer = service.GetCustomer(int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString()));

            var tableOneIds = new List<int> { 903, 904, 905, 909, 910 };
            var tableTwoIds = new List<int> { 906, 907, 908, 911, 912 };

            var tableOneList = tableFields.Where(f => tableOneIds.Contains(f.Id)).OrderBy(f => f.RowIndex);
            var tableTwoList = tableFields.Where(f => tableTwoIds.Contains(f.Id)).OrderBy(f => f.RowIndex);

            var tableOneItems = new Dictionary<RowItem, List<FieldDescriptorDto>>();
            var tableTwoItems = new Dictionary<RowItem, List<FieldDescriptorDto>>();
            var previousRowIndex = -1;
            RowItem currentItem = null;

            foreach (var field in tableOneList)
            {
                var rowindex = field.RowIndex.Value;
                if (rowindex != previousRowIndex)
                {
                    currentItem = new RowItem();
                    currentItem.RowIndex = rowindex;
                    tableOneItems.Add(currentItem, new List<FieldDescriptorDto>());
                    previousRowIndex = rowindex;
                }

                tableOneItems[currentItem].Add(field);
                switch (field.Id)
                {
                    case 903:
                        currentItem.Date = field.DateValue;
                        break;
                    case 904:
                        currentItem.Title = field.StringValue;
                        break;
                    case 905:
                        currentItem.Value = field.DecimalValue;
                        break;
                }
            }
            foreach (var field in tableTwoList)
            {
                var rowindex = field.RowIndex.Value;
                if (rowindex != previousRowIndex)
                {
                    currentItem = new RowItem();
                    currentItem.RowIndex = rowindex;
                    tableTwoItems.Add(currentItem, new List<FieldDescriptorDto>());
                    previousRowIndex = rowindex;
                }

                tableTwoItems[currentItem].Add(field);
                switch (field.Id)
                {
                    case 906:
                        currentItem.Date = field.DateValue;
                        break;
                    case 907:
                        currentItem.Title = field.StringValue;
                        break;
                    case 908:
                        currentItem.Value = GenericCalculations.GetValue(field.DecimalValue);
                        break;
                }
            }
            //Remove null rows
            foreach (var item in tableOneItems.Keys.Where(s => s.Date == null || s.Value == null).ToList())
            {
                tableOneItems.Remove(item);
            }
            foreach (var item in tableTwoItems.Keys.Where(s => s.Date == null || s.Value == null).ToList())
            {
                tableTwoItems.Remove(item);
            }

            var currentValue = previousYearValue;
            RowItem previousItem = null;
            int dayCount = 0;
            
            // Do calculatations
            if (tableOneItems.Any())
            {
                foreach (var item in tableOneItems.Keys.OrderBy(s => s.Date))
                {
                    currentValue += GenericCalculations.GetValue(item.Value);
                    tableOneItems[item].FirstOrDefault(f => f.Id == 909).DecimalValue = currentValue;
                    item.CumulatedValue = currentValue.Value;
                    if (previousItem != null)
                    {
                        dayCount = CalculateDayCount(item.Date, previousItem.Date);
                        tableOneItems[previousItem].FirstOrDefault(f => f.Id == 910).DecimalValue = dayCount;
                        previousItem.DayCount = dayCount;
                    }
                    previousItem = item;
                }

                dayCount = CalculateDayCount(endOfBusinessYear, previousItem.Date) + 1;
                tableOneItems[previousItem].FirstOrDefault(f => f.Id == 910).DecimalValue = dayCount;
                previousItem.DayCount = dayCount;
            }

            currentValue = previousYearValueTableTwo;
            previousItem = null;

            // Do calculatations
            dayCount = 0;
            if (tableTwoItems.Any())
            {
                foreach (var item in tableTwoItems.Keys.OrderBy(s => s.Date))
                {
                    currentValue += GenericCalculations.GetValue(item.Value);
                    tableTwoItems[item].FirstOrDefault(f => f.Id == 911).DecimalValue = currentValue;
                    item.CumulatedValue = currentValue.Value;
                    if (previousItem != null)
                    {
                        dayCount = CalculateDayCount(item.Date, previousItem.Date);
                        tableTwoItems[previousItem].FirstOrDefault(f => f.Id == 912).DecimalValue = dayCount;
                        previousItem.DayCount = dayCount;
                    }
                    previousItem = item;
                }

                dayCount = CalculateDayCount(endOfBusinessYear, previousItem.Date) + 1;
                tableTwoItems[previousItem].FirstOrDefault(f => f.Id == 912).DecimalValue = dayCount;
                previousItem.DayCount = dayCount;
            }

            foreach (var field in otherFields)
            {
                switch(field.Id)
                {
                    case 913: // A folyósított kötelezettségek után fizetett kamat
                        {
                            field.DecimalValue = Calculate913(otherFields, service, sessionId);
                            break;
                        }
                    case 914: // ST átlagos állomány
                        {
                            field.DecimalValue = Calculate914(tableOneItems, service, sessionId);
                            break;
                        }
                    case 915: // Kötelezettség átlagos állomány 
                        {
                            field.DecimalValue = Calculate914(tableTwoItems, service, sessionId);
                            break;
                        }
                    case 916: // Alultőkésítés miatti adóalap korrekció 
                        {
                            field.DecimalValue = Calculate916(otherFields, service, sessionId);
                            break;
                        }
                }
            }
        }

        private static int CalculateDayCount(DateTimeOffset? first, DateTimeOffset? second)
        {
            DateTimeOffset f = first == null ? DateTimeOffset.UtcNow : new DateTimeOffset(first.Value.Year, first.Value.Month, first.Value.Day, 0, 0, 0, TimeSpan.Zero);
            DateTimeOffset s = second == null ? DateTimeOffset.UtcNow :  new DateTimeOffset(second.Value.Year, second.Value.Month, second.Value.Day, 0, 0, 0, TimeSpan.Zero);

            return (int)(f - s).TotalDays;
        }

        public static void ReCalculateValues(IDataService service, Guid sessionId)
        {
            var fields = service.GetPageFields(9, sessionId);
            var tableFields = service.GetTableData(9, sessionId);
            var fullTableFields = AlultokesitesModel.RemoveDefaultFieldsBeforeSave(tableFields);

            CalculateValues(fullTableFields, service, sessionId, fields);
            service.UpdateFieldValues(fields, sessionId);
        }

        private static decimal? Calculate916(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ha Kötelezettség átlagos állomány = 0, akkor 0
            // Ha Saját tőke negatív = igaz, akkor = A folyósított kötelezettségek után fizetett kamat
            // Kötelezettség átlagos állomány >= 3 * ST átlagos állomány, akkor (Kötelezettség átlagos állomány - 3 * ST átlagos állomány) / Kötelezettség átlagos állomány *A folyósított kötelezettségek után fizetett kamat

            // if f915 = 0 => 0
            var f915 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 915)?.DecimalValue);
            if (f915 == 0)
                return 0;

            // if f900 => f913
            var f900 = fields.FirstOrDefault(f => f.Id == 900)?.BoolFieldValue;
            if(f900.Value)
            {
                return fields.FirstOrDefault(f => f.Id == 913)?.DecimalValue;
            }

            // if 915 >= 3 * f914 => (f915 - 3 * f914) / f915 * f913
            var f914 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 914)?.DecimalValue);
            var f913 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 913)?.DecimalValue);
            if(f915 >= 3 * f914)
            {
                var result = (f915 - 3 * f914) / f915 * f913;
                if (result != 0)
                    return result;
            }

            return null;
        }

        private static decimal? Calculate914(Dictionary<RowItem, List<FieldDescriptorDto>> tableOneItems, IDataService service, Guid sessionId)
        {
            //	Szum(Kumulált összeg * napok száma) / 365

            decimal result = 0;
            foreach(var value in tableOneItems.Keys)
            {
                var tmp = (value.CumulatedValue * value.DayCount) / 365;
                result += tmp;
            }

            if (result == 0)
                return null;

            return result;
        }

        private static decimal? Calculate913(List<FieldDescriptorDto> fields, IDataService service, Guid sessionId)
        {
            // Ráfordításként elszámolt kamat + Aktivált kamat
            // f901 + f902

            var f901 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 901)?.DecimalValue);
            var f902 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 902)?.DecimalValue);
            return f901 + f902;
        }

        private class RowItem
        {
            public DateTimeOffset? Date { get; set; }
            public string Title { get; set; }
            public Decimal? Value { get; set; }
            public Decimal DayCount { get; set; }
            public Decimal CumulatedValue { get; set; }

            public int RowIndex { get; set; }
        }
    }

}