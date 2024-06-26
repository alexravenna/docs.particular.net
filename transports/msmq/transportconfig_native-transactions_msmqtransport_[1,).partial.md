### Native transactions

In versions 1.0 and above, the MSMQ transport uses native transactions to support the `ReceiveOnly` and `SendsAtomicWithReceive` levels. With `SendsAtomicWithReceive` the native transaction for receiving messages is shared with sending operations. That means the message receive operation, and any send or publish operations, are committed atomically. When using `ReceiveOnly`, the transaction is not shared with sending operations and dispatched messages may not be rolled back in case the receiving transaction needs to abort.

> [!WARNING]
> The MSMQ transport in NServiceBus version 6.0 does not distinguish between `ReceiveOnly` and `SendsAtomicWithReceive` and both levels behave as `SendsAtomicWithReceive`.