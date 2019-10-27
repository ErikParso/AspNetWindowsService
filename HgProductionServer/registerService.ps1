
$serviceName = "HgProductionServer"
$path = (Get-Location)

If (Get-Service $serviceName -ErrorAction SilentlyContinue) {
    If ((Get-Service $serviceName).Status -eq 'Running') {
        Stop-Service $serviceName
        Write-Host "Stopping $serviceName"
    } Else {
        Write-Host "$serviceName found, but it is not running."
    }
} Else {
    Write-Host "$serviceName not found"
}

sc.exe delete $serviceName

New-Service `
    -Name $serviceName `
    -BinaryPathName "$path/ProductionServer.exe" `
    -Description "Helios Green production server simulation." `
    -DisplayName "HgProductionServer" `
    -StartupType Automatic

sc.exe start $serviceName

Read-Host -Prompt "Press Enter to exit"