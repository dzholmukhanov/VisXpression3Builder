using System;
namespace VisXpression3Builder.Lib.Models
{
    public class InvalidGraphException : Exception
    {
        public InvalidGraphException(string message) : base(message) { }
        public InvalidGraphException(string message, Exception innerException) : base(message, innerException) { }
    }
}