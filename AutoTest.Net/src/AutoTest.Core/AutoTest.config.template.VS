<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <!-- Build executables. Supported are MSBuild.exe and Mono's xbuild (Optional) -->
  <!-- You have the possibility to specify multiple build executables based on framework version or -->
  <!-- product version from within the csproj/vbproj file using the framework attribute. -->
  <!-- If you remove all configured build executables AutoTest.NET will start tracking assembly changes -->
  <!-- instead and you can use Visual Studio or equal to build and AutoTest.NET will -->
  <!-- run all tests for any built assembly. -->
  <BuildExecutable parameters="">C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe</BuildExecutable>
  
  <!-- If set it will always use msbuild to build solution file instead of only changed projects -->
  <!-- when AutoTest.NET is set to watch a solution file -->
  <WhenWatchingSolutionBuildSolution>true</WhenWatchingSolutionBuildSolution>
  
  <!-- Machine.Specifications test runner. (Optional) -->
  <MachineSpecificationsTestRunner>C:\WhereverYouHave\MSpec\mspec.exe</MachineSpecificationsTestRunner>
  
  <!-- Makes the file change watcher start in paused state -->
  <!-- To have changes run in this mode you will have to publish FileChangeMessages manually -->
  <StartPaused>true</StartPaused>
  
  <!-- Code editor configuration. You can specify a code editor to open files in. (Optional) -->
  <!-- The [[CodeFile]] tag is the file to be edited and the [[LineNumber]] tag is -->
  <!-- the line to goto. These will be filled in by the application when executing -->
  <!-- the statement -->
  <CodeEditor>
  <Executable>C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe</Executable>
  <Arguments>/Edit "[[CodeFile]]" /command "Edit.Goto [[LineNumber]]"</Arguments>
  </CodeEditor>
  
  <!-- When set this will enable logging to the debug.log file in the root directory -->
  <Debugging>false</Debugging>
  
  <!-- Additional parameters to msbuild command line (Optional) -->
  <!-- <MSBuildAdditionalParameters></MSBuildAdditionalParameters> -->
  
  <!-- To change the default recycle size of the logs (1024000 = 1M) Set the number of bytes you -->
  <!-- want the logs to roll at here -->
  <!--<LogRecycleSize>1024000000</LogRecycleSize>-->
  
  <!-- Rerun failed tests before doing a full test run (Optional) -->
  <!--<RerunFailedTestsFirst>true</RerunFailedTestsFirst>-->
  
  <!-- Directory to watch (Optional) -->
  <!-- You can have zero or multiple watch directories specified in the config file. -->
  <!-- When AutoTest.NET starts it will let you pick from the specified directories -->
  <!-- or you can choose to pass a directory when executing the application. Usually -->
  <!-- the solution root folder -->
  <!--<DirectoryToWatch>C:\WhateverDirectoryYouWantTo\Watch</DirectoryToWatch>-->
  
  <!-- Whether to use AutoTest.NET's own test runner (Optional)-->
  <!--<UseAutoTestTestRunner>false</UseAutoTestTestRunner>-->
  
  <!-- Testing test assemblies in parallel-->
  <!--<RunAssembliesInParallel>true</RunAssembliesInParallel>-->
  
  <!-- NUnit test runner. Specifies the path to the nunit console test runner (Optional) -->
  <!-- Default NUnit runner is AutoTest.TestRunner. This is an optional console runner -->
  <!-- User <UseAutoTestTestRunner>false</UseAutoTestTestRunner> to disable default runner -->
  <!-- You have the possibility to specify multiple build executables based on framework version -->
  <!-- from within the csproj/vbproj file using the framework attribute. -->
  <!-- (framework only, not framework and product version like with the build exe) -->
  <!--<NUnitTestRunner>C:\WhereverYouHave\NUnit\nunit-console.exe</NUnitTestRunner>-->
  
  <!-- XUnit test runner. (Optional) -->
  <!-- Default XUnit runner is AutoTest.TestRunner. This is an optional console runner -->
  <!-- User <UseAutoTestTestRunner>false</UseAutoTestTestRunner> to disable default runner -->
  <!-- Has support for the framework attribute like NUnit test runner config -->
  <!--<XUnitTestRunner>C:\WhereverYouHave\XUnit\xunit.console.exe</XUnitTestRunner>-->
  
  <!-- MSTest test runner. (Optional) -->
  <!-- Has support for the framework attribute like NUnit test runner config -->
  <!--<MSTestRunner>C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\MSTest.exe</MSTestRunner>-->
  
  <!-- Wildcard expressions for excluding test assemblies (Optional)-->
  <!--<ShouldIgnoreProject>-->
  <!--  <Project>*MyIgnoredProject.csproj</Project>-->
  <!--</ShouldIgnoreProject>-->

  <!-- Wildcard expressions for excluding test assemblies (Optional)-->
  <!--<ShouldIgnoreTestAssembly>-->
  <!--	<Assembly>*whatever.dll</Assembly>-->
  <!--</ShouldIgnoreTestAssembly>-->
  
  <!-- Test categories to ignore. Only supported by NUnit (Optional) -->
  <!--<ShouldIgnoreTestCategories>-->
  <!--	<Category>MyIgnoreCategory</Category>-->
  <!--</ShouldIgnoreTestCategories>-->
  
  <!-- Notification provider (Optional) -->
  <!-- The supported providers right now are notify-send on linux and growl on -->
  <!-- windows and Mac. notify-send will configure automatically while growl -->
  <!-- might need some configuration. You can specify when to push notifications-->
  <!-- below. -->
  <!--<notify_on_run_started>true</notify_on_run_started>-->
  <!--<notify_on_run_completed>true</notify_on_run_completed>-->
  	
  <!-- Growl notify path (Optional) -->
  <!-- If growl doesn't work out of the box use the setting below to set the path -->
  <!-- to growlnotify(.exe) -->
  <!--<growlnotify>Path to growlnotify application</growlnotify>-->
  	
  <!-- To specify a set of files\directories that autotest should not track changes (Optional) -->
  <!-- for create a file containing those exceptions. All patterns should use / -->
  <!-- instead of \ It works the same way as the .gitignore file in the git source -->
  <!-- control system. (bin/Debug, bin/Release, bin/x86, obj/Debug, obj/Release and -->
  <!-- obj/x86 are ignored by default. The file is expected at the root of the path to watch -->
  <!-- This setting is not in use when using assembly tracking -->
  <!--<IgnoreFile>.gitignore</IgnoreFile>-->
  	
  <!-- Change detection delay (Optional) -->
  <!-- The time in milliseconds that AuotTest.NET will wait between finding changes -->
  <!-- and running build and tests -->
  <!--<changedetectiondelay>200</changedetectiondelay>-->
  
  <!-- Watch all files in watch directories (Optional) -->
  <!-- By default AutoTest.NET only looks for changes to files belonging to watched projects -->
  <!-- With this setting it will look for changes to all files in watched folders -->
  <!--<WatchAllFiles>true</WatchAllFiles>-->
  
  <!-- Custom output folder (Optional) -->
  <!-- Custom folder to use as as build output folder -->
  <!--<CustomOutput>bin\MyOwnOutputFolder</CustomOutput>-->
  
  <!-- Runs test runner in compatibility mode. A lot of test runners out there simply swallow -->
  <!-- app domain and background thread exception. Compatibility mode does the same -->
  <!--<TestRunnerCompatibilityMode>true</TestRunnerCompatibilityMode>-->
  
  <!-- Adjust watch path to nearest common path (Optional) -->
  <!-- Whether to adjust watch path to the lowest common path used by all projects -->
  <!-- included in a solution -->
  <!--<UseLowestCommonDenominatorAsWatchPath>false</UseLowestCommonDenominatorAsWatchPath>-->
</configuration>
