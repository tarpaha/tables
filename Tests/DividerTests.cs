using Solver;

namespace Tests;

public class DividerTests
{
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(5)]
    public void Divider_ThrowsException_OnInvalidData(int personsCount)
    {
        Assert.That(() => Divider.DivideByTables(personsCount, true), Throws.ArgumentException);
        Assert.That(() => Divider.DivideByTables(personsCount, false), Throws.ArgumentException);
    }

    [TestCase(3, new [] { 3 })]
    [TestCase(4, new [] { 4 })]
    [TestCase(6, new [] { 3, 3 })]
    [TestCase(7, new [] { 4, 3 })]
    [TestCase(8, new [] { 4, 4 })]
    [TestCase(9, new [] { 3, 3, 3 })]
    [TestCase(10, new [] { 4, 3, 3 })]
    [TestCase(11, new [] { 4, 4, 3 })]
    [TestCase(12, new [] { 4, 4, 4 })]
    public void Divider_Works_CorrectlyWithPrefer4(int personsCount, int[] tables)
    {
        Assert.That(Divider.DivideByTables(personsCount, true), Is.EqualTo(tables));
    }
    
    [TestCase(3, new [] { 3 })]
    [TestCase(4, new [] { 4 })]
    [TestCase(6, new [] { 3, 3 })]
    [TestCase(7, new [] { 3, 4 })]
    [TestCase(8, new [] { 4, 4 })]
    [TestCase(9, new [] { 3, 3, 3 })]
    [TestCase(10, new [] { 3, 3, 4 })]
    [TestCase(11, new [] { 3, 4, 4 })]
    [TestCase(12, new [] { 3, 3, 3, 3 })]
    public void Divider_Works_CorrectlyWithPrefer3(int personsCount, int[] tables)
    {
        Assert.That(Divider.DivideByTables(personsCount, false), Is.EqualTo(tables));
    }
}