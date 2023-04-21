#pragma warning disable IDE0065 // Die using-Anweisung wurde falsch platziert.
global using System;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
global using System.Xml.Serialization;

global using MQTTnet;
global using MQTTnet.Client;
global using MQTTnet.Formatter;
global using MQTTnet.Internal;
global using MQTTnet.Protocol;

global using System.Text.Json.Serialization;

global using ProtoBuf;

global using Serilog;

global using SparkplugNet.Core;
global using SparkplugNet.Core.Application;
global using SparkplugNet.Core.Data;
global using SparkplugNet.Core.Enumerations;
global using SparkplugNet.Core.Extensions;
global using SparkplugNet.Core.Interfaces;
global using SparkplugNet.Core.Messages;
global using SparkplugNet.Core.Node;
global using SparkplugNet.Core.Topics;
global using SparkplugNet.VersionB.Data;

global using VersionBData = SparkplugNet.VersionB.Data;
global using VersionBDataTypeEnum = SparkplugNet.VersionB.Data.DataType;
global using VersionBProtoBuf = SparkplugNet.VersionB.ProtoBuf;
global using SystemCancellationToken = System.Threading.CancellationToken;
#pragma warning restore IDE0065 // Die using-Anweisung wurde falsch platziert.