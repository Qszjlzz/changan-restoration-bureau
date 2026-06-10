param(
    [string]$BuildRoot = "",
    [int]$LaunchCount = 2,
    [int]$LaunchTimeoutSec = 25,
    [ValidateSet("careful_exhibit", "quick_reuse")]
    [string]$Branch = "careful_exhibit"
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($BuildRoot)) {
    $BuildRoot = Join-Path (Split-Path $PSScriptRoot -Parent) "Build\ChanganRestorationBureau"
}

function Remove-ExistingProcess {
    $existing = Get-Process ChanganRestorationBureau -ErrorAction SilentlyContinue
    if ($existing) {
        $existing | Stop-Process -Force
        Start-Sleep -Milliseconds 500
    }
}

$exePath = Join-Path $BuildRoot "ChanganRestorationBureau.exe"
if (-not (Test-Path $exePath)) {
    throw "Missing build: $exePath"
}

$playerLog = Join-Path $env:USERPROFILE "AppData\LocalLow\DefaultCompany\ChanganRestorationBureau\Player.log"
$markers = @(
    "[Changan] ProofRuntimeBootstrap.Awake complete",
    "[Changan] Commission accepted id=lotus_roof_tile_night_market",
    "[Changan] Objective marked cleared",
    "[Changan] Objective marked sampled",
    "[Changan] Objective marked repaired",
    "[Changan] Day summary shown",
    "[Changan] Smoke autoplay completed"
)

if ($Branch -eq "careful_exhibit") {
    $markers += "[Changan] Objective marked displayed"
    $markers += "[Changan] Outcome resolved id=careful_exhibit"
} else {
    $markers += "[Changan] Outcome resolved id=quick_reuse"
}

$runResults = @()

for ($launchIndex = 1; $launchIndex -le $LaunchCount; $launchIndex++) {
    Remove-ExistingProcess
    if (Test-Path $playerLog) {
        Remove-Item -LiteralPath $playerLog -Force
    }

    $process = Start-Process -FilePath $exePath -WorkingDirectory $BuildRoot -ArgumentList '-screen-fullscreen','0','-screen-width','1280','-screen-height','720','-smoke-play','-smoke-branch',$Branch -PassThru
    $completed = $process.WaitForExit($LaunchTimeoutSec * 1000)
    if (-not $completed) {
        Stop-Process -Id $process.Id -Force
        throw "Smoke launch $launchIndex timed out."
    }

    if (-not (Test-Path $playerLog)) {
        throw "Missing Player.log after launch $launchIndex."
    }

    $logText = Get-Content $playerLog -Raw
    $missingMarkers = @($markers | Where-Object { $logText -notmatch [regex]::Escape($_) })
    $crashSignal = $logText -match "level0|Position out of bounds|Crash!!!|NullReferenceException|Smoke autoplay failed"

    $runResults += [pscustomobject]@{
        Launch = $launchIndex
        ExitCode = $process.ExitCode
        MissingMarkers = ($missingMarkers -join "; ")
        CrashSignal = $crashSignal
    }
}

"Smoke play report"
foreach ($result in $runResults) {
    "Launch=$($result.Launch) ExitCode=$($result.ExitCode) CrashSignal=$($result.CrashSignal) MissingMarkers=$($result.MissingMarkers)"
}

$failed = $runResults | Where-Object { $_.ExitCode -ne 0 -or $_.CrashSignal -or $_.MissingMarkers }
if ($failed) {
    throw "Smoke play failed. See report above."
}

"Smoke play passed."
