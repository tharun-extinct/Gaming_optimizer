# EdgeOptimizer.Settings.Wpf

Initial WPF (.NET 10 LTS) shell for the Settings UI.

Current state:
- The Rust Runner + Engine service own optimization and cleanup execution.
- This WPF app is the future orchestration UI client.

Next integration step:
- Generate C# protocol types from the shared Protobuf contract.
- Request the authoritative profile/active-state snapshot from Runner.
- Send profile edits and orchestration commands to Runner; never open `state.db` directly.
