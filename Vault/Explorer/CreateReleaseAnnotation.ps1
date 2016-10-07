# Copyright (c) Microsoft Corporation. All rights reserved. 
# Licensed under the MIT License. See License.txt in the project root for license information. 

# Copied from https://github.com/Microsoft/ApplicationInsights-Home/blob/master/API/CreateReleaseAnnotation.ps1
# See also https://azure.microsoft.com/en-us/documentation/articles/app-insights-annotations/
# Sample usage .\CreateReleaseAnnotation.ps1 -applicationId "<appId>" -apiKey "<apiKey>" -releaseFilePath "<path to .exe with file version>" -releaseProperties @{"ReleaseDescription"="Release with annotation";"TriggerBy"="John Doe"}
param(
    [parameter(Mandatory = $true)][string]$applicationId,
    [parameter(Mandatory = $true)][string]$apiKey,
    [parameter(Mandatory = $true)][string]$releaseFilePath,
    [parameter(Mandatory = $false)]$releaseProperties
)

$releaseName = (Get-Item $releaseFilePath).VersionInfo.FileVersion
Write-Host "Creating release annotation $releaseName in ApplicationInsights" -ForegroundColor Cyan

# background info on how fwlink works: After you submit a web request, many sites redirect through a series of intermediate pages before you finally land on the destination page.
# So when calling Invoke-WebRequest, the result it returns comes from the final page in any redirect sequence. Hence, I set MaximumRedirection to 0, as this prevents the call to 
# be redirected. By doing this, we get a resposne with status code 302, which indicates that there is a redirection link from the response body. We grab this redirection link and 
# construct the url to make a release annotation.
# Here's how this logic is going to works
# 1. Client send http request, such as:  http://go.microsoft.com/fwlink/?LinkId=625115
# 2. FWLink get the request and find out the destination URL for it, such as:  http://www.bing.com
# 3. FWLink generate a new http response with status code “302” and with destination URL “http://www.bing.com”. Send it back to Client.
# 4. Client, such as a powershell script, knows that status code “302” means redirection to new a location, and the target location is “http://www.bing.com”
function GetRequestUrlFromFwLink($fwLink)
{
    $request = Invoke-WebRequest -Uri $fwLink -MaximumRedirection 0 -UseBasicParsing -ErrorAction Ignore
    if ($request.StatusCode -eq "302") {
        return $request.Headers.Location
    }
    
    return $null
}

function CreateAnnotation($grpEnv)
{
    $retries = 1
    $success = $false
    while (!$success -and $retries -lt 6) {
        $location = "$grpEnv/applications/$applicationId/Annotations?api-version=2015-11"
            
        Write-Host "Invoke a web request for $location to create a new release annotation. Attempting $retries"
        set-variable -Name createResultStatus -Force -Scope Local -Value $null
        set-variable -Name createResultStatusDescription -Force -Scope Local -Value $null
        set-variable -Name result -Force -Scope Local

        try {
            $result = Invoke-WebRequest -Uri $location -Method Put -Body $bodyJson -Headers $headers -ContentType "application/json; charset=utf-8" -UseBasicParsing
        } catch {
            if ($_.Exception){
                if($_.Exception.Response) {
                    $createResultStatus = $_.Exception.Response.StatusCode.value__
                    $createResultStatusDescription = $_.Exception.Response.StatusDescription
                }
                else {
                    $createResultStatus = "Exception"
                    $createResultStatusDescription = $_.Exception.Message
                }
            }
        }

        if ($result -eq $null) {
            if ($createResultStatus -eq $null) {
                $createResultStatus = "Unknown"
            }
            if ($createResultStatusDescription -eq $null) {
                $createResultStatusDescription = "Unknown"
            }
        }
        else {
                $success = $true			         
        }

        if ($createResultStatus -eq 409 -or $createResultStatus -eq 404 -or $createResultStatus -eq 401) # no retry when conflict or unauthorized or not found
        {
            break
        }

        $retries = $retries + 1
        sleep 1
    }

    $createResultStatus
    $createResultStatusDescription
    return
}

# Need powershell version 3 or greater for script to run
$minimumPowershellMajorVersion = 3
if ($PSVersionTable.PSVersion.Major -lt $minimumPowershellMajorVersion) {
   Write-Host "Need powershell version $minimumPowershellMajorVersion or greater to create release annotation"
   return
}

$currentTime = (Get-Date).ToUniversalTime()
$annotationDate = $currentTime.ToString("MMddyyyy_HHmmss")
set-variable -Name requestBody -Force -Scope Script
$requestBody = @{}
$requestBody.Id = [GUID]::NewGuid()
$requestBody.AnnotationName = $releaseName
$requestBody.EventTime = $currentTime.GetDateTimeFormats("s")[0] # GetDateTimeFormats returns an array
$requestBody.Category = "Deployment"

if ($releaseProperties -eq $null) {
    $properties = @{}
} else {
    $properties = $releaseProperties    
}
$properties.Add("ReleaseName", $releaseName)

$requestBody.Properties = ConvertTo-Json($properties) -Compress

$bodyJson = [System.Text.Encoding]::UTF8.GetBytes(($requestBody | ConvertTo-Json))
$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
$headers.Add("X-AIAPIKEY", $apiKey)

set-variable -Name createAnnotationResult1 -Force -Scope Local -Value $null
set-variable -Name createAnnotationResultDescription -Force -Scope Local -Value ""

# get redirect link from fwlink
$requestUrl = GetRequestUrlFromFwLink("http://go.microsoft.com/fwlink/?prd=11901&pver=1.0&sbp=Application%20Insights&plcid=0x409&clcid=0x409&ar=Annotations&sar=Create%20Annotation")
if ($requestUrl -eq $null) {
    $output = "Failed to find the redirect link to create a release annotation"
    throw $output
}

$createAnnotationResult1, $createAnnotationResultDescription = CreateAnnotation($requestUrl)
if ($createAnnotationResult1) 
{
     $output = "Failed to create an annotation with Id: {0}. Error {1}, Description: {2}." -f $requestBody.Id, $createAnnotationResult1, $createAnnotationResultDescription
     throw $output
}

$str = "Release annotation created. Id: {0}." -f $requestBody.Id
Write-Host $str -ForegroundColor Green

