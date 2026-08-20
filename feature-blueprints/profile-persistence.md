# Profile persistence and startup restoration

## Outcome

Profiles and the last active profile survive restart in Runner-owned SQLite state and are delivered to Settings on connection without replaying optimization side effects.

## Current verified status

**Status:** Partial

SQLite schema v2, the v1→v2 migration, legacy import, transactional profile/activation updates, previous-valid backup, Runner startup restoration, and Settings state snapshots are implemented. Rust tests were added but cannot be executed in the current environment because Cargo is unavailable.

## Architecture dependencies

- [State ownership and persistence](../architecture.md#state-ownership-and-persistence)
- [Failure and recovery](../architecture.md#failure-and-recovery)

## Feature-specific implications

Runner is the only process that opens `state.db`. The active WPF Settings client currently has no persistence transport; once IPC is implemented, Runner's snapshot is authoritative and every UI mutation is forwarded to Runner.

## Related blueprints

### Required

- [IPC contracts](ipc-contracts.md) — Settings needs a state request/snapshot exchange.

### Impact checks

- [Settings client](settings-client.md) — WPF must hydrate from Runner.

## Relevant implementation and tests

- `crates/core/src/state_store.rs` — schema, transactions, migration input, and unit tests.
- `crates/runner/src/main.rs` — ownership, startup load, and IPC persistence.
- `apps/EdgeOptimizer.Settings.Wpf` — WPF hydration is pending Runner IPC.

## Acceptance criteria

- [x] Restarting Runner restores the last valid active-profile identity.
- [x] Startup restoration does not terminate processes or re-run optimization.
- [x] Deleting an active profile clears activation transactionally.
- [x] An unknown profile cannot become active.
- [ ] Copy crosshair assets into managed storage.
- [x] Back up the previous valid database before state mutations.
- [ ] Add automated recovery from the previous-valid backup.
- [ ] Add versioned portable JSON import/export.
- [ ] Remove legacy direct JSON writes after the WPF client uses Runner commands.

## Remaining gaps

Windows integration and migration tests have not run locally. Asset management, automated backup recovery, and portable import/export remain planned.
