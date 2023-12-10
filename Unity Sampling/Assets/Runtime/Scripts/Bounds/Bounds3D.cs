using System;
using UnityEngine;

namespace SBaier.Sampling
{
    public interface Bounds3D
    {
        bool Contains(Vector3 point);
    }
}
