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
        Write-Output "Error: $ContainerName is not assigned the expected status or exit code."
        Write-Output "Expected status: $ExpectedStatus"
        Write-Output "Current status: $ContainerState.Status"
        Write-Output "Expected exit code: $ExpectedExitCode"
        Write-Output "Current exit code: $ContainerState.ExitCode"

        return 1
    }
}

# Set parameters and variables
param ($DockerFolderPath)
Set-Variable -Name "DockerComposeOutputFileName" -Value "docker-compose-output.txt"

# Output docker-compose logs
cd $DockerFolderPath
docker-compose logs
docker-compose logs > $DockerComposeOutputFileName

# Check id docker-compose output contains errors
$SEL = Select-String -Path $DockerComposeOutputFileName -Pattern "Error"
if ($SEL -ne $null)
{
    Write-Host "Error: docker-compose run command FAILED! Check output logs for more information."
    return 1
}
else
{
    Write-Host "docker-compose run command SUCCEEDED!"

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