# Gaming Optimizer

A comprehensive Rust desktop application for Windows that optimizes gaming performance through an intuitive GUI interface. Features process management, customizable crosshair overlays, and gaming profile management with system tray integration.

## Features

### 🎮 Complete GUI Application
- **Modern Interface**: Full-featured GUI built with ICED framework
- **Profile Management**: Create, edit, save, and delete gaming profiles through visual interface
- **Live Process Browser**: Browse and select running processes to terminate
- **Real-time Status**: Live feedback on all operations and system state

### 🎯 Advanced Crosshair Overlay
- **Custom PNG Crosshairs**: Support for any PNG image with transparency
- **Live Position Adjustment**: Arrow buttons for pixel-perfect crosshair positioning
- **Independent Process**: Crosshair runs as separate executable that survives app closure
- **DWM Composition**: Uses Windows Desktop Window Manager like Xbox Game Bar for fullscreen compatibility
- **Click-through**: Completely transparent to mouse clicks
- **Always-on-top**: Aggressive topmost enforcement for gaming compatibility

### ⚡ Process Optimization
- **Smart Process Killing**: Automatically terminate unwanted background applications
- **Safety Protection**: Built-in blocklist prevents killing critical system processes
- **Live Process List**: Real-time view of running processes with CPU/memory stats
- **Process Filtering**: Search and filter through running applications

### 🎮 Gaming Profiles
- **Multiple Profiles**: Create unlimited gaming profiles for different games
- **One-click Activation**: Switch between profiles instantly
- **Process Groups**: Define which processes to kill per profile
- **Crosshair Settings**: Per-profile crosshair configuration
- **Fan Control**: Optional max fan speed toggle for better cooling

### 🖥️ System Tray Integration
- **Quick Access**: Tray icon for instant profile switching
- **Minimize to Tray**: Application minimizes to system tray
- **Context Menu**: Full profile management from tray
- **Status Indicators**: Visual feedback on active profiles

### 🔧 Advanced Features
- **Crosshair Centering**: One-click reset to screen center
- **Offset Controls**: Fine-tune crosshair position with live preview
- **Image Validation**: Automatic PNG validation and error reporting
- **Profile Persistence**: Automatic saving and loading of all settings
- **Low Resource Usage**: Optimized for minimal system impact

## Tech Stack

- **Language**: Rust (2021 edition)
- **GUI Framework**: ICED - Modern, cross-platform GUI library
- **Platform**: Windows 10/11 only
- **System Integration**:
  - `windows` crate - Direct Windows API access for DWM composition
  - `tray-icon` - System tray functionality
  - `sysinfo` - Process enumeration and management
  - `image` - PNG loading and processing
  - `rfd` - Native file dialogs

## Build Requirements

- Rust 1.70+ (with cargo)
- Windows 10/11
- Visual Studio Build Tools (for Windows API)

## Building

### Debug Build
```bash
cargo build
```

### Release Build (Optimized)
```bash
cargo build --release
```

The release build is optimized for:
- Small binary size (LTO enabled, symbols stripped)
- Fast execution (opt-level 3)
- Low memory footprint

### Output Location
- Debug: `target/debug/gaming_optimizer.exe`
- Release: `target/release/gaming_optimizer.exe`
- Crosshair: `target/release/crosshair.exe` (separate process)

## Running

```bash
# Run debug build
cargo run

# Run release build
cargo run --release

# Or run the executable directly
./target/release/gaming_optimizer.exe
```

The application will start with a full GUI window. You can minimize it to the system tray for background operation.

## Configuration

### Data Directory

The application automatically stores configuration and profiles in:
```
%APPDATA%\GamingOptimizer\
├── profiles.json        # Gaming profiles (auto-managed)
└── crosshairs/          # Optional: Store crosshair images here
```

### Using the GUI

#### Creating Your First Profile
1. **Launch the application**
2. **Click "New Profile"** in the top-left
3. **Enter a profile name** (e.g., "Fortnite", "CS2", "Valorant")
4. **Configure settings**:
   - **Process Selection**: Browse running processes and check which ones to kill
   - **Crosshair**: Click "Select Image" to choose a PNG crosshair
   - **Position**: Use arrow buttons (▲▼◀▶) to adjust crosshair position
   - **Fan Control**: Toggle "Max Fan Speed" if desired
5. **Click "Save Profile"**

#### Activating a Profile
1. **Select a profile** from the left panel
2. **Click "Activate Profile"**
3. The app will:
   - Kill all selected processes
   - Launch the crosshair overlay (if configured)
   - Show status messages for all operations

#### Adjusting Crosshair Live
- **Arrow Buttons**: Click ▲▼◀▶ to move crosshair by 1 pixel
- **Center Button**: Click ⊙ to reset to screen center
- **Changes apply instantly** - no need to reactivate profile

#### Managing Processes
- **Refresh Button**: Updates the live process list
- **Filter Box**: Search for specific processes
- **Checkboxes**: Select which processes to kill when profile activates

## Crosshair Requirements

- **Format**: PNG with transparency support
- **Size**: Any size (automatically centered)
- **Transparency**: Alpha channel for proper blending
- **Location**: Any accessible path (file picker included)

The application validates images automatically and shows error messages for invalid files.

## Fullscreen Game Compatibility

### How It Works
The crosshair uses **DWM (Desktop Window Manager) composition** - the same technology that powers:
- Xbox Game Bar overlays
- Discord in-game overlays
- NVIDIA GeForce Experience overlays
- Steam overlay

### Fortnite Fullscreen Support
**✅ Works with Fortnite "Fullscreen" mode** (default setting)

Modern Windows automatically enables "Fullscreen Optimizations" which secretly uses borderless windowed mode internally. This allows DWM overlays to appear.

**To ensure compatibility:**
1. Right-click `FortniteClient-Win64-Shipping.exe`
2. Properties → Compatibility
3. **UNCHECK** "Disable fullscreen optimizations" (leave it enabled)

### True Exclusive Fullscreen
❌ **Will NOT work** with true exclusive fullscreen (optimizations disabled). This is rare and only used by very old games.

## Protected Processes

The following critical Windows processes cannot be killed for system stability:

- `csrss.exe` - Client Server Runtime
- `dwm.exe` - Desktop Window Manager
- `explorer.exe` - Windows Explorer
- `lsass.exe` - Local Security Authority
- `services.exe` - Services Control Manager
- `smss.exe` - Session Manager
- `system` - System process
- `wininit.exe` - Windows Init
- `winlogon.exe` - Windows Logon
- `svchost.exe` - Service Host

## Usage Workflow

### Gaming Session Setup
1. **Launch Gaming Optimizer**
2. **Create/select your game profile**
3. **Configure processes to kill** (Discord, browsers, etc.)
4. **Set up crosshair** (select PNG, adjust position)
5. **Activate profile** before launching game
6. **Launch your game** in fullscreen mode
7. **Crosshair appears automatically** over the game

### During Gaming
- Crosshair stays visible over fullscreen games
- Use tray icon for quick profile switching
- Crosshair survives if main app closes
- Adjust position anytime with arrow controls

### Ending Session
- Deactivate profile to restore killed processes
- Or simply exit the game (processes auto-restart)

## System Tray Features

- **Profile Switching**: Quick access to all profiles
- **Overlay Toggle**: Show/hide crosshair instantly
- **Minimize**: GUI minimizes to tray
- **Exit**: Clean shutdown of all components

## Troubleshooting

### Crosshair not showing over game
- Ensure game uses "Fullscreen" (not "Windowed Fullscreen")
- Check "Disable fullscreen optimizations" is **unchecked** in game properties
- Verify crosshair image is valid PNG with transparency
- Try activating profile after game is running

### Processes not killing
- Verify process names are correct (include .exe)
- Check if process is in protected list
- Try running Gaming Optimizer as administrator
- Some processes may require special permissions

### GUI not responding
- Check Windows Event Viewer for errors
- Ensure all Windows features are enabled
- Try running as administrator

### Crosshair position wrong
- Use arrow buttons (▲▼◀▶) to adjust live
- Click ⊙ to center on screen
- Changes apply instantly without restarting

## Project Structure

```
Gaming_optimizer/
├── Cargo.toml              # Dependencies and build config
├── src/
│   ├── main.rs             # Application entry point
│   ├── gui/
│   │   ├── mod.rs          # Main ICED GUI application
│   │   ├── profile_editor.rs # Profile editing interface
│   │   └── styles.rs       # UI theming
│   ├── bin/
│   │   └── crosshair.rs    # Standalone crosshair process
│   ├── crosshair_overlay.rs # Crosshair launcher
│   ├── tray.rs             # System tray management
│   ├── process.rs          # Process enumeration/killing
│   ├── profile.rs          # Profile data structures
│   ├── config.rs           # Configuration management
│   ├── image_picker.rs     # File dialog utilities
│   ├── common_apps.rs      # Common application database
│   └── ipc.rs              # Inter-process communication
├── target/
│   ├── debug/              # Debug builds
│   └── release/            # Release builds (gaming_optimizer.exe, crosshair.exe)
└── README.md
```

## Development

### Testing
```bash
# Run tests
cargo test

# Run tests with output
cargo test -- --nocapture
```

### Code Quality
```bash
# Format code
cargo fmt

# Run linter
cargo clippy
```

## Known Limitations

- **Windows only** - Uses Windows-specific DWM APIs
- **Single monitor** - Centers on primary display only
- **Manual activation** - No auto-detection of running games
- **PNG only** - Crosshair images must be PNG format

## Future Enhancements

Potential features for future versions:

- Multi-monitor support with monitor selection
- Game detection and auto-profile activation
- Global hotkeys for overlay toggle
- Crosshair library with built-in designs
- Performance metrics overlay
- Profile sharing/import
- Advanced process rules (CPU/memory thresholds)
- Custom crosshair shapes (not just images)
- Overlay opacity controls
- Profile scheduling (time-based activation)

## License

This project is provided as-is for gaming optimization purposes.

## Support

For issues or questions:
1. Check this README for common solutions
2. Verify game compatibility settings
3. Test with different fullscreen modes
4. Check application status messages
5. Ensure proper administrator permissions