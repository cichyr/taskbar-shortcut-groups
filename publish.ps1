#$supportedArchitectures = "win-x86", "win-x64"
$supportedArchitectures = "win-x86"
$publishOutputPath = "$PSScriptRoot\TaskbarShortcutGroups.AvaloniaUI\bin\Release\net8.0-windows"
$outputPath = "$PSScriptRoot\Output\TaskbarShortcutGroups"
$visibleFiles = "Licenses", "TaskbarShortcutGroups.AvaloniaUI.exe", "TaskbarShortcutGroups.AvaloniaUI.dll", "TaskbarShortcutGroups.AvaloniaUI.runtimeconfig.json"

Write-Output "Build will be performed for following architectures: $supportedArchitectures"

if (Test-Path "$PSScriptRoot\Output")
{
    Write-Output "Clearing old publish output... "
    Remove-Item "$PSScriptRoot\Output" -Recurse -ErrorAction SilentlyContinue | Out-Null
}

if (Test-Path "$publishOutputPath")
{
    Write-Output "Clearing old build output... "
    Remove-Item "$publishOutputPath" -Recurse -ErrorAction SilentlyContinue | Out-Null
}

New-Item -ItemType Directory -Force -Path $outputPath -ErrorAction SilentlyContinue | Out-Null
New-Item -ItemType Directory -Force -Path "$outputPath\Assemblies" -ErrorAction SilentlyContinue | Out-Null

foreach ($arch in $supportedArchitectures)
{
    Write-Output "Building $arch application..."

    Remove-Item "$outputPath" -Recurse -ErrorAction SilentlyContinue | Out-Null
    New-Item -ItemType Directory -Force -Path $outputPath -ErrorAction SilentlyContinue | Out-Null
    New-Item -ItemType Directory -Force -Path "$outputPath\Assemblies" -ErrorAction SilentlyContinue | Out-Null

    Write-Output "Publishing..."
    dotnet publish "$PSScriptRoot\TaskbarShortcutGroups.sln" -r $arch --no-self-contained -f net8.0-windows -c Release --force -p:PublishReadyToRun=true --nologo -consoleLoggerParameters:NoSummary

    Write-Output "Creating shortcut..."
    Copy-Item -Path "$publishOutputPath\$arch\publish\*" -Destination "$outputPath" -Recurse
    foreach ($file in Get-ChildItem $outputPath)
    {
        if (!($visibleFiles.Contains($file.Name)))
        {
            $file.Attributes = $file.Attributes -bor "Hidden"
        }
    }
        
        
#    $WshShell = New-Object -ComObject WScript.Shell
#    $shortcut = $WshShell.CreateShortcut("$outputPath\Taskbar Shortcut Groups.lnk")
#    $shortcut.TargetPath = "$outputPath\Assemblies\TaskbarShortcutGroups.Avalonia.exe"
#    $shortcut.WorkingDirectory = "$outputPath\"
#    $shortcut.Save()

#    Write-Output "Compressing..."
#    Compress-Archive -Path $outputPath -DestinationPath "$outputPath-$arch.zip" -Force
    
    Write-Output "$arch application ready!"
}

Write-Output "Done!"