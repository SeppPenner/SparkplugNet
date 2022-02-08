SparkplugNet
====================================

SparkplugNet is a library to use the Sparkplug industrial IoT (IIoT) standard in .Net. It uses [MQTTnet](https://github.com/chkr1011/MQTTnet) in the background.

[![Build status](https://ci.appveyor.com/api/projects/status/w6pu8fcav4n7651t?svg=true)](https://ci.appveyor.com/project/SeppPenner/sparkplugnet)
[![GitHub issues](https://img.shields.io/github/issues/SeppPenner/SparkplugNet.svg)](https://github.com/SeppPenner/SparkplugNet/issues)
[![GitHub forks](https://img.shields.io/github/forks/SeppPenner/SparkplugNet.svg)](https://github.com/SeppPenner/SparkplugNet/network)
[![GitHub stars](https://img.shields.io/github/stars/SeppPenner/SparkplugNet.svg)](https://github.com/SeppPenner/SparkplugNet/stargazers)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://raw.githubusercontent.com/SeppPenner/SparkplugNet/master/License.txt)
[![Nuget](https://img.shields.io/badge/SparkplugNet-Nuget-brightgreen.svg)](https://www.nuget.org/packages/SparkplugNet/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/SparkplugNet.svg)](https://www.nuget.org/packages/SparkplugNet/)
[![Known Vulnerabilities](https://snyk.io/test/github/SeppPenner/SparkplugNet/badge.svg)](https://snyk.io/test/github/SeppPenner/SparkplugNet)
[![Gitter](https://badges.gitter.im/SparkplugNet/community.svg)](https://gitter.im/SparkplugNet/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

## Available for
* NetFramework 4.6.2
* NetFramework 4.7
* NetFramework 4.7.2
* NetFramework 4.8
* NetStandard 2.0
* NetStandard 2.1
* NetCore 3.1
* Net 5.0
* Net 6.0

## Net Core and Net Framework latest and LTS versions
* https://dotnet.microsoft.com/download/dotnet-framework
* https://dotnet.microsoft.com/download/dotnet-core
* https://dotnet.microsoft.com/download/dotnet

## Structure
Sparkplug distinguishes between 5 different types of logical parts:

|Part|Description|
|-|-|
|Primary application|The main application that needs to be always available to work with the data and store metrics.|
|Applications|Other applications that work with the sent data and metrics.|
|EoN nodes (Later only called nodes)|Logical devices that bundle data from end-of-network devices and publish their data.|
|MQTT enabled devices|Sensors that work as a "hybrid" version of a node and a device and publish metrics and data.|
|Non MQTT-enabled devices|Sensors that push data to nodes where the nodes publish their data to MQTT on their behalf.|

## Basic usage
For basic usage, see the [How to use file](./HowToUse.md) or the [example project](https://github.com/SeppPenner/SparkplugNet/blob/master/src/SparkplugNet.Examples/Program.cs).

## Requirements
* MQTT broker that implements 100% of the MQTT 3.1.1 specification
* MQTT broker with QoS 0 and 1 support
* MQTT broker with retained messages support
* MQTT broker with last will and testament support
* MQTT broker with a flexible security system

## Recommended brokers
* [MQTTnet.Server](https://github.com/chkr1011/MQTTnet.Server) for simple MQTT testing.
* [HiveMQ CE](https://github.com/hivemq/hivemq-community-edition) with the [Sparkplug InfluxDB Extension](https://github.com/hivemq/hivemq-sparkplug-influxdb-extension/) for Sparkplug testing.
* [Ignition](https://inductiveautomation.com/ignition/) for Sparkplug testing.

## Supported Sparkplug standards
* Version 2.2, spAv1.0 and spBv1.0.

## Special notes
* Although it's not required to publish a **BDSEQ** metric with all messages according to the specification,
this library includes it in any `spBv1.0` message except the state messages because I think it's useful.
* The library uses [Serilog](https://serilog.net/) for logging purposes because it's very extendable (`Log.Logger` or similar stuff in the examples refers to the Serilog library).

## Further resources
* https://www.eclipse.org/tahu/spec/Sparkplug%20Topic%20Namespace%20and%20State%20ManagementV2.2-with%20appendix%20B%20format%20-%20Eclipse.pdf
* https://documents.opto22.com/2357_Industrial_Strength_MQTT_Sparkplug_B.pdf
* https://github.com/eclipse/tahu
* https://github.com/eclipse/kura/blob/develop/kura/org.eclipse.kura.core.cloud/src/main/protobuf/kurapayload.proto
* https://github.com/eclipse/tahu/blob/master/sparkplug_b/sparkplug_b_c_sharp.proto
* https://protogen.marcgravell.com/
* https://stackoverflow.com/questions/66199386/protobuf-net-how-to-use-the-any-data-type
* http://www.steves-internet-guide.com/spsparkplug-payloads-and-messages/

Change history
--------------

See the [Changelog](https://github.com/SeppPenner/SparkplugNet/blob/master/Changelog.md).
