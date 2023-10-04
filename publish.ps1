dotnet publish -r win-x86 -f net6.0-windows -c Release --no-self-contained --force -o "output\Assemblies"

$WshShell = New-Object -ComObject WScript.Shell
$shortcut = $WshShell.CreateShortcut("$PSScriptRoot\output\Taskbar Shortcut Groups.lnk")
$shortcut.TargetPath = "$PSScriptRoot\output\Assemblies\TaskbarShortcutGroups.exe"
$shortcut.WorkingDirectory = "$PSScriptRoot\output\"
$shortcut.Save()