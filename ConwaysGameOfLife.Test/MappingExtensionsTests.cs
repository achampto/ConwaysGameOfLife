using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConwaysGameOfLife.Test
{
    [TestClass, TestCategory("BVT")]
    public class MappingExtensionsTests
    {
        [TestMethod]
        public void MappingRoundTripWorksAsExpected()
        {
            var max = long.MaxValue;
            var min = long.MinValue;
            var zero = 0L;

            var uMax = max.Map();
            var uMin = min.Map();
            var uZero = zero.Map();

            var sMax = uMax.Map();
            var sMin = uMin.Map();
            var sZero = uZero.Map();

            sMax.Should().Be(max);
            sMin.Should().Be(min);
            sZero.Should().Be(zero);

        }

    }
}
