using NUnit.Framework;
using System;
using UnityEngine;

namespace SBaier.Sampling.Tests
{
    public abstract class BoundsTest
    {
		private Bounds _bounds;
        private bool _isWithinBounds;
        private Vector3 _testPoint;


        [Test]
        public void Contains_ReturnsTrueOnPointInside()
        {
            for (int i = 0; i < TestValuesAmount; i++)
            {
                GivenNewBounds(i);
                _testPoint = GetPointInside(i);
                WhenContainsIsCalled();
                ThenWhithinBoundsIs(true);
            }
        }

		[Test]
        public void Contains_ReturnsFalseOnPointOutside()
        {
            for (int i = 0; i < TestValuesAmount; i++)
            {
                GivenNewBounds(i);
                _testPoint = GetPointOutside(i);
                WhenContainsIsCalled();
                ThenWhithinBoundsIs(false);
            }
        }

        [Test]
        public void ToString_ContainsExpectedInformation()
        {
            for (int i = 0; i < TestValuesAmount; i++)
            {
                GivenNewBounds(i);
                string toString = WhenToStringIsCalled();
                ThenToStringContainsExpectedInformation(toString, i);
            }
        }

        [Test]
        public void Constructor_InvalidBoundsThrowException()
		{
            for (int i = 0; i < TestValuesAmount; i++)
            {
                TestDelegate test = () => GivenNewInvalidBounds(i);
                ThenThrowsInvalidBoundsException(test);
            }
        }

		private void GivenNewInvalidBounds(int index)
		{
            _bounds = CreateInvalidBounds(index);
		}

		private void GivenNewBounds(int index)
		{
            _bounds = CreateBounds(index);
		}


        private void WhenContainsIsCalled()
        {
            _isWithinBounds = _bounds.Contains(_testPoint);
        }

        private string WhenToStringIsCalled()
        {
            return _bounds.ToString();
        }

        private void ThenWhithinBoundsIs(bool expected)
        {
            Assert.AreEqual(expected, _isWithinBounds, $"'{nameof(Bounds.Contains)}' of '{_bounds.ToString()}' returns '{_isWithinBounds}' for '{_testPoint}' when '{expected}' was expected.");
        }

        private void ThenThrowsInvalidBoundsException(TestDelegate test)
        {
            Assert.Throws<Bounds.InvalidBoundsException>(test);
        }


        protected abstract Bounds CreateBounds(int index);
        protected abstract Bounds CreateInvalidBounds(int index);
        protected abstract Vector3 GetPointInside(int index);
        protected abstract Vector3 GetPointOutside(int index);
        protected abstract int TestValuesAmount { get; }
        protected abstract void ThenToStringContainsExpectedInformation(string toString, int index);
    }
}
