using System;

namespace WindowsFormsApp1.Models
{
    /// <summary>
    /// Podatkovni razred za stranko. Brez logike - samo lastnosti, ki jih Dapper preslika iz tabele customers.
    /// Takšen razred (POCO) je neodvisen od baze in WinForms, zato ga lahko uporabljajo vse plasti in testi.
    /// </summary>
    public class Customer
    {
        // Id dodeli baza (AUTO_INCREMENT). Nova, še neshranjena stranka ima Id = 0;
        // repozitorij ga po INSERT-u nastavi iz LAST_INSERT_ID().
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // INTERVJU: optimistično zaklepanje. Vrednost si zapomnimo ob branju; UPDATE uspe samo,
        // če je v bazi še vedno ista (WHERE ... AND updated_at = @UpdatedAt). Če jo je vmes
        // spremenil nekdo drug, UPDATE ne najde vrstice in uporabnika opozorimo, namesto da bi
        // tiho prepisali njegove spremembe ("lost update").
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Ime in priimek v eni vrstici (za prikaz v naslovih in sporočilih).
        /// </summary>
        public string FullName
        {
            get { return ((FirstName ?? string.Empty) + " " + (LastName ?? string.Empty)).Trim(); }
        }

        /// <summary>
        /// Plitva kopija - obrazec za urejanje dela na kopiji, da preklic ne spremeni originala.
        /// Plitva zadošča, ker so vse lastnosti nespremenljivi tipi (string, int, DateTime).
        /// </summary>
        public Customer Clone()
        {
            return (Customer)MemberwiseClone();
        }
    }
}
