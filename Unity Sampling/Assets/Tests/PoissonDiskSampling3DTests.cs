using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace SBaier.Sampling.Tests
{
    public class PoissonDiskSampling3DTests
    {
		private const int _retries = 50;

		private const float _epsilon = 0.01f;
		private readonly int[] _testSeeds = new int[] {0, 42, 123, 2, 1024 };
		private readonly int[] _testSamplesAmount = new int[] {3, 1, 10, 21, 100 };
		private readonly float[] _testMinDistance = new float[] { 2, 1.2f, 11f, 0, 0.7f };
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

		private PoissonDiskSampling3D _sampling;
        private List<Vector3> _samples;
		

		[Test]
        public void PoissonDiskSampling3D_Sample_ResultHasExpectedAmountOfSamples()
        {
			for (int i = 0; i < _testSamplesAmount.Length; i++)
			{
				GivenANewPoissonDiskSampling(i);
				WhenSampleIsCalled(CreateParameters(i));
				ThenResultAmountEqualsRequestedAmount(_testSamplesAmount[i]);
				TearDown();
			}
        }


		[Test]
        public void PoissonDiskSampling3D_Sample_SamplesHaveMinimalSpacing()
        {
			for (int i = 0; i < _testSamplesAmount.Length; i++)
			{
				GivenANewPoissonDiskSampling(i);
				WhenSampleIsCalled(CreateParameters(i));
				ThenSamplesHaveMinimalSpacing(_testMinDistance[i]);
				TearDown();
			}
        }


		[Test]
        public void PoissonDiskSampling3D_Sample_SamplesAreInBounds()
        {
			for (int i = 0; i < _testSamplesAmount.Length; i++)
			{
				GivenANewPoissonDiskSampling(i);
				WhenSampleIsCalled(CreateParameters(i));
				ThenSamplesAreWithinBounds(_testBounds[i]);
				TearDown();
			}
        }

		[Test]
		public void PoissonDiskSampling3D_NegativeAmountThrowsException()
		{
			for (int i = 0; i < _invalidSamplesAmount.Length; i++)
			{
				GivenANewPoissonDiskSampling(i);
				TestDelegate test = () => WhenSampleIsCalled(CreateParametersWithInvalidAmount(i));
				ThenThrowsInvalidAmountException(test);
				TearDown();
			}
		}

		[Test]
		public void PoissonDiskSampling3D_NegativeMinDistanceThrowsException()
		{
			for (int i = 0; i < _invalidMinDistance.Length; i++)
			{
				GivenANewPoissonDiskSampling(i);
				TestDelegate test = () => WhenSampleIsCalled(CreateParametersWithInvalidMinDistance(i));
				ThenThrowsInvalidMinDistanceException(test);
				TearDown();
			}
		}

		[Test]
		public void PoissonDiskSampling3D_InvalidBoundsThrowsException()
		{
			for (int i = 0; i < _invalidBounds.Length; i++)
			{
				GivenANewPoissonDiskSampling(i);
				TestDelegate test = () => WhenSampleIsCalled(CreateParametersWithInvalidBounds(i));
				ThenThrowsInvalidBoundsException(test);
				TearDown();
			}
		}

		[Test]
		public void PoissonDiskSampling3D_InvalidStartPositionThrowsException()
		{
			for (int i = 0; i < _invalidStartPositions.Length; i++)
			{
				GivenANewPoissonDiskSampling(i);
				TestDelegate test = () => WhenSampleIsCalled(CreateParametersWithInvalidStartPosition(i));
				ThenThrowsInvalidStartPositionException(test);
				TearDown();
			}
		}

		private void GivenANewPoissonDiskSampling(int index)
        {
            _sampling = new PoissonDiskSampling3D(CreateRandom(index), _retries);
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

		private void ThenThrowsInvalidAmountException(TestDelegate test)
		{
			Assert.Throws<PoissonDiskSampling3D.InvalidAmountException>(test);
		}

		private void ThenThrowsInvalidMinDistanceException(TestDelegate test)
		{
			Assert.Throws<PoissonDiskSampling3D.InvalidMinDistanceException>(test);
		}

		private void ThenThrowsInvalidBoundsException(TestDelegate test)
		{
			Assert.Throws<PoissonDiskSampling3D.InvalidBoundsException>(test);
		}

		private void ThenThrowsInvalidStartPositionException(TestDelegate test)
		{
			Assert.Throws<PoissonDiskSampling3D.InvalidStartPositionException>(test);
		}

		[TearDown]
		public void TearDown()
		{
			CleanSamples();
		}

		private void CleanSamples()
		{
			_samples = null;
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
	}
}