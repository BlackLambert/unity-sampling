using UnityEngine;

namespace SBaier.Sampling
{
    public class RectangleBounds : Bounds2D
	{
		private readonly Vector2 _bottomLeftCorner;
		private readonly Vector2 _size;

		public RectangleBounds(Vector2 bottomLeftCorner, Vector2 size)
		{
			ValidateSize(size);
			_bottomLeftCorner = bottomLeftCorner;
			_size = size;
		}

		private void ValidateSize(Vector2 size)
		{
			if (size.x < 0 || size.y < 0)
				throw new InvalidBoundsException($"Please provide a positive size value for {nameof(RectangleBounds)}.");
		}

		public bool Contains(Vector2 point)
		{
			Vector2 transformedPoint = point - _bottomLeftCorner;
			return transformedPoint.x >= 0 &&
				transformedPoint.y >= 0 &&
				transformedPoint.x <= _size.x &&
				transformedPoint.y <= _size.y;
		}
    }
}
