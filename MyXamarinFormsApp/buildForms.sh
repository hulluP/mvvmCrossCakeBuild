#!/bin/sh
mono tools/nuget/nuget.exe update -self
mono tools/nuget/nuget.exe install xunit.runner.console -OutputDirectory tools -ExcludeVersion
mono tools/nuget/nuget.exe install Cake -OutputDirectory tools -ExcludeVersion
dotnet dotnet-cake build.cake 
xcrun simctl install booted  src/MyXamarinFormsApp.iOS/bin/iPhoneSimulator/MyXamarinFormsApp.iOS.app 