#pragma warning disable IDE0065 // Die using-Anweisung wurde falsch platziert.
global using System.Collections.Concurrent;
global using System.ComponentModel;
global using System.Text;

global using MQTTnet;
global using MQTTnet.Client;
global using MQTTnet.Client.Options;
global using MQTTnet.Client.Publishing;
global using MQTTnet.Formatter;
global using MQTTnet.Protocol;

global using ProtoBuf;

global using Serilog;

global using SparkplugNet.Core.Application;
global using SparkplugNet.Core.Enumerations;
global using SparkplugNet.Core.Extensions;
global using SparkplugNet.Core.Messages;
global using SparkplugNet.Core.Node;

global using VersionAData = SparkplugNet.VersionA.Data;
global using VersionAProtoBuf = SparkplugNet.VersionA.ProtoBuf;
global using VersionBData = SparkplugNet.VersionB.Data;
global using VersionBProtoBuf = SparkplugNet.VersionB.ProtoBuf;

global using SystemCancellationToken = System.Threading.CancellationToken;
global using VersionBDataTypeEnum = SparkplugNet.VersionB.Data.DataType;
#pragma warning restore IDE0065 // Die using-Anweisung wurde falsch platziert.