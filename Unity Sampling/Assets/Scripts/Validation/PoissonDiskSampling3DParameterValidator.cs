using System;
using UnityEngine;

namespace SBaier.Sampling
{
	public class PoissonDiskSampling3DParameterValidator : Validator<PoissonDiskSampling3D.Parameters>
	{
		public void Validate(PoissonDiskSampling3D.Parameters parameters)
		{
			ValidateAmount(parameters.Amount);
			ValidateMinDistance(parameters.MinDistance);
			ValidateBounds(parameters.Bounds);
			ValidateStartPosition(parameters.Bounds, parameters.StartPosition);
		}

		private void ValidateAmount(int amount)
		{
			if (amount <= 0)
				throw new InvalidAmountException();
		}

		private void ValidateMinDistance(float minDistance)
		{
			if (minDistance < 0)
				throw new InvalidMinDistanceException();
		}

		private void ValidateBounds(Vector3 bounds)
		{
			if (bounds.x < 0 ||
				bounds.y < 0 ||
				bounds.z < 0)
				throw new InvalidBoundsException();
		}

		private void ValidateStartPosition(Vector3 bounds, Vector3 startPos)
		{
			Bounds boundsObject = new Bounds(bounds / 2, bounds);
			if (!boundsObject.Contains(startPos))
				throw new InvalidStartPositionException();
		}

		public class InvalidAmountException : ArgumentException { }
		public class InvalidMinDistanceException : ArgumentException { }
		public class InvalidBoundsException : ArgumentException { }
		public class InvalidStartPositionException : ArgumentException { }
	}
}
