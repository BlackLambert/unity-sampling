using System;
using UnityEngine;

namespace SBaier.Sampling
{
	public class PoissonDiskSampling2DParameterValidator : Validator<PoissonDiskSampling2D.Parameters>
	{
		public void Validate(PoissonDiskSampling2D.Parameters parameters)
		{
			ValidateAmount(parameters.Amount);
			ValidateMinDistance(parameters.MinDistance);
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

		private void ValidateStartPosition(Bounds2D bounds, Vector2 startPos)
		{
			if (!bounds.Contains(startPos))
				throw new InvalidStartPositionException();
		}

		public class InvalidAmountException : ArgumentException { }
		public class InvalidMinDistanceException : ArgumentException { }
		public class InvalidStartPositionException : ArgumentException { }
	}
}
