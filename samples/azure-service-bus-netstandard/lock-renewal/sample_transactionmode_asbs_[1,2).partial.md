1. The transport must use the default [`SendsAtomicWithReceive`](/transports/transactions.md#transactions-transport-transaction-sends-atomic-with-receive) transaction mode for the sample to work.
1. When using lock renewal with the [outbox feature](/nservicebus/outbox/), the transport transaction mode has to be **explicitly** set to [`SendsAtomicWithReceive`](/transports/transactions.md#transactions-transport-transaction-sends-atomic-with-receive) in the endpoint configuration code.