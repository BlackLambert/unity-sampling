using NUnit.Framework;
using UnityEngine;

namespace SBaier.Sampling.Tests
{
	public class SphereBoundsTest : BoundsTest
	{
		protected Vector3[] _pointsInside => new Vector3[]
		{
			Vector3.zero,
			new Vector3(0.05f, 0.25f, -0.05f),
			new Vector3(-0.5f, 1f, 1.3f),
			Vector3.zero,
			new Vector3(0, 0, 150)
		};

		protected Vector3[] _pointsOutside => new Vector3[]
		{
			new Vector3(15, 20, 5),
			new Vector3(0.5f, 0, -1.01f),
			new Vector3(-1, -1, -3.34f),
			new Vector3(0, 2, 0),
			new Vector3(150.001f, -0.001f, -1f)
		};

		private readonly float[] _testRadius = new float[]
		{
			10,
			1,
			3.33f,
			0,
			150
		};

		private readonly float[] _testInvalidRadius = new float[]
		{
			-10,
			-1,
			-3.33f,
			-0.01f,
			-150
		};

		private readonly Vector3[] _testStartPositions = new Vector3[]
		{
			Vector3.zero,
			new Vector3(-10, -10, 10),
			new Vector3(-3, 3, 100),
			new Vector3(2, 2.5f, -1f),
			new Vector3(-0.1f, 20, -0.1f)
		};

		protected override int TestValuesAmount => _testRadius.Length;

		protected override Bounds CreateBounds(int index)
		{
			return new SphereBounds(_testStartPositions[index], _testRadius[index]);
		}

		protected override Vector3 GetPointInside(int index) => _pointsInside[index] + _testStartPositions[index];

		protected override Vector3 GetPointOutside(int index) => _pointsOutside[index] + _testStartPositions[index];

		protected override void ThenToStringContainsExpectedInformation(string toString, int index)
		{
			Assert.IsTrue(toString.Contains(_testRadius[index].ToString()));
			Assert.IsTrue(toString.Contains(_testStartPositions[index].ToString()));
			Assert.IsTrue(toString.Contains(nameof(SphereBounds)));
		}

		protected override Bounds CreateInvalidBounds(int index)
		{
			return new SphereBounds(_testStartPositions[index], _testInvalidRadius[index]);
		}
	}
}
