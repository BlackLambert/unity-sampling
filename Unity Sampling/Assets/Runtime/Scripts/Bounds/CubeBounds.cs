using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Sampling
{
	public class CubeBounds : Bounds3D
	{
		private readonly Vector3 _leftFront;
		private Vector3 _size;

		public CubeBounds(Vector3 leftFront, Vector3 size)
		{
			ValidateSize(size);
			this._leftFront = leftFront;
			this._size = size;
		}

		private void ValidateSize(Vector3 size)
		{
			if (size.x < 0 || size.y < 0 || size.z < 0)
				throw new InvalidBoundsException($"Please provide a positive size value for {nameof(CubeBounds)}.");
		}

		public bool Contains(Vector3 point)
		{
			Vector3 transformedPoint = point - _leftFront;
			return transformedPoint.x <= _size.x &&
				transformedPoint.y <= _size.y &&
				transformedPoint.z <= _size.z &&
				transformedPoint.x >= 0 &&
				transformedPoint.y >= 0 &&
				transformedPoint.z >= 0;
		}

		public override string ToString()
		{
			return $"{nameof(CubeBounds)}({nameof(_leftFront)} {_leftFront.ToString()} | {nameof(_size)} {_size.ToString()})";
		}
	}
}
