# Settings client

## Outcome

An on-demand, unprivileged WPF client edits profiles and presents Runner state while consuming no memory when closed.

## Current verified status

**Status:** Partial

The current production-capable UI is Rust/Iced. The WPF application now has a profile-scoped presentation shell and preview implementations of Dashboard, Crosshair, Macros, and System Tweaks. Its editable state is intentionally memory-only, and orchestration actions remain disabled because generated IPC bindings and the Runner client are not implemented.

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

Runner hydration, generated protocol bindings, IPC transport, durable commands, CI coverage, and Windows UI automation remain planned. A local .NET 10 SDK build is still required to verify the new presentation screens.
