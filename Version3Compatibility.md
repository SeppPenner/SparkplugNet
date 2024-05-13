# Version 3.0 Sparkplug compatibility matrix

|Rule id|Short description|Okay|
|-|-|-|
|tck-id-intro-sparkplug-host-state|Host applications must publish STATE messages at any time.|✔️|
|tck-id-intro-group-id-string|Group id must be UTF8.|✔️|
|tck-id-intro-group-id-chars|Group id must only use characters in MQTT spec.|✔️|
|tck-id-intro-edge-node-id-string|Edge node id must be UTF8.|✔️|
|tck-id-intro-edge-node-id-chars|Edge node id must only use characters in MQTT spec.|✔️|
|tck-id-intro-device-id-string|Device id must be UTF8.|✔️|
|tck-id-intro-device-id-chars|Device id must only use characters in MQTT spec.|✔️|
|tck-id-intro-edge-node-id-uniqueness|Edge node id must be unique.|Depends on user, can't be checked. ⚠️|
|tck-id-principles-rbe-recommended|Edge nodes need to instantly publish messages (No batching).|✔️|
|tck-id-principles-birth-certificates-order|Birth certificates must be the first MQTT messages published by any edge node or any host application.|✔️|
|tck-id-principles-persistence-clean-session-311|Clean session = `true` for edge node birth packet on MQTT v3.1.1.|✔️|
|tck-id-principles-persistence-clean-session-50|Clean start = `true` and session expiry interval = 0 for edge node birth packet on MQTT v5.0.|✔️|
|tck-id-components-ph-state|Host applications must publish STATE messages at any time.|✔️|
|tck-id-topic-structure|Namespace specification must be used by all: `namespace/group_id/message_type/edge_node_id/[device_id]`.|✔️|
|tck-id-topic-structure-namespace-a|Namespace must equal `spBv1.0`.|✔️|
|tck-id-topic-structure-namespace-valid-group-id|Group id must be UTF8 without `+` and `#`.|✔️|
|tck-id-topic-structure-namespace-unique-edge-node-descriptor|Group id and edge node id in combination must be unique.|Depends on user, can't be checked. ⚠️|
|tck-id-topic-structure-namespace-valid-edge-node-id|Edge node id must be UTF8 without `+` and `#`.|✔️|
|tck-id-topic-structure-namespace-valid-device-id|Device id must be UTF8 without `+` and `#`.|✔️|
|tck-id-topic-structure-namespace-unique-device-id|Device id must be unique within the same edge node.|Depends on user, can't be checked. ⚠️|
|tck-id-topic-structure-namespace-duplicate-device-id-across-edge-node|Device id may be not unique across different nodes.|✔️|
|tck-id-topic-structure-namespace-device-id-associated-message-types|The device_id mustn't be included within NBIRTH, NDEATH, NDATA, NCMD, and STATE based topics|✔️|
|tck-id-topics-nbirth-topic|NBIRTH topic must be of form `namespace/group_id/NBIRTH/edge_node_id`.|✔️|
|tck-id-topics-nbirth-mqtt|NBIRTH must be published with QoS = `0` and retain flag = `false`.|✔️|
|tck-id-topics-nbirth-seq-num|NBIRTH must include a sequence number = `0`.|✔️|
|tck-id-topics-nbirth-timestamp|NBIRTH must include a sent at timestamp.|✔️|
|tck-id-topics-nbirth-metric-reqs|NBIRTH must include all metrics the edge node will report.|✔️|
|tck-id-topics-nbirth-metrics|At a minimum each metric must include the metric name, datatype, and current value.|✔️|
|tck-id-topics-nbirth-templates|NBIRTH must include all templates the edge node will report.|✔️|
|tck-id-topics-nbirth-bdseq-included|NBIRTH must include a BDSEQ number.|✔️|
|tck-id-topics-nbirth-bdseq-matching|BDSEQ number must match the one in the packet's will payload.|✔️|
|tck-id-topics-nbirth-bdseq-increment|BDSEQ number must start at `0` and increment on every new connect.|✔️|
|tck-id-topics-nbirth-rebirth-metric|NBIRTH message must include a metric with the name `Node Control/Rebirth` of type boolean and value `false`.|✔️|
|tck-id-topics-ndata-topic|NDATA topic must be of form `namespace/group_id/NDATA/edge_node_id`.|✔️|
|tck-id-topics-ndata-mqtt|NDATA must be published with QoS = `0` and retain flag = `false`.|✔️|
|tck-id-topics-ndata-seq-num|NDATA must include a sequence number between 0 and 255 which increments on each new message.|✔️|
|tck-id-topics-ndata-timestamp|NDATA must include a sent at timestamp.|✔️|
|tck-id-topics-ndata-payload|NDATA must include all changed edge node metrics since the last report.|✔️|
|tck-id-topics-ndeath-topic|NDEATH topic must be of form `namespace/group_id/NDEATH/edge_node_id`.|✔️|
|tck-id-topics-ndeath-payload|NDEATH must only include a BDSEQ number.|✔️|
|tck-id-topics-ndeath-seq|NDEATH must not include a sequence number.|✔️|
|tck-id-topics-ncmd-topic|NCMD topic must be of form `namespace/group_id/NCMD/edge_node_id`.|✔️|
|tck-id-topics-ncmd-mqtt|NCMD must be published with QoS = `0` and retain flag = `false`.|✔️|
|tck-id-topics-ncmd-timestamp|NCMD must include a sent at timestamp.|✔️|
|tck-id-topics-ncmd-payload|NCMD must include the metrics that are written to the edge node.|✔️|
|tck-id-topics-dbirth-topic|DBIRTH topic must be of form `namespace/group_id/DBIRTH/edge_node_id/device_id`.|✔️|
|tck-id-topics-dbirth-mqtt|DBIRTH must be published with QoS = `0` and retain flag = `false`.|✔️|
|tck-id-topics-dbirth-seq|DBIRTH must include a sequence number between 0 and 255 which increments on each new message.|✔️|
|tck-id-topics-dbirth-timestamp|DBIRTH must include a sent at timestamp.|✔️|
|tck-id-topics-dbirth-metric-reqs|DBIRTH must include all metrics the device will report.|✔️|
|tck-id-topics-dbirth-metrics|At a minimum each metric must include the metric name, datatype, and current value.|✔️|
|tck-id-topics-ddata-topic|DDATA topic must be of form `namespace/group_id/DDATA/edge_node_id/device_id`.|✔️|
|tck-id-topics-ddata-mqtt|DDATA must be published with QoS = `0` and retain flag = `false`.|✔️|
|tck-id-topics-ddata-seq-num|DDATA must include a sequence number between 0 and 255 which increments on each new message.|✔️|
|tck-id-topics-ddata-timestamp|DDATA must include a sent at timestamp.|✔️|
|tck-id-topics-ddata-payload|DDATA must include all changed edge node metrics since the last report.|✔️|
|tck-id-topics-ddeath-topic|DDEATH topic must be of form `namespace/group_id/DDEATH/edge_node_id/device_id`.|✔️|
|tck-id-topics-ddeath-mqtt|DDEATH must be published with QoS = `0` and retain flag = `false`.|✔️|
|tck-id-topics-ddeath-seq-num|DDEATH must include a sequence number between 0 and 255 which increments on each new message.|✔️|
|tck-id-topics-dcmd-topic|DCMD topic must be of form `namespace/group_id/DCMD/edge_node_id/device_id`.|✔️|
|tck-id-topics-dcmd-mqtt|DCMD must be published with QoS = `0` and retain flag = `false`.|✔️|
|tck-id-topics-dcmd-timestamp|DCMD must include a sent at timestamp.|✔️|
|tck-id-topics-dcmd-payload|DCMD must include the metrics that are written to the device.|✔️|
|ck-id-host-topic-phid-birth-message|Host application must publish a birth message as first message.|✔️|
|tck-id-host-topic-phid-birth-qos|Birth STATE must be published with QoS = `1`.|✔️|
|tck-id-host-topic-phid-birth-retain|Birth STATE must be published with retain flag = `true`.|✔️|
|tckid-host-topic-phid-birth-topic|Birth STATE topic must be of form `spBv1.0/STATE/sparkplug_host_id`.|✔️|
|tck-id-host-topic-phid-birth-sub-required|Host application must subscribe to the messages for all edge nodes and devices.|✔️|
|tck-id-host-topic-phid-birth-required|Host application must publish a STATE message after the subscription.|✔️|
|tck-id-host-topic-phid-birth-payload|Birth STATE must be UTF8 JSON with `online:true` and `timestamp:124335` (UTC time in milliseconds since Epoch).|✔️|
|tck-id-host-topic-phid-birth-payload-timestamp|Birth STATE timestamp must be the same as in the will message.|✔️|
|tck-id-host-topic-phid-death-qos|Death STATE must be published with QoS = `1`.|✔️|
|tck-id-host-topic-phid-death-retain|Death STATE must be published with retain flag = `true`.|✔️|
|tck-id-host-topic-phid-death-topic|Death STATE topic must be of form `spBv1.0/STATE/sparkplug_host_id`.|✔️|
|tck-id-host-topic-phid-death-required|Death STATE packet is the will message from the connect message.|✔️|
|tck-id-host-topic-phid-death-payload|Death STATE must be UTF8 JSON with `online:false` and `timestamp:124335` (UTC time in milliseconds since Epoch).|✔️|
|tck-id-host-topic-phid-death-payload-connect|Death STATE message contains a UTC timestamp when the connect message was sent to the server.|✔️|
|tck-id-host-topic-phid-death-payload-disconnect-clean|Host application must send death STATE message before disconnect.|✔️|
|ck-id-host-topic-phid-death-payload-disconnect-with-no-disconnect-packet|Host application must send death STATE message before disconnect if no MQTT disconnect message is used.|✔️|
|tck-id-case-sensitivity-sparkplug-ids|Ids should be case sensitive.|Depends on user, not mandatory⚠️|
|tck-id-case-sensitivity-metric-names|Metric names should be case sensitive.|Depends on user, not mandatory⚠️|
|tck-id-message-flow-phid-sparkplug-clean-session-311|Host application must set clean session = `true` for MQTT v3.1.1 on connect.|✔️|
|tck-id-message-flow-phid-sparkplug-clean-session-50|Host application must set clean start = `true` and session expiry interval = 0 for MQTT v5.0 on connect.|✔️|
|tck-id-message-flow-phid-sparkplug-subscription|Host application must immediately subscribe after connect.|✔️|
|tck-id-message-flow-phid-sparkplug-state-publish|Host application must immediately publish STATE after subscription.|✔️|
|tck-id-message-flow-phid-sparkplug-state-publish-payload|Birth STATE must be UTF8 JSON with `online:true` and `timestamp:124335` (UTC time in milliseconds since Epoch).|✔️|
|tck-id-message-flow-phid-sparkplug-state-publish-payload-timestamp|Birth STATE timestamp must be the same as in the will message.|✔️|
|?|Host application must set all metrics to stale if the connection is lost.|✔️|
|tck-id-message-flow-hid-sparkplug-state-message-delivered|If host application gets a STATE message with `online:false` on their own identifier, it must immediately publish a new birth STATE message.|✔️|
|tck-id-message-flow-edge-node-ncmd-subscribe|Edge node must subscribe to topic of form `spBv1.0/group_id/NCMD/edge_node_id` with QoS = `1`.|✔️|
|tck-id-message-flow-edge-node-birth-publish-connect|Edge node must establish a MQTT session before publishing.|✔️|
|tck-id-message-flow-edge-node-birth-publish-will-message|Edge node must send a will message on connect.|✔️|
|tck-id-message-flow-edge-node-birth-publish-will-message-topic|Edge node must send a will message to topic of form `spBv1.0/group_id/NDEATH/edge_node_id`|✔️|
|tck-id-message-flow-edge-node-birth-publish-will-message-payload|Edge node will message must be ProtoBuf encoded.|✔️|
|tck-id-message-flow-edge-node-birth-publish-will-message-payload-bdSeq|BDSEQ (int64) metric of edge node death message must be the incremented by one in respect of the connect message. (Values must be between 0 and 255).|✔️|
|tck-id-message-flow-edge-node-birth-publish-will-message-qos|Edge node will message QoS must be set to `1`.|✔️|
|tck-id-message-flow-edge-node-birth-publish-will-message-will-retained|Edge node will message retain flag must be set to `false`.|✔️|
|tck-id-message-flow-edge-node-birth-publish-phid-wait|If the edge node is configured to wait for the primary host application, it must verify the application online state.|✔️|
|tck-id-message-flow-edge-node-birth-publish-phid-wait-id|Edge node must validate the primary host application id.|✔️|
|tck-id-message-flow-edge-node-birth-publish-phid-wait-online|Edge node must validate the primary host application online state.|✔️|
|tck-id-message-flow-edge-node-birth-publish-phid-wait-timestamp|Edge node must validate the primary host application timestamp to be the latest of the STATE messages.|✔️|
|tck-id-message-flow-edge-node-birth-publish-nbirth-topic|NBIRTH topic must be of form `spBv1.0/group_id/NBIRTH/edge_node_id`|✔️|
|tck-id-message-flow-edge-node-birth-publish-nbirth-payload|NBIRTH payload must be ProtoBuf encoded.|✔️|
|tck-id-message-flow-edge-node-birth-publish-nbirth-payload-bdSeq|NBIRTH must include a BDSEQ metric (int64) with the same number as in the connect packet.|✔️|
|tck-id-message-flow-edge-node-birth-publish-nbirth-qos|NBIRTH must use QoS = `0`.|✔️|
|tck-id-message-flow-edge-node-birth-publish-nbirth-retained|NBIRTH must use the retain flag must be set to `false`.|✔️|
|tck-id-message-flow-edge-node-birth-publish-nbirth-payload-seq|NBIRTH must include a sequence number = `0`.|✔️|
|tck-id-message-flow-edge-node-birth-publish-phid-offline|If a STATE message with greater timestamp and online = `false` is recevied, NDEATH needs to be sent.|✔️|
|tck-id-operational-behavior-edge-node-intentional-disconnect-ndeath|Edge node must publish NDEATH before disconnect.|✔️|
|tck-id-operational-behavior-edge-node-intentional-disconnect-packet|Edge node may send MQTT disconnect packet immediately after the NDEATH message.|✔️|
|tck-id-operational-behavior-edge-node-termination-host-action-ndeath-node-offline|Host application must mark node as offline after NDEATH is received (And set the timestamp).|✔️|
|tck-id-operational-behavior-edge-node-termination-host-action-ndeath-node-tags-stale|Host application must mark node metrics as stale after NDEATH is received (And set the timestamp).|✔️|
|tck-id-operational-behavior-edge-node-termination-host-action-ndeath-devices-offline|Host application must mark node related devices as offline after NDEATH is received (And set the timestamp).|✔️|
|tck-id-operational-behavior-edge-node-termination-host-action-ndeath-devices-tags-stale|Host application must mark node related device metrics as stale after NDEATH is received (And set the timestamp).|✔️|
|tck-id-operational-behavior-edge-node-termination-host-offline|If edge node is configured to wait for primary application, it must validate the timestamp and offline STATE messages and disconnect on offline STATE messages.|✔️|
|tck-id-operational-behavior-edge-node-termination-host-offline-reconnect|The edge node must reconnect to the MQTT server immediately after disconnect.|✔️|
|tck-id-operational-behavior-edge-node-termination-host-offline-timestamp|If edge node receives an offline STATE message with invalid timestamp (older than the one from the last STATE message), it must ignore it and not disconnect.|✔️|
|tck-id-message-flow-device-dcmd-subscribe|If the device allows writing to outputs, it must subscribe to topics in the form of `spBv1.0/group_id/DCMD/edge_node_id/device_id`.|✔️|
|tck-id-message-flow-device-birth-publish-nbirth-wait|NBIRTH must be sent prior to DBIRTH message.|✔️|
|tck-id-message-flow-device-birth-publish-dbirth-topic|DBIRTH topic must be of form `spBv1.0/group_id/DBIRTH/edge_node_id/device_id`|✔️|
|tck-id-message-flow-device-birth-publish-dbirth-match-edge-node-topic|DBIRTH topic must match the NBIRTH topic that was sent before.|Depends on user, can't be checked. ⚠️|
|tck-id-message-flow-device-birth-publish-dbirth-payload|DBIRTH payload must be ProtoBuf encoded.|✔️|
|tck-id-message-flow-device-birth-publish-dbirth-qos|DBIRTH must use QoS = `0`.|✔️|
|tck-id-message-flow-device-birth-publish-dbirth-retained|DBIRTH must have the retain flag set to `false`.|✔️|
|tck-id-message-flow-device-birth-publish-dbirth-payload-seq|DBIRTH must include a sequence number between 0 and 255 and incremented according to the prior edge node message sent.|✔️|
|tck-id-operational-behavior-device-ddeath|If edge node loses connection to the device, it must publish a DDEATH on behalf of a device.|✔️|
|tck-id-operational-behavior-edge-node-termination-host-action-ddeath-devices-offline|Host application must mark device as offline after DDEATH is received (And set the timestamp).|✔️|
|tck-id-operational-behavior-edge-node-termination-host-action-ddeath-devices-tags-stale|Host application must mark device metrics as stale after DDEATH is received (And set the timestamp).|✔️|
|tck-id-operational-behavior-host-reordering-param|Host applications should provice a reorder timeout parameter.|✔️|
|tck-id-operational-behavior-host-reordering-start|If host application is configured to us a reorder timeout, it must start a timer to start the reorder timeout window if an out-of-order message arrives.|✔️|
|tck-id-operational-behavior-host-reordering-rebirth|If reorder timeout on host application expires, the host application must send a NCMD message with a `Node Control/Rebirth` request of type boolean and value `true`.|✔️|
|tck-id-operational-behavior-host-reordering-success|If the missing message is recevied before the timer window elapses, the timer must be stopped and normal operation must be proceeded.|✔️|
|tck-id-operational-behavior-primary-application-state-with-multiple-servers-state-subs|In multiple server scenarios, the host application must send a STATE message and all edge nodes must subscribe to the STATE message if configured to use a primary application.|✔️|
|tck-id-operational-behavior-primary-application-state-with-multiple-servers-state|Host application always must send the STATE message as first message on all servers.|✔️|
|tck-id-operational-behavior-primary-application-state-with-multiple-servers-single-server|Edge nodes must only be connected to one server at once.|✔️|
|tck-id-operational-behavior-primary-application-state-with-multiple-servers-walk|If the primary application gets offline, the edge node must terminate its connection and move on to the next MQTT server.|✔️|
|tck-id-operational-behavior-edge-node-birth-sequence-wait|Edge node must wait for STATE message on the new server to publish its NBIRTH message.|✔️|
|tck-id-operational-behavior-host-application-host-id|Host application id must be unique.|Depends on user, can't be checked. ⚠️|
|tck-id-operational-behavior-host-application-connect-will|Host application must include a will message on connect.|✔️|
|tck-id-operational-behavior-host-application-connect-will-topic|Death STATE topic must be of form `spBv1.0/STATE/sparkplug_host_id`.|✔️|
|tck-id-operational-behavior-host-application-connect-will-payload|Death STATE must be UTF8 JSON with `online:false` and `timestamp:124335` (UTC time in milliseconds since Epoch).|✔️|
|tck-id-operational-behavior-host-application-connect-will-qos|Host application must publish death STATE message with QoS = `1`.|✔️|
|tck-id-operational-behavior-host-application-connect-will-retained|Host application must publish death STATE message with retain flag = `true`.|✔️|
|tck-id-operational-behavior-host-application-connect-birth|Host application must send a birth STATE message immediatley after connecting.|✔️|
|tck-id-operational-behavior-host-application-connect-birth-topic|Birth STATE topic must be of form `spBv1.0/STATE/sparkplug_host_id`.|✔️|
|tck-id-operational-behavior-host-application-connect-birth-payload|Birth STATE must be UTF8 JSON with `online:true` and `timestamp:124335` (UTC time in milliseconds since Epoch).|✔️|
|tck-id-operational-behavior-host-application-connect-birth-qos|Host application must publish birth STATE message with QoS = `1`.|✔️|
|tck-id-operational-behavior-host-application-connect-birth-retained|Host application must publish birth STATE message with retain flag = `true`.|✔️|
|tck-id-operational-behavior-host-application-multi-server-timestamp|Host application must maintain a STATE message timestamp on a per MQTT server basis.|✔️|
|tck-id-operational-behavior-host-application-termination|If the host application disconnects intentionally, it must publish a DEATH message.|✔️|
|tck-id-operational-behavior-host-application-death-topic|Death STATE topic must be of form `spBv1.0/STATE/sparkplug_host_id`.|✔️|
|tck-id-operational-behavior-host-application-death-payload|Death STATE must be UTF8 JSON with `online:false` and `timestamp:124335` (UTC time in milliseconds since Epoch).|✔️|
|tck-id-operational-behavior-host-application-death-qos|Host application must publish death STATE message with QoS = `1`.|✔️|
|tck-id-operational-behavior-host-application-death-retained|Host application must publish death STATE message with retain flag = `true`.|✔️|
|tck-id-operational-behavior-host-application-disconnect-intentional|On intentional disconnect, a MQTT disconnect packet may be sent immediately after the death STATE message is published.|✔️|
|tck-id-operational-behavior-data-publish-nbirth|NBIRTH must include all metrics ever published by a node.|✔️|
|tck-id-operational-behavior-data-publish-nbirth-values|NBIRTH metric values must be set or `is_null` must be set to `true` and no value must be set.|✔️|
|tck-id-operational-behavior-data-publish-nbirth-change|NDATA should only be published on edge node metric changes.|✔️|
|tck-id-operational-behavior-data-publish-nbirth-order|For all metrics with `is_historical:false`, NBIRTH and NDATA must keep data in chronological order.|✔️|
|tck-id-operational-behavior-data-publish-dbirth|DBIRTH must include all metrics ever published by a device.|✔️|
|tck-id-operational-behavior-data-publish-dbirth-values|DBIRTH metric values must be set or `is_null` must be set to `true` and no value must be set.|✔️|
|tck-id-operational-behavior-data-publish-dbirth-change|DDATA should only be published on edge node metric changes.|✔️|
|tck-id-operational-behavior-data-publish-dbirth-order|For all metrics with `is_historical:false`, DBIRTH and DDATA must keep data in chronological order.|✔️|
|tck-id-operational-behavior-data-commands-rebirth-name|NBIRTH message must include a metric with the name `Node Control/Rebirth` of type boolean and value `false`.|✔️|
|tck-id-operational-behavior-data-commands-rebirth-name-aliases|NBIRTH must not include an alias for the `Node Control/Rebirth` metric.|✔️|
|tck-id-operational-behavior-data-commands-rebirth-datatype|NBIRTH `Node Control/Rebirth` metric must be of type boolean.|✔️|
|tck-id-operational-behavior-data-commands-rebirth-value|NBIRTH `Node Control/Rebirth` metric value must be set to `false`.|✔️|
|tck-id-operational-behavior-data-commands-ncmd-rebirth-verb|Host application rebirth request message for edge nodes must use NCMD message.|✔️|
|tck-id-operational-behavior-data-commands-ncmd-rebirth-name|Host application rebirth request message for edge nodes must use the metric name `Node Control/Rebirth`.|✔️|
|tck-id-operational-behavior-data-commands-ncmd-rebirth-value|Host application rebirth request message for edge nodes must use the metric value `true`.|✔️|
|tck-id-operational-behavior-data-commands-rebirth-action-1|When edge node recevied a rebirth request, it mus timediately stop sending NDATA messages.|✔️|
|tck-id-operational-behavior-data-commands-rebirth-action-2|After edge node stops sending NDATA requests, it must send a NBIRTH and DBIRTH messages.|✔️|
|tck-id-operational-behavior-data-commands-rebirth-action-3|NBIRTH must include the same BDSEQ metric as in the previous MQTT connect packet.|✔️|
|tck-id-operational-behavior-data-commands-ncmd-verb|EDGE node command must use NCMD message.|✔️|
|tck-id-operational-behavior-data-commands-ncmd-metric-name|NCMD should include a known metric name.|✔️|
|tck-id-operational-behavior-data-commands-ncmd-metric-value|NCMD must include a compatible metric value according to the known metrics.|✔️|
|tck-id-operational-behavior-data-commands-dcmd-verb|Device command must use DCMD message.|✔️|
|tck-id-operational-behavior-data-commands-dcmd-metric-name|DCMD should include a known metric name.|✔️|
|tck-id-operational-behavior-data-commands-dcmd-metric-value|DCMD must include a compatible metric value according to the known metrics.|✔️|
|tck-id-payloads-timestamp-in-UTC|Payload timestamps must be in UTC in the form of an unsigned 64-bit integer representing the number of milliseconds since epoch (Jan 1, 1970).|✔️|
|tck-id-payloads-sequence-num-always-included|All edge node messages except NDEATH must include a sequence number.|✔️|
|tck-id-payloads-sequence-num-zero-nbirth|NBIRTH must include a sequence number between 0 and 255.|✔️|
|tck-id-payloads-sequence-num-incrementing|All subsequent edge node messages must have incrementing sequence numbers.|✔️|
|tck-id-payloads-name-requirement|Metric names must be set unless aliasses are used.|✔️|
|tck-id-payloads-alias-uniqueness|If supplied, aliasses must be unique accross the edge nodes metrics.|✔️|
|tck-id-payloads-alias-birth-requirement|NBIRTH and DBIRTH must include metric names and alias.|✔️|
|tck-id-payloads-alias-data-cmd-requirement|NDATA, DDATA, NCMD, and DCMD must include only an alias and not metric name.|✔️|
|tck-id-payloads-name-birth-data-requirement|Timestamp must be included in all metrics with NBIRTH, DBIRTH, NDATA, and DDATA messages.|✔️|
|tck-id-payloads-name-cmd-requirement|Timestamp may be included in all metrics with NCMD and DCMD messages.|✔️|
|tck-id-payloads-metric-timestamp-in-UTC|Timestamp must be in UTC.|✔️|
|tck-id-payloads-metric-datatype-value-type|Data type must be uint32.|✔️|
|tck-id-payloads-metric-datatype-value|Data type must be a known value.|✔️|
|tck-id-payloads-metric-datatype-req|Data type must be included in every metric in NBIRTH and DBIRTH messages.|✔️|
|tck-id-payloads-metric-datatype-not-req|Data type shouldn't be included in every metric in NDATA, NCMD, DDATA and DCMD messages.|✔️|
|tck-id-payloads-propertyset-keys-array-size|PropertySet must include as many keys as values.|✔️|
|tck-id-payloads-propertyset-values-array-size|PropertySet must include as many values as keys.|✔️|
|tck-id-payloads-metric-propertyvalue-type-type|PropertyValue type must be uint32.|✔️|
|tck-id-payloads-metric-propertyvalue-type-value|PropertyValue type must be a known value.|✔️|
|tck-id-payloads-metric-propertyvalue-type-req|PropertyValue type must be included in NBIRTH and DBIRTH messages.|✔️|
|tck-id-payloads-propertyset-quality-value-type|Quality value type must be int32.|✔️|
|tck-id-payloads-propertyset-quality-value-value|Quality value must be 0 (BAD), 192 (GOOD) or 500 (STALE).|✔️|
|tck-id-payloads-dataset-column-size|DataSet num_of_columns must be uint64 and equal the number of columns.|✔️|
|tck-id-payloads-dataset-column-num-headers|DataSet columns must be the same size as the types array.|✔️|
|tck-id-payloads-dataset-types-def|DataSet types must be array of uint32 representing the data types.|✔️|
|tck-id-payloads-dataset-types-num|DataSet types must be the same size as the columns array.|✔️|
|tck-id-payloads-dataset-types-type|DataSet types must be array of uint32 representing the data types.|✔️|
|tck-id-payloads-dataset-types-value|DataSet types must be known values.|✔️|
|tck-id-payloads-dataset-parameter-type-req|DataSet types array must be included in all data sets.|✔️|
|tck-id-payloads-template-dataset-value|DataSet.DataSetValue value must be of type uint32, uint64, float, double, bool, or string.|✔️|
|tck-id-payloads-template-definition-nbirth-only|Template definitions must be only included in NBIRTH messages.|✔️|
|tck-id-payloads-template-definition-is-definition|Template definition must have `is_definition` set to `true`.|✔️|
|tck-id-payloads-template-definition-ref|Template definition must omit the `template_ref` field.|✔️|
|tck-id-payloads-template-definition-members|Template defintion must include all member metrics that will be included in the template.|✔️|
|tck-id-payloads-template-definition-nbirth|Template defintion must be included in the NBIRTH for all template instances that are included in the NBIRTH or DBIRTH messages.|✔️|
|tck-id-payloads-template-definition-parameters|Template defintion must include all parameters that will be included in the template.|✔️|
|tck-id-payloads-template-definition-parameters-default|Template defintion may include parameters (They act as default if not set in the template).|✔️|
|tck-id-payloads-template-instance-is-definition|Template instance must have `is_definition` set to `false`.|✔️|
|tck-id-payloads-template-instance-ref|Template must have the reference to the type of the template set.|✔️|
|tck-id-payloads-template-instance-members|Template instance must only include members that are known from the template definition.|✔️|
|tck-id-payloads-template-instance-members-birth|Template instance in NBIRTH or DBIRTH must include all the members from the template definition.|✔️|
|tck-id-payloads-template-instance-members-data|Template instance in NDATA or DDATE may include a subset of members of the template definition.|✔️|
|tck-id-payloads-template-instance-parameters|Template instance may include parameter value for any parameter defined in the template definition.|✔️|
|tck-id-payloads-template-version|If included, the template version must be a UTF8 string.|✔️|
|tck-id-payloads-template-ref-definition|`template_ref` must be omitted if the template is a definition.|✔️|
|tck-id-payloads-template-ref-instance|`template_ref` must be a UTF8 string representing a template definition id.|✔️|
|tck-id-payloads-template-is-definition|`is_definition` must be included in every template instance and definition.|✔️|
|tck-id-payloads-template-is-definition-definition|`is_definition` must be set to `true` for template definitions.|✔️|
|tck-id-payloads-template-is-definition-instance|`is_definition` must be set to `false` for template instances.|✔️|
|tck-id-payloads-template-parameter-name-required|Template parameter name must be included in any parameter definition.|✔️|
|tck-id-payloads-template-parameter-name-type|Template parameter name must be a UTF8 string.|✔️|
|tck-id-payloads-template-parameter-value-type|Template parameter type must be uint32.|✔️|
|tck-id-payloads-template-parameter-type-value|Template parameter type must be one of the data type enumeration.|✔️|
|ck-id-payloads-template-parameter-type-req|Template parameter type must be included in the NBIRTH and DBIRTH messages.|✔️|
|tck-id-payloads-template-parameter-value|Template parameter value must be of type uint32, uint64, float, double, bool, or string.|✔️|
|tck-id-payloads-nbirth-timestamp|NBIRTH must include a sent at timestamp.|✔️|
|tck-id-payloads-nbirth-edge-node-descriptor|Edge node id must be unique.|Depends on user, can't be checked. ⚠️|
|tck-id-payloads-nbirth-seq|NBIRTH must include a sequence number between 0 and 255.|✔️|
|tck-id-payloads-nbirth-bdseq|NBIRTH must include a BDSEQ metric.|✔️|
|tck-id-payloads-nbirth-bdseq-repeat|BDSEQ metric must match the one in the packet's will payload.|✔️|
|tck-id-payloads-nbirth-rebirth-req|NBIRTH message must include a metric with the name `Node Control/Rebirth` of type boolean and value `false`.|✔️|
|tck-id-payloads-nbirth-qos|NBIRTH must be published with QoS = `0`.|✔️|
|tck-id-payloads-nbirth-retain|NBIRTH must be published with retain flag = `false`.|✔️|
|tck-id-payloads-dbirth-timestamp|DBIRTH must include a sent at timestamp.|✔️|
|tck-id-payloads-dbirth-seq|DBIRTH must include a sequence number|✔️|
|tck-id-payloads-dbirth-seq-inc|Sequence number in DBIRTH must increment between 0 and 255.|✔️|
|tck-id-payloads-dbirth-order|DBIRTH must be sent immediately after the NBIRTH and before any NDATA or DDATA message (per edge node).|✔️|
|tck-id-payloads-dbirth-qos|DBIRTH must be published with QoS = `0`.|✔️|
|tck-id-payloads-dbirth-retain|DBIRTH must be published with retain flag = `false`.|✔️|
|tck-id-payloads-ndata-timestamp|NDATA must include a sent at timestamp.|✔️|
|tck-id-payloads-ndata-seq|NDATA must include a sequence number|✔️|
|tck-id-payloads-ndata-seq-inc|Sequence number in NDATA must increment between 0 and 255.|✔️|
|tck-id-payloads-ndata-order|NDATA must only be sent after all NBIRTH and DBIRTH messages are sent (per edge node).|✔️|
|tck-id-payloads-ndata-qos|NDATA must be published with QoS = `0`.|✔️|
|tck-id-payloads-ndata-retain|NDATA must be published with retain flag = `false`.|✔️|
|tck-id-payloads-ddata-timestamp|DDATA must include a sent at timestamp.|✔️|
|tck-id-payloads-ddata-seq|DDATA must include a sequence number|✔️|
|tck-id-payloads-ddata-seq-inc|Sequence number in DDATA must increment between 0 and 255.|✔️|
|tck-id-payloads-ddata-order|DDATA must only be sent after all NBIRTH and DBIRTH messages are sent (per edge node).|✔️|
|tck-id-payloads-ddata-qos|DDATA must be published with QoS = `0`.|✔️|
|tck-id-payloads-ddata-retain|DDATA must be published with retain flag = `false`.|✔️|
|tck-id-payloads-ncmd-timestamp|NCMD must include a sent at timestamp.|✔️|
|tck-id-payloads-ncmd-seq|NCMD must include a sequence number.|✔️|
|tck-id-payloads-ncmd-qos|NCMD must be published with QoS = `0`.|✔️|
|tck-id-payloads-ncmd-retain|NCMD must be published with retain flag = `false`.|✔️|
|tck-id-payloads-dcmd-timestamp|DCMD must include a sent at timestamp.|✔️|
|tck-id-payloads-dcmd-seq|DCMD must include a sequence number.|✔️|
|tck-id-payloads-dcmd-qos|DCMD must be published with QoS = `0`.|✔️|
|tck-id-payloads-dcmd-retain|DCMD must be published with retain flag = `false`.|✔️|
|tck-id-payloads-ndeath-seq|NDEATH mustn't include a sequence number.|✔️|
|tck-id-payloads-ndeath-will-message|NDEATH must be will message in the MQTT connect packet.|✔️|
|tck-id-payloads-ndeath-will-message-qos|NDEATH must be published with QoS = `1`.|✔️|
|tck-id-payloads-ndeath-will-message-retain|NDEATH must be published with retain flag = `false`.|✔️|
|tck-id-payloads-ndeath-bdseq|NDEATH must include the same BDSEQ metric as the MQTT connect packet.|✔️|
|tck-id-payloads-ndeath-will-message-publisher|NDEATH should be published before disconnecting the edge node.|✔️|
|tck-id-payloads-ndeath-will-message-publisher-disconnect-mqtt311|If MQTTv3.1.1 is used, the edge node must publish NDEATH before the MQTT disconnect packet.|✔️|
|tck-id-payloads-ndeath-will-message-publisher-disconnect-mqtt50|If MQTTv5.0 is used, the edge node must publish the NDEATH must be sent in the `Disconnect with Will Message` reason code.|✔️|
|tck-id-payloads-ddeath-timestamp|DDEATH must include a sent at timestamp.|✔️|
|tck-id-payloads-ddeath-seq|DDEATH must include a sequence number.|✔️|
|tck-id-payloads-ddeath-seq-inc|Sequence number in DDEATH must increment between 0 and 255.|✔️|
|tck-id-payloads-ddeath-seq-number|Sequence number in DDEATH must be set to ensure order of the sessions.|✔️|
|tck-id-payloads-state-will-message|Death STATE message must be published under the topic of `spBv1.0/STATE/[sparkplug_host_id]`.|✔️|
|tck-id-payloads-state-will-message-qos|Death STATE message must be published with QoS = `1`.|✔️|
|tck-id-payloads-state-will-message-retain|Death STATE message must be published with retain flag = `true`.|✔️|
|tck-id-payloads-state-will-message-payload|Death STATE must be UTF8 JSON with `online:false` and `timestamp:124335` (UTC time in milliseconds since Epoch).|✔️|
|tck-id-payloads-state-subscribe|After connecting, the host application must subscribe to `spBv1.0/STATE/[sparkplug_host_id]`.|✔️|
|tck-id-payloads-state-birth|After subscribung, the host application must publish a STATE message on `spBv1.0/STATE/[sparkplug_host_id]` with QoS = `1` and the retain flag = `true`.|✔️|
|tck-id-payloads-state-birth-payload|Birth STATE must be UTF8 JSON with `online:true` and `timestamp:124335` (UTC time in milliseconds since Epoch).|✔️|
|tck-id-conformance-primary-host|The host applications must publish STATE messages on birth and death.|✔️|
|tck-id-conformance-mqtt-qos0|Sparkplug conformant MQTT servers must support publish and subscribe on QoS = `0`.|Not in the scope of this (client) library. ⚠️|
|tck-id-conformance-mqtt-qos1|Sparkplug conformant MQTT servers must support publish and subscribe on QoS = `1`.|Not in the scope of this (client) library. ⚠️|
|tck-id-conformance-mqtt-will-messages|Sparkplug conformant MQTT servers must support all aspects of will messages including use of the retain flag and QoS = `1`.|Not in the scope of this (client) library. ⚠️|
|tck-id-conformance-mqtt-retained|Sparkplug conformant MQTT servers must support all aspects of the retain flag.|Not in the scope of this (client) library. ⚠️|
|ck-id-conformance-mqtt-aware-basic|Sparkplug aware MQTT servers must support all aspects of a Sparkplug Compliant MQTT server.|Not in the scope of this (client) library. ⚠️|
|tck-id-conformance-mqtt-aware-store|Sparkplug aware MQTT servers must store NBIRTH and DBIRTH messages as they pass through the MQTT server.|Not in the scope of this (client) library. ⚠️|
|tck-id-conformance-mqtt-aware-nbirth-mqtt-topic|Sparkplug aware MQTT servers must make NBIRTH messages available on a topic of the form `namespace/group_id/NBIRTH/edge_node_id`.|Not in the scope of this (client) library. ⚠️|
|tck-id-conformance-mqtt-aware-nbirth-mqtt-retain|Sparkplug aware MQTT servers must make NBIRTH messages available on a topic of the form `namespace/group_id/NBIRTH/edge_node_id` with the MQTT retain flag set to `true`.|Not in the scope of this (client) library. ⚠️|
|tck-id-conformance-mqtt-aware-dbirth-mqtt-topic|Sparkplug aware MQTT servers must make DBIRTH messages available on a topic of the form `namespace/group_id/DBIRTH/edge_node_id/device_id`.|Not in the scope of this (client) library. ⚠️|
|tck-id-conformance-mqtt-aware-dbirth-mqtt-retain|Sparkplug aware MQTT servers must make DBIRTH messages available on a topic of the form `namespace/group_id/DBIRTH/edge_node_id/device_id` with the MQTT `retain flag` set to `true`.|Not in the scope of this (client) library. ⚠️|
|tck-id-conformance-mqtt-aware-ndeath-timestamp|Sparkplug aware MQTT servers may replace the timestmap of NDEATH messages. If it does, it must set the timestamp to the UTC time at which it attempts to deliver the NDEATH to subscribed clients.|Not in the scope of this (client) library. ⚠️|






* NBIRTH must include all templates the edge node will report. --> Template storage?!
* NBIRTH message must include a metric with the name `Node Control/Rebirth` of type boolean and value `false`.
* NBirth kann auch andere Properties und Node Controls enthalten (Siehe seite 22 respektive 29)
* DBIRTH must include all metrics the device will report. --> Device metric store checks?!
* DBIRTH kann auch andere Properties und Node Controls enthalten (Siehe seite 25 respektive 32)
* Host application must subscribe to the messages for all edge nodes and devices. --> On success: Host application must publish a state message after the subscription.
* Birth STATE timestamp must be the same as in the will message.
* Host application must set all metrics to stale if the connection is lost.
* If host application gets a STATE message with `online:false` on their own identifier, it must immediately publish a new birth STATE message.
* Ablaufdiagramme nochmal checken
* BDSEQ (int64) metric of edge node death message must be the incremented by one in respect of the connect message. (Values must be between 0 and 255).
* if ocnfigureds to wait for the primary host application, The edge node must validate the primary host application id. --> The edge node must validate the primary host application online state. --> The edge node must validate the primary host application timestamp to be the latest of the STATE messages. --> If a STATE message with greater timestamp and online = `false` is recevied, NDEATH needs to be sent.
* Host application must mark node as offline after NDEATH is received (And set the timestamp).
* Host application must mark node metrics as stale after NDEATH is received (And set the timestamp).
* Host application must mark node related devices as offline after NDEATH is received (And set the timestamp).
* Host application must mark node related device metrics as stale after NDEATH is received (And set the timestamp).
* If edge node is configured to wait for primary application, it must validate the timestamp and offline STATE messages and disconnect on offline STATE messages.
* The edge node must reconnect to the MQTT server immediately after disconnect.
* If edge node receives an offline STATE message with invalid timestamp (older than the one from the last STATE message), it must ignore it and not disconnect.
* If the device allows writing to outputs, it must subscribe to topics in the form of `spBv1.0/group_id/DCMD/edge_node_id/device_id`.
* NBIRTH Must be sent prior to DBIRTH message.
* DBIRTH topic must match the NBIRTH topic that was sent before. --> How to validate?
* DBIRTH must include a sequence number between 0 and 255 and incremented according to the prior edge node message sent.
* If edge node loses connection to the device, it must publish a DDEATH on behalf of a device.
* Host application must mark device as offline after DDEATH is received (And set the timestamp).
* Host application must mark device metrics as stale after DDEATH is received (And set the timestamp).
* Allow multiple MQTT servers for all instances?
* Host applications should provice a reorder timeout parameter.
* If host application is configured to us a reorder timeout, it must start a timer to start the reorder timeout window if an out-of-order message arrives
* If reorder timeout on host application expires, the host application must send a NCMD message with a `Node Control/Rebirth` request of type boolean and value `true`.
* If the missing message is recevied before the timer window elapses, the timer must be stopped and normal operation must be proceeded.
* In multiple server scenarios, the host application must send a STATE message and all edge nodes must subscribe to the STATE message if configured to use a primary application.
* Host application always must send the STATE message as first message on all servers.
* Edge nodes must only be connected to one server at once.
* If the primary application gets offline, the edge node must terminate its connection and move on to the next MQTT server.
* Edge node must wait for STATE message on the new server to publish its NBIRTH message. --> Specify wait timeout
* Host application must maintain a STATE message timestamp on a per MQTT server basis.
* Option to manually disconnect an application?!
* NBIRTH metric value must be set or is_null must be set to `true` and no value must be set. --> FOr all metrics!
* All metrics with `is_historical:false`, NBIRTH and NDATA must keep data in chronological order.
* Disallow metrics with same name and timestamp (Timestamps must be unique for each metric)
* For all metrics with `is_historical:false`, DBIRTH and DDATA must keep data in chronological order
* NBIRTH must not include an alias for the `Node Control/Rebirth` metric.
* Host application rebirth message for edge nodes must use NCMD with `Node Control/Rebirth` metric and value `true`.
* When edge node recevied a rebirth request, it mus timediately stop sending NDATA messages. --> After edge node stops sending NDATA requests, it must send a NBIRTH and DBIRTH messages. --> NBIRTH must include the same BDSEQ metric as in the previous MQTT connect packet.
* NCMD and DCMD should include a known metric name.
* NCMD and DCMD must include a compatible metric value according to the known metrics.
* Protobuf payload latest version checken?!
* Payload timestamps must be in UTC in the form of an unsigned 64-bit integer representing the number of milliseconds since epoch (Jan 1, 1970)
* All edge node messages except NDEATH must include a sequence number.
* NBIRTH must include a sequence number between 0 and 255.
* All subsequent node messages must have incrementing sequence numbers.
* Metric names must be set unless aliasses are used.
* If supplied, aliasses must be unique accross the edge nodes metrics.
* NBIRTH and DBIRTH must include metric names and alias.
* NDATA, DDATA, NCMD, and DCMD must include only an alias and not metric name.
* Timestamp must be included in all NBIRTH, DBIRTH, NDATA, and DDATA messages (Metrics).
* Timestamp may be included in all metrics with NCMD and DCMD messages.
* Data type must be uint32.
* Deprecate version A of Sparkplug.
* Data type must be a known value.
* Data type must be included in every metric definition in NBIRTH and DBIRTH messages.
* Data type shouldn't be included in every metric in NDATA, NCMD, DDATA and DCMD messages.
* PropertySet must include as many keys as values.
* PropertyValue must be uint32.
* PropertyValue must be a known value.
* PropertyValue type must be included in NBIRTH and DBIRTH messages.
* Quality value type must be int32.
* Quality value must be 0 (BAD), 192 (GOOD) or 500 (STALE).
* DataSet num_of_columns must be uint64 and equal the number of columns.
* DataSet columns must be the same size as the types array.
* DataSet types must be array of uint32 representing the data types.
* DataSet types must be known values.
* DataSet types array must be included in all data sets.
* DataSet.DataSetValue value must be of type uint32, uint64, float, double, bool, or string.
* Template definitions must be only included in NBIRTH messages.
* Template definition must have `is_definition` set to `true`.
* Template definition must omit the `template_ref` field.
* Template defintion must include all member metrics that will be included in the template.
* Template defintion must be included in the NBIRTH for all template instances that are included in the NBIRTH or DBIRTH messages.
* Template defintion must include all parameters that will be included in the template.
* Template defintion may include parameters (They act as default if not set in the template).
* Template instance must have `is_definition` set to `false`.
* A template must have the reference to the type of the template set.
* A template instance must only include members that are known from the template definition.
* Template instance in NBIRTH or DBIRTH must include all the members from the template definition.
* Template instance in NDATA or DDATE may include a subset of members of the template definition.
* Template instance may include parameter value for any parameter defined in the template definition.
* If included, the template version must be a UTF8 string.
* `template_ref` must be omitted if the template is a definition.
* `template_ref` must be a UTF8 string representing a template definition id.
* `is_definition` must be included in every template instance and definition.
* `is_definition` must be set to `true` for template definitions.
* `is_definition` must be set to `false` for template instances.
* The parameter name must be included in any parameter definition in the templates.
* Template parameter name must be a UTF8 string.
* Template parameter type must be uint32 and one of the type enum.
* Template parameter type must be included in the NBIRTH and DBIRTH messages
* Template parameter value must be of type uint32, uint64, float, double, bool, or string.
* Todo: Check data types and ProtoBuf data / models.
* DBIRTH must be sent immediately after the NBIRTH and before any NDATA or DDATA messages.
* If MQTTv5.0 is used, the edge node must publish the NDEATH must be sent in the `Disconnect with Will Message` reason code