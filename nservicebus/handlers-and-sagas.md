---
title: Handlers and Sagas
summary: A brief introduction to handlers and sagas
component: Core
reviewed: 2025-02-19
related:
 - nservicebus/handlers
 - nservicebus/sagas
 - samples/saga
---

There are two standard ways of executing code when a message is processed: [handlers](handlers/) and [sagas](sagas/).

Handler instances are instantiated on a per-message basis, executed, and then disposed of. These are sometimes referred to as “stateless handlers”.

Saga instances are also instantiated on a per-message basis, executed, and then disposed of. However they differ from handlers in that, once instantiated, they are passed an instance of a "Data" class. The "Saga Data" is persistent state that is shared between a given saga type based on a key. These are sometimes referred to as “stateful handlers”.

Other concepts that both handlers and sagas share:

 * [Recoverability](/nservicebus/recoverability/) (i.e. what happens when message processing fails)
 * Executed within the same [pipeline](/nservicebus/pipeline)
 * Detected via [assembly scanning](/nservicebus/hosting/assembly-scanning.md)
