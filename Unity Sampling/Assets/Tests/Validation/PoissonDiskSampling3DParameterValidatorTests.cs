using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace SBaier.Sampling.Tests
{
    public class PoissonDiskSampling3DParameterValidatorTests
    {
		private readonly int[] _testSamplesAmount = new int[] { 3, 21, 10, 1, 100 };
		private readonly float[] _testMinDistance = new float[] { 2, 1.2f, 11f, 2, 25f };
		private readonly Vector3[] _testBounds = new Vector3[]
		{
			new Vector3(10, 20, 5),
			new Vector3(0.1f, 200, 10),
			new Vector3(3, 20, 200),
			new Vector3(0, 0, 0),
			new Vector3(150, 150, 150)
		};

		private readonly Vector3[] _testStartPositions = new Vector3[]
		{
			new Vector3(0, 0, 0),
			new Vector3(0.05f, 100, 5),
			new Vector3(1, 1, 1),
			new Vector3(0, 0, 0),
			new Vector3(149, 0, 0)
		};

		private readonly int[] _invalidSamplesAmount = new int[] { -1, -11, -19, -3, 0 };
		private readonly float[] _invalidMinDistance = new float[] { -2, -1.2f, -11f, -0.001f, -0.7f };

		private readonly Vector3[] _invalidBounds = new Vector3[]
		{
			new Vector3(-10, 20, 5),
			new Vector3(-0.1f, -200, -10),
			new Vector3(3, -20, 200),
			new Vector3(-0.01f, 0.1f, -0.7f),
			new Vector3(150, -150, -50)
		};

		private readonly Vector3[] _invalidStartPositions = new Vector3[]
		{
			new Vector3(-3, 3, 0),
			new Vector3(0.2f, 100, 5),
			new Vector3(3, 20, -200),
			new Vector3(0.01f, 0.1f, 0.7f),
			new Vector3(140, 151, 50)
		};

		private Validator<PoissonDiskSampling3D.Parameters> _validator;



		[Test]
		public void Validate_NegativeAmountThrowsException()
		{
			for (int i = 0; i < _invalidSamplesAmount.Length; i++)
			{
				GivenANewValidator();
				TestDelegate test = () => WhenValidateIsCalled(CreateParametersWithInvalidAmount(i));
				ThenThrowsInvalidAmountException(test);
			}
		}

		[Test]
		public void Validate_NegativeMinDistanceThrowsException()
		{
			for (int i = 0; i < _invalidMinDistance.Length; i++)
			{
				GivenANewValidator();
				TestDelegate test = () => WhenValidateIsCalled(CreateParametersWithInvalidMinDistance(i));
				ThenThrowsInvalidMinDistanceException(test);
			}
		}

		[Test]
		public void Validate_InvalidBoundsThrowsException()
		{
			for (int i = 0; i < _invalidBounds.Length; i++)
			{
				GivenANewValidator();
				TestDelegate test = () => WhenValidateIsCalled(CreateParametersWithInvalidBounds(i));
				ThenThrowsInvalidBoundsException(test);
			}
		}

		[Test]
		public void Validate_InvalidStartPositionThrowsException()
		{
			for (int i = 0; i < _invalidStartPositions.Length; i++)
			{
				GivenANewValidator();
				TestDelegate test = () => WhenValidateIsCalled(CreateParametersWithInvalidStartPosition(i));
				ThenThrowsInvalidStartPositionException(test);
			}
		}

		[Test]
		public void Validate_NoExceptionOnValidParameters()
		{
			for (int i = 0; i < _invalidStartPositions.Length; i++)
			{
				GivenANewValidator();
				TestDelegate test = () => WhenValidateIsCalled(CreateParameters(i));
				ThenThrowsNoException(test);
			}
		}

		private void GivenANewValidator()
		{
			_validator = new PoissonDiskSampling3DParameterValidator();
		}

		private void WhenValidateIsCalled(PoissonDiskSampling3D.Parameters parameters)
		{
			_validator.Validate(parameters);
		}

		private PoissonDiskSampling3D.Parameters CreateParameters(int index)
		{
			int amount = _testSamplesAmount[index];
			float minDistance = _testMinDistance[index];
			Vector3 bounds = _testBounds[index];
			Vector3 startPos = _testStartPositions[index];

			return new PoissonDiskSampling3D.Parameters(amount, minDistance, bounds, startPos);
		}

		private PoissonDiskSampling3D.Parameters CreateParametersWithInvalidAmount(int index)
		{
			int amount = _invalidSamplesAmount[index];
			float minDistance = _testMinDistance[index];
			Vector3 bounds = _testBounds[index];
			Vector3 startPos = _testStartPositions[index];

			return new PoissonDiskSampling3D.Parameters(amount, minDistance, bounds, startPos);
		}

		private PoissonDiskSampling3D.Parameters CreateParametersWithInvalidMinDistance(int index)
		{
			int amount = _testSamplesAmount[index];
			float minDistance = _invalidMinDistance[index];
			Vector3 bounds = _testBounds[index];
			Vector3 startPos = _testStartPositions[index];

			return new PoissonDiskSampling3D.Parameters(amount, minDistance, bounds, startPos);
		}

		private PoissonDiskSampling3D.Parameters CreateParametersWithInvalidBounds(int index)
		{
			int amount = _testSamplesAmount[index];
			float minDistance = _testMinDistance[index];
			Vector3 bounds = _invalidBounds[index];
			Vector3 startPos = _testStartPositions[index];

			return new PoissonDiskSampling3D.Parameters(amount, minDistance, bounds, startPos);
		}

		private PoissonDiskSampling3D.Parameters CreateParametersWithInvalidStartPosition(int index)
		{
			int amount = _testSamplesAmount[index];
			float minDistance = _testMinDistance[index];
			Vector3 bounds = _testBounds[index];
			Vector3 startPos = _invalidStartPositions[index];

			return new PoissonDiskSampling3D.Parameters(amount, minDistance, bounds, startPos);
		}

		private void ThenThrowsInvalidAmountException(TestDelegate test)
		{
			Assert.Throws<PoissonDiskSampling3DParameterValidator.InvalidAmountException>(test);
		}

		private void ThenThrowsInvalidMinDistanceException(TestDelegate test)
		{
			Assert.Throws<PoissonDiskSampling3DParameterValidator.InvalidMinDistanceException>(test);
		}

		private void ThenThrowsInvalidBoundsException(TestDelegate test)
		{
			Assert.Throws<PoissonDiskSampling3DParameterValidator.InvalidBoundsException>(test);
		}

		private void ThenThrowsInvalidStartPositionException(TestDelegate test)
		{
			Assert.Throws<PoissonDiskSampling3DParameterValidator.InvalidStartPositionException>(test);
		}

		private void ThenThrowsNoException(TestDelegate test)
		{
			Assert.DoesNotThrow(test);
		}
	}
}
