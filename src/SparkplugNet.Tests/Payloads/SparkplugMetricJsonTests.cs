// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparkplugMetricJsonTests.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Tests the JSON extension for Sparkplug B metrics.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Tests.Payloads;

/// <summary>
/// Tests the optional JSON converters for Sparkplug B values.
/// </summary>
[TestClass]
public sealed class SparkplugMetricJsonTests
{
    /// <summary>
    /// Deserializes the nested metric and numeric data types from the issue report.
    /// </summary>
    /// <param name="useNewtonsoft">Whether to use Newtonsoft.Json.</param>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestReadTemplateFromIssue98(bool useNewtonsoft)
    {
        const string json = """
            [{
                "Name": "configuration/ABC",
                "Alias": null,
                "Timestamp": null,
                "IsHistorical": null,
                "IsTransient": null,
                "IsNull": false,
                "MetaData": null,
                "Properties": null,
                "Value": {
                    "Version": "",
                    "Metrics": [{
                        "Name": "product",
                        "Alias": null,
                        "Value": "ABC",
                        "DataType": 12
                    }],
                    "Parameters": [],
                    "TemplateRef": "",
                    "IsDefinition": null
                },
                "DataType": 19
            }]
            """;

        var metric = Deserialize(json, useNewtonsoft).Single();
        Assert.AreEqual("configuration/ABC", metric.Name);
        Assert.AreEqual(VersionBData.DataType.Template, metric.DataType);
        var template = metric.Value as VersionBData.Template;
        Assert.IsNotNull(template);
        var product = template.Metrics.Single();
        Assert.AreEqual("product", product.Name);
        Assert.AreEqual(VersionBData.DataType.String, product.DataType);
        Assert.AreEqual("ABC", product.Value);
    }

    /// <summary>
    /// Checks that either serializer can read and write complex, nullable, and precisely typed values.
    /// </summary>
    /// <param name="useNewtonsoft">Whether to use Newtonsoft.Json.</param>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void TestRoundTripNestedValues(bool useNewtonsoft)
    {
        const string isoString = "2025-12-31T23:58:00Z";
        var time = new DateTimeOffset(2025, 12, 31, 23, 58, 0, TimeSpan.Zero);
        var guid = Guid.Parse("1c2c2805-5d20-4ad5-b9fe-36f05fa23724");
        var set = new VersionBData.DataSet
        {
            NumberOfColumns = 2,
            Columns = ["label", "total"],
            Types = [(uint)VersionBData.DataType.String, (uint)VersionBData.DataType.UInt64],
            Rows = [new() { Elements = [new(VersionBData.DataType.String, isoString), new(VersionBData.DataType.UInt64, ulong.MaxValue)] }]
        };
        var propertySets = new VersionBData.PropertySetList
        {
            PropertySets = [new() { Keys = ["revision"], Values = [new(VersionBData.DataType.UInt16, (ushort)7)] }]
        };
        var metrics = new List<VersionBData.Metric>
        {
            new("configuration", VersionBData.DataType.Template, new VersionBData.Template
            {
                Metrics =
                [
                    new("when", VersionBData.DataType.DateTime, time),
                    new("times", VersionBData.DataType.DateTimeArray, new[] { time }),
                    new("code", VersionBData.DataType.Uuid, guid),
                    new("data", VersionBData.DataType.DataSet, set),
                    new("bytes", VersionBData.DataType.Bytes, new byte[] { 127, 255 }),
                    new("signed", VersionBData.DataType.Int16Array, new short[] { -5, 20 }),
                    new("null", VersionBData.DataType.Int32, null)
                ],
                Parameters = [new("threshold", VersionBData.DataType.Double, 1.25)]
            })
            {
                Alias = ulong.MaxValue,
                Timestamp = 42,
                IsHistorical = true,
                IsTransient = false,
                MetaData = new() { FileName = "config.json" },
                Properties = new()
                {
                    Keys = ["options", "identifier"],
                    Values = [new(VersionBData.DataType.PropertySetList, propertySets), new(VersionBData.DataType.Uuid, null)]
                }
            }
        };

        var json = Serialize(metrics, useNewtonsoft);
        var metric = Deserialize(json, useNewtonsoft).Single();
        Assert.AreEqual(ulong.MaxValue, metric.Alias);
        Assert.AreEqual(42UL, metric.Timestamp);
        Assert.AreEqual(true, metric.IsHistorical);
        Assert.AreEqual(false, metric.IsTransient);
        Assert.AreEqual("config.json", metric.MetaData?.FileName);
        Assert.IsNotNull(metric.Properties);
        var roundTrippedSets = metric.Properties.Values[0].Value as VersionBData.PropertySetList;
        Assert.IsNotNull(roundTrippedSets);
        Assert.AreEqual((ushort)7, roundTrippedSets.PropertySets.Single().Values.Single().Value);
        Assert.AreEqual(true, metric.Properties.Values[1].IsNull);
        Assert.AreEqual(VersionBData.DataType.Uuid, metric.Properties.Values[1].DataType);

        var template = metric.Value as VersionBData.Template;
        Assert.IsNotNull(template);
        var parameter = template.Parameters.Single();
        Assert.AreEqual("threshold", parameter.Name);
        Assert.AreEqual(1.25, parameter.Value);
        var nested = template.Metrics.ToDictionary(item => item.Name);
        var dataSet = nested["data"].Value as VersionBData.DataSet;
        Assert.IsNotNull(dataSet);
        var cells = dataSet.Rows.Single().Elements;
        Assert.AreEqual(isoString, cells[0].Value);
        Assert.AreEqual(ulong.MaxValue, cells[1].Value);
        Assert.AreEqual(time, nested["when"].Value);
        CollectionAssert.AreEqual(new[] { time }, nested["times"].Value as DateTimeOffset[]);
        Assert.AreEqual(guid, nested["code"].Value);
        CollectionAssert.AreEqual(new byte[] { 127, 255 }, nested["bytes"].Value as byte[]);
        CollectionAssert.AreEqual(new short[] { -5, 20 }, nested["signed"].Value as short[]);
        Assert.IsTrue(nested["null"].IsNull);
        Assert.IsNull(nested["null"].Value);
        Assert.AreEqual(VersionBData.DataType.Int32, nested["null"].DataType);
    }

    /// <summary>
    /// Deserializes a metric list with the selected serializer.
    /// </summary>
    /// <param name="json">The JSON list.</param>
    /// <param name="useNewtonsoft">Whether to use Newtonsoft.Json.</param>
    /// <returns>The deserialized metrics.</returns>
    private static List<VersionBData.Metric> Deserialize(string json, bool useNewtonsoft) => useNewtonsoft
        ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<VersionBData.Metric>>(json, CreateNewtonsoftSettings())!
        : JsonSerializer.Deserialize<List<VersionBData.Metric>>(json, CreateSystemTextOptions())!;

    /// <summary>
    /// Serializes a metric list with the selected serializer.
    /// </summary>
    /// <param name="metrics">The metrics.</param>
    /// <param name="useNewtonsoft">Whether to use Newtonsoft.Json.</param>
    /// <returns>The serialized JSON list.</returns>
    private static string Serialize(List<VersionBData.Metric> metrics, bool useNewtonsoft) => useNewtonsoft
        ? Newtonsoft.Json.JsonConvert.SerializeObject(metrics, CreateNewtonsoftSettings())
        : JsonSerializer.Serialize(metrics, CreateSystemTextOptions());

    /// <summary>
    /// Registers the System.Text.Json converter with web options.
    /// </summary>
    /// <returns>The JSON serializer options.</returns>
    private static JsonSerializerOptions CreateSystemTextOptions() =>
        new(JsonSerializerDefaults.Web) { Converters = { new SparkplugJsonConverter() } };

    /// <summary>
    /// Registers the Newtonsoft.Json converter with camel case property names.
    /// </summary>
    /// <returns>The JSON serializer settings.</returns>
    private static Newtonsoft.Json.JsonSerializerSettings CreateNewtonsoftSettings() => new()
    {
        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
        Converters = { new SparkplugNewtonsoftJsonConverter() }
    };
}
