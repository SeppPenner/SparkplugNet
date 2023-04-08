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

<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-8-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->

## Available for
* NetStandard 2.0
* NetStandard 2.1
* Net 6.0
* Net 7.0

## Net Core and Net Framework latest and LTS versions
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

## Recommended clients
* [MQTT.fx](http://www.mqttfx.jensd.de/) has a Sparkplug data decoder. The binaries for version 1.7.1 can also be taken from https://github.com/SeppPenner/mqttfx171-backup (The software is now under development by a company and requires a license for version 1.7.1+).

## Supported Sparkplug standards
* Version 1.3.0 and above: Sparkplug, version 3.0, spBv1.0.
* Version 1.2.0 and below: Sparkplug, version 2.2, spAv1.0 and spBv1.0.

## Special notes
* Version 1.1.0 introduces the async event pattern and deprecates the "old, synchronous" events, Version 1.2.0+ will remove the old events completely. (BREAKING)
* Although it's not required to publish a **BDSEQ** metric with all messages according to the specification,
this library includes it in any `spBv1.0` message except the state messages in versions up to 1.0.0 because I think it's useful.
From version 1.1.0 on, this behaviour can be controlled by setting `AddSessionNumberToCommandMessages` in the application options and `AddSessionNumberToDataMessages` in the node options.
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

## Contributors ✨

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tbody>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="https://franzhuber23.blogspot.de/"><img src="https://avatars.githubusercontent.com/u/9639361?v=4?s=100" width="100px;" alt="HansM"/><br /><sub><b>HansM</b></sub></a><br /><a href="https://github.com/SeppPenner/SparkplugNet/commits?author=SeppPenner" title="Code">💻</a> <a href="https://github.com/SeppPenner/SparkplugNet/commits?author=SeppPenner" title="Documentation">📖</a> <a href="#example-SeppPenner" title="Examples">💡</a> <a href="#maintenance-SeppPenner" title="Maintenance">🚧</a> <a href="#projectManagement-SeppPenner" title="Project Management">📆</a> <a href="https://github.com/SeppPenner/SparkplugNet/commits?author=SeppPenner" title="Tests">⚠️</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/insightdocs"><img src="https://avatars.githubusercontent.com/u/23101485?v=4?s=100" width="100px;" alt="insightdocs"/><br /><sub><b>insightdocs</b></sub></a><br /><a href="https://github.com/SeppPenner/SparkplugNet/commits?author=insightdocs" title="Tests">⚠️</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/OffTravel"><img src="https://avatars.githubusercontent.com/u/19183574?v=4?s=100" width="100px;" alt="OffTravel"/><br /><sub><b>OffTravel</b></sub></a><br /><a href="https://github.com/SeppPenner/SparkplugNet/commits?author=OffTravel" title="Code">💻</a> <a href="https://github.com/SeppPenner/SparkplugNet/commits?author=OffTravel" title="Tests">⚠️</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/cjmurph"><img src="https://avatars.githubusercontent.com/u/2868949?v=4?s=100" width="100px;" alt="cjmurph"/><br /><sub><b>cjmurph</b></sub></a><br /><a href="https://github.com/SeppPenner/SparkplugNet/commits?author=cjmurph" title="Code">💻</a> <a href="https://github.com/SeppPenner/SparkplugNet/commits?author=cjmurph" title="Tests">⚠️</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/BoBiene"><img src="https://avatars.githubusercontent.com/u/23037659?v=4?s=100" width="100px;" alt="Bo Biene"/><br /><sub><b>Bo Biene</b></sub></a><br /><a href="https://github.com/SeppPenner/SparkplugNet/commits?author=BoBiene" title="Code">💻</a> <a href="https://github.com/SeppPenner/SparkplugNet/commits?author=BoBiene" title="Tests">⚠️</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/TimJoehnk"><img src="https://avatars.githubusercontent.com/u/93274944?v=4?s=100" width="100px;" alt="Tim Jöhnk"/><br /><sub><b>Tim Jöhnk</b></sub></a><br /><a href="https://github.com/SeppPenner/SparkplugNet/commits?author=TimJoehnk" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/Patrick2607"><img src="https://avatars.githubusercontent.com/u/9799823?v=4?s=100" width="100px;" alt="Patrick.GK"/><br /><sub><b>Patrick.GK</b></sub></a><br /><a href="https://github.com/SeppPenner/SparkplugNet/commits?author=Patrick2607" title="Code">💻</a> <a href="https://github.com/SeppPenner/SparkplugNet/commits?author=Patrick2607" title="Tests">⚠️</a> <a href="#infra-Patrick2607" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a></td>
    </tr>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/geraldasp"><img src="https://avatars.githubusercontent.com/u/1334535?v=4?s=100" width="100px;" alt="Gerald Asp"/><br /><sub><b>Gerald Asp</b></sub></a><br /><a href="https://github.com/SeppPenner/SparkplugNet/commits?author=geraldasp" title="Tests">⚠️</a> <a href="https://github.com/SeppPenner/SparkplugNet/commits?author=geraldasp" title="Code">💻</a></td>
    </tr>
  </tbody>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!