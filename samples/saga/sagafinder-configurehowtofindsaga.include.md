> [!NOTE]
> In this sample the implementation of the `ConfigureHowToFindSaga` method is empty since a saga finder is provided for each message type handled by the saga. It is not required to provide a saga finder for every message type. A mix of standard saga mappings using `ConfigureHowToFindSaga`, and saga finders, is a valid scenario.
