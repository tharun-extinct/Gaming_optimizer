# Feature Blueprint Manifest

## Loading protocol

1. Match work by intent and synonyms.
2. Load the primary blueprint and only its linked architecture sections.
3. Load impact checks only when the change can affect them.
4. For shared contract changes, load the complete architecture and all listed consumers.
5. Inspect code and tests before changing a status claim.

## Router

| Task concepts | Primary blueprint | Architecture sections | Impact checks | Principal code and tests |
|---|---|---|---|---|
| profile, active profile, startup restore, SQLite, migration, import/export | [profile persistence](profile-persistence.md) | [State ownership and persistence](../architecture.md#state-ownership-and-persistence), [Failure and recovery](../architecture.md#failure-and-recovery) | [settings client](settings-client.md), [IPC contracts](ipc-contracts.md) | `crates/core/src/state_store.rs`, `crates/runner/src/main.rs`, `crates/core/src/gui/mod.rs` |
| WPF, C#, UI, Settings | [settings client](settings-client.md) | [Component boundaries](../architecture.md#component-boundaries), [IPC and protocol boundary](../architecture.md#ipc-and-protocol-boundary) | [profile persistence](profile-persistence.md), [IPC contracts](ipc-contracts.md) | `apps/EdgeOptimizer.Settings.Wpf`, `crates/core/src/gui` |
| named pipe, Protobuf, Bincode, authentication, framing | [IPC contracts](ipc-contracts.md) | [IPC and protocol boundary](../architecture.md#ipc-and-protocol-boundary), [Privilege and identity](../architecture.md#privilege-and-identity) | [settings client](settings-client.md), [privileged broker](privileged-broker.md) | `crates/core/src/ipc.rs`, `engine_ipc.rs`, `orchestration.rs` |
| service, EngineSvc, SYSTEM, cleanup, elevation | [privileged broker](privileged-broker.md) | [Privilege and identity](../architecture.md#privilege-and-identity), [Failure and recovery](../architecture.md#failure-and-recovery) | [IPC contracts](ipc-contracts.md), [process safety](process-safety.md) | `crates/engine_service`, `scripts/*engine-service*`, `crates/core/src/engine_ipc.rs` |
| protected process, kill, termination safety | [process safety](process-safety.md) | [Process safety](../architecture.md#process-safety), [Privilege and identity](../architecture.md#privilege-and-identity) | [privileged broker](privileged-broker.md) | `crates/core/src/process.rs` |

## Status vocabulary

- `Implemented`: verified in current code and tests.
- `Partial`: some acceptance criteria are implemented.
- `Planned`: required behavior is not implemented.
- `Deprecated`: retained only for migration or compatibility.
- `Unknown`: evidence is insufficient.
