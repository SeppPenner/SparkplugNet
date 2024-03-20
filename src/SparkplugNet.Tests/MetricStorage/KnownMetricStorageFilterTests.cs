// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownMetricStorageFilterTests.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A test class for the known metric storage (The filter function).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.MetricStorage;

/// <summary>
/// A test class for the known metric storage (The filter function).
/// </summary>
[TestClass]
public sealed class KnownMetricStorageFilterTests
{
    /// <summary>
    /// Tests the version A metric storage with a valid metric.
    /// </summary>
    [TestMethod]
    public void TestFilterVersionAValidMetric()
    {
        IEnumerable<VersionAData.KuraMetric> metrics =
        [
            new("TestMetric1", VersionAData.DataType.Double, 123.45)
        ];

        IEnumerable<VersionAData.KuraMetric> filterMetrics =
        [
            new("TestMetric1", VersionAData.DataType.Double, 128.56)
        ];

        var node = new VersionAMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
        Assert.IsNotNull(node);
        Assert.IsTrue(node.KnownMetrics.Count() == 1);

        // Test filter function.
        var filteredMetrics = node.KnownMetricsStorage.FilterMetrics(filterMetrics, SparkplugMessageType.NodeData);
        Assert.IsTrue(filteredMetrics.Count() == 1);
    }

    /// <summary>
    /// Tests the version A metric storage with an invalid metric (No valid name).
    /// </summary>
    [TestMethod]
    public void TestFilterVersionAInvalidMetricNoName()
    {
        IEnumerable<VersionAData.KuraMetric> metrics =
        [
            new("TestMetric1", VersionAData.DataType.Double, 123.45)
        ];

        IEnumerable<VersionAData.KuraMetric> filterMetrics =
        [
            new(string.Empty, VersionAData.DataType.Double, 128.56)
        ];

        var node = new VersionAMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
        Assert.IsNotNull(node);
        Assert.IsTrue(node.KnownMetrics.Count() == 1);

        // Test filter function.
        var filteredMetrics = node.KnownMetricsStorage.FilterMetrics(filterMetrics, SparkplugMessageType.NodeData);
        Assert.IsTrue(!filteredMetrics.Any());
    }

    /// <summary>
    /// Tests the version A metric storage with an invalid metric (Unknown metric name).
    /// </summary>
    [TestMethod]
    public void TestFilterVersionAInvalidMetricUnknownMetricName()
    {
        IEnumerable<VersionAData.KuraMetric> metrics =
        [
            new("TestMetric1", VersionAData.DataType.Double, 123.45)
        ];

        IEnumerable<VersionAData.KuraMetric> filterMetrics =
        [
            new("TestMetric2", VersionAData.DataType.Double, 128.56)
        ];

        var node = new VersionAMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
        Assert.IsNotNull(node);
        Assert.IsTrue(node.KnownMetrics.Count() == 1);

        // Test filter function.
        var filteredMetrics = node.KnownMetricsStorage.FilterMetrics(filterMetrics, SparkplugMessageType.NodeData);
        Assert.IsTrue(!filteredMetrics.Any());
    }

    /// <summary>
    /// Tests the version A metric storage with an invalid metric (wrong device type).
    /// </summary>
    [TestMethod]
    public void TestFilterVersionAInvalidMetricWrongDeviceType()
    {
        IEnumerable<VersionAData.KuraMetric> metrics =
        [
            new("TestMetric1", VersionAData.DataType.Double, 123.45)
        ];

        IEnumerable<VersionAData.KuraMetric> filterMetrics =
        [
            new("TestMetric1", VersionAData.DataType.Float, 128.56)
        ];

        var node = new VersionAMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
        Assert.IsNotNull(node);
        Assert.IsTrue(node.KnownMetrics.Count() == 1);

        // Test filter function.
        var filteredMetrics = node.KnownMetricsStorage.FilterMetrics(filterMetrics, SparkplugMessageType.NodeData);
        Assert.IsTrue(!filteredMetrics.Any());
    }

    /// <summary>
    /// Tests the version B metric storage with a valid metric (Alias only).
    /// </summary>
    [TestMethod]
    public void TestFilterVersionBValidAliasOnly()
    {
        var metric = new VersionBData.Metric(string.Empty, VersionBData.DataType.Double, 123.45)
        {
            Alias = 1
        };
        IEnumerable<VersionBData.Metric> metrics =
        [
            metric
        ];

        var filterMetric = new VersionBData.Metric(string.Empty, VersionBData.DataType.Double, 128.56)
        {
            Alias = 1
        };
        IEnumerable<VersionBData.Metric> filterMetrics =
        [
            filterMetric
        ];

        var node = new VersionBMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
        Assert.IsNotNull(node);
        Assert.IsTrue(node.KnownMetrics.Count() == 1);

        // Test filter function.
        var filteredMetrics = node.KnownMetricsStorage.FilterMetrics(filterMetrics, SparkplugMessageType.NodeData);
        Assert.IsTrue(filteredMetrics.Count() == 1);
    }

    /// <summary>
    /// Tests the version B metric storage with a valid metric (Name only).
    /// </summary>
    [TestMethod]
    public void TestFilterVersionBValidNameOnly()
    {
        IEnumerable<VersionBData.Metric> metrics =
        [
            new("TestMetric1", VersionBData.DataType.Double, 123.45)
        ];

        IEnumerable<VersionBData.Metric> filterMetrics =
        [
            new("TestMetric1", VersionBData.DataType.Double, 128.56)
        ];

        var node = new VersionBMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
        Assert.IsNotNull(node);
        Assert.IsTrue(node.KnownMetrics.Count() == 1);

        // Test filter function.
        var filteredMetrics = node.KnownMetricsStorage.FilterMetrics(filterMetrics, SparkplugMessageType.NodeData);
        Assert.IsTrue(filteredMetrics.Count() == 1);
    }

    /// <summary>
    /// Tests the version B metric storage with a valid metric (Alias and name).
    /// </summary>
    [TestMethod]
    public void TestFilterVersionBValidAliasAndName()
    {
        var metric = new VersionBData.Metric("TestMetric1", VersionBData.DataType.Double, 123.45)
        {
            Alias = 1
        };
        IEnumerable<VersionBData.Metric> metrics =
        [
            metric
        ];

        var filterMetric = new VersionBData.Metric("TestMetric1", VersionBData.DataType.Double, 128.56)
        {
            Alias = 1
        };
        IEnumerable<VersionBData.Metric> filterMetrics =
        [
            filterMetric
        ];

        var node = new VersionBMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
        Assert.IsNotNull(node);
        Assert.IsTrue(node.KnownMetrics.Count() == 1);

        // Test filter function.
        var filteredMetrics = node.KnownMetricsStorage.FilterMetrics(filterMetrics, SparkplugMessageType.NodeData);
        Assert.IsTrue(filteredMetrics.Count() == 1);
    }






    ///// <summary>
    ///// Tests the version B metric storage with a valid metric (Name only).
    ///// </summary>
    //[TestMethod]
    //public void TestFilterVersionBValidMetricNameOnly()
    //{
    //    IEnumerable<VersionBData.Metric> metrics =
    //    [
    //        new("TestMetric1", VersionBData.DataType.Double, 123.45)
    //    ];

    //    var node = new VersionBMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
    //    Assert.IsNotNull(node);
    //    Assert.IsTrue(node.KnownMetrics.Count() == 1);
    //}

    ///// <summary>
    ///// Tests the version B metric storage with a valid metric (Alias only).
    ///// </summary>
    //[TestMethod]
    //public void TestFilterVersionBValidMetricAliasOnly()
    //{
    //    var metric = new VersionBData.Metric(string.Empty, VersionBData.DataType.Double, 123.45)
    //    {
    //        Alias = 1
    //    };

    //    IEnumerable<VersionBData.Metric> metrics =
    //    [
    //        metric
    //    ];

    //    var node = new VersionBMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
    //    Assert.IsNotNull(node);
    //    Assert.IsTrue(node.KnownMetrics.Count() == 1);
    //}

    ///// <summary>
    ///// Tests the version B metric storage with a valid metric (Name and alias).
    ///// </summary>
    //[TestMethod]
    //public void TestFilterVersionBValidMetricNameAndAlias()
    //{
    //    var metric = new VersionBData.Metric("TestMetric1", VersionBData.DataType.Double, 123.45)
    //    {
    //        Alias = 1
    //    };

    //    IEnumerable<VersionBData.Metric> metrics =
    //    [
    //        metric
    //    ];

    //    var node = new VersionBMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
    //    Assert.IsNotNull(node);
    //    Assert.IsTrue(node.KnownMetrics.Count() == 1);
    //}

    ///// <summary>
    ///// Tests the version B metric storage with an invalid metric.
    ///// </summary>
    //[TestMethod]
    //[ExpectedException(typeof(InvalidMetricException))]
    //public void TestFilterVersionBInvalidMetric()
    //{
    //    IEnumerable<VersionBData.Metric> metrics =
    //    [
    //        new(string.Empty, VersionBData.DataType.Double, 123.45)
    //    ];

    //    _ = new VersionBMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
    //}
}
