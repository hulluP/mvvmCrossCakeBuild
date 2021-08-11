#addin "Cake.Xamarin&version=3.1.0"
#addin "Cake.FileHelpers&version=4.0.1"
#addin "Cake.AndroidAppManifest&version=1.1.2"
#addin "Cake.Plist&version=0.7.0"

var assemblyInfoFile = File("./CommonAssemblyInfo.cs");

var solutionFile = File("./MyXamarinFormsApp.sln");

var appleProjectFile = File("./src/MyXamarinFormsApp.iOS/MyXamarinFormsApp.iOS.csproj");
var plistFile = File("./src/MyXamarinFormsApp.iOS/Info.plist");

var androidProjectFile = File("./src/MyXamarinFormsApp.Droid/MyXamarinFormsApp.Droid.csproj");
var manifestFile = File("./src/MyXamarinFormsApp.Droid/Properties/AndroidManifest.xml");

// should MSBuild treat any errors as warnings.
var treatWarningsAsErrors = "false";

// Parse release notes
var releaseNotes = ParseReleaseNotes("./RELEASENOTES.md");

// Get version
var version = releaseNotes.Version.ToString();
var epoch = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
var semVersion = string.Format("{0}.{1}", version, epoch);

   
Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("BuildApple")
    .Does (() =>
{

});    

Task("Clean")
    .Does (() =>
{
    var iOsBin = "./src/MyXamarinFormsApp.iOS/bin";

    if (DirectoryExists(iOsBin)) 
    {
        DeleteDirectory(iOsBin, new DeleteDirectorySettings {
        Recursive = true,
        Force = true });
    }
    var iOsObj = "./src/MyXamarinFormsApp.iOS/obj/";
    if (DirectoryExists(iOsObj))
    {
        DeleteDirectory(iOsObj, new DeleteDirectorySettings {
        Recursive = true,
        Force = true });
        }
    var androidBin = "./src/MyXamarinFormsApp.Droid/bin/";
   if (DirectoryExists(androidBin))
    {    DeleteDirectory(androidBin, new DeleteDirectorySettings {
    Recursive = true,
    Force = true });
    }
    var AndroidObj = "./src/MyXamarinFormsApp.Droid/obj/";
    if (DirectoryExists(AndroidObj))
    {
    DeleteDirectory(AndroidObj, new DeleteDirectorySettings {
    Recursive = true,
    Force = true });
    }
});    

Task("BuildApple")
    .IsDependentOn("RestorePackages")
    .IsDependentOn("UpdateAssemblyInfo")
    .IsDependentOn("UpdateApplePlist")
    .Does (() =>
{
    // debug build used for Xamarin UI Test
    MSBuild(appleProjectFile, settings =>
      settings.SetConfiguration("Debug")
          .WithTarget("Build")
          .WithProperty("Platform", "iPhoneSimulator")
          .WithProperty("OutputPath", "bin/iPhoneSimulator/")
          .WithProperty("BuildIpa", "true")
          .WithProperty("TreatWarningsAsErrors", treatWarningsAsErrors));


});

Task("UpdateApplePlist")
    .Does (() =>
{
    dynamic plist = DeserializePlist(plistFile);

    plist["CFBundleShortVersionString"] = version;
    plist["CFBundleVersion"] = semVersion;

    SerializePlist(plistFile, plist);
});

Task("UpdateAssemblyInfo")
    .Does (() =>
{
    CreateAssemblyInfo(assemblyInfoFile, new AssemblyInfoSettings() {
        Product = "MyXamarinFormsApp",
        Version = version,
        FileVersion = version,
        InformationalVersion = semVersion,
        Copyright = "Copyright (c) Level28"
    });
});




Task("RestorePackages")
    .Does (() =>
{
    NuGetRestore(solutionFile);
});

RunTarget("Build");