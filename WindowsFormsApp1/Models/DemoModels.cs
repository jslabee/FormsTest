using System.Data;

namespace WindowsFormsApp1.Models
{
    /// <summary>
    /// Rezultat enega primera iz obrazca "Demo napak": besedilo z razlago in po želji podatki
    /// (npr. rezultat EXPLAIN ali najdene vrstice), ki jih obrazec prikaže v mreži.
    /// </summary>
    public class DemoResult
    {
        public string Text { get; set; }

        // Karkoli, kar zna prikazati DataGridView (DataTable ali seznam objektov); null = mreža ostane prazna.
        public object GridData { get; set; }
    }

    /// <summary>
    /// Rezultat demo poizvedbe: čas izvajanja, število vrstic in načrt izvajanja (EXPLAIN).
    /// </summary>
    public class QueryPlanResult
    {
        public string Sql { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public int RowCount { get; set; }

        // DataTable (System.Data) ni vezan na WinForms. Uporabimo ga, ker so stolpci EXPLAIN-a
        // odvisni od verzije MySQL - ne želimo fiksnega razreda.
        public DataTable Plan { get; set; }
    }

    /// <summary>
    /// Rezultat demo odpiranja povezav.
    /// </summary>
    public class ConnectionDemoResult
    {
        public int Opened { get; set; }

        // Koliko različnih fizičnih povezav je videl strežnik (CONNECTION_ID()).
        public int DistinctServerConnections { get; set; }

        // Sporočilo napake (npr. "Too many connections") ali null.
        public string Error { get; set; }
    }
}
