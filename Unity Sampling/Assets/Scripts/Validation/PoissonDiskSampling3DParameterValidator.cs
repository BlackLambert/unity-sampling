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

		private void ValidateStartPosition(Bounds bounds, Vector3 startPos)
		{
			if (!bounds.Contains(startPos))
				throw new InvalidStartPositionException();
		}

		public class InvalidAmountException : ArgumentException { }
		public class InvalidMinDistanceException : ArgumentException { }
		public class InvalidStartPositionException : ArgumentException { }
	}
}
