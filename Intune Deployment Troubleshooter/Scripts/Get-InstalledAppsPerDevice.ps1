#*******************************************************************
# Author: James Everett
# Written: 12/11/2022
# Purpose: Obtains apps installed on device.
# Usage: .\Get-InstalledAppsPerDevice.ps1 -Hostname DESKTOP-A2476OD
#*******************************************************************
param (
	[parameter(Mandatory=$true)]
	[ValidateNotNullorEmpty()]
    [string]$Hostname = $Null
)

<# Run this as an admin if needed
Install-Module Microsoft.Graph.DeviceManagement
#>

Import-Module Microsoft.Graph.DeviceManagement

# Auth to MSGraph
Connect-MgGraph -Scopes "DeviceManagementManagedDevices.Read.All"

# Select beta if needed
Select-MgProfile -Name "beta"

# Obtain the Azure Object ID
foreach($object in (Get-MgDeviceManagementManagedDevice -All))
{
	# Obtain Device.ID
	$device = (Get-MgDeviceManagementManagedDevice -ManagedDeviceId $object.id)
	
	If($device.deviceName -eq $Hostname)
	{
		foreach($app in (Get-MgDeviceManagementManagedDeviceDetectedApp -ManagedDeviceId $device.id -All))
		{
			Write-Output "$($app.displayName):$($app.Version);"
		}
		break
	}
}
