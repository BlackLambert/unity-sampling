using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace SBaier.Sampling.Tests
{
    public class PoissonDiskSampling3DTests
    {
		private const int _retries = 50;

		private const float _epsilon = 0.01f;
		private const int _highAmount = 500;
		private readonly int[] _testSeeds = new int[] {0, 42, 123, 2, 1024 };
		private readonly int[] _testSamplesAmount = new int[] {3, 21, 10, 1, 100 };
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

		private PoissonDiskSampling3D _sampling;
        private List<Vector3> _samples;
		Mock<Validator<PoissonDiskSampling3D.Parameters>> _validatorMock;
		private bool _validateCalled = false;

		[Test]
        public void Sample_ResultHasExpectedAmountOfSamples()
        {
			for (int i = 0; i < _testSamplesAmount.Length; i++)
			{
				GivenADefaultSetup(i);
				WhenSampleIsCalled(CreateParameters(i));
				ThenResultAmountEqualsRequestedAmount(_testSamplesAmount[i]);
				TearDown();
			}
        }


		[Test]
        public void Sample_SamplesHaveMinimalSpacing()
        {
			for (int i = 0; i < _testSamplesAmount.Length; i++)
			{
				GivenADefaultSetup(i);
				WhenSampleIsCalled(CreateParameters(i));
				ThenSamplesHaveMinimalSpacing(_testMinDistance[i]);
				TearDown();
			}
        }


		[Test]
        public void Sample_SamplesAreInBounds()
        {
			for (int i = 0; i < _testSamplesAmount.Length; i++)
			{
				GivenADefaultSetup(i);
				WhenSampleIsCalled(CreateParameters(i));
				ThenSamplesAreWithinBounds(_testBounds[i]);
				TearDown();
			}
        }

		[Test]
		public void Sample_UnreachableAmountCausesException()
		{
			for (int i = 0; i < _testSamplesAmount.Length; i++)
			{
				GivenADefaultSetup(i);
				TestDelegate test = () => WhenSampleIsCalled(CreateParametersWithHighAmount(i));
				ThenThrowsSamplingException(test);
				TearDown();
			}
		}

		[Test]
		public void Sample_ValidateIsCalled()
		{
			for (int i = 0; i < _testSamplesAmount.Length; i++)
			{
				GivenADefaultSetup(i);
				WhenSampleIsCalled(CreateParameters(i));
				ThenValidateOfTheValidatorIsCalled();
				TearDown();
			}
		}

		private void GivenADefaultSetup(int index)
		{
			GivenADefaultValidatorMock();
			GivenANewPoissonDiskSampling(index);
		}

		private void GivenADefaultValidatorMock()
		{
			_validatorMock = new Mock<Validator<PoissonDiskSampling3D.Parameters>>();
			_validatorMock.Setup(p => p.Validate(It.IsAny<PoissonDiskSampling3D.Parameters>())).Callback(() => _validateCalled = true);
		}

		private void GivenANewPoissonDiskSampling(int index)
        {
            _sampling = new PoissonDiskSampling3D(CreateRandom(index), _retries, _validatorMock.Object);
		}

		private void WhenSampleIsCalled(PoissonDiskSampling3D.Parameters parameters)
		{
			_samples = _sampling.Sample(parameters);
		}

		private void ThenResultAmountEqualsRequestedAmount(int amount)
		{
			Assert.AreEqual(amount, _samples.Count);
		}

		private void ThenSamplesHaveMinimalSpacing(float minDistance)
		{
			int amount = _samples.Count;
			for (int i = 0; i < amount; i++)
			{
				for (int j = i + 1; j < amount; j++)
				{
					Vector3 first = _samples[i];
					Vector3 second = _samples[j];
					Assert.GreaterOrEqual((first - second).magnitude + _epsilon, minDistance,
						$"Two samples [{first.ToString()} | {second.ToString()}] have smaller distance than {minDistance}.");
				}
			}
		}


		private void ThenSamplesAreWithinBounds(Vector3 bounds)
		{
			foreach (Vector3 sample in _samples)
			{
				Assert.GreaterOrEqual(sample.x, 0);
				Assert.GreaterOrEqual(sample.y, 0);
				Assert.GreaterOrEqual(sample.z, 0);
				Assert.LessOrEqual(sample.x, bounds.x + _epsilon);
				Assert.LessOrEqual(sample.y, bounds.y + _epsilon);
				Assert.LessOrEqual(sample.z, bounds.z + _epsilon);
			}
		}

		

		private void ThenThrowsSamplingException(TestDelegate test)
		{
			Assert.Throws<PoissonDiskSampling3D.SamplingException>(test);
		}

		private void ThenValidateOfTheValidatorIsCalled()
		{
			Assert.IsTrue(_validateCalled);
		}

		[TearDown]
		public void TearDown()
		{
			_samples = null;
			_validateCalled = false;
		}

		private System.Random CreateRandom(int index)
		{
			return new System.Random(_testSeeds[index]);
		}


		private PoissonDiskSampling3D.Parameters CreateParameters(int index)
		{
			int amount = _testSamplesAmount[index];
			float minDistance = _testMinDistance[index];
			Vector3 bounds = _testBounds[index];
			Vector3 startPos = _testStartPositions[index];

			return new PoissonDiskSampling3D.Parameters(amount, minDistance, bounds, startPos);
		}

		private PoissonDiskSampling3D.Parameters CreateParametersWithHighAmount(int index)
		{
			int amount = _highAmount;
			float minDistance = _testMinDistance[index];
			Vector3 bounds = _testBounds[index];
			Vector3 startPos = _testStartPositions[index];

			return new PoissonDiskSampling3D.Parameters(amount, minDistance, bounds, startPos);
		}
	}
}