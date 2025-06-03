# NextPCName
A user-friendly Windows utility to discover and copy the next available PC (computer) names in your organization's Active Directory.
Ideal for IT administrators and deployment teams who want to ensure consistent, conflict-free PC naming — every time.

# Main Features
Active Directory Integration:
Directly queries your company’s AD for all registered computer names.

Custom Naming Rules:
Easily configure the prefix (e.g. “PC”, “LAPTOP”, “CLIENT”) and digit format (e.g. PC001, CLIENT0001, etc.).

Bulk Name Discovery:
Instantly generate and copy as many available PC names as you need (limit set in Settings) to the clipboard for mass deployments.

Modern Settings Menu:
Simple settings dialog lets you adjust AD path, naming rules, and enable/disable features — no config file editing required.

Robust Error Handling:
Detects AD connectivity issues, shows clear error messages, and avoids false “available” names if lookup fails.

Optional Confetti Celebration:
Turn on a colorful confetti overlay for a little joy after a successful name lookup.

Clipboard Integration:
All found names are automatically copied for quick pasting — perfect for MDT, Intune, or manual domain joins.

Polished, Minimal UI:
Responsive layout, status messages, and a fun “Made by JF” signature (with link to this repo).

# Typical Use Cases
Deploying new Windows clients in an enterprise

Avoiding duplicate computer names

Ensuring naming standard consistency

Automating device provisioning with ready-to-use names

# How It Works
Enter how many PC names you want (default 1–50).

Click “Find next available name(s)” — or just press Enter.

The tool checks your AD for all computer names, then suggests the next unused numbers in sequence.

You see up to 6 names on screen (for clarity), but all are copied to your clipboard.

No available names? Or can’t connect to AD? You get a clear status message, not a fake name!

# Getting Started
Download the installer from the releases tab.

Run the setup — shortcuts and uninstaller included.

Double-click to launch.

See the built-in “Settings” menu to set your company’s AD path and preferred naming format.

# Support / Issues
Open an issue on GitHub or contact me for help and feature requests!

# ☕ Support
If you like this tool, you can Buy Me a Coffee — it helps keep the projects alive!
