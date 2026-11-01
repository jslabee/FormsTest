using System.Collections.Generic;
using System.IO;
using System.Text;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Services
{
    /// <summary>
    /// Branje in pisanje strank v CSV obliki. Dela z nizi/TextReaderjem, ne z datotekami,
    /// zato ga je mogoče testirati brez diska (StringReader).
    /// </summary>
    // Ločilo je podpičje: slovenski Excel (decimalna vejica) privzeto uporablja ";" in
    // datoteko z vejicami bi odprl kot en sam stolpec.
    public static class CustomerCsv
    {
        public const char Delimiter = ';';

        public const string Header = "Ime;Priimek;Email;Telefon";

        /// <summary>
        /// Stranke zapiše v CSV niz (z glavo).
        /// </summary>
        public static string Write(IEnumerable<Customer> customers)
        {
            // StringBuilder namesto "+=" v zanki: niz je nespremenljiv, "+=" bi vsakič ustvaril nov niz.
            var builder = new StringBuilder();
            builder.AppendLine(Header);

            foreach (Customer customer in customers)
            {
                builder.Append(Escape(customer.FirstName)).Append(Delimiter)
                       .Append(Escape(customer.LastName)).Append(Delimiter)
                       .Append(Escape(customer.Email)).Append(Delimiter)
                       .Append(Escape(customer.Phone))
                       .AppendLine();
            }

            return builder.ToString();
        }

        /// <summary>
        /// Prebere stranke iz CSV. Prazne vrstice preskoči, glavo (prva vrstica "Ime;...") prepozna.
        /// Polja z narekovaji ("Novak; ml.") so podprta, prelomi vrstic znotraj polja pa ne.
        /// </summary>
        public static List<CsvCustomerRow> Parse(TextReader reader)
        {
            var rows = new List<CsvCustomerRow>();
            int lineNumber = 0;
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // Glavo prepoznamo po vsebini, ne po položaju - datoteka je lahko tudi brez nje.
                if (lineNumber == 1 && line.TrimStart().StartsWith("Ime" + Delimiter))
                {
                    continue;
                }

                List<string> fields = ParseLine(line);

                rows.Add(new CsvCustomerRow
                {
                    LineNumber = lineNumber,
                    Customer = new Customer
                    {
                        FirstName = GetField(fields, 0),
                        LastName = GetField(fields, 1),
                        Email = GetField(fields, 2),
                        Phone = GetField(fields, 3)
                    }
                });
            }

            return rows;
        }

        /// <summary>
        /// Razdeli eno vrstico na polja. Upošteva narekovaje: znotraj njih ";" ni ločilo,
        /// "" pa pomeni en narekovaj.
        /// </summary>
        public static List<string> ParseLine(string line)
        {
            var fields = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        // Podvojen narekovaj znotraj polja je en dobesedni narekovaj.
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            current.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        current.Append(c);
                    }
                }
                else if (c == '"')
                {
                    inQuotes = true;
                }
                else if (c == Delimiter)
                {
                    fields.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            fields.Add(current.ToString());
            return fields;
        }

        /// <summary>
        /// Vrednost za CSV: če vsebuje ločilo, narekovaj ali prelom vrstice, jo damo v narekovaje.
        /// </summary>
        public static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            bool needsQuotes = value.IndexOf(Delimiter) >= 0 || value.IndexOf('"') >= 0 ||
                               value.IndexOf('\r') >= 0 || value.IndexOf('\n') >= 0;

            return needsQuotes ? "\"" + value.Replace("\"", "\"\"") + "\"" : value;
        }

        // Manjkajoče polje (prekratka vrstica) vrne null, enako kot prazno polje.
        private static string GetField(List<string> fields, int index)
        {
            if (index >= fields.Count)
            {
                return null;
            }

            string value = fields[index].Trim();
            return value.Length == 0 ? null : value;
        }
    }
}
