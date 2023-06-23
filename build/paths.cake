public class BuildPaths
{
    public BuildFiles Files { get; private set; }
    public BuildDirectories Directories { get; private set; }
    public ICollection<ChocolateyNuSpecContent> ChocolateyFiles { get; private set; }

    public static BuildPaths GetPaths(
        ICakeContext context,
        string projectName,
        string configuration,
        string semVersion
        )
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }
        if (string.IsNullOrEmpty(configuration))
        {
            throw new ArgumentNullException("configuration");
        }
        if (string.IsNullOrEmpty(semVersion))
        {
            throw new ArgumentNullException("semVersion");
        }

        var buildDir = context.Directory("./bin") + context.Directory("AnyOS.AnyCPU." + configuration);
        var artifactsDir = (DirectoryPath)(context.Directory("./artifacts") + context.Directory("v" + semVersion));
        context.EnsureDirectoryExists(artifactsDir);

        var artifactsBinDir = artifactsDir.Combine("bin");
        var artifactsBinNet45 = artifactsBinDir.Combine("net45");
        var artifactsBinNetCoreApp10 = artifactsBinDir.Combine("netcoreapp1.0");
        var testResultsDir = artifactsDir.Combine("test-results");
         context.EnsureDirectoryExists(testResultsDir);

        var nugetRoot = artifactsDir.Combine("nuget");
        var testingDir = buildDir + context.Directory("./" + projectName + ".Tests");

        var outFiles = new FilePath[] {
            context.File("Mono.CSharp.dll"),
            context.File("Autofac.dll"),
            context.File("NuGet.Core.dll")
        };

        var outAssemblyPaths = outFiles.Concat(new FilePath[] {"LICENSE"})
            .Select(file => buildDir.Path.CombineWithFilePath(file))
            .ToArray();

        var testingAssemblyPaths = new FilePath[] {
            testingDir + context.File(projectName + ".Tests.dll"),
            testingDir + context.File(projectName + ".Tests.pdb"),
            testingDir + context.File(projectName + ".Tests.xml")
        };

        var repoFilesPaths = new FilePath[] {
            "LICENSE",
            "README.md",
            "ReleaseNotes.md"
        };

        var artifactSourcePaths = outAssemblyPaths.Concat(testingAssemblyPaths.Concat(repoFilesPaths)).ToArray();

        var relPath = new DirectoryPath("./").MakeAbsolute(context.Environment).GetRelativePath(artifactsBinNet45.MakeAbsolute(context.Environment));
        var chocolateyFiles = outFiles.Concat(new FilePath[] {"LICENSE"})
            .Select(file => new ChocolateyNuSpecContent {Source = "../" + relPath.CombineWithFilePath(file).FullPath})
            .ToArray();

        var zipArtifactPathCoreClr = artifactsDir.CombineWithFilePath(projectName + "-bin-coreclr-v" + semVersion + ".zip");
        var zipArtifactPathDesktop = artifactsDir.CombineWithFilePath(projectName + "-bin-net45-v" + semVersion + ".zip");

        var testCoverageOutputFilePath = testResultsDir.CombineWithFilePath("OpenCover.xml");

        // Directories
        var buildDirectories = new BuildDirectories(
            artifactsDir,
            testingDir,
            testResultsDir,
            nugetRoot,
            artifactsBinDir,
            artifactsBinNet45,
            artifactsBinNetCoreApp10);

        // Files
        var buildFiles = new BuildFiles(
            context,
            outAssemblyPaths,
            testingAssemblyPaths,
            repoFilesPaths,
            artifactSourcePaths,
            zipArtifactPathCoreClr,
            zipArtifactPathDesktop,
            testCoverageOutputFilePath);

        return new BuildPaths
        {
            Files = buildFiles,
            Directories = buildDirectories,
            ChocolateyFiles = chocolateyFiles
        };
    }
}

public class BuildFiles
{
    public ICollection<FilePath> AssemblyPaths { get; private set; }
    public ICollection<FilePath> TestingAssemblyPaths { get; private set; }
    public ICollection<FilePath> RepoFilesPaths { get; private set; }
    public ICollection<FilePath> ArtifactsSourcePaths { get; private set; }
    public FilePath ZipArtifactPathCoreClr { get; private set; }
    public FilePath ZipArtifactPathDesktop { get; private set; }
    public FilePath TestCoverageOutputFilePath { get; private set; }

    public BuildFiles(
        ICakeContext context,
        FilePath[] outAssemblyPaths,
        FilePath[] testingAssemblyPaths,
        FilePath[] repoFilesPaths,
        FilePath[] artifactsSourcePaths,
        FilePath zipArtifactPathCoreClr,
        FilePath zipArtifactPathDesktop,
        FilePath testCoverageOutputFilePath
        )
    {
        AssemblyPaths = Filter(context, outAssemblyPaths);
        TestingAssemblyPaths = Filter(context, testingAssemblyPaths);
        RepoFilesPaths = Filter(context, repoFilesPaths);
        ArtifactsSourcePaths = Filter(context, artifactsSourcePaths);
        ZipArtifactPathCoreClr = zipArtifactPathCoreClr;
        ZipArtifactPathDesktop = zipArtifactPathDesktop;
        TestCoverageOutputFilePath = testCoverageOutputFilePath;
    }

    private static FilePath[] Filter(ICakeContext context, FilePath[] files)
    {
        // Not a perfect solution, but we need to filter PDB files
        // when building on an OS that's not Windows (since they don't exist there).
        if(!context.IsRunningOnWindows())
        {
            return files.Where(f => !f.FullPath.EndsWith("pdb")).ToArray();
        }
        return files;
    }
}

public class BuildDirectories
{
    public DirectoryPath Artifacts { get; private set; }
    public DirectoryPath Tests { get; private set; }
    public DirectoryPath TestResults { get; private set; }
    public DirectoryPath NugetRoot { get; private set; }
    public DirectoryPath ArtifactsBin { get; private set; }
    public DirectoryPath ArtifactsBinNet45 { get; private set; }
    public DirectoryPath ArtifactsBinNetCoreApp10 { get; private set; }
    public ICollection<DirectoryPath> ToClean { get; private set; }

    public BuildDirectories(
        DirectoryPath artifactsDir,
        DirectoryPath testingDir,
        DirectoryPath testResultsDir,
        DirectoryPath nugetRoot,
        DirectoryPath artifactsBinDir,
        DirectoryPath artifactsBinNet45,
        DirectoryPath artifactsBinNetCoreApp10
        )
    {
        Artifacts = artifactsDir;
        Tests = testingDir;
        TestResults = testResultsDir;
        NugetRoot = nugetRoot;
        ArtifactsBin = artifactsBinDir;
        ArtifactsBinNet45 = artifactsBinNet45;
        ArtifactsBinNetCoreApp10 = artifactsBinNetCoreApp10;
        ToClean = new[] {
            Artifacts,
            TestResults,
            NugetRoot,
            ArtifactsBin,
            ArtifactsBinNet45,
            ArtifactsBinNetCoreApp10
        };
    }
}