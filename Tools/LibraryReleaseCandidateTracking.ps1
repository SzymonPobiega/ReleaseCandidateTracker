
<#
.Synopsis
	Creates new release candidate
\#>
function Create-Candidate([string]$server, [string]$product, [string]$version, [string]$state, [string]$scipt_file) {
    $result = .\curl -s -w "|%{http_code}" --data "State=${state}&VersionNumber=${version}&ProductName=${product}" "${server}/API/Create"
    Check-Success $result
    $result = .\curl -s -w "|%{http_code}" --upload-file $scipt_file "${server}/ReleaseCandidateAPI/AttachScript/${version}"   
    Check-Success $result
    Write-Host "Release candidate $version created successfully"             
}

<#
.Synopsis
	Updates the state of a release candidate
\#>
function Update-CandidateState([string]$server, [string]$version, [string]$state) {
    $result = .\curl -s -w "|%{http_code}" --data "State=${state}" "${server}/ReleaseCandidateAPI/UpdateState/${version}"    
    Check-Success $result
    Write-Host "State of release candidate $version changed to $state"
}  


<#
.Synopsis
    Lists top n recent release candidates	
\#>
function List-Candidates([string]$server) {
    $result = .\curl -s "${server}/ReleaseCandidateAPI/List"    
    return $result    
}

<#
.Synopsis
    Gets the version deployed to environments
\#>
function List-Environments([string]$server) {
    $result = .\curl -s "${server}/EnvironmentAPI/List"    
    return $result    
}


<#
.Synopsis
	Updates the state of a release candidate
\#>
function Update-CandidateDeployment([string]$server, [string]$version, [string]$environment, [bool]$success) {
    $result = .\curl -s -w "|%{http_code}" --data "Environment=${environment}&Success=${success}" "${server}/ReleaseCandidateAPI/MarkAsDeployed/${version}"    
    Check-Success $result
    Write-Host "Release candidate $version marked as deployed to $environment environment"
}

function Check-Success($curl_output) {
    $parts = $curl_output.split("|")
    $code = $parts[1]
    $message = $parts[0]
    if ($code -eq "500") {
        throw $message
    }    
}  