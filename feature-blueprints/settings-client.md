# Settings client

## Outcome

An on-demand, unprivileged WPF client edits profiles and presents Runner state while consuming no memory when closed.

## Current verified status

**Status:** Partial

The current production-capable UI is Rust/Iced. The WPF application now has a profile-scoped presentation shell, testable view-models, and preview implementations of Dashboard, Crosshair, Macros, and System Tweaks. Its editable state is intentionally memory-only, and orchestration actions remain disabled because generated IPC bindings and the Runner client are not implemented. A Windows CI job builds the client and runs logic plus non-interactive STA smoke tests.

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
- [Crosshair overlay](crosshair-overlay.md) — WPF owns preview presentation but not overlay lifecycle or assets.
- [Macro automation](macro-automation.md) — WPF owns editing presentation but not hooks or input execution.
- [System Tweaks](system-tweaks.md) — WPF presents choices without performing cleanup, termination, or machine changes.

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

Runner hydration, generated protocol bindings, IPC transport, durable commands, and interactive Windows UI automation remain planned. GitHub Actions is the build/test authority because the local .NET 10 SDK is unavailable.
