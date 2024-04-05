using System.Linq;
using CSharpExtensions.Dynamic;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CSharpExtensions.Tests;

public class DynamicModelTests
{
    [Test]
    public void TestDynamicModelSet()
    {
        var model = new DynamicModel();

        var result = model.TestProperty;

        dynamic dModel = new DynamicModel();
        
        //dynamic dModel = new DynamicModel(new string[] { "MyValOne" });
         
        dModel.MyValOne = "My Val 1";
        dModel.MyValTwo = 2;
        dModel.AnotherModel.MyValThree = "Child";

        var t = dModel as DynamicModel;

        var fields = t.GetDynamicMemberNames().ToArray();

        var json = JsonConvert.SerializeObject(dModel, Formatting.Indented);
    }
}