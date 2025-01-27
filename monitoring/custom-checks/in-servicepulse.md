---
title: Managing custom checks in ServicePulse
summary: Describes how ServicePulse monitors custom check activity
reviewed: 2024-11-06
redirects:
  - servicepulse/intro-endpoints-custom-checks
---

ServicePulse monitors the health and activity of an NServiceBus endpoint using [Heartbeats](/monitoring/heartbeats/) and [Custom Checks](/monitoring/custom-checks/).

The main dashboard shows a custom checks icon, which will indicate if there are any failing custom checks.

![Custom checks dashboard notification showing a failing custom check](custom-checks-dashboard-notification.png)

Click this icon to go to the custom checks details page. This page shows a list of all custom checks and their current status.

![Custom checks details page](custom-checks-details.png)

Each custom check includes information about the endpoint instance that reported the status and how long ago the status was updated.

## Muting custom checks

When a custom check fails, it will continue to make the main Custom Checks badge on the dashboard red until the custom check reports success.

Sometimes a custom check reports an error that is easily solved. The status of the custom check will not be updated in ServicePulse until the custom check is executed again.

If it is a one-off custom check, the endpoint hosting the check will need to be restarted to execute it again. If it is a periodic custom check, it will be automatically executed again after the scheduled period.

Rather than waiting for the failing custom check to be executed again to update its status, it can be muted. Muted custom checks are removed from ServicePulse and will not contribute to the main custom checks dashboard badge.

Whenever a muted custom check is executed and reports its status to ServiceControl, it is automatically un-muted.
