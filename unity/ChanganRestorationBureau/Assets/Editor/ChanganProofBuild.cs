using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ChanganProofBuild
{
    private const string ScenePath = ChanganPlayableProofSceneBuilder.ScenePath;
    private const string BuildFolder = "../../../Build/ChanganRestorationBureau";

    public static void BuildWindowsProof()
    {
        ChanganPlayableProofSceneBuilder.BuildScene();
        ChanganProofSceneValidator.ValidateProofScene();
        EditorSceneManager.OpenScene(ScenePath);

        var absoluteBuildFolder = Path.GetFullPath(Path.Combine(Application.dataPath, BuildFolder));
        Directory.CreateDirectory(absoluteBuildFolder);
        var exePath = Path.Combine(absoluteBuildFolder, "ChanganRestorationBureau.exe");

        var options = new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = exePath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.CleanBuildCache
        };

        var report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new System.Exception($"Build failed: {report.summary.result}");
        }

        Debug.Log($"Built Windows proof: {exePath}");
    }
}
