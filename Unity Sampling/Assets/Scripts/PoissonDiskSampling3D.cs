using System;
using System.Collections.Generic;
using UnityEngine;

namespace SBaier.Sampling
{
	public class PoissonDiskSampling3D
	{
		private const float _epsilon = 0.0001f;
		private readonly System.Random _random;
		private List<int> _openList = new List<int>();
		private Vector3 _bounds;
		private float _minDistance;
		private int _amount;
		private List<Vector3> _samples;
		private int _currentIndex;
		private int _maxSamplingRetries;
		private Vector3 _startPosition;
		private Validator<Parameters> _parametersValidator;

		public PoissonDiskSampling3D(System.Random random,
			int maxSamplingRetries,
			Validator<Parameters> parametersValidator)
		{
			_random = random;
			_maxSamplingRetries = maxSamplingRetries;
			_parametersValidator = parametersValidator;
		}

		public List<Vector3> Sample(Parameters parameters)
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
			_samples = new List<Vector3>(_amount);
			_currentIndex = 0;
		}

		private void InitParameterFields(Parameters parameters)
		{
			_bounds = parameters.Bounds;
			_minDistance = parameters.MinDistance;
			_amount = parameters.Amount;
			_startPosition = parameters.StartPosition;

		}

		private Vector3 CreateSample()
		{
			if (_currentIndex == 0)
				return CreateStartSample();
			return CreateSampleFromOpenList();
		}

		private Vector3 CreateSampleFromOpenList()
		{
			if (_openList.Count == 0)
				throw new SamplingException();

			int randomOpenIndex = _random.Next(_openList.Count);
			Vector3 openSample = _samples[_openList[randomOpenIndex]];
			for (int i = 0; i < _maxSamplingRetries; i++)
			{
				Vector3 point = GetRandomPointAround(openSample);
				if (IsValid(point))
					return point;
			}

			_openList.RemoveAt(randomOpenIndex);
			return CreateSampleFromOpenList();
		}

		private Vector3 CreateStartSample()
		{
			return _startPosition;
		}

		private Vector3 GetRandomPointAround(Vector3 point)
		{
			float lat = (float)_random.NextDouble() * 2 * Mathf.PI - Mathf.PI;
			float lon = Mathf.Acos(2 * (float)_random.NextDouble() - 1);
			float x = Mathf.Cos(lat) * Mathf.Cos(lon);
			float y = Mathf.Cos(lat) * Mathf.Sin(lon);
			float z = Mathf.Sin(lat);
			return new Vector3(x, y, z) * _minDistance + point;
		}

		private bool IsValid(Vector3 point)
		{
			return IsWithinBounds(point) &&
				HasMinDistanceToOtherSamples(point);
		}

		private bool HasMinDistanceToOtherSamples(Vector3 point)
		{
			foreach (Vector3 sample in _samples)
			{
				if ((sample - point).magnitude + _epsilon < _minDistance)
					return false;
			}
			return true;
		}

		private bool IsWithinBounds(Vector3 point)
		{
			return point.x <= _bounds.x &&
				point.y <= _bounds.y &&
				point.z <= _bounds.z &&
				point.x >= 0 &&
				point.y >= 0 &&
				point.z >= 0;
		}

		public class Parameters
		{
			public Parameters(int amount, 
				float minDistance, 
				Vector3 bounds, 
				Vector3 startPosition)
			{
				Amount = amount;
				MinDistance = minDistance;
				Bounds = bounds;
				StartPosition = startPosition;
			}

			public int Amount { get; }
			public float MinDistance { get; }
			public Vector3 Bounds { get; }
			public Vector3 StartPosition { get; }
		}

		
		public class SamplingException : ArgumentException 
		{
			public SamplingException() : base("Sampling failed. Please increase the amount of retries, " +
				"increase the size of the bounds or decrease the samples amount") { }
		}
	}
}