> [!NOTE]
> Using the sender-side distribution feature in combination with the distributor (deprecated) will affect the routing of [delayed retries](/nservicebus/recoverability/configure-delayed-retries.md). These messages will be routed directly to the distributor instead of the endpoint instance even though these messages were originally sent to the endpoint instance.
