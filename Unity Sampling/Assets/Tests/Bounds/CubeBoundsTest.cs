using NUnit.Framework;
using UnityEngine;

namespace SBaier.Sampling.Tests
{
	public class CubeBoundsTest : BoundsTest
	{
		protected Vector3[] _pointsInside => new Vector3[]
		{
			Vector3.zero,
			new Vector3(0.05f, 100, 5),
			new Vector3(0, 20, 200),
			Vector3.zero,
			new Vector3(150, 150, 150)
		};

		protected Vector3[] _pointsOutside => new Vector3[]
		{
			new Vector3(15, 20, 5),
			new Vector3(0.5f, 210, -1f),
			new Vector3(-1, -1, -1),
			new Vector3(0, 2, 0),
			new Vector3(150.001f, -0.001f, -1f)
		};

		private readonly Vector3[] _testSizes = new Vector3[]
		{
			new Vector3(10, 20, 5),
			new Vector3(0.1f, 200, 10),
			new Vector3(3, 20, 200),
			Vector3.zero,
			new Vector3(150, 150, 150)
		};

		private readonly Vector3[] _testInvalidSizes = new Vector3[]
		{
			new Vector3(-10, 20, 5),
			new Vector3(0.1f, -200, 10),
			new Vector3(3, -20, -200),
			new Vector3(-0.03f, -9, -2),
			new Vector3(-150, -150, 150)
		};

		private readonly Vector3[] _testStartPositions = new Vector3[]
		{
			Vector3.zero,
			new Vector3(-10, -10, 10),
			new Vector3(-3, 3, 100),
			new Vector3(2, 2.5f, -1f),
			new Vector3(-0.1f, 20, -0.1f)
		};


		private readonly Vector3[] _invalidBounds = new Vector3[]
		{
			new Vector3(-10, 20, 5),
			new Vector3(-0.1f, -200, -10),
			new Vector3(3, -20, 200),
			new Vector3(-0.01f, 0.1f, -0.7f),
			new Vector3(150, -150, -50)
		};

		protected override int TestValuesAmount => _testSizes.Length;

		protected override Bounds3D CreateBounds(int index)
		{
			return new CubeBounds(_testStartPositions[index], _testSizes[index]);
		}

		protected override Vector3 GetPointInside(int index) => _pointsInside[index] + _testStartPositions[index];

		protected override Vector3 GetPointOutside(int index) => _pointsOutside[index] + _testStartPositions[index];

		protected override void ThenToStringContainsExpectedInformation(string toString, int index)
		{
			Assert.IsTrue(toString.Contains(_testSizes[index].ToString()));
			Assert.IsTrue(toString.Contains(_testStartPositions[index].ToString()));
			Assert.IsTrue(toString.Contains(nameof(CubeBounds)));
		}

		protected override Bounds3D CreateInvalidBounds(int index)
		{
			return new CubeBounds(_testStartPositions[index], _testInvalidSizes[index]);
		}
	}
}
