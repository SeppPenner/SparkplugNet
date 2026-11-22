// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Drives the official Sparkplug TCK against SparkplugNet over MQTT.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

const string resultTopic = "SPARKPLUG_TCK/RESULT";
const string controlTopic = "SPARKPLUG_TCK/TEST_CONTROL";
const string resultConfigTopic = "SPARKPLUG_TCK/RESULT_CONFIG";
const string promptTopic = "SPARKPLUG_TCK/CONSOLE_PROMPT";
const string replyTopic = "SPARKPLUG_TCK/CONSOLE_REPLY";
const string defaultLog = "SparkplugTCKResults.log";
const string edge = "edge";
const string device = "device";

if (args is ["host-worker", var workerHost, var workerClient, "311" or "500"])
{
    var worker = new SparkplugApplication([], SparkplugSpecificationVersion.Version30);
    await worker.Start(new SparkplugApplicationOptions("127.0.0.1", 1883, workerClient, scadaHostIdentifier: workerHost,
        isPrimaryApplication: true, mqttProtocolVersion: ParseProtocol(args[3])));
    await Console.In.ReadLineAsync();
    await worker.PublishOfflineState();
    Console.WriteLine("Offline STATE sent.");
    await Task.Delay(Timeout.InfiniteTimeSpan);
    return 0;
}

if (args is not (["host" or "edge", _, "311" or "500"] or ["host", "SessionTermination", "311" or "500", "without-disconnect"]))
{
    Console.Error.WriteLine("Usage: Runner <host|edge> <scenario> <311|500> [without-disconnect]");
    return 2;
}

var profile = args[0];
var scenario = args[1];
if (profile == "host" && scenario is not ("SessionEstablishment" or "SessionTermination" or "EdgeSessionTermination" or "MessageOrdering" or "SendCommand" or "MultipleBroker")
    || profile == "edge" && scenario is not ("SessionEstablishment" or "SessionTermination" or "SendData" or "SendComplexData" or "ReceiveCommand" or "PrimaryHost" or "MultipleBroker"))
{
    Console.Error.WriteLine("The requested scenario does not belong to the profile.");
    return 2;
}

var protocol = ParseProtocol(args[2]);
var withoutDisconnect = args.Length == 4;
var multipleBrokers = scenario == "MultipleBroker";
const string secondaryTckEndpoint = "tcp://broker-secondary:1883";
var secondaryEndpoint = new SparkplugBrokerEndpoint("127.0.0.1", 1884);
var run = $"tck-{Guid.NewGuid():N}";
var host = $"{run}-host";
var group = $"{run}-group";
var onlineTopic = $"spBv1.0/STATE/{host}";
var deviceDeathTopic = $"spBv1.0/{group}/DDEATH/{edge}/{device}";
var nodeDeathTopic = $"spBv1.0/{group}/NDEATH/{edge}";
var nodeDataTopic = $"spBv1.0/{group}/NDATA/{edge}";
var deviceDataTopic = $"spBv1.0/{group}/DDATA/{edge}/{device}";
var hostReceivesDeaths = scenario == "EdgeSessionTermination";
var hostReorders = scenario == "MessageOrdering";
var hostSendsCommands = scenario == "SendCommand";
var edgeSendsData = scenario is "SendData" or "SendComplexData";
var testCreatesHost = profile == "edge" && scenario is "SessionEstablishment" or "ReceiveCommand" or "PrimaryHost" or "MultipleBroker";
var summary = "Summary Test Results for " + (scenario switch
{
    "EdgeSessionTermination" => "Sparkplug Host Edge Node Session Termination Test",
    "MessageOrdering" => "Host Message Ordering",
    "SendComplexData" => "Edge Send Complex Data",
    "PrimaryHost" => "Edge Primary Host",
    "MultipleBroker" => $"{profile} Multiple Broker",
    _ => $"{profile} {scenario}"
});
const int reorderMilliseconds = 1000;
var log = $"SparkplugTCK-{profile}-{scenario}-{args[2]}{(withoutDisconnect ? "-without-disconnect" : "")}-{run}.log";
using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(multipleBrokers ? 55 : 30));
using var controller = new MqttClientFactory().CreateMqttClient();
var result = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
var hostOnline = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var prompts = Channel.CreateUnbounded<string>();
var deviceDeathTimestamp = new TaskCompletionSource<ulong>(TaskCreationOptions.RunContinuationsAsynchronously);
var nodeDeathObservedAt = new TaskCompletionSource<ulong>(TaskCreationOptions.RunContinuationsAsynchronously);
var observedDeviceData = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var observedNodeData = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

controller.ApplicationMessageReceivedAsync += message =>
{
    var payload = Encoding.UTF8.GetString(message.ApplicationMessage.Payload.ToArray());
    var topic = message.ApplicationMessage.Topic;
    if (topic == resultTopic && payload.Contains(summary, StringComparison.OrdinalIgnoreCase))
    {
        result.TrySetResult(payload);
    }
    else if (topic == onlineTopic && payload.Contains("\"online\":true", StringComparison.OrdinalIgnoreCase))
    {
        hostOnline.TrySetResult();
    }
    else if (topic == promptTopic)
    {
        prompts.Writer.TryWrite(payload);
    }
    else if (topic == deviceDeathTopic)
    {
        using var stream = new MemoryStream(message.ApplicationMessage.Payload.ToArray());
        deviceDeathTimestamp.TrySetResult(Serializer.Deserialize<SparkplugPayloadMetadata>(stream).Timestamp);
    }
    else if (topic == nodeDeathTopic)
    {
        nodeDeathObservedAt.TrySetResult(UtcNow());
    }
    else if (topic == deviceDataTopic)
    {
        observedDeviceData.TrySetResult();
        if (hostReorders)
        {
            using var stream = new MemoryStream(message.ApplicationMessage.Payload.ToArray());
            Console.WriteLine($"Received wire sequence {Serializer.Deserialize<SparkplugPayloadMetadata>(stream).Sequence}.");
        }
    }
    else if (topic == nodeDataTopic)
    {
        observedNodeData.TrySetResult();
    }

    return Task.CompletedTask;
};

await controller.ConnectAsync(new MqttClientOptionsBuilder()
    .WithTcpServer("127.0.0.1", 1883)
    .WithClientId($"{run}-controller")
    .Build(), deadline.Token);
await controller.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
    .WithTopicFilter(resultTopic)
    .WithTopicFilter(onlineTopic)
    .WithTopicFilter(promptTopic)
    .WithTopicFilter(deviceDeathTopic)
    .WithTopicFilter(nodeDeathTopic)
    .WithTopicFilter(deviceDataTopic)
    .WithTopicFilter(nodeDataTopic)
    .Build(), deadline.Token);

var logFile = $"TCK result file: {log}";
Console.WriteLine(logFile);
await Send(resultConfigTopic, $"NEW_RESULT-LOG {log}", deadline.Token);
SparkplugApplication? application = null;
SparkplugNode? node = null;
Process? hostWorker = null;
try
{
    var target = profile == "host" && scenario is "SessionEstablishment" or "SessionTermination"
        ? host : $"{host} {group} {edge} {device}";
    if (hostReorders)
    {
        target += $" {reorderMilliseconds}";
    }
    if (multipleBrokers)
    {
        target = profile == "host" ? $"{host} {secondaryTckEndpoint}" : $"{target} {secondaryTckEndpoint}";
    }

    if (!testCreatesHost)
    {
        application = new SparkplugApplication([], SparkplugSpecificationVersion.Version30,
            new WarningLogger<SparkplugBase<Metric>.KnownMetricStorage>());
        if (hostReorders)
        {
            application.DeviceDataReceived += args =>
            {
                Console.WriteLine($"Delivered sequence {args.SequenceNumber}.");
                return Task.CompletedTask;
            };
        }
        if (profile == "edge" || hostReceivesDeaths || hostReorders || hostSendsCommands)
        {
            await StartPrimaryHost(application);
            await hostOnline.Task.WaitAsync(deadline.Token);
        }
    }

    if (hostReceivesDeaths)
    {
        node = new SparkplugNode([new("Speed", DataType.Int32, 42)], SparkplugSpecificationVersion.Version30);
        await node.Start(new SparkplugNodeOptions("127.0.0.1", 1883, $"{run}-edge", scadaHostIdentifier: host,
            mqttProtocolVersion: SparkplugMqttProtocolVersion.V311, groupIdentifier: group, edgeNodeIdentifier: edge, cancellationToken: deadline.Token)
        {
            WaitForPrimaryHost = true
        });
        await node.PublishDeviceBirthMessage([new("Temperature", DataType.Int32, 21)], device);
        while (!application!.MetricStatesBySource.ContainsKey((group, edge, device)))
        {
            await Task.Delay(20, deadline.Token);
        }
    }

    await Send(controlTopic, $"NEW_TEST {profile} {scenario}Test {target}", deadline.Token);
    if (profile == "host")
    {
        if (hostReceivesDeaths)
        {
            await node!.PublishDeviceDeathMessage(device);
            await CheckHostDeathPrompts(application!);
        }
        else if (hostSendsCommands)
        {
            await SendHostCommands(application!);
        }
        else if (withoutDisconnect)
        {
            hostWorker = StartHostWorker(host, $"{run}-primary", args[2]);
            await hostOnline.Task.WaitAsync(deadline.Token);
            await hostWorker.StandardInput.WriteLineAsync();
            var confirmation = await hostWorker.StandardOutput.ReadLineAsync(deadline.Token);
            if (confirmation != "Offline STATE sent.")
            {
                throw new InvalidOperationException($"The host worker did not publish offline STATE. {confirmation}");
            }
            hostWorker.Kill();
            await hostWorker.WaitForExitAsync(deadline.Token);
        }
        else if (!hostReorders)
        {
            await StartPrimaryHost(application!);
            if (scenario == "SessionTermination")
            {
                await application!.Stop();
            }
        }
    }
    else
    {
        if (testCreatesHost && scenario != "PrimaryHost")
        {
            await hostOnline.Task.WaitAsync(deadline.Token);
        }

        var (metrics, deviceMetrics, nodeData, deviceData) = CreateEdgeMetrics();
        node = new SparkplugNode(metrics, SparkplugSpecificationVersion.Version30);
        if (scenario is "ReceiveCommand" or "PrimaryHost" or "MultipleBroker")
        {
            node.KnownDevices[device] = new SparkplugBase<Metric>.KnownMetricStorage(deviceMetrics);
        }
        await node.Start(new SparkplugNodeOptions("127.0.0.1", 1883, $"{run}-edge", scadaHostIdentifier: host,
            mqttProtocolVersion: protocol, groupIdentifier: group, edgeNodeIdentifier: edge, cancellationToken: deadline.Token,
            reconnectInterval: TimeSpan.FromMilliseconds(50))
        {
            WaitForPrimaryHost = true,
            AdditionalBrokers = multipleBrokers ? [secondaryEndpoint] : []
        });
        if (scenario is not ("ReceiveCommand" or "PrimaryHost" or "MultipleBroker"))
        {
            await node.PublishDeviceBirthMessage([.. deviceMetrics], device);
        }
        if (edgeSendsData)
        {
            await node.PublishMetrics(nodeData);
            await node.PublishDeviceData(deviceData, device);
            if (scenario == "SendComplexData")
            {
                await Task.WhenAll(observedNodeData.Task, observedDeviceData.Task).WaitAsync(deadline.Token);
                await Send(controlTopic, "END_TEST", deadline.Token);
            }
        }
        else if (scenario == "SessionTermination")
        {
            await node.PublishDeviceDeathMessage(device);
            await node.Stop();
        }
    }

    var report = await result.Task.WaitAsync(deadline.Token);
    Console.WriteLine(report);
    return report.Contains("OVERALL: PASS", StringComparison.Ordinal) ? 0 : 1;
}
catch (Exception exception) when (exception is MQTTnet.Exceptions.MqttCommunicationTimedOutException
    || (exception is OperationCanceledException && deadline.IsCancellationRequested))
{
    deadline.Cancel();
    Console.Error.WriteLine($"The TCK scenario timed out. {exception.Message}");
    return 3;
}
finally
{
    try
    {
        await Cleanup();
    }
    catch (Exception exception) when (deadline.IsCancellationRequested
        && exception is OperationCanceledException or MQTTnet.Exceptions.MqttCommunicationException)
    {
        Console.Error.WriteLine($"The fixture did not respond during cleanup. {exception.Message}");
    }
}

/// <summary>
/// Stops the local test clients and returns the extension to its default log.
/// </summary>
async Task Cleanup()
{
    using var cleanup = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    if (hostWorker is not null)
    {
        using (hostWorker)
        {
            if (!hostWorker.HasExited)
            {
                hostWorker.Kill();
                await hostWorker.WaitForExitAsync(cleanup.Token);
            }
        }
    }

    if (!result.Task.IsCompleted)
    {
        await Send(controlTopic, "END_TEST", cleanup.Token);
    }

    if (node?.IsRunning == true)
    {
        try
        {
            await node.Stop().WaitAsync(cleanup.Token);
        }
        catch (MQTTnet.Exceptions.MqttClientDisconnectedException) when (!node.IsConnected)
        {
            // The TCK can end the primary host session as the node is stopping.
        }
    }

    if (application?.IsRunning == true)
    {
        await application.Stop().WaitAsync(cleanup.Token);
    }

    await Send(resultConfigTopic, $"NEW_RESULT-LOG {defaultLog}", cleanup.Token);
}

/// <summary>
/// Starts a host in a separate process so the test can close its connection without an MQTT DISCONNECT.
/// </summary>
static Process StartHostWorker(string host, string clientId, string protocol)
{
    var executable = Environment.ProcessPath ?? throw new InvalidOperationException("The runner executable is unavailable.");
    var start = new ProcessStartInfo(executable)
    {
        UseShellExecute = false,
        CreateNoWindow = true,
        RedirectStandardInput = true,
        RedirectStandardOutput = true
    };
    if (Path.GetFileNameWithoutExtension(executable) == "dotnet")
    {
        start.ArgumentList.Add(typeof(SparkplugPayloadMetadata).Assembly.Location);
    }

    start.ArgumentList.Add("host-worker");
    start.ArgumentList.Add(host);
    start.ArgumentList.Add(clientId);
    start.ArgumentList.Add(protocol);
    return Process.Start(start) ?? throw new InvalidOperationException("The primary host worker could not start.");
}

/// <summary>
/// Parses the runner's MQTT version argument.
/// </summary>
static SparkplugMqttProtocolVersion ParseProtocol(string value) =>
    value == "500" ? SparkplugMqttProtocolVersion.V500 : SparkplugMqttProtocolVersion.V311;

/// <summary>
/// Sends a command to the TCK extension.
/// </summary>
Task Send(string topic, string payload, CancellationToken cancellationToken) =>
    controller.PublishAsync(new MqttApplicationMessageBuilder()
        .WithTopic(topic)
        .WithPayload(payload)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .Build(), cancellationToken);

/// <summary>
/// Connects the SparkplugNet Host Application as a primary host.
/// </summary>
Task StartPrimaryHost(SparkplugApplication application) =>
    application.Start(new SparkplugApplicationOptions("127.0.0.1", 1883, $"{run}-primary", scadaHostIdentifier: host,
        isPrimaryApplication: true, mqttProtocolVersion: protocol, cancellationToken: deadline.Token,
        reconnectInterval: multipleBrokers ? TimeSpan.FromMilliseconds(200) : null)
    {
        MessageReorderTimeout = hostReorders ? TimeSpan.FromMilliseconds(reorderMilliseconds) : null,
        IncludeDataTypesInCommandMessages = hostSendsCommands,
        AdditionalBrokers = multipleBrokers ? [secondaryEndpoint] : []
    });

/// <summary>
/// Sends the commands requested by the TCK against metrics announced by its edge fixture.
/// </summary>
async Task SendHostCommands(SparkplugApplication application)
{
    var rebirth = new Metric("Node Control/Rebirth", DataType.Boolean, true);
    await prompts.Reader.ReadAsync(deadline.Token);
    while (!application.MetricStatesBySource.TryGetValue((group, edge, null), out var edgeState)
        || !edgeState.Metrics.ContainsKey("TCK_metric/Int32")
        || !application.MetricStatesBySource.TryGetValue((group, edge, device), out var deviceState)
        || !deviceState.Metrics.ContainsKey("Temperature")
        || !deviceState.Metrics.ContainsKey("EdgeNode Control/Rebirth"))
    {
        await Task.Delay(20, deadline.Token);
    }
    await application.PublishNodeCommand([rebirth], group, edge);

    await prompts.Reader.ReadAsync(deadline.Token);
    await application.PublishNodeCommand([new("TCK_metric/Int32", DataType.Int32, 42)], group, edge);

    await prompts.Reader.ReadAsync(deadline.Token);
    await application.PublishDeviceCommand([new("EdgeNode Control/Rebirth", DataType.Boolean, true)], group, edge, device);

    await prompts.Reader.ReadAsync(deadline.Token);
    await application.PublishDeviceCommand([new("Temperature", DataType.Int16, (short)42)], group, edge, device);
}

/// <summary>
/// Answers each TCK question only after checking the corresponding state in the host.
/// </summary>
async Task CheckHostDeathPrompts(SparkplugApplication application)
{
    for (var question = 1; question <= 6; question++)
    {
        var prompt = await prompts.Reader.ReadAsync(deadline.Token);
        var byDeviceDeath = question <= 2;
        var checkDevice = question is 1 or 2 or 5 or 6;
        var checkMetrics = question % 2 == 0;
        var referenceTime = await (byDeviceDeath ? deviceDeathTimestamp.Task : nodeDeathObservedAt.Task).WaitAsync(deadline.Token);
        using var checkDeadline = CancellationTokenSource.CreateLinkedTokenSource(deadline.Token);
        checkDeadline.CancelAfter(TimeSpan.FromSeconds(3));
        var passed = false;
        try
        {
            while (!passed)
            {
                var states = application.MetricStatesBySource;
                if (states.TryGetValue((group, edge, checkDevice ? device : null), out var state)
                    && state.MetricStatus == SparkplugMetricStatus.Offline && state.StatusTimestamp is ulong timestamp)
                {
                    var correctTime = byDeviceDeath ? timestamp == referenceTime
                        : timestamp + 2000 >= referenceTime && timestamp <= UtcNow() && timestamp > deviceDeathTimestamp.Task.Result;
                    passed = correctTime && (!checkMetrics || (state.MetricsAreStale && !state.Metrics.IsEmpty));
                }

                if (!passed)
                {
                    await Task.Delay(20, checkDeadline.Token);
                }
            }
        }
        catch (OperationCanceledException) when (!deadline.IsCancellationRequested)
        {
            // Let the TCK record the failed observation.
        }

        Console.WriteLine($"{(passed ? "PASS" : "FAIL")} - {prompt}");
        await Send(replyTopic, passed ? "PASS" : "FAIL", deadline.Token);
        if (question == 2)
        {
            await node!.Stop();
        }
    }
}

/// <summary>
/// Returns the local UTC timestamp in Unix milliseconds.
/// </summary>
static ulong UtcNow() => (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

/// <summary>
/// Creates distinct aliases and, for SendData, a definition and matching template instances.
/// </summary>
(Metric[] Metrics, Metric[] DeviceMetrics, Metric[] NodeData, Metric[] DeviceData) CreateEdgeMetrics()
{
    if (!edgeSendsData)
    {
        var nodeMetric = new Metric("Speed", DataType.Int32, 42) { Alias = 1 };
        var deviceMetric = new Metric("Temperature", DataType.Int32, 21) { Alias = 2 };
        return ([nodeMetric], [deviceMetric], [nodeMetric], [deviceMetric]);
    }

    const string type = "Measurement";
    var definitionName = $"_types_/{type}";
    var definition = new Metric(definitionName, DataType.Template, new Template
    {
        IsDefinition = true,
        Version = "1.0",
        Metrics = [new("value", DataType.Int32, 0)],
        Parameters = [new("unit", DataType.String, "C")]
    });

    var nodeMetrics = new List<Metric> { definition, Instance("NodeMeasurement", 1, 42) };
    if (scenario == "SendComplexData")
    {
        nodeMetrics.Add(new Metric("Samples", DataType.DataSet, new DataSet(new Dictionary<string, DataType>
        {
            ["Temperature"] = DataType.Int32,
            ["Label"] = DataType.String
        })
        {
            Rows = [new Row { Elements = [new(DataType.Int32, 42), new(DataType.String, "outside")] }]
        })
        {
            Properties = new PropertySet
            {
                Keys = ["Quality", "Description"],
                Values = [new(DataType.Int32, 192), new(DataType.String, "Samples")]
            }
        });
    }

    return ([.. nodeMetrics], [Instance("DeviceMeasurement", 2, 21)],
        [Instance(string.Empty, 1, 43)], [Instance(string.Empty, 2, 22)]);

    Metric Instance(string name, ulong alias, int value) => new(name, DataType.Template, new Template
    {
        IsDefinition = false,
        Version = "1.0",
        TemplateRef = definitionName,
        Metrics = [new("value", DataType.Int32, value)],
        Parameters = [new("unit", DataType.String, "C")]
    })
    {
        Alias = alias,
        IsHistorical = false
    };
}

/// <summary>
/// Reads the timestamp field independently from the original Sparkplug B payload.
/// </summary>
[ProtoContract]
internal sealed class SparkplugPayloadMetadata
{
    /// <summary>
    /// Gets or sets the timestamp in Unix milliseconds.
    /// </summary>
    [ProtoMember(1)]
    public ulong Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the edge sequence number.
    /// </summary>
    [ProtoMember(3)]
    public ulong? Sequence { get; set; }
}

/// <summary>
/// Reports library warnings that would otherwise be hidden during a TCK run.
/// </summary>
internal sealed class WarningLogger<T> : ILogger<T>
{
    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => logLevel is >= LogLevel.Warning and < LogLevel.None;

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (this.IsEnabled(logLevel))
        {
            Console.Error.WriteLine(formatter(state, exception));
            if (exception is not null && logLevel >= LogLevel.Error)
            {
                Console.Error.WriteLine(exception);
            }
        }
    }
}
