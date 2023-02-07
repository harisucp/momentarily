using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CGerome.DataAccessLayer.Entities;

namespace CGerome.EntityConverters
{
    public class InvoiceToHistoryInvoice
    {
        public static GrHistoryInvoice Map(GrInvoice invoice)
        {
            return new GrHistoryInvoice()
                       {
                           Amount = invoice.Amount,
                           Date = invoice.Date,
                           Docnumber = invoice.Docnumber,
                           GeromeId = invoice.GeromeId,
                           //after Map we need set proper InvoiceHeader (GrHistoryInvoiceHeaders)
                           //InvoiceHeader = invoice.InvoiceHeader,
                           LongName = invoice.LongName,
                           Paid = invoice.Paid,
                           Price = invoice.Price,
                           ProductId = invoice.ProductId,
                           SellPrice = invoice.SellPrice,
                           VendorId = invoice.VendorId,                           
                       };
        }

        public static GrInvoice Map(GrHistoryInvoice invoice)
        {
            return new GrInvoice()
            {
                Amount = invoice.Amount,
                Date = invoice.Date,
                Docnumber = invoice.Docnumber,
                GeromeId = invoice.GeromeId,
                //after Map we need set proper InvoiceHeader (GrHistoryInvoiceHeaders)
                //InvoiceHeader = invoice.InvoiceHeader,
                LongName = invoice.LongName,
                Paid = invoice.Paid,
                Price = invoice.Price,
                ProductId = invoice.ProductId,
                SellPrice = invoice.SellPrice,
                VendorId = invoice.VendorId,
            };
        }
    }

    /// <summary>
    /// Creates new GrHistoryInvoiceHeaders Based on GrInvoiceHeaders
    /// </summary>
    public class InvoiceHeadersToHistoryInvoiceHeaders
    {
        public static GrHistoryInvoiceHeaders Map(GrInvoiceHeaders header)
        {
            var historyInvoiceHeaders = new GrHistoryInvoiceHeaders()
                                            {
                                                Date = header.Date,
                                                Docnumber = header.Docnumber,
                                                Paid = header.Paid,
                                                Vendor = header.Vendor,
                                                KursIn = header.KursIn,
                                                KursOut = header.KursOut,
                                                Summ = header.Summ
                                            };

            historyInvoiceHeaders.Invoice = header.Invoice.Select
                (
                    x => { 
                        var historyInvoice = InvoiceToHistoryInvoice.Map(x);
                        historyInvoice.InvoiceHeader = historyInvoiceHeaders;
                        return historyInvoice;
                    }
                ).ToList();

            return historyInvoiceHeaders;
        }

        public static GrInvoiceHeaders Map(GrHistoryInvoiceHeaders header)
        {
            var invoiceHeaders = new GrInvoiceHeaders()
            {
                Date = header.Date,
                Docnumber = header.Docnumber,
                Paid = header.Paid,
                Vendor = header.Vendor,
                KursIn = header.KursIn,
                KursOut = header.KursOut,
                Summ = header.Summ,
            };

            invoiceHeaders.Invoice = header.Invoice.Select
                (
                    x =>
                    {
                        var invoice = InvoiceToHistoryInvoice.Map(x);
                        invoice.InvoiceHeader = invoiceHeaders;
                        return invoice;
                    }
                ).ToList();

            return invoiceHeaders;
        }
    }
}
