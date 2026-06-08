# MASA DFI (Digital Forensics Investigator)

## Overview

MASA DFI is a comprehensive, enterprise-grade Digital Forensics and Incident Response (DFIR) application built using VB.NET, WPF, and .NET 8.0. Designed for security analysts, forensic investigators, and system administrators, MASA DFI provides a centralized, deeply integrated platform to extract, analyze, and document critical artifacts from Windows operating systems.

The application operates with a dark, professional, high-contrast terminal aesthetic, minimizing visual fatigue while providing clear and structured presentation of dense forensic data.

## Core Capabilities

The architecture of MASA DFI is modular, breaking down the forensic investigation into distinct, highly specialized components:

### 1. System Logging and Events
- **Event Log Analyzer**: Asynchronously parses and aggregates Windows Event Logs (System, Security, and Application) to identify anomalies, unauthorized access attempts, and system failures.

### 2. Artifact Extraction
- **Browser History Analyzer**: Directly queries SQLite databases associated with Chromium-based browsers (Google Chrome, Microsoft Edge) and Mozilla Firefox to reconstruct user navigation timelines.
- **USB Device History**: Scrapes the Windows Registry (`HKLM\SYSTEM\CurrentControlSet\Enum\USBSTOR`) to identify and log all historically connected USB mass storage devices.
- **Jump Lists Analyzer**: Parses Windows Automatic Destinations to reveal recently accessed files and documents, bypassing standard user-level clearing mechanisms.

### 3. File System Forensics
- **Alternate Data Streams (ADS) Scanner**: Deeply scans NTFS file systems to identify hidden data streams often utilized by rootkits and sophisticated malware to obfuscate payloads.
- **Prefetch Analyzer**: Parses Windows Prefetch (`.pf`) files to track execution history, frequency, and exact timestamps of applications run on the target machine.
- **File Recovery**: Analyzes the Windows Recycle Bin (`$Recycle.Bin`) to enumerate and potentially restore deleted artifacts and track user deletion events.

### 4. Live System Analysis
- **System Analyzers**: Provides real-time insights into active processes, memory consumption, and startup programs (Registry Run keys).
- **Network Connections**: Executes and parses `netstat` output to identify established connections, listening ports, and associated Process IDs (PIDs).
- **DNS Cache Viewer**: Retrieves local DNS resolution cache to identify recent remote server communications, with administrative capabilities to flush the cache.
- **Windows Services Analyzer**: Enumerates system services via WMI/CIM, mapping service states to their physical executable paths to identify rogue or masquerading services.

### 5. Malware and Threat Hunting
- **Suspicious File Scanner**: Performs cryptographic hashing (MD5, SHA-1, SHA-256) on target files. Calculates Shannon Entropy to identify highly compressed, packed, or encrypted executables typical of malware payloads.

### 6. Correlation and Reporting
- **Timeline Explorer**: Consolidates events from multiple modules into a unified, chronological timeline to reconstruct the sequence of a cyber incident.
- **Evidence Export**: Generates tamper-evident forensic reports. Supports exporting data to PDF, HTML, JSON, and CSV formats for chain-of-custody documentation and external tool ingestion.

## Technical Requirements

- **Framework**: .NET 8.0 Desktop Runtime
- **Operating System**: Windows 10 / Windows 11
- **Privileges**: Administrative privileges are strictly required. MASA DFI interacts with restricted Registry hives, low-level system APIs, and protected directories (e.g., `C:\Windows\Prefetch`, Security Event Logs).

## Deployment and Compilation

The project is configured for seamless compilation via the .NET CLI.

```bash
# Navigate to the project directory
cd MasaDFI

# Restore dependencies and build the application
dotnet build -c Release

# Execute the application
dotnet run
```

### Dependencies
- `System.Data.SQLite.Core`: Required for direct interaction with browser history databases.
- `Newtonsoft.Json`: Required for parsing and formatting JSON evidence exports.
- `PdfSharp`: Utilized for the generation of professional, structured PDF reports.

## Architecture Guidelines

- **UI/UX**: The application utilizes a custom, decoupled WPF styling architecture. All generic controls (`DataGrid`, `Button`, `TabControl`) inherit global styles defined in `Application.xaml` to ensure a consistent, dark-mode terminal aesthetic.
- **Asynchronous Execution**: Heavy I/O operations (Event Log parsing, recursive file scanning, PowerShell process execution) are wrapped in `System.Threading.Tasks.Task.Run` to prevent UI thread blocking.
- **Interoperability**: Specific modules leverage PowerShell subprocesses (`Get-CimInstance`, `Get-Item -Stream`) to interact with complex Windows APIs without requiring extensive P/Invoke declarations.

## Disclaimer

MASA DFI is developed strictly for lawful digital forensics, incident response, and authorized security auditing. The developers assume no liability for the misuse of this tool in unauthorized environments.
