using UnityEngine;

namespace SBaier.Sampling
{
    public interface Bounds2D
    {
        bool Contains(Vector2 point);
    }
}
