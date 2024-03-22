using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using LanguageExt;
using static LanguageExt.Prelude;
using static CSharpExtensions.XElementTryExtensions;

namespace CSharpExtensions.Tests;

public class XElementTryExtensionsTests
{
    private string _sampleXml = null!;
    
    [OneTimeSetUp]
    protected void GetSampleXml() =>
        _sampleXml = LoadSampleXml();

    [Test]
    public void TestTryFailWithWrappedException()
    {
        var result = TryParseXml(_sampleXml)
            .TryElementValue("head","depositor","email_address");
        
        Assert.IsTrue(result.IsSucc());
        Assert.AreEqual("pfeeney@crossref.org", result.GetValue());
    }

    private string LoadSampleXml()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("CSharpExtensions.Tests.Resources.TestSample.xml");
        
        var tr = new StreamReader(stream!);
        return tr.ReadToEnd();
    }
}