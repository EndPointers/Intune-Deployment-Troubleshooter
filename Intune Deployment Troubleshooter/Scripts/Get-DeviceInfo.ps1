#****************************************************************
# Author: James Everett
# Written: 11/23/2022
# Purpose: Get device info.
# Usage: .\Get-DeviceInfo.ps1
#****************************************************************
param (
	[parameter(Mandatory=$true)]
	[ValidateNotNullorEmpty()]
    [string]$hostname = $Null
)

<# Run this as an admin if needed
Install-Module Microsoft.Graph.DeviceManagement
Install-Module Microsoft.Graph.Identity.DirectoryManagement
#>

Import-Module Microsoft.Graph.DeviceManagement
Import-Module Microsoft.Graph.Identity.DirectoryManagement

# Auth to MSGraph
Connect-MgGraph -Scopes "DeviceManagementManagedDevices.Read.All, Device.Read.All"

# Obtain the Azure Object ID from DisplayName
foreach($object in (Get-MgDeviceManagementManagedDevice -All))
{
	If($hostname -eq $object.deviceName)
	{
		$device = (Get-MgDeviceManagementManagedDevice -ManagedDeviceId $object.id)
		Write-Output ""
		Write-Output "Primary User: $($device.userPrincipalName)"
		Write-Output "Hostname: $($device.deviceName)"
		Write-Output "Operating System: $($device.operatingSystem)"
		Write-Output "Operating System Version: $($device.osVersion)"
		Write-Output "Model: $($device.model)"
		Write-Output "Manufacturer: $($device.manufacturer)"
		Write-Output "Serial Number: $($device.serialNumber)"
		Write-Output "Is Encrypted: $($device.isEncrypted)"
		Write-Output "Device ID: $($device.id)"
	}
}
