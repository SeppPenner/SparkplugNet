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
        var filteredMetrics = node.KnownMetricsStorage.FilterMetrics(filterMetrics, SparkplugMessageType.NodeBirth);
        Assert.IsTrue(filteredMetrics.Count() == 1);
    }

    /// <summary>
    /// Tests the version B metric storage with an invalid metric (Alias and name not set).
    /// </summary>
    [TestMethod]
    public void TestFilterVersionBInvalidAliasAndNameNotSet()
    {
        var metric = new VersionBData.Metric("TestMetric1", VersionBData.DataType.Double, 123.45)
        {
            Alias = 1
        };
        IEnumerable<VersionBData.Metric> metrics =
        [
            metric
        ];

        IEnumerable<VersionBData.Metric> filterMetrics =
        [
            new(string.Empty, VersionBData.DataType.Double, 128.56)
        ];

        var node = new VersionBMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
        Assert.IsNotNull(node);
        Assert.IsTrue(node.KnownMetrics.Count() == 1);

        // Test filter function.
        var filteredMetrics = node.KnownMetricsStorage.FilterMetrics(filterMetrics, SparkplugMessageType.NodeData);
        Assert.IsTrue(!filteredMetrics.Any());
    }

    /// <summary>
    /// Tests the version B metric storage with an invalid metric, alias not set (Name is unknown).
    /// </summary>
    [TestMethod]
    public void TestFilterVersionBInvalidAliasNotSetAndUnknownName()
    {
        var metric = new VersionBData.Metric("TestMetric1", VersionBData.DataType.Double, 123.45)
        {
            Alias = 1
        };
        IEnumerable<VersionBData.Metric> metrics =
        [
            metric
        ];

        IEnumerable<VersionBData.Metric> filterMetrics =
        [
            new("TestMetric2", VersionBData.DataType.Double, 128.56)
        ];

        var node = new VersionBMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
        Assert.IsNotNull(node);
        Assert.IsTrue(node.KnownMetrics.Count() == 1);

        // Test filter function.
        var filteredMetrics = node.KnownMetricsStorage.FilterMetrics(filterMetrics, SparkplugMessageType.NodeData);
        Assert.IsTrue(!filteredMetrics.Any());
    }

    /// <summary>
    /// Tests the version B metric storage with an invalid metric, alias not set (Data type is unknown).
    /// </summary>
    [TestMethod]
    public void TestFilterVersionBInvalidAliasNotSetAndUnknownDataType()
    {
        var metric = new VersionBData.Metric("TestMetric1", VersionBData.DataType.Double, 123.45)
        {
            Alias = 1
        };
        IEnumerable<VersionBData.Metric> metrics =
        [
            metric
        ];

        IEnumerable<VersionBData.Metric> filterMetrics =
        [
            new("TestMetric1", VersionBData.DataType.Float, 128.56f)
        ];

        var node = new VersionBMain.SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
        Assert.IsNotNull(node);
        Assert.IsTrue(node.KnownMetrics.Count() == 1);

        // Test filter function.
        var filteredMetrics = node.KnownMetricsStorage.FilterMetrics(filterMetrics, SparkplugMessageType.NodeData);
        Assert.IsTrue(!filteredMetrics.Any());
    }

    /// <summary>
    /// Tests the version B metric storage with an invalid metric, name not set (NBIRTH with an alias only).
    /// </summary>
    [TestMethod]
    public void TestFilterVersionBInvalidNameNotSetNbirthWithAliasOnly()
    {
        var metric = new VersionBData.Metric("TestMetric1", VersionBData.DataType.Double, 123.45)
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
        var filteredMetrics = node.KnownMetricsStorage.FilterMetrics(filterMetrics, SparkplugMessageType.NodeBirth);
        Assert.IsTrue(!filteredMetrics.Any());
    }

    /// <summary>
    /// Tests the version B metric storage with an invalid metric, name not set (The alias is unknown).
    /// </summary>
    [TestMethod]
    public void TestFilterVersionBInvalidNameNotSetUnknownAlias()
    {
        var metric = new VersionBData.Metric("TestMetric1", VersionBData.DataType.Double, 123.45)
        {
            Alias = 1
        };
        IEnumerable<VersionBData.Metric> metrics =
        [
            metric
        ];

        var filterMetric = new VersionBData.Metric(string.Empty, VersionBData.DataType.Double, 128.56)
        {
            Alias = 2
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
        Assert.IsTrue(!filteredMetrics.Any());
    }

    /// <summary>
    /// Tests the version B metric storage with an invalid metric, name not set (Data type is unknown).
    /// </summary>
    [TestMethod]
    public void TestFilterVersionBInvalidNameNotSetUnknownDataType()
    {
        var metric = new VersionBData.Metric("TestMetric1", VersionBData.DataType.Double, 123.45)
        {
            Alias = 1
        };
        IEnumerable<VersionBData.Metric> metrics =
        [
            metric
        ];

        var filterMetric = new VersionBData.Metric(string.Empty, VersionBData.DataType.Float, 128.56f)
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
        Assert.IsTrue(!filteredMetrics.Any());
    }

    /// <summary>
    /// Tests the version B metric storage with an invalid metric, name and alias set (NDATA message with alias and name).
    /// </summary>
    [TestMethod]
    public void TestFilterVersionBInvalidNameAndAliasSetNDataWithAliasAndName()
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
        Assert.IsTrue(!filteredMetrics.Any());
    }

    /// <summary>
    /// Tests the version B metric storage with an invalid metric, name and alias set (Name is unknown).
    /// </summary>
    [TestMethod]
    public void TestFilterVersionBInvalidNameAndAliasSetUnknownName()
    {
        var metric = new VersionBData.Metric("TestMetric1", VersionBData.DataType.Double, 123.45)
        {
            Alias = 1
        };
        IEnumerable<VersionBData.Metric> metrics =
        [
            metric
        ];

        var filterMetric = new VersionBData.Metric("TestMetric2", VersionBData.DataType.Double, 128.56)
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
        var filteredMetrics = node.KnownMetricsStorage.FilterMetrics(filterMetrics, SparkplugMessageType.NodeBirth);
        Assert.IsTrue(!filteredMetrics.Any());
    }

    /// <summary>
    /// Tests the version B metric storage with an invalid metric, name and alias set (Data type is unknown).
    /// </summary>
    [TestMethod]
    public void TestFilterVersionBInvalidNameAndAliasSetUnknownDataType()
    {
        var metric = new VersionBData.Metric("TestMetric1", VersionBData.DataType.Double, 123.45)
        {
            Alias = 1
        };
        IEnumerable<VersionBData.Metric> metrics =
        [
            metric
        ];

        var filterMetric = new VersionBData.Metric("TestMetric1", VersionBData.DataType.Float, 128.56f)
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
        var filteredMetrics = node.KnownMetricsStorage.FilterMetrics(filterMetrics, SparkplugMessageType.NodeBirth);
        Assert.IsTrue(!filteredMetrics.Any());
    }
}
