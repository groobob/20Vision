[Setup]
AppName=20Vision
AppVersion=1.0.0
AppPublisher=groobob
DefaultDirName={autopf}\20Vision
DefaultGroupName=20Vision
OutputBaseFilename=20Vision-v1.0.0-setup-win-x64
OutputDir=installer
Compression=lzma
SolidCompression=yes
WizardStyle=modern
DisableWelcomePage=no
DisableDirPage=no
DisableProgramGroupPage=no
SetupIconFile=Assets\20vision.ico
ArchitecturesInstallIn64BitMode=x64compatible
ArchitecturesAllowed=x64compatible


[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional tasks:"
Name: "startmenuicon"; Description: "Create a Start Menu shortcut"; GroupDescription: "Additional tasks:"
Name: "startup"; Description: "Run 20Vision on Windows startup"; GroupDescription: "Additional tasks:"


[Files]
Source: "bin\Release\net10.0\publish\20Vision-v1.0-portable-win-x64\*"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\20Vision"; Filename: "{app}\20Vision.exe"; Tasks: startmenuicon
Name: "{commondesktop}\20Vision"; Filename: "{app}\20Vision.exe"; Tasks: desktopicon

[Registry]
Root: HKCU; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "20Vision"; ValueData: "{app}\20Vision.exe"; Flags: uninsdeletevalue; Tasks: startup

[Run]
Filename: "{app}\20Vision.exe"; Description: "Launch 20Vision"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}"