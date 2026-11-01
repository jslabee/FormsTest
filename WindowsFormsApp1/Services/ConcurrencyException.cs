using System;

namespace WindowsFormsApp1.Services
{
    /// <summary>
    /// Zapis je vmes spremenil drug uporabnik (optimistično zaklepanje ni uspelo).
    /// Lasten tip izjeme, da ga obrazec ujame posebej in uporabniku ponudi osvežitev,
    /// namesto splošnega sporočila o napaki.
    /// </summary>
    // [Serializable] in konstruktorji po vzoru .NET smernic za izjeme.
    [Serializable]
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException()
        {
        }

        public ConcurrencyException(string message)
            : base(message)
        {
        }

        public ConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // Konstruktor za deserializacijo (zahteva ga [Serializable] na .NET Frameworku).
        protected ConcurrencyException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
