using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Sampling.Examples
{
    public abstract class BoundsSettings2D : ScriptableObject
    {
        public abstract Bounds2D GetBounds();
        public abstract Vector2 GetCenter();
        public abstract Type BoundsType { get; }

        public enum Type
        {
            Rectangle
        }
    }
}
