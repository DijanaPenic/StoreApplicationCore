#!/usr/bin/env pwsh

# Set parameters
param ($DockerFolderPath, $LogsPath)

# Functions
function Check-DockerContainer {
    param (
        [string]$ContainerName,
        [string]$ExpectedStatus,
        [int]$ExpectedExitCode = 0
    )

    $ContainerName = "docker_$ContainerName"
    $ContainerState = $(docker container inspect -f '{{json .State}}' $ContainerName) | ConvertFrom-Json

    if ($ContainerState.Status -ne $ExpectedStatus -or $ContainerState.ExitCode -ne $ExpectedExitCode)
    {
        Write-Error "Error: $ContainerName is not assigned the expected status or exit code."
        Write-Output "Expected status: $ExpectedStatus"
        Write-Output "Expected exit code: $ExpectedExitCode"
    }
}

# Set variables
Set-Variable -Name "DockerComposeUpFilePath" -Value "$LogsPath/docker-compose-up.txt"

# Output docker-compose logs
cd $DockerFolderPath
docker-compose logs > $DockerComposeUpFilePath
docker-compose ps

# Check id docker-compose output contains errors
$SEL = Select-String -Path $DockerComposeUpFilePath -Pattern "Error"
if ($SEL -ne $null)
{
    Write-Error "Error: docker-compose run command FAILED! Check output logs for more information (Store > Docker > Logs)."
}
else
{
    Write-Output "docker-compose run command SUCCEEDED!"

    # Check containers status
    $RunningContainers = 
    @(
        'postgres-server_1',
        'redis-master_1',
        'redis-sentinel_1',
        'redis-sentinel_2',
        'redis-sentinel_3', 
        'redis-slave-1_1', 
        'redis-slave-2_1', 
        'web-api_1'
     )
    foreach ($Container in $RunningContainers)
    {
        Check-DockerContainer -ContainerName $Container -ExpectedStatus "running"
    }

    $FinishedContainers = 
    @(
        'database-update_1'
     )
    foreach ($Container in $FinishedContainers)
    {
        Check-DockerContainer -ContainerName $Container -ExpectedStatus "exited"
    }
}
