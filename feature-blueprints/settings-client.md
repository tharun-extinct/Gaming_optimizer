# Settings client

## Outcome

An on-demand, unprivileged WPF client edits profiles and presents Runner state while consuming no memory when closed.

## Current verified status

**Status:** Partial

The current functional UI is Rust/Iced. The WPF application is a static placeholder; its .NET target migration is staged, but it has no generated IPC client or feature UI.

## Architecture dependencies

- [Component boundaries](../architecture.md#component-boundaries)
- [IPC and protocol boundary](../architecture.md#ipc-and-protocol-boundary)

## Feature-specific implications

WPF never owns durable state or privileged operations. It requests a snapshot from Runner, submits validated commands, and exits completely when closed.

## Related blueprints

### Required

- [IPC contracts](ipc-contracts.md) — generated C# types are required before replacing Iced.

### Impact checks

- [Profile persistence](profile-persistence.md) — profile CRUD and startup hydration must remain Runner-owned.

## Relevant implementation and tests

- `apps/EdgeOptimizer.Settings.Wpf` — future presentation client.
- `crates/core/src/gui` — transitional Iced client.

## Acceptance criteria

- [ ] Build on .NET 10 LTS.
- [ ] Hydrate profiles and active state from Runner.
- [ ] Perform profile CRUD through versioned IPC.
- [ ] Exit fully when the window closes.
- [ ] Include WPF build and contract tests in CI.

## Remaining gaps

All functional WPF screens, generated protocol bindings, IPC transport, and CI coverage remain planned.
