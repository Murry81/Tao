using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaoContracts.Contracts;
using TaoDatabaseService.Interfaces;

namespace TaoWebApplication.Calculators
{
    public class TaggalSzembeniKotelezettsegCalculation
    {
        public static void CalculateValues(List<FieldDescriptorDto> tableFields, IDataService service, Guid sessionId, List<FieldDescriptorDto> otherFields)
        {
            var endOfBusinessYear = service.GetFieldById(32, sessionId).DateValue.Value;

            //Get the previous year value
            var valueFieldsForTable = service.GetFieldsByFieldIdList(new List<int> { 1002 }, sessionId).Where(f => f.RowIndex < 1).ToList();
            var previousYearValue = GenericCalculations.SumList(valueFieldsForTable);
           
            var customer = service.GetCustomer(int.Parse(System.Web.HttpContext.Current.Session["CustomerId"].ToString()));

            var tableIds = new List<int> { 1000, 1001, 1002, 1003, 1004 };
            var tableList = tableFields.Where(f => tableIds.Contains(f.Id)).OrderBy(f => f.RowIndex);
            var tableItems = new Dictionary<RowItem, List<FieldDescriptorDto>>();

            var previousRowIndex = -1;
            RowItem currentItem = null;

            foreach (var field in tableList)
            {
                var rowindex = field.RowIndex.Value;
                if (rowindex != previousRowIndex)
                {
                    currentItem = new RowItem();
                    currentItem.RowIndex = rowindex;
                    tableItems.Add(currentItem, new List<FieldDescriptorDto>());
                    previousRowIndex = rowindex;
                }

                tableItems[currentItem].Add(field);
                switch (field.Id)
                {
                    case 1000:
                        currentItem.Date = field.DateValue;
                        break;
                    case 1001:
                        currentItem.Title = field.StringValue;
                        break;
                    case 1002:
                        currentItem.Value = GenericCalculations.GetValue(field.DecimalValue);
                        break;
                }
            }
    
            //Remove null rows
            foreach (var item in tableItems.Keys.Where(s => s.Date == null || s.Value == null).ToList())
            {
                tableItems.Remove(item);
            }

            var currentValue = previousYearValue;
            RowItem previousItem = null;
            // Do calculatations
            foreach (var item in tableItems.Keys.OrderBy(s => s.Date))
            {
                currentValue += (item.Title == "Törlesztés" ? -1 * item.Value : item.Value);
                tableItems[item].FirstOrDefault(f => f.Id == 1003).DecimalValue = currentValue;
                item.CumulatedValue = currentValue.Value;
                if (previousItem != null)
                {
                    tableItems[previousItem].FirstOrDefault(f => f.Id == 1004).DecimalValue = (int)(item.Date - previousItem.Date).Value.TotalDays;
                    previousItem.DayCount = (int)(item.Date - previousItem.Date).Value.TotalDays;
                }
                previousItem = item;
            }

            tableItems[previousItem].FirstOrDefault(f => f.Id == 1004).DecimalValue = (int)(endOfBusinessYear - previousItem.Date).Value.TotalDays;
            previousItem.DayCount = (int)(endOfBusinessYear - previousItem.Date).Value.TotalDays;

            foreach (var field in otherFields.OrderBy(s => s.Id))
            {
                switch (field.Id)
                {
                    case 1005: // A taggal szembeni kötelezettség átlagos állománya
                        {
                            field.DecimalValue = Calculate1005(tableItems, service, sessionId);
                            break;
                        }
                    case 1006: // Adóalap korrekció nyereségminimum esetén
                        {
                            field.DecimalValue = Calculate1006(otherFields , previousYearValue);
                            break;
                        }
                }
            }
        }

        private static decimal? Calculate1006(List<FieldDescriptorDto> fields, decimal? nyito)
        {
            // Ha A taggal szembeni kötelezettség átlagos állománya nagyobb, mint a nyitó állomány, 
            // akkor = 0,5 * (A taggal szembeni kötelezettség átlagos állomány - Kötelezettség állományváltozás összeg előző évi záró)
            // if f1005 > nyito => 0.5 * (f1005 - nyito)

            var f1005 = GenericCalculations.GetValue(fields.FirstOrDefault(f => f.Id == 1005)?.DecimalValue);

            if (f1005 > nyito)
            {
                return (decimal)0.5 * (f1005 - nyito);
            }

            return null;
        }

        private static decimal? Calculate1005(Dictionary<RowItem, List<FieldDescriptorDto>> tableOneItems, IDataService service, Guid sessionId)
        {
            //	Szum(Kumulált összeg * napok száma) / 365

            decimal result = 0;
            foreach (var value in tableOneItems.Keys)
            {
                var tmp = (value.CumulatedValue * value.DayCount) / 365;
                result += tmp;
            }

            if (result == 0)
                return null;

            return result;
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