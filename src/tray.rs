use crate::profile::Profile;
use crate::ipc::{TrayChannels, GuiToTray, TrayToGui};
use anyhow::{anyhow, Result};
use std::collections::HashMap;
use std::sync::mpsc::TryRecvError;
use tray_icon::menu::{Menu, MenuEvent, MenuItem, PredefinedMenuItem, Submenu};
use tray_icon::{TrayIcon, TrayIconBuilder};

/// System tray manager
pub struct TrayManager {
    tray_icon: TrayIcon,
    menu: Menu,
    profile_submenu: Submenu,
    overlay_toggle: MenuItem,
    settings_item: MenuItem,
    exit_item: MenuItem,
    // Track profile menu items by their ID
    profile_items: HashMap<tray_icon::menu::MenuId, String>,
    none_item_id: Option<tray_icon::menu::MenuId>,
}

/// Events that can be triggered from the tray menu
#[derive(Debug, Clone, PartialEq)]
pub enum TrayEvent {
    ProfileSelected(String),
    ProfileDeactivated,
    OverlayToggled,
    OpenSettings,
    Exit,
}

impl TrayManager {
    /// Create a new system tray icon with menu
    pub fn new(profiles: &[Profile], active_profile: Option<&str>) -> Result<Self> {
        // Create menu items
        let menu = Menu::new();

        // Title item (non-clickable)
        let title = MenuItem::new("Gaming Optimizer", false, None);
        menu.append(&title)
            .map_err(|e| anyhow!("Failed to add title item: {}", e))?;

        // Separator
        menu.append(&PredefinedMenuItem::separator())
            .map_err(|e| anyhow!("Failed to add separator: {}", e))?;

        // Profiles submenu
        let profile_submenu = Submenu::new("Profiles", true);
        let (profile_items, none_item_id) = Self::populate_profile_submenu(&profile_submenu, profiles, active_profile)?;
        menu.append(&profile_submenu)
            .map_err(|e| anyhow!("Failed to add profiles submenu: {}", e))?;

        // Separator
        menu.append(&PredefinedMenuItem::separator())
            .map_err(|e| anyhow!("Failed to add separator: {}", e))?;

        // Overlay toggle (initially disabled)
        let overlay_toggle = MenuItem::new("☐ Overlay Visible", active_profile.is_some(), None);
        menu.append(&overlay_toggle)
            .map_err(|e| anyhow!("Failed to add overlay toggle: {}", e))?;

        // Separator
        menu.append(&PredefinedMenuItem::separator())
            .map_err(|e| anyhow!("Failed to add separator: {}", e))?;

        // Settings
        let settings_item = MenuItem::new("⚙ Open Settings", true, None);
        menu.append(&settings_item)
            .map_err(|e| anyhow!("Failed to add settings item: {}", e))?;

        // Exit
        let exit_item = MenuItem::new("❌ Exit", true, None);
        menu.append(&exit_item)
            .map_err(|e| anyhow!("Failed to add exit item: {}", e))?;

        // Create tray icon
        let tray_icon = TrayIconBuilder::new()
            .with_menu(Box::new(menu.clone()))
            .with_tooltip("Gaming Optimizer - Inactive")
            .build()
            .map_err(|e| anyhow!("Failed to create tray icon: {}", e))?;

        Ok(TrayManager {
            tray_icon,
            menu,
            profile_submenu,
            overlay_toggle,
            settings_item,
            exit_item,
            profile_items,
            none_item_id,
        })
    }

    /// Populate the profiles submenu with current profiles
    fn populate_profile_submenu(
        submenu: &Submenu,
        profiles: &[Profile],
        active_profile: Option<&str>,
    ) -> Result<(HashMap<tray_icon::menu::MenuId, String>, Option<tray_icon::menu::MenuId>)> {
        let mut profile_items = HashMap::new();
        let mut none_item_id = None;

        if profiles.is_empty() {
            let no_profiles = MenuItem::new("(No profiles - open Settings)", false, None);
            submenu
                .append(&no_profiles)
                .map_err(|e| anyhow!("Failed to add no profiles item: {}", e))?;
        } else {
            // Add each profile
            for profile in profiles {
                let is_active = active_profile == Some(&profile.name);
                let label = if is_active {
                    format!("✓ {}", profile.name)
                } else {
                    profile.name.clone()
                };
                let item = MenuItem::new(label, true, None);
                profile_items.insert(item.id().clone(), profile.name.clone());
                submenu
                    .append(&item)
                    .map_err(|e| anyhow!("Failed to add profile item: {}", e))?;
            }

            // Add separator
            submenu
                .append(&PredefinedMenuItem::separator())
                .map_err(|e| anyhow!("Failed to add separator: {}", e))?;

            // Add "(None)" option to deactivate profile
            let none_item = MenuItem::new("(None)", true, None);
            none_item_id = Some(none_item.id().clone());
            submenu
                .append(&none_item)
                .map_err(|e| anyhow!("Failed to add none item: {}", e))?;
        }

        Ok((profile_items, none_item_id))
    }

    /// Update the tray menu with new profiles list
    pub fn update_profiles(&mut self, profiles: &[Profile], active_profile: Option<&str>) -> Result<()> {
        // Create new profiles submenu
        let new_submenu = Submenu::new("Profiles", true);
        let (profile_items, none_item_id) = Self::populate_profile_submenu(&new_submenu, profiles, active_profile)?;

        // Update stored profile items
        self.profile_items = profile_items;
        self.none_item_id = none_item_id;
        self.profile_submenu = new_submenu;

        Ok(())
    }

    /// Update tooltip to show active profile
    pub fn set_active_profile(&mut self, profile_name: Option<&str>) -> Result<()> {
        let tooltip = if let Some(name) = profile_name {
            format!("Gaming Optimizer - {}", name)
        } else {
            "Gaming Optimizer - Inactive".to_string()
        };

        self.tray_icon.set_tooltip(Some(tooltip))
            .map_err(|e| anyhow!("Failed to set tooltip: {}", e))?;

        Ok(())
    }

    /// Update overlay toggle state
    pub fn set_overlay_visible(&mut self, visible: bool, enabled: bool) -> Result<()> {
        self.overlay_toggle.set_enabled(enabled);

        // Update text to show checkmark when visible
        let text = if visible {
            "☑ Overlay Visible"
        } else {
            "☐ Overlay Visible"
        };
        self.overlay_toggle.set_text(text);

        Ok(())
    }

    /// Poll for menu events and convert to TrayToGui messages
    pub fn poll_events(&self) -> Option<TrayToGui> {
        if let Ok(event) = MenuEvent::receiver().try_recv() {
            return self.handle_menu_event(event);
        }
        None
    }

    /// Handle a menu event and convert to TrayToGui
    fn handle_menu_event(&self, event: MenuEvent) -> Option<TrayToGui> {
        let event_id = event.id;

        // Check if it's a profile item
        if let Some(profile_name) = self.profile_items.get(&event_id) {
            return Some(TrayToGui::ActivateProfile(profile_name.clone()));
        }

        // Check for "(None)" deactivation
        if let Some(ref none_id) = self.none_item_id {
            if &event_id == none_id {
                return Some(TrayToGui::DeactivateProfile);
            }
        }

        // Check overlay toggle
        if event_id == self.overlay_toggle.id() {
            return Some(TrayToGui::ToggleOverlay);
        }

        // Check settings
        if event_id == self.settings_item.id() {
            return Some(TrayToGui::OpenSettings);
        }

        // Check exit
        if event_id == self.exit_item.id() {
            return Some(TrayToGui::Exit);
        }

        None
    }
}

/// Run the tray in its own thread, communicating via channels
pub fn run_tray_thread(channels: TrayChannels, initial_profiles: Vec<Profile>, active_profile: Option<String>) {
    std::thread::spawn(move || {
        // Create the tray manager
        let mut tray = match TrayManager::new(&initial_profiles, active_profile.as_deref()) {
            Ok(t) => t,
            Err(e) => {
                eprintln!("Failed to create tray: {}", e);
                return;
            }
        };
        
        let mut profiles = initial_profiles;
        let mut current_active = active_profile;
        
        loop {
            // Check for messages from GUI
            match channels.from_gui.try_recv() {
                Ok(msg) => match msg {
                    GuiToTray::ProfilesUpdated(new_profiles) => {
                        profiles = new_profiles;
                        let _ = tray.update_profiles(&profiles, current_active.as_deref());
                    }
                    GuiToTray::ActiveProfileChanged(new_active) => {
                        current_active = new_active;
                        let _ = tray.set_active_profile(current_active.as_deref());
                    }
                    GuiToTray::OverlayVisibilityChanged(visible) => {
                        let _ = tray.set_overlay_visible(visible, current_active.is_some());
                    }
                    GuiToTray::Shutdown => {
                        break;
                    }
                },
                Err(TryRecvError::Empty) => {}
                Err(TryRecvError::Disconnected) => {
                    // GUI has closed, exit tray thread
                    break;
                }
            }
            
            // Poll for tray menu events
            if let Some(event) = tray.poll_events() {
                if let Err(_) = channels.to_gui.send(event.clone()) {
                    // GUI receiver disconnected
                    break;
                }
                
                // Exit immediately if exit requested
                if matches!(event, TrayToGui::Exit) {
                    break;
                }
            }
            
            // Small sleep to avoid busy-waiting
            std::thread::sleep(std::time::Duration::from_millis(50));
        }
    });
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::profile;

    #[test]
    fn test_tray_event_equality() {
        let event1 = TrayEvent::Exit;
        let event2 = TrayEvent::Exit;
        assert_eq!(event1, event2);

        let event3 = TrayEvent::ProfileSelected("Test".to_string());
        let event4 = TrayEvent::ProfileSelected("Test".to_string());
        assert_eq!(event3, event4);
    }
}
