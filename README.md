<h1>NextPCName</h1>
<p>
A user-friendly Windows utility to discover and copy the next available PC (computer) names in your organization's Active Directory.<br>
Ideal for IT administrators and deployment teams who want to ensure consistent, conflict-free PC naming — every time.
</p>

<h2>Main Features</h2>
<ul>
  <li><b>Active Directory Integration:</b> Directly queries your company’s AD for all registered computer names.</li>
  <li><b>Custom Naming Rules:</b> Easily configure the prefix (e.g. “PC”, “LAPTOP”, “CLIENT”) and digit format (e.g. PC001, CLIENT0001, etc.).</li>
  <li><b>Bulk Name Discovery:</b> Instantly generate and copy as many available PC names as you need (limit set in Settings) to the clipboard for mass deployments.</li>
  <li><b>Modern Settings Menu:</b> Simple settings dialog lets you adjust AD path, naming rules, and enable/disable features — no config file editing required.</li>
  <li><b>Robust Error Handling:</b> Detects AD connectivity issues, shows clear error messages, and avoids false “available” names if lookup fails.</li>
  <li><b>Optional Confetti Celebration:</b> Turn on a colorful confetti overlay for a little joy after a successful name lookup.</li>
  <li><b>Clipboard Integration:</b> All found names are automatically copied for quick pasting — perfect for MDT, Intune, or manual domain joins.</li>
  <li><b>Polished, Minimal UI:</b> Responsive layout, status messages, and a fun “Made by JF” signature (with link to this repo).</li>
</ul>

<h2>Typical Use Cases</h2>
<ul>
  <li>Deploying new Windows clients in an enterprise</li>
  <li>Avoiding duplicate computer names</li>
  <li>Ensuring naming standard consistency</li>
  <li>Automating device provisioning with ready-to-use names</li>
</ul>

<h2>How It Works</h2>
<ol>
  <li>Enter how many PC names you want (default 1–50).</li>
  <li>Click <b>“Find next available name(s)”</b> — or just press Enter.</li>
  <li>The tool checks your AD for all computer names, then suggests the next unused numbers in sequence.</li>
  <li>You see up to 6 names on screen (for clarity), but all are copied to your clipboard.</li>
  <li>No available names? Or can’t connect to AD? You get a clear status message, not a fake name!</li>
</ol>

<h2>Getting Started</h2>
<ul>
  <li>Download the installer from the releases tab.</li>
  <li>Run the setup — shortcuts and uninstaller included.</li>
  <li>Double-click to launch.</li>
  <li>See the built-in <b>“Settings”</b> menu to set your company’s AD path and preferred naming format.</li>
</ul>

<h2>Support / Issues</h2>
<p>
Open an <a href="https://github.com/Lett3rs/NextPCName/issues">issue on GitHub</a> or contact me for help and feature requests!
</p>

<h2>☕ Support</h2>
<p>
If you like this tool, you can <a href="https://www.coff.ee/lett3rs" target="_blank"><b>Buy Me a Coffee</b></a> — it helps keep the projects alive!
<br>
</a>
</p>
