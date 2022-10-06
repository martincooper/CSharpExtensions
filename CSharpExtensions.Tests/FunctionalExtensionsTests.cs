using System.Linq;
using NUnit.Framework;
using static CSharpExtensions.FunctionalExtensions;

namespace CSharpExtensions.Tests;

public class FunctionalExtensionsTests
{
    [Test]
    public void TestSimpleDo()
    {
        int x = 0;
        var result = Do(() => x = 100);
        
        Assert.IsNotNull(result);
        Assert.AreEqual(100, x);
    }
    
    [Test]
    public void TestSimpleDoInComputationExpression()
    {
        var total = 0;

        var result =
            from indexes in Enumerable.Range(1, 10)
            let _ = Do(() => total = 100)
            select indexes;    
        
        Assert.IsTrue(result.Length() == 10);
        Assert.AreEqual(100, total);
    }
    
    [Test]
    public void TestDoActionInSelect()
    {
        var total = 0;

        var result = Enumerable
            .Range(1, 10)
            .Do(() => total = 100);
        
        Assert.IsTrue(result.Length() == 10);
        Assert.AreEqual(100, total);
    }
    
    [Test]
    public void TestDoFuncInSelect()
    {
        var total = 0;

        var result = Enumerable
            .Range(1, 10)
            .Do(indexes => total = indexes.Sum());
        
        Assert.IsTrue(result.Length() == 10);
        Assert.AreEqual(55, total);
    }
    
    [Test]
    public void TestDoSelectWithValue()
    {
        var logMessage = string.Empty;

        var result = Do(() => logMessage = "Starting")
            .Select(Enumerable.Range(1, 10))
            .Where(r => r > 5)
            .Sum();
        
        Assert.AreEqual("Starting", logMessage);
        Assert.AreEqual(40, result);
    }
    
    [Test]
    public void TestDoSelectWithFunc()
    {
        var logMessage = string.Empty;

        var result = Do(() => logMessage = "Starting")
            .Select(() => Enumerable.Range(1, 10))
            .Where(r => r > 5)
            .Sum();
        
        Assert.AreEqual("Starting", logMessage);
        Assert.AreEqual(40, result);
    }
    
    [Test]
    public void TestDoOnObject()
    {
        var startValue = Enumerable.Range(1, 10);
        var logMessage = string.Empty;

        var result = startValue
            .Do(i => logMessage = $"Starting {i.Length()} items.")
            .Where(r => r > 5)
            .Sum();
        
        Assert.AreEqual("Starting 10 items.", logMessage);
        Assert.AreEqual(40, result);
    }
    
    [Test]
    public void TestMapTo()
    {
        var startValue = 10;

        var result = startValue
            .MapTo(num => num * 20);
        
        Assert.AreEqual(200, result);
    }
}