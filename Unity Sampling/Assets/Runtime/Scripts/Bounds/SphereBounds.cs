using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Sampling
{
	public class SphereBounds : Bounds3D
	{
		private Vector3 _center;
		private float _radius;

		public SphereBounds(Vector3 center, float radius)
		{
			ValidateRadius(radius);
			this._center = center;
			this._radius = radius;
		}

		private void ValidateRadius(float radius)
		{
			if (radius < 0)
				throw new InvalidBoundsException($"Please provide a positive radius value for {nameof(SphereBounds)}");
		}

		public bool Contains(Vector3 point)
		{
			Vector3 transformedPoint = point - _center;
			return transformedPoint.magnitude <= _radius;
		}

		public override string ToString()
		{
			return $"{nameof(SphereBounds)}({nameof(_center)} {_center.ToString()} | {nameof(_radius)} {_radius.ToString()})";
		}
	}
}
