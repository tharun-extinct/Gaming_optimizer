# Settings client

## Outcome

An on-demand, unprivileged WinUI client edits profiles and presents Runner state while consuming no memory when closed.

## Current verified status

**Status:** Partial

WinUI 3 is the active Settings UI and is launched on demand by Runner from the packaged `EdgeOptimizer.Settings.WinUI.exe`. It has a profile-scoped presentation shell, testable view-models, and preview implementations of Dashboard, Crosshair, Macros, and System Tweaks. Its editable state is intentionally memory-only, and orchestration actions remain disabled because generated IPC bindings and the Runner client are not implemented. A Windows CI job tests the UI-independent logic, compiles WinUI XAML, and publishes a self-contained client artifact. Runtime UI smoke automation remains planned.

## Architecture dependencies

- [Component boundaries](../architecture.md#component-boundaries)
- [IPC and protocol boundary](../architecture.md#ipc-and-protocol-boundary)

## Feature-specific implications

WinUI 3 never owns durable state or privileged operations. It requests a snapshot from Runner, submits validated commands, and exits completely when closed.

## Related blueprints

### Required

- [IPC contracts](ipc-contracts.md) — generated C# types are required before enabling Runner-backed behavior.

### Impact checks

- [Profile persistence](profile-persistence.md) — profile CRUD and startup hydration must remain Runner-owned.
- [Crosshair overlay](crosshair-overlay.md) — WinUI owns preview presentation but not overlay lifecycle or assets.
- [Macro automation](macro-automation.md) — WinUI owns editing presentation but not hooks or input execution.
- [System Tweaks](system-tweaks.md) — WinUI 3 presents choices without performing cleanup, termination, or machine changes.

## Relevant implementation and tests

- `apps/EdgeOptimizer.Settings.Core` — UI-independent models, contracts, and view-model logic.
- `apps/EdgeOptimizer.Settings.WinUI` — active WinUI presentation client.
- `crates/runner/src/main.rs` — launches the packaged WinUI client.

## Acceptance criteria

- [ ] Build on .NET 10 LTS.
- [ ] Hydrate profiles and active state from Runner.
- [ ] Perform profile CRUD through versioned IPC.
- [ ] Exit fully when the window closes.
- [x] Runner launches the packaged WinUI Settings client.
- [ ] Include WinUI 3 build and contract tests in CI.

## Remaining gaps

Runner hydration, generated protocol bindings, IPC transport, durable commands, and interactive Windows UI automation remain planned. GitHub Actions is the build/test authority because the local .NET 10 SDK is unavailable.
