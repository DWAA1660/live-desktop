#define AppName "Live Desktop"
#define AppVersion "1.1.0"

[Setup]
AppId={{1474B482-C5AC-44B5-9240-161C2C63F756}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher=DWAA1660
AppPublisherURL=https://github.com/DWAA1660/live-desktop
AppSupportURL=https://github.com/DWAA1660/live-desktop/issues
DefaultDirName={localappdata}\Programs\LiveDesktop
DefaultGroupName=Live Desktop
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0
OutputDir=..\artifacts\installer
OutputBaseFilename=LiveDesktop-Setup-1.1.0
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\LiveDesktop.exe
AppMutex=Local\LiveDesktop.BeaverAndRain
CloseApplications=yes
RestartApplications=no
LicenseFile=..\LICENSE

[Tasks]
Name: "startup"; Description: "Start Live Desktop automatically when I sign in to Windows"; GroupDescription: "Startup:"
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Shortcuts:"; Flags: unchecked

[Files]
Source: "..\artifacts\app\*"; DestDir: "{app}"; Excludes: "test-results\*"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\LICENSE"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\Live Desktop"; Filename: "{app}\LiveDesktop.exe"; Parameters: "--apply"; WorkingDir: "{app}"
Name: "{group}\Uninstall Live Desktop"; Filename: "{uninstallexe}"
Name: "{autodesktop}\Live Desktop"; Filename: "{app}\LiveDesktop.exe"; Parameters: "--apply"; Tasks: desktopicon

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "LiveDesktop"; ValueData: """{app}\LiveDesktop.exe"" --startup"; Tasks: startup; Flags: uninsdeletevalue
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueName: "LiveDesktop"; Tasks: not startup; Flags: deletevalue

[Run]
Filename: "{app}\LiveDesktop.exe"; Parameters: "--apply"; Description: "Launch Live Desktop"; Flags: nowait postinstall skipifsilent

[Code]
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  StartupCommand: String;
begin
  if CurUninstallStep = usUninstall then
    if RegQueryStringValue(HKCU, 'Software\Microsoft\Windows\CurrentVersion\Run', 'LiveDesktop', StartupCommand) then
      if Pos(ExpandConstant('{app}\LiveDesktop.exe'), StartupCommand) > 0 then
        RegDeleteValue(HKCU, 'Software\Microsoft\Windows\CurrentVersion\Run', 'LiveDesktop');
end;
