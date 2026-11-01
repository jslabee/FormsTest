using System.Collections.Generic;
using System.Text.RegularExpressions;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Services
{
    /// <summary>
    /// Pravila za veljavno stranko. Vrne seznam napak, da jih obrazec lahko prikaže.
    /// Pravila so na enem mestu, zato jih uporabljajo obrazec (ErrorProvider), servis
    /// (varovalka pred shranjevanjem) in uvoz CSV - in jih je mogoče testirati brez UI.
    /// </summary>
    public static class CustomerValidator
    {
        // Namerno preprost vzorec: nekaj@nekaj.domena, brez presledkov.
        // Popolna RFC validacija emaila z regexom ni smiselna.
        // RegexOptions.Compiled: regex se prevede enkrat (static readonly) in je nato hitrejši.
        private static readonly Regex EmailRegex =
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        /// <summary>
        /// Vrne vse napake stranke (prazen seznam = stranka je veljavna).
        /// </summary>
        public static List<string> Validate(Customer customer)
        {
            var errors = new List<string>();

            string firstNameError = ValidateFirstName(customer.FirstName);
            if (firstNameError != null)
            {
                errors.Add(firstNameError);
            }

            string emailError = ValidateEmail(customer.Email);
            if (emailError != null)
            {
                errors.Add(emailError);
            }

            return errors;
        }

        /// <summary>
        /// Napaka za polje Ime ali null. Ločena metoda, da jo obrazec uporabi v Validating dogodku posameznega polja.
        /// </summary>
        public static string ValidateFirstName(string firstName)
        {
            return string.IsNullOrWhiteSpace(firstName) ? "Ime je obvezno." : null;
        }

        /// <summary>
        /// Napaka za polje Email ali null. Email ni obvezen, če pa je vpisan, mora biti pravilen.
        /// </summary>
        public static string ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            return IsValidEmail(email) ? null : "Email ni v pravilni obliki.";
        }

        /// <summary>
        /// Ali je niz v obliki nekaj@nekaj.domena.
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            return email != null && EmailRegex.IsMatch(email.Trim());
        }

        /// <summary>
        /// Poenoti podatke pred shranjevanjem: obreže presledke, prazna neobvezna polja spremeni v null
        /// (v bazi NULL = "ni podatka", prazen niz bi bil "podatek, ki je prazen").
        /// </summary>
        public static void Normalize(Customer customer)
        {
            customer.FirstName = customer.FirstName?.Trim();
            customer.LastName = EmptyToNull(customer.LastName);
            customer.Email = EmptyToNull(customer.Email);
            customer.Phone = EmptyToNull(customer.Phone);
        }

        private static string EmptyToNull(string value)
        {
            string trimmed = value?.Trim();
            return string.IsNullOrEmpty(trimmed) ? null : trimmed;
        }
    }
}
