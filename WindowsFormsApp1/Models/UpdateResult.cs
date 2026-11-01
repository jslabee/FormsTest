namespace WindowsFormsApp1.Models
{
    /// <summary>
    /// Izid posodobitve z optimističnim zaklepanjem. Repozitorij ne meče izjem za pričakovane
    /// poslovne primere, ampak vrne izid - odločitev, kaj z njim, je na servisu.
    /// </summary>
    public enum UpdateResult
    {
        // Vrstica je bila posodobljena.
        Updated,

        // Vrstice ni več (nekdo jo je izbrisal).
        NotFound,

        // Vrstica obstaja, a jo je vmes spremenil nekdo drug (updated_at se ne ujema).
        Conflict
    }
}
