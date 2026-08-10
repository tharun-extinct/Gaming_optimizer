# EdgeOptimizer.Settings.Wpf

WPF (.NET 10 LTS) presentation client for Edge Optimizer.

Current state:
- The Rust Runner + Engine service own optimization and cleanup execution.
- The WPF client now includes the shared profile-scoped shell and preview implementations of Dashboard, Crosshair, Macros, and System Tweaks.
- Preview edits are memory-only. Commands that require orchestration, durable state, process discovery, cleanup, macro playback, or activation stay disabled until Runner IPC is available.
- The window exits fully when closed and does not open Runner's database or perform privileged operations.

Next integration step:
- Generate C# protocol types from the shared Protobuf contract.
- Request the authoritative profile/active-state snapshot from Runner.
- Send profile edits and orchestration commands to Runner; never open `state.db` directly.
