// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownMetricStorageTests.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A test class for the known metric storage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.MetricStorage;

/// <summary>
/// A test class for the known metric storage.
/// </summary>
[TestClass]
public sealed class KnownMetricStorageTests
{
    /// <summary>
    /// Tests the version A metric storage with a valid metric.
    /// </summary>
    [TestMethod]
    public void TestAddVersionAValidMetric()
    {
        IEnumerable<VersionAData.KuraMetric> metrics =
        [
            new("TestMetric1", VersionAData.DataType.Double, 123.45)
        ];

        var node = new VersionAMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
        Assert.IsNotNull(node);
        Assert.IsTrue(node.KnownMetrics.Count() == 1);
    }

    /// <summary>
    /// Tests the version A metric storage with an invalid metric (No valid name).
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidMetricException))]
    public void TestAddVersionAInvalidMetricNoName()
    {
        IEnumerable<VersionAData.KuraMetric> metrics =
        [
            new(string.Empty, VersionAData.DataType.Double, 123.45)
        ];

        _ = new VersionAMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
    }

    /// <summary>
    /// Tests the version B metric storage with a valid metric (Name only).
    /// </summary>
    [TestMethod]
    public void TestAddVersionBValidMetricNameOnly()
    {
        IEnumerable<VersionBData.Metric> metrics =
        [
            new("TestMetric1", VersionBData.DataType.Double, 123.45)
        ];

        var node = new VersionBMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
        Assert.IsNotNull(node);
        Assert.IsTrue(node.KnownMetrics.Count() == 1);
    }

    /// <summary>
    /// Tests the version B metric storage with a valid metric (Alias only).
    /// </summary>
    [TestMethod]
    public void TestAddVersionBValidMetricAliasOnly()
    {
        var metric = new VersionBData.Metric(string.Empty, VersionBData.DataType.Double, 123.45)
        {
            Alias = 1
        };

        IEnumerable<VersionBData.Metric> metrics =
        [
            metric
        ];

        var node = new VersionBMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
        Assert.IsNotNull(node);
        Assert.IsTrue(node.KnownMetrics.Count() == 1);
    }

    /// <summary>
    /// Tests the version B metric storage with a valid metric (Name and alias).
    /// </summary>
    [TestMethod]
    public void TestAddVersionBValidMetricNameAndAlias()
    {
        var metric = new VersionBData.Metric("TestMetric1", VersionBData.DataType.Double, 123.45)
        {
            Alias = 1
        };

        IEnumerable<VersionBData.Metric> metrics =
        [
            metric
        ];

        var node = new VersionBMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
        Assert.IsNotNull(node);
        Assert.IsTrue(node.KnownMetrics.Count() == 1);
    }

    /// <summary>
    /// Tests the version B metric storage with an invalid metric.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidMetricException))]
    public void TestAddVersionBInvalidMetric()
    {
        IEnumerable<VersionBData.Metric> metrics =
        [
            new(string.Empty, VersionBData.DataType.Double, 123.45)
        ];

        _ = new VersionBMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
    }
}
