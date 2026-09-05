# Macro automation

## Outcome

A player can create profile-scoped keyboard and mouse action sequences, assign non-conflicting shortcuts, and have Runner safely control recording and cancellable playback in an unprivileged Macro worker.

## Current verified status

**Status:** Partial

Code inspection on 2026-09-05 confirms Rust domain types for actions, shortcuts, repeat modes, validation, and profile serialization, plus a standalone worker containing global-hotkey and input-simulation code. The WPF preview supports profile selection, case-insensitive search, create, duplicate, delete, and add/remove-step behavior with unit tests.

The WPF editor is memory-only and does not capture shortcuts or recording. The worker currently exposes a private Bincode pipe directly to Settings, does not sit behind Runner, and its hotkey-registration refresh condition is a permanent placeholder, so received configurations are not registered for playback. `UntilKeyPressed` currently executes once, and cancellation, safe input release, acknowledgements, and deterministic execution tests are absent.

## Architecture dependencies

- [Component boundaries](../architecture.md#component-boundaries)
- [State ownership and persistence](../architecture.md#state-ownership-and-persistence)
- [IPC and protocol boundary](../architecture.md#ipc-and-protocol-boundary)
- [Failure and recovery](../architecture.md#failure-and-recovery)

## Feature-specific implications

### Component boundaries

Settings edits macro definitions but never registers global hooks or sends synthetic input. Runner owns the worker process and active-profile configuration; recording and playback remain in the unprivileged Macro worker.

### State ownership and persistence

Macro definitions, enabled state, action order, shortcut, and repeat configuration are profile-scoped Runner state. Worker execution state and in-progress recording are ephemeral and must not be restored as side effects at startup.

### IPC and protocol boundary

WPF sends edit, record, test, and cancellation intents to Runner. Runner validates the active profile and coordinates worker configuration, acknowledgements, progress, errors, disconnects, and shutdown through the shared versioned contract.

### Failure and recovery

Malformed actions, shortcut conflicts, worker failure, or cancellation must fail closed. Any keys or buttons held by a partial macro must be released, and startup restoration must never replay a macro.

## Related blueprints

### Required

- [Profile persistence](profile-persistence.md) — owns durable macro definitions and active-profile restoration.
- [IPC contracts](ipc-contracts.md) — defines WPF/Runner and Runner/worker commands, events, framing, and correlation.

### Impact checks

- [Settings client](settings-client.md) — owns macro editor presentation, navigation, accessibility, and disabled states.

## Relevant implementation and tests

- `crates/core/src/macro_config.rs` — Rust action, shortcut, repeat, validation, and configuration types with unit tests.
- `crates/core/src/input_recorder.rs` — transitional Windows keyboard recording implementation.
- `crates/core/src/gui/macro_editor.rs` — transitional Iced macro editor.
- `crates/macro/src` — standalone worker, hotkey listener, input hooks/senders, executor, and private IPC implementation.
- `apps/EdgeOptimizer.Settings.Wpf/ViewModels/MacrosViewModel.cs` — memory-only macro collection and sequence editing.
- `tests/EdgeOptimizer.Settings.Wpf.Tests/MacrosViewModelTests.cs` — filtering, selection, CRUD, duplication, and step mutation tests.

## Acceptance criteria

- [x] Represent keyboard, mouse, delay, shortcut, enablement, and repeat data in the Rust profile model.
- [x] Validate non-empty macro names, action presence, basic shortcut shape, and case-insensitive name uniqueness.
- [x] Keep WPF macro selection valid across create, duplicate, and delete preview operations.
- [x] Edit only the selected profile's preview collection.
- [ ] Make Runner the sole owner of Macro worker startup, configuration, cancellation, and shutdown.
- [ ] Generate shared commands/events for configuration, recording, playback, cancellation, acknowledgement, errors, and worker disconnects.
- [ ] Register and unregister shortcuts when the active configuration changes.
- [ ] Reject shortcut conflicts and invalid repeat counts, stop keys, delays, coordinates, and unsupported keys.
- [ ] Implement true finite-repeat, until-key, and cancellation behavior with guaranteed key/button release.
- [ ] Prevent editing transitions that conflict with recording or playback.
- [ ] Test playback planning with fake clocks and input senders; never inject real input in hosted tests.
- [ ] Verify hooks, focus interactions, and input cleanup only in an isolated Windows integration environment.

## Remaining gaps

Runner ownership, generated IPC, working hotkey refresh, WPF recording and shortcut capture, cancellation, complete repeat semantics, conflict validation, safe input cleanup, and deterministic worker tests remain planned. The current direct Settings-to-worker pipe conflicts with the target component boundary and is transitional.
