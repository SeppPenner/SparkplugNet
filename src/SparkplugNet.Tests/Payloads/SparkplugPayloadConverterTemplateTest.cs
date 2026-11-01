// -------------------------------------------------------------------------------------------------
// <copyright file="SparkplugPayloadConverterTemplateTest.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test templates in the version B payload converter.
// </summary>
// -------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Payloads;

/// <summary>
/// A class to test templates in the version B payload converter.
/// </summary>
[TestClass]
public sealed class SparkplugPayloadConverterTemplateTest
{
    /// <summary>
    /// Tests that a template definition without a reference has none on the wire.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBTemplateDefinitionOmitsTemplateRef()
    {
        var proto = VersionBMain.PayloadConverter.ConvertVersionBTemplate(new VersionBData.Template { IsDefinition = true });

        Assert.IsNull(proto?.TemplateRef);
    }

    /// <summary>
    /// Tests that a template instance keeps its template reference.
    /// </summary>
    [TestMethod]
    public void TestConvertVersionBTemplateInstanceKeepsTemplateRef()
    {
        var proto = VersionBMain.PayloadConverter.ConvertVersionBTemplate(new VersionBData.Template { IsDefinition = false, TemplateRef = "Type" });

        Assert.AreEqual("Type", proto?.TemplateRef);
    }
}
