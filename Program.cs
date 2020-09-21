using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace multi_reader
{
    class Program
    {

        static void Main(string[] args) => MainStart(args).GetAwaiter().GetResult();

        static async Task MainStart(string[] args)
        {
            var sourceFile = @"/home/karan/workings/MPC Flipkart Data/r.csv";
            LoadStateCodes();
            var invoices = GetInvoices(sourceFile);
            (await invoices)?.ForEach(i => Console.WriteLine($"{i.Number} {i.Date.ToString("dd-MM-yyyy")}"));
        }

        static async Task<List<RawInvoice>> GetInvoices(string filepath)
        {
            var lines = await File.ReadAllLinesAsync(filepath);
            var sales = new List<FlipkartSale>();
            foreach (var line in lines.Skip(1))
            {
                var x = 0;
                double d;
                DateTime date;

                var data = line.Split('\t');
                data = data.Select(d => d.Replace("\"", string.Empty)).ToArray();

                var sale = new FlipkartSale();
                sale.SalerGstin = data[x++].Replace("\"","");
                sale.OrderId = data[x++].Replace("\"","");
                sale.OrderItemId = data[x++].Replace("\"","");
                sale.Product = data[x++].Replace("\"","");
                sale.FSN = data[x++].Replace("\"","");
                sale.SKU = data[x++].Replace("\"","");
                sale.HSN = data[x++].Replace("\"","");
                sale.EventType = data[x++].Replace("\"","");
                sale.SubEventType = data[x++].Replace("\"","");
                sale.OrderType = data[x++].Replace("\"","");
                sale.FulfilmentType = data[x++].Replace("\"","");
                sale.OrderDate = DateTime.TryParseExact(data[x++].Replace("\"","").Split(' ').FirstOrDefault(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.AssumeLocal, out date) ? date : default;
                sale.OrderApprovalDate = DateTime.TryParseExact(data[x++].Replace("\"","").Split(' ').FirstOrDefault(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.AssumeLocal, out date) ? date : default;
                sale.ItemQuantity = double.TryParse(data[x++], out d) ? d : default;
                sale.ShippedFromState = data[x++].Replace("\"","");
                sale.PriceBeforeDiscount = double.TryParse(data[x++], out d) ? d : default;
                sale.Discount = double.TryParse(data[x++], out d) ? d : default;
                sale.SellerShareDiscount = double.TryParse(data[x++], out d) ? d : default;
                sale.BankOfferShare = double.TryParse(data[x++], out d) ? d : default;
                sale.PriceAfterDiscount = double.TryParse(data[x++], out d) ? d : default;
                sale.ShippingCharges = double.TryParse(data[x++], out d) ? d : default;
                sale.InvoiceAmount = double.TryParse(data[x++], out d) ? d : default;
                sale.TypeOfTax = data[x++].Replace("\"","");
                sale.TaxableValue = double.TryParse(data[x++], out d) ? d : default;
                x++;
                x++;
                x++;
                x++;
                x++;
                x++;
                sale.IGSTRate = double.TryParse(data[x++], out d) ? d : default;
                sale.IGSTAmount = double.TryParse(data[x++], out d) ? d : default;
                sale.CGSTRate = double.TryParse(data[x++], out d) ? d : default;
                sale.CGSTAmount = double.TryParse(data[x++], out d) ? d : default;
                x++;
                sale.SGSTAmount = double.TryParse(data[x++], out d) ? d : default;
                sale.TCSIGSTRate = double.TryParse(data[x++], out d) ? d : default;
                sale.TCSIGSTAmount = double.TryParse(data[x++], out d) ? d : default;
                sale.TCSCGSTRate = double.TryParse(data[x++], out d) ? d : default;
                sale.TCSCGSTAmount = double.TryParse(data[x++], out d) ? d : default;
                sale.TCSSGSTRate = double.TryParse(data[x++], out d) ? d : default;
                sale.TCSSGSTAmount = double.TryParse(data[x++], out d) ? d : default;
                sale.TotalTCSDeducted = double.TryParse(data[x++], out d) ? d : default;
                x++;
                sale.BuyerInvoiceDate = DateTime.TryParseExact(data[x++].Replace("\"","").Split(' ').FirstOrDefault(), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.AssumeLocal, out date) ? date : default;
                x++;
                x++;
                x++;
                sale.CustomersDeliveryPinCode = data[x++].Replace("\"","");
                sale.CustomersDeliveryState = data[x++].Replace("\"","");

                sales.Add(sale);
            }

            var invoices = new List<RawInvoice>();

            foreach (var _gS in sales.Where(s => s.EventType == "Sale").GroupBy(s => s.CustomersDeliveryState))
            {
                var state = _gS.Key;
                var invoice = GetRawInvoice(_gS);
                invoice.POS = stateCodeCollection[state];
                invoices.Add(invoice);
            }
            
            var json = JsonConvert.SerializeObject(invoices);
            File.WriteAllText("invocies.json", json);

            // Console.WriteLine(sales.Count);
            return null;
        }

        static RawInvoice GetRawInvoice(IEnumerable<FlipkartSale> sales)
        {
            var invoice = new RawInvoice();
            
            foreach (var sale in sales)
            {
                var found = invoice.RawInvoiceItems.Find(r => r.Name == sale.Product);
                if(found == null)
                {
                    found = new RawInvoiceItem();
                    found.Name = sale.Product;
                    found.Hsn = sale.HSN;
                    found.Quantity = 0;
                    found.TaxableValue = 0;
                    found.AmountIgst = 0;
                    found.AmountCgst = 0;
                    found.AmountSgst = 0;

                    invoice.RawInvoiceItems.Add(found);
                }
                var factor = 1;
                if(sale.EventType == "Return")
                    factor = -1;
                
                found.Quantity += sale.ItemQuantity.GetValueOrDefault(0) * factor;
                found.TaxableValue += sale.TaxableValue.GetValueOrDefault(0) * factor;
                found.AmountIgst += sale.IGSTAmount.GetValueOrDefault(0) * factor;
                found.AmountCgst += sale.CGSTAmount.GetValueOrDefault(0) * factor;
                found.AmountSgst += sale.SGSTAmount.GetValueOrDefault(0) * factor;
            }
            return invoice;
        }

        static Dictionary<string, int> stateCodeCollection = new Dictionary<string, int>();
        static void LoadStateCodes()
        {
            var lines = File.ReadAllLines("statecodes");
            for (int i = 0; i < lines.Length; i++)
            {
                stateCodeCollection.Add(lines[i + 1], int.TryParse(lines[i], out var r) ? r : default);
                i++;
            }
        }
    }

    public class FlipkartSale
    {
        public string SalerGstin { get; set; }
        public string OrderId { get; set; }
        public string OrderItemId { get; set; }
        public string Product { get; set; }
        public string FSN { get; set; }
        public string SKU { get; set; }
        public string HSN { get; set; }
        public string EventType { get; set; }
        public string SubEventType { get; set; }
        public string OrderType { get; set; }
        public string FulfilmentType { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? OrderApprovalDate { get; set; }
        public double? ItemQuantity { get; set; }
        public string ShippedFromState { get; set; }
        public double? PriceBeforeDiscount { get; set; }
        public double? Discount { get; set; }
        public double? SellerShareDiscount { get; set; }
        public double? BankOfferShare { get; set; }
        public double? PriceAfterDiscount { get; set; }
        public double? ShippingCharges { get; set; }
        public double? InvoiceAmount { get; set; }
        public string TypeOfTax { get; set; }
        public double? TaxableValue { get; set; }
        public double? IGSTRate { get; set; }
        public double? IGSTAmount { get; set; }
        public double? CGSTRate { get; set; }
        public double? CGSTAmount { get; set; }
        public double? SGSTAmount { get; set; }
        public double? TCSIGSTRate { get; set; }
        public double? TCSIGSTAmount { get; set; }
        public double? TCSCGSTRate { get; set; }
        public double? TCSCGSTAmount { get; set; }
        public double? TCSSGSTRate { get; set; }
        public double? TCSSGSTAmount { get; set; }
        public double? TotalTCSDeducted { get; set; }
        public DateTime? BuyerInvoiceDate { get; set; }
        public string CustomersDeliveryPinCode { get; set; }
        public string CustomersDeliveryState { get; set; }
    }
}
