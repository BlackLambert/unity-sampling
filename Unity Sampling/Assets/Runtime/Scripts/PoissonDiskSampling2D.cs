using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Sampling
{
    public class PoissonDiskSampling2D
    {
		private const float _epsilon = 0.0001f;
		private readonly System.Random _random;
		private readonly List<int> _openList = new List<int>();
		private Bounds2D _bounds;
		private float _minDistance;
		private int _amount;
		private List<Vector2> _samples;
		private int _currentIndex;
		private int _maxSamplingTries;
		private Vector2 _startPosition;
		private Validator<Parameters> _parametersValidator;

		public PoissonDiskSampling2D(
			System.Random random,
			int maxSamplingTries,
			Validator<Parameters> parametersValidator)
		{
			ValidateTries(maxSamplingTries);

			_random = random;
			_maxSamplingTries = maxSamplingTries;
			_parametersValidator = parametersValidator;
		}

		private void ValidateTries(int maxSamplingTries)
		{
			if (maxSamplingTries <= 0)
				throw new InvalidTriesAmountException();
		}

		public List<Vector2> Sample(Parameters parameters)
		{
			Init(parameters);

			while (_currentIndex < _amount)
			{
				_samples.Add(CreateSample());
				_openList.Add(_currentIndex);
				_currentIndex++;
			}
			return _samples;
		}

		private void Init(Parameters parameters)
		{
			_parametersValidator.Validate(parameters);
			Reset();
			InitParameterFields(parameters);
		}

		private void Reset()
		{
			_openList.Clear();
			_samples = new List<Vector2>(_amount);
			_currentIndex = 0;
		}

		private void InitParameterFields(Parameters parameters)
		{
			_bounds = parameters.Bounds;
			_minDistance = parameters.MinDistance;
			_amount = parameters.Amount;
			_startPosition = parameters.StartPosition;

		}

		private Vector2 CreateSample()
		{
			if (_currentIndex == 0)
				return CreateStartSample();
			return CreateSampleFromOpenList();
		}

		private Vector2 CreateSampleFromOpenList()
		{
			if (_openList.Count == 0)
				throw new SamplingException();

			int randomOpenIndex = _random.Next(_openList.Count);
			Vector2 openSample = _samples[_openList[randomOpenIndex]];
			for (int i = 0; i < _maxSamplingTries; i++)
			{
				Vector2 point = GetRandomPointAround(openSample);
				if (IsValid(point))
					return point;
			}

			_openList.RemoveAt(randomOpenIndex);
			return CreateSampleFromOpenList();
		}

		private Vector2 CreateStartSample()
		{
			return _startPosition;
		}

		private Vector2 GetRandomPointAround(Vector2 point)
		{
			float theta = (float)_random.NextDouble() * 2 * Mathf.PI;
			float x = Mathf.Cos(theta);
			float y = Mathf.Sin(theta);
			return new Vector2(x, y) * _minDistance + point;
		}

		private bool IsValid(Vector2 point)
		{
			return _bounds.Contains(point) &&
				HasMinDistanceToOtherSamples(point);
		}

		private bool HasMinDistanceToOtherSamples(Vector2 point)
		{
			foreach (Vector2 sample in _samples)
			{
				if ((sample - point).magnitude + _epsilon < _minDistance)
					return false;
			}
			return true;
		}

		public class Parameters
		{
			public Parameters(int amount,
				float minDistance,
				Bounds2D bounds,
				Vector2 startPosition)
			{
				Amount = amount;
				MinDistance = minDistance;
				Bounds = bounds;
				StartPosition = startPosition;
			}

			public int Amount { get; }
			public float MinDistance { get; }
			public Bounds2D Bounds { get; }
			public Vector2 StartPosition { get; }
		}


		public class SamplingException : ArgumentException
		{
			public SamplingException() : base("Sampling failed. Please increase the amount of retries, " +
				"increase the size of the bounds or decrease the samples amount")
			{ }
		}


		public class InvalidTriesAmountException : ArgumentException
		{
			public InvalidTriesAmountException() : base($"Please provide a valid amount of sampling tries.") { }
		}
	}
}
