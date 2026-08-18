// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTests.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A test class for the string extension methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Extensions;

/// <summary>
/// A test class for the string extension methods.
/// </summary>
[TestClass]
public sealed class StringExtensionsTests
{
    /// <summary>
    /// Resets the global strict identifier checking after each test.
    /// </summary>
    [TestCleanup]
    public void ResetStrictIdentifierChecking()
    {
        SparkplugGlobals.UseStrictIdentifierChecking = false;
    }

    /// <summary>
    /// Tests a valid identifier without strict identifier checking.
    /// </summary>
    [TestMethod]
    public void TestValidIdentifier()
    {
        Assert.IsTrue("group1".IsIdentifierValid());
    }

    /// <summary>
    /// Tests the identifiers that are invalid without strict identifier checking, the MQTT topic
    /// separator and the two MQTT wildcards.
    /// </summary>
    [TestMethod]
    public void TestInvalidIdentifier()
    {
        Assert.IsFalse(string.Empty.IsIdentifierValid());
        Assert.IsFalse("   ".IsIdentifierValid());
        Assert.IsFalse("group/1".IsIdentifierValid());
        Assert.IsFalse("group#1".IsIdentifierValid());
        Assert.IsFalse("group+1".IsIdentifierValid());
    }

    /// <summary>
    /// Tests an identifier that is only valid without strict identifier checking.
    /// </summary>
    [TestMethod]
    public void TestIdentifierWithDot()
    {
        Assert.IsTrue("group.1".IsIdentifierValid());

        SparkplugGlobals.UseStrictIdentifierChecking = true;
        Assert.IsFalse("group.1".IsIdentifierValid());
    }

    /// <summary>
    /// Tests a valid identifier with strict identifier checking.
    /// </summary>
    [TestMethod]
    public void TestValidIdentifierWithStrictChecking()
    {
        SparkplugGlobals.UseStrictIdentifierChecking = true;
        Assert.IsTrue("group1".IsIdentifierValid());
        Assert.IsTrue("Edge_Node-1".IsIdentifierValid());
    }

    /// <summary>
    /// Tests the identifiers that are invalid with strict identifier checking.
    /// </summary>
    [TestMethod]
    public void TestInvalidIdentifierWithStrictChecking()
    {
        SparkplugGlobals.UseStrictIdentifierChecking = true;
        Assert.IsFalse(string.Empty.IsIdentifierValid());
        Assert.IsFalse("   ".IsIdentifierValid());
        Assert.IsFalse("group,1".IsIdentifierValid());
        Assert.IsFalse("group@1".IsIdentifierValid());
        Assert.IsFalse("group$1".IsIdentifierValid());
        Assert.IsFalse("group(1)".IsIdentifierValid());
        Assert.IsFalse("group/1".IsIdentifierValid());
        Assert.IsFalse("group+1".IsIdentifierValid());
    }
}
