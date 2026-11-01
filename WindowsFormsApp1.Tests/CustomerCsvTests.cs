using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Tests
{
    /// <summary>
    /// Testi za branje/pisanje CSV. Ker CustomerCsv dela s TextReaderjem in nizi, testi ne
    /// potrebujejo datotek na disku (StringReader).
    /// </summary>
    [TestClass]
    public class CustomerCsvTests
    {
        [TestMethod]
        public void Parse_SkipsHeaderAndEmptyLines_KeepsLineNumbers()
        {
            string csv = "Ime;Priimek;Email;Telefon\r\nJanez;Novak;janez@example.si;041 111 222\r\n\r\nŠpela;Krajnc;;\r\n";

            List<CsvCustomerRow> rows = CustomerCsv.Parse(new StringReader(csv));

            Assert.AreEqual(2, rows.Count);
            Assert.AreEqual(2, rows[0].LineNumber);
            Assert.AreEqual("Janez", rows[0].Customer.FirstName);
            Assert.AreEqual(4, rows[1].LineNumber);
            Assert.AreEqual("Špela", rows[1].Customer.FirstName);
            Assert.IsNull(rows[1].Customer.Email, "Prazno polje mora biti null.");
        }

        [TestMethod]
        public void ParseLine_QuotedFieldWithDelimiterAndQuote_IsOneField()
        {
            List<string> fields = CustomerCsv.ParseLine("Janez;\"Novak; ml. \"\"Janč\"\"\";j@x.si");

            Assert.AreEqual(3, fields.Count);
            Assert.AreEqual("Novak; ml. \"Janč\"", fields[1]);
        }

        [TestMethod]
        public void Parse_ShortLine_MissingFieldsAreNull()
        {
            List<CsvCustomerRow> rows = CustomerCsv.Parse(new StringReader("Ana"));

            Assert.AreEqual("Ana", rows[0].Customer.FirstName);
            Assert.IsNull(rows[0].Customer.LastName);
            Assert.IsNull(rows[0].Customer.Phone);
        }

        [TestMethod]
        public void Write_ThenParse_RoundTripPreservesSpecialCharacters()
        {
            var customers = new List<Customer>
            {
                new Customer { FirstName = "Žiga", LastName = "Kovačič; st.", Email = "ziga@example.si", Phone = "040 \"službeni\"" },
                new Customer { FirstName = "Ana" }
            };

            string csv = CustomerCsv.Write(customers);
            List<CsvCustomerRow> rows = CustomerCsv.Parse(new StringReader(csv));

            Assert.AreEqual(2, rows.Count);
            Assert.AreEqual("Kovačič; st.", rows[0].Customer.LastName);
            Assert.AreEqual("040 \"službeni\"", rows[0].Customer.Phone);
            Assert.AreEqual("Ana", rows[1].Customer.FirstName);
            Assert.IsNull(rows[1].Customer.Email);
        }

        [TestMethod]
        public void Escape_PlainValue_IsNotQuoted()
        {
            Assert.AreEqual("Novak", CustomerCsv.Escape("Novak"));
            Assert.AreEqual(string.Empty, CustomerCsv.Escape(null));
        }
    }
}
