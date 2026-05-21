$ErrorActionPreference = "Stop"

$Unity = "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe"
$Project = Join-Path $PSScriptRoot "unity\ChanganRestorationBureau"
$Log = Join-Path $PSScriptRoot "unity-build-proof.log"

& $Unity -batchmode -quit -projectPath $Project -executeMethod ChanganProofSceneBuilder.BuildAssetProofScene -logFile $Log
& $Unity -batchmode -quit -projectPath $Project -executeMethod ChanganProofSceneValidator.ValidateProofScene -logFile (Join-Path $PSScriptRoot "unity-validate-proof.log")
& $Unity -batchmode -quit -projectPath $Project -executeMethod ChanganProofBuild.BuildWindowsProof -logFile (Join-Path $PSScriptRoot "unity-build-player.log")

Write-Host "Unity proof scene, validation, and Windows build commands finished."
