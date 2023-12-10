using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Sampling.Examples
{
    public abstract class BoundsSettings3D : ScriptableObject
    {
        public abstract Bounds3D GetBounds();
        public abstract Vector3 GetCenter();
        public abstract Type BoundsType { get; }

        public enum Type
		{
            Sphere,
            Cube
		}
    }
}
