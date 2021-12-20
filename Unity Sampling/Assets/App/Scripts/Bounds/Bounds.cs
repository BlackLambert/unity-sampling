using System;
using UnityEngine;

namespace SBaier.Sampling
{
    public interface Bounds
    {
        bool Contains(Vector3 point);

        public class InvalidBoundsException : ArgumentException 
        {
            public InvalidBoundsException() : base("Please provide bounds with valid values") { }
            public InvalidBoundsException(string message) : base(message) { }
        }
    }
}
