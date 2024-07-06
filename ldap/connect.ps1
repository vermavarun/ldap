$username = "pu*"
$password = "Gg****"

$ldapPath = "LDAP://169.***.249.***:389"

$directoryEntry = New-Object System.DirectoryServices.DirectoryEntry($ldapPath, $username, $password)

try {
    $directoryEntry.RefreshCache()
    Write-Host "ok"
}
catch {
    Write-Host "failed"
}