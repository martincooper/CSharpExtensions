using System;
using NUnit.Framework;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Prelude;

namespace CSharpExtensions.Tests;

public class LanguageExtExtensionsTests
{
    [Test]
    public void TestTryFailWithWrappedException()
    {
        var myTry = Try<Unit>(new Exception("Original Error"));
        var result = myTry.MapFail(new ArgumentException("Wrapped Error"));
        
        Assert.IsTrue(result.IsFail());
        Assert.AreEqual(null, result.GetException().ValueUnsafe().InnerException);
        Assert.AreEqual("Wrapped Error", result.GetException().ValueUnsafe().Message);
    }
    
    [Test]
    public void TestTryFailFuncWithWrappedException()
    {
        var myTry = Try<Unit>(new Exception("Original Error"));
        var result = myTry.MapFail(ex => new ArgumentException("Wrapped Error", ex));
        
        Assert.IsTrue(result.IsFail());
        Assert.AreEqual("Original Error", result.GetException().ValueUnsafe().InnerException?.Message);
        Assert.AreEqual("Wrapped Error", result.GetException().ValueUnsafe().Message);
    }
}