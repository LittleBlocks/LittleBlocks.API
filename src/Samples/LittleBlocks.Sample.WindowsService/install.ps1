$executable = "<PathToExecutable>\LittleBlocks.Sample.WindowsService.Exe"
$user = "NT_AUTHORITY\LocalService"
$service = "LittleBlocksWindowsService"
$serviceName = "LittleBlocks Windows Service"
$serviceDescription = "LittleBlocks Windows Service to showcase hosting as service"

$acl = Get-Acl $executable
$aclRuleArgs = $user, "Read,Write,ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($aclRuleArgs)
$acl.SetAccessRule($accessRule)
$acl | Set-Acl $executable


New-Service -Name $service -BinaryPathName $executable -Credential $user -Description $serviceDescription -DisplayName $serviceName -StartupType Automatic
