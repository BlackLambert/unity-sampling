using System;

namespace SBaier.Sampling
{
    public class InvalidBoundsException : Exception
    {
        public InvalidBoundsException() : base("Please provide bounds with valid values") { }
        public InvalidBoundsException(string message) : base(message) { }
    }
}
