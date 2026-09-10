; Inno Setup Script
; AlghoritmsAndDataStructures - Алгоритмы и структуры данных
; Устанавливает WPF-приложение и консольный лаунчер в одну папку {app}

#define MyAppName "AlghoritmsAndDataStructures"
#define MyAppVersion "1.0"
#define MyAppPublisher "ТулГУ"

[Setup]
AppId={{B7B57F2A-9D1E-4C6B-A8E1-3F0F5C8D12AB}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
; Установка в корень C: — приложение пишет history.json рядом с exe без прав администратора
DefaultDirName=C:\AlghoritmsAndDataStructures
DefaultGroupName={#MyAppName}
OutputDir=C:\Users\Danila\source\repos\AlghoritmsAndDataStructures\installer
OutputBaseFilename=setup
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\AlgorithmsLauncher.exe

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
; === WPF-приложение: все файлы bin\Release, включая зависимости и папки локалей ===
Source: "C:\Users\Danila\source\repos\AlghoritmsAndDataStructures\AlghoritmsAndDataStructures\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "*.pdb,history.json"
; === Консольный лаунчер ===
Source: "C:\Users\Danila\source\repos\AlghoritmsAndDataStructures\AlgorithmsLauncher\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "*.pdb"

[Icons]
Name: "{group}\Алгоритмы и структуры данных (WPF)"; Filename: "{app}\AlghoritmsAndDataStructures.exe"
Name: "{group}\Алгоритмы и структуры данных (Консольный лаунчер)"; Filename: "{app}\AlgorithmsLauncher.exe"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\Алгоритмы и структуры данных (Лаунчер)"; Filename: "{app}\AlgorithmsLauncher.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\AlgorithmsLauncher.exe"; Description: "Запустить лаунчер"; Flags: nowait postinstall skipifsilent