#****************************************************************
# Author: James Everett
# Written: 12/05/2022
# Purpose: Initiate syncDevice
# Usage: .\Sync-DevicePerHostname.ps1
#****************************************************************
param (
	[parameter(Mandatory=$true)]
	[ValidateNotNullorEmpty()]
    [string]$hostname = $Null
)

<# Run this as an admin if needed
Install-Module Microsoft.Graph.DeviceManagement
Install-Module Microsoft.Graph.DeviceManagement.Actions
#>

Import-Module Microsoft.Graph.DeviceManagement
Import-Module Microsoft.Graph.DeviceManagement.Actions

# Auth to MSGraph
Connect-MgGraph -Scopes "DeviceManagementManagedDevices.PrivilegedOperations.All"

# Obtain the Azure Object ID from Hostname
foreach($object in (Get-MgDeviceManagementManagedDevice -All))
{
	If($hostname -eq $object.deviceName)
	{
		# Obtain Device.ID
		$device = (Get-MgDeviceManagementManagedDevice -ManagedDeviceId $object.id)
		try {
			# Initiate syncDevice
			Sync-MgDeviceManagementManagedDevice -ManagedDeviceId $device.Id
			Write-Output ""
			Write-Output "Hostname: $($hostname)"
			Write-Output "Device ID: $($device.Id)"
			Write-Output ""
			Write-Output "The synchronization request has been sent..."
		} catch {
			Write-Output ""
			Write-Output "Initiate sync for device $($hostname) failed!"
		}
	}
}
