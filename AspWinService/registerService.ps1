$serviceName = "AspWinService"
$path = (Get-Location)

sc.exe delete $serviceName
New-Service `
    -Name $serviceName `
    -BinaryPathName "$path/AspWinService.exe" `
    -Description "Asp net core application hosted in windows service." `
    -DisplayName "Asp windows service" `
    -StartupType Automatic

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