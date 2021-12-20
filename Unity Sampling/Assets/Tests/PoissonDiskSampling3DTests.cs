using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace SBaier.Sampling.Tests
{
    public class PoissonDiskSampling3DTests
    {
		private const int _tries = 50;

		private const float _epsilon = 0.01f;
		private const int _highAmount = 500;
		private const float _outOfBoundsChance = 0.25f;
		private const int _fullOutOfBoundsChance = 1;
		private readonly int[] _testSeeds = new int[] {0, 42, 123, 2, 1024 };
		private readonly int[] _invalidTriesAmount = new int[] {-1, -10, -2, -200, 0 };
		private readonly int[] _testSamplesAmount = new int[] {3, 21, 10, 1, 100 };
		private readonly float[] _testMinDistance = new float[] { 2, 1.2f, 11f, 2, 25f };

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
		private Mock<Bounds> _boundsMock;
		private Dictionary<Vector3, bool> _sampleToIsWithinBounds = new Dictionary<Vector3, bool>();

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
				ThenSamplesAreWithinBounds();
				TearDown();
			}
        }

		[Test]
		public void Sample_UnreachableAmountCausesException()
		{
			for (int i = 0; i < _testSamplesAmount.Length; i++)
			{
				GivenASetupWithAlwaysOutOfBounds(i);
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


		[Test]
		public void Constructor_InvalidRetriesThrowException()
		{
			for (int i = 0; i < _invalidTriesAmount.Length; i++)
			{
				TestDelegate test = () => GivenASetupWithInvalidTriesAmount(i);
				ThenThrowsInvalidTriesAmountException(test);
				TearDown();
			}
		}

		private void GivenADefaultSetup(int index)
		{
			Setup();
			GivenADefaultValidatorMock();
			GivenMockedBounds(_outOfBoundsChance);
			GivenANewPoissonDiskSampling(index);
		}

		private void GivenASetupWithInvalidTriesAmount(int index)
		{
			Setup();
			GivenADefaultValidatorMock();
			GivenMockedBounds(_outOfBoundsChance);
			GivenANewPoissonDiskSamplingWithInvalidTries(index);
		}

		private void GivenASetupWithAlwaysOutOfBounds(int index)
		{
			Setup();
			GivenADefaultValidatorMock();
			GivenMockedBounds(_fullOutOfBoundsChance);
			GivenANewPoissonDiskSampling(index);
		}

		private void GivenADefaultValidatorMock()
		{
			_validatorMock = new Mock<Validator<PoissonDiskSampling3D.Parameters>>();
			_validatorMock.Setup(p => p.Validate(It.IsAny<PoissonDiskSampling3D.Parameters>())).Callback(() => _validateCalled = true);
		}

		private void GivenANewPoissonDiskSampling(int index)
        {
            _sampling = new PoissonDiskSampling3D(CreateRandom(index), _tries, _validatorMock.Object);
		}

		private void GivenANewPoissonDiskSamplingWithInvalidTries(int index)
        {
            _sampling = new PoissonDiskSampling3D(CreateRandom(index), _invalidTriesAmount[index], _validatorMock.Object);
		}


		private void GivenMockedBounds(float falseChance)
		{
			_boundsMock = new Mock<Bounds>(); 
			_boundsMock.Setup(b => b.Contains(It.IsAny<Vector3>())).Returns<Vector3>(v => CreateContainsReturnValue(v, falseChance));
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


		private void ThenSamplesAreWithinBounds()
		{
			foreach (Vector3 sample in _samples)
				Assert.True(_sampleToIsWithinBounds.ContainsKey(sample) ? _sampleToIsWithinBounds[sample] : true);
		}

		

		private void ThenThrowsSamplingException(TestDelegate test)
		{
			Assert.Throws<PoissonDiskSampling3D.SamplingException>(test);
		}

		private void ThenThrowsInvalidTriesAmountException(TestDelegate test)
		{
			Assert.Throws<PoissonDiskSampling3D.InvalidTriesAmountException>(test);
		}

		private void ThenValidateOfTheValidatorIsCalled()
		{
			Assert.IsTrue(_validateCalled);
		}

		[SetUp]
		public void Setup()
		{
			UnityEngine.Random.InitState(0);
		}

		[TearDown]
		public void TearDown()
		{
			_samples = null;
			_validateCalled = false;
			_sampleToIsWithinBounds.Clear();
		}

		private System.Random CreateRandom(int index)
		{
			return new System.Random(_testSeeds[index]);
		}


		private PoissonDiskSampling3D.Parameters CreateParameters(int index)
		{
			int amount = _testSamplesAmount[index];
			float minDistance = _testMinDistance[index];
			Vector3 startPos = _testStartPositions[index];
			return new PoissonDiskSampling3D.Parameters(amount, minDistance, _boundsMock.Object, startPos);
		}

		private PoissonDiskSampling3D.Parameters CreateParametersWithHighAmount(int index)
		{
			int amount = _highAmount;
			float minDistance = _testMinDistance[index];
			Vector3 startPos = _testStartPositions[index];
			return new PoissonDiskSampling3D.Parameters(amount, minDistance, _boundsMock.Object, startPos);
		}


		private bool CreateContainsReturnValue(Vector3 point, float falseChance)
		{
			bool value = UnityEngine.Random.value > falseChance;
			_sampleToIsWithinBounds[point] = value;
			return value;
		}
	}
}