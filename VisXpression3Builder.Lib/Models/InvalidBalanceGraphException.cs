using System;
namespace VisXpression3Builder.Lib.Models
{
    public class InvalidBalanceGraphException : Exception
    {
        public InvalidBalanceGraphException(string message) : base(message) { }
        public InvalidBalanceGraphException(string message, Exception innerException) : base(message, innerException) { }
    }
}