/// Inter-Process Communication between GUI and System Tray
use std::sync::mpsc::{Sender, Receiver, channel};
use crate::profile::Profile;

/// Messages from GUI to Tray
#[derive(Debug, Clone)]
pub enum GuiToTray {
    /// Update profiles list
    ProfilesUpdated(Vec<Profile>),
    /// Active profile changed
    ActiveProfileChanged(Option<String>),
    /// Overlay visibility changed
    OverlayVisibilityChanged(bool),
    /// Request tray to exit
    Shutdown,
}

/// Messages from Tray to GUI
#[derive(Debug, Clone)]
pub enum TrayToGui {
    /// User selected a profile from tray
    ActivateProfile(String),
    /// User deactivated profile from tray
    DeactivateProfile,
    /// User toggled overlay from tray
    ToggleOverlay,
    /// User requested to open settings/GUI
    OpenSettings,
    /// User requested exit
    Exit,
}

/// Channel pair for IPC communication
pub struct IpcChannels;

impl IpcChannels {
    /// Create a new pair of channels for GUI <-> Tray communication
    pub fn new() -> (GuiChannels, TrayChannels) {
        let (gui_to_tray_tx, gui_to_tray_rx) = channel();
        let (tray_to_gui_tx, tray_to_gui_rx) = channel();
        
        let gui = GuiChannels {
            to_tray: gui_to_tray_tx,
            from_tray: tray_to_gui_rx,
        };
        
        let tray = TrayChannels {
            from_gui: gui_to_tray_rx,
            to_gui: tray_to_gui_tx,
        };
        
        (gui, tray)
    }
}

/// Channels held by the GUI side (not Clone - stored in static)
pub struct GuiChannels {
    pub to_tray: Sender<GuiToTray>,
    pub from_tray: Receiver<TrayToGui>,
}

/// Channels held by the Tray side
pub struct TrayChannels {
    pub from_gui: Receiver<GuiToTray>,
    pub to_gui: Sender<TrayToGui>,
}
