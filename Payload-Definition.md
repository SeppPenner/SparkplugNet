# Payload definition

## Application

|Method|Topic|Type|Payload|
|-|-|-|-|
|Publish|`STATE/scada_host_id`|A|`OFFLINE`|
|Publish|`STATE/scada_host_id`|B|`OFFLINE`|
|Publish|`STATE/scada_host_id`|A|`ONLINE`|
|Publish|`STATE/scada_host_id`|B|`ONLINE`|
|Publish|`spAv1.0/group_id/NCMD/edge_node_id`|A|?|
|Publish|`spBv1.0/group_id/NCMD/edge_node_id`|B|?|
|Publish|`spAv1.0/group_id/DCMD/edge_node_id/device_id`|A|?|
|Publish|`spBv1.0/group_id/DCMD/edge_node_id/device_id`|B|?|
|Subscribe|`spAv1.0/#`|A|-|
|Subscribe|`spBv1.0/#`|B|-|

## Node

|Method|Topic|Type|Payload|
|-|-|-|-|
|Publish|`spAv1.0/group_id/NDEATH/edge_node_id`|A|`type: PayloadA`|
|Publish|`spBv1.0/group_id/NDEATH/edge_node_id`|B|`type: PayloadB`|
|Publish|`spAv1.0/group_id/NBIRTH/edge_node_id`|A|`type: PayloadA`|
|Publish|`spBv1.0/group_id/NBIRTH/edge_node_id`|B|`type: PayloadB`|
|Publish|`spAv1.0/group_id/NDATA/edge_node_id`|A|?|
|Publish|`spBv1.0/group_id/NDATA/edge_node_id`|B|?|
|Subscribe|`spAv1.0/group_id/NCMD/edge_node_id`|A|?|
|Subscribe|`spBv1.0/group_id/NCMD/edge_node_id`|B|?|
|Subscribe|`spAv1.0/group_id/DCMD/edge_node_id/#`|A|?|
|Subscribe|`spBv1.0/group_id/DCMD/edge_node_id/#`|B|?|
|Subscribe|`STATE/scada_host_id`|A|`OFFLINE`|
|Subscribe|`STATE/scada_host_id`|B|`OFFLINE`|
|Subscribe|`STATE/scada_host_id`|A|`ONLINE`|
|Subscribe|`STATE/scada_host_id`|B|`ONLINE`|

## Device

|Method|Topic|Type|Payload|
|-|-|-|-|
|Publish|`spAv1.0/group_id/DBIRTH/edge_node_id/device_id`|A|`type: PayloadA`|
|Publish|`spBv1.0/group_id/DBIRTH/edge_node_id/device_id`|B|`type: PayloadB`|
|Publish|`spAv1.0/group_id/DDEATH/edge_node_id/device_id`|A|`type: PayloadA`|
|Publish|`spBv1.0/group_id/DDEATH/edge_node_id/device_id`|B|`type: PayloadB`|
|Publish|`spAv1.0/group_id/DDATA/edge_node_id/device_id`|A|?|
|Publish|`spBv1.0/group_id/DDATA/edge_node_id/device_id`|B|?|
|Subscribe|`spAv1.0/group_id/DCMD/edge_node_id/device_id`|A|?|
|Subscribe|`spBv1.0/group_id/DCMD/edge_node_id/device_id`|B|?|