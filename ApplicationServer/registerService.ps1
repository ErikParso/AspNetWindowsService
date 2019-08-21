
$serviceName = "ApplicationServer"
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
    -BinaryPathName "$path/ApplicationServer.exe" `
    -Description "Helios Green application server simulation." `
    -DisplayName "ApplicationServer" `
    -StartupType Automatic

sc.exe start $serviceName

Read-Host -Prompt "Press Enter to exit"

# $serviceName = "AspWinService"
# $serviceUserName = "AspWinServiceUser"
# $path = (Get-Location)

# sc.exe delete $serviceNamepowershell
# Remove-LocalUser -Name $serviceUserName

# New-LocalUser -Name $serviceUserName -NoPassword
# Grant-UserRight -Account $serviceUserName -Right SeServiceLogonRight

# $acl = Get-Acl "$path"
# $aclRuleArgs = "DESKTOP-3GKG7JK\AspWinServiceUser", "Read,Write,ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
# $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($aclRuleArgs)
# $acl.SetAccessRule($accessRule)
# $acl | Set-Acl "$path"

# New-Service `
#     -Name $serviceName `
#     -BinaryPathName "$path/AspWinService.exe" `
#     -Credential "DESKTOP-3GKG7JK\$serviceUserName" `
#     -Description "Asp net core application hosted in windows service." `
#     -DisplayName "Asp windows service" `
#     -StartupType Automatic

# Read-Host -Prompt "Press Enter to exit"