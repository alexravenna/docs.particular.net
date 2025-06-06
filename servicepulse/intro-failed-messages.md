---
title: Failed Message Monitoring
summary: Describes how ServicePulse detects and monitors failed messages.
component: ServicePulse
reviewed: 2025-05-19
related:
- servicepulse/intro-failed-message-retries
- servicepulse/message-details
---

When an NServiceBus endpoint fails to process a message, it performs a set of configurable attempts to recover from message failure. These attempts are referred to as "immediate retries" and "delayed retries" and in many cases allow the endpoint to overcome intermittent communication failures. See [recoverability](/nservicebus/recoverability/) for more details.

If the automatic retry attempts also fail, the endpoint forwards the failed message to the error queue defined for all endpoints in the system. See [Auditing with NServiceBus](/nservicebus/operations/auditing.md) for more details.

ServicePulse (via ServiceControl) monitors the error queue and displays the current status and details of failed messages as an indicator in the ServicePulse dashboard.

![Failed Messages indicator](images/indicators-failed-message.png 'width=500')

In addition, ServicePulse provides a Failed Messages page to assist in examining failed messages and taking specific actions on them.

## Failed Messages page

Both the "Failed Messages" indicator in the Dashboard and the "Failed Messages" link in the navigation bar link to the Failed Messages screen. This page is split into various tabs.

### Failed message groups tab

The first tab in the Failed Messages page shows error groups. A group is a set of failed messages grouped according to criteria such as the same exception type.

This tab shows two lists, described below.

#### Last 10 completed retry requests list

This list is collapsed by default and shows information about the last ten completed group retry requests.

![Last 10 completed retry requests list](images/last-completed-group-retries.png 'width=500')

A completed retry request represents a completed operation where messages from a given group were sent to the corresponding queue for processing. This means those messages may not have been processed yet. [Learn more about retrying failed messages](/servicepulse/intro-failed-message-retries.md).

#### Failed groups list

This list shows all groups of currently failed messages.

![Failed Message Groups list](images/failed-message-groups.png 'width=500')

The display of failed message groups can be changed via the "Group by" drop-down menu, according to the following classification types:

 * **Exception Type and Stack Trace** – Groups messages by both exception type and stack trace. This is the default classification method.
 * **Message Type** – Groups messages by message type.
 * **Endpoint Address** – Groups messages by endpoint address where the failure occurred.
 * **Endpoint Instance** – Groups messages by endpoint instance identifier where the failure occurred.
 * **Endpoint Name** – Groups messages by name of the endpoint where the failure occurred.

> [!NOTE]
> The number of listed groups may vary depending on the selected classification type.

##### Managing failed message groups

The following actions can be performed on a failed message group:

 * **View messages** – Shows all individual messages contained in the group.
 * **Request retry** – Sends all failed messages to the corresponding queue to attempt processing again. When a failed group retry request is initiated, ServicePulse will present the progress of the operation.

![Failed message groups retry in progress](images/failed-group-retry-in-progress.png 'width=500')

 * **Delete group** – Deletes all messages in the selected group from the error queue. [Learn more about deleting messages](/servicepulse/intro-archived-messages.md).
 * **Add note** – Allows adding a freetext note for the group. Notes are automatically removed after the group is retried.

![Failed message groups note](images/notes.png 'width=500')

### Listing messages

Individual failed messages can be viewed in one of the following two ways:

- **Inside a failed message group** – In the "Failed Messages Group" tab, click the "View messages" link from a failed message group entry.
- **All messages without any grouping** – Via the "All messages" tab.

![Failed Messages Page](images/intro-failed-messages-failed-messages-page.png 'width=500')

Both of these message list views allow for taking actions on an individual message, on custom message selections, or on all messages contained in the view.

> [!NOTE]
> Retrying a small number of individual messages can be useful for testing system fixes before deciding to retry several messages in a group. This is because retrying several messages can take a long time and delay other ServiceControl operations.

The following actions can also be taken on each message or a selection of messages:

* **Retry** – Sends the message(s) to be reprocessed by the corresponding endpoint.
* **Delete** – Deletes message(s).
* **Export** – Export message(s) to a downloadable CSV file.

### Message details page

The failed [message details](message-details.md) page is shared with [All Messages](all-messages.md).

#### Sharing message data from ServicePulse

The URL from that message's page can be copied to share the details of a specific message from ServicePulse.

## Deleted Messages

### Deleted Message Groups

This list shows all groups of deleted messages.

![Deleted Message Groups Tab](images/archivegroups.png 'width=500')

The display of deleted message groups can be changed via the "Group by" drop-down menu, according to the following classification types:

 * **Exception Type and Stack Trace** – Groups messages by both exception type and stack trace. This is the default classification method.
 * **Message Type** – Groups messages by message type.
 * **Endpoint Address** – Groups messages by endpoint address where the failure occurred.
 * **Endpoint Instance** – Groups messages by endpoint instance identifier where the failure occurred.
 * **Endpoint Name** – Groups messages by name of the endpoint where the failure occurred.

> [!NOTE]
> The number of listed groups may vary depending on the selected classification type.

### Deleted Messages

Failed messages that cannot be processed successfully (or could not be retried due to various application-specific reasons) can be deleted and later viewed in the Deleted Messages tab.

![Deleted Messages Tab](images/archive.png 'width=500')

Learn more about [deleting messages in ServicePulse](/servicepulse/intro-archived-messages.md).
