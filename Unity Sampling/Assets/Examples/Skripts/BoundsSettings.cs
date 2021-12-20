using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Sampling.Examples
{
    public abstract class BoundsSettings : ScriptableObject
    {
        public abstract Bounds GetBounds();
        public abstract Vector3 GetCenter();
        public abstract Type BoundsType { get; }

        public enum Type
		{
            Sphere,
            Cube
		}
    }
}
