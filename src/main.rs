#![windows_subsystem = "windows"]

mod config;
mod overlay;
mod process;
mod profile;
mod tray;
mod gui;
mod ipc;
mod common_apps;
mod image_picker;

use anyhow::Result;
use ipc::IpcChannels;
use config::get_data_directory;
use profile::load_profiles;

fn main() -> Result<()> {
    // Create IPC channels
    let (gui_channels, tray_channels) = IpcChannels::new();
    
    // Load initial profiles
    let initial_profiles = if let Ok(data_dir) = get_data_directory() {
        load_profiles(&data_dir).unwrap_or_default()
    } else {
        Vec::new()
    };
    
    // Spawn tray in separate thread
    tray::run_tray_thread(tray_channels, initial_profiles, None);
    
    // Run GUI application (blocks until GUI closes)
    gui::run_with_channels(gui_channels)?;
    
    Ok(())
}
