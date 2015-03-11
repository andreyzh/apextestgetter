#Apex Test Getter

This program can be used to get the list of all internal APEX test classes in a particular SalesForce.com org. It is meant to be used together with continous integration server and Force.com migration tool since it will take build file as main input. 

It was created to address the irritating fact that Force.com runAllTests attribute will run BOTH internal tests and managed packages which is not always desired. If you want to run only internal tests the workaround was to specify each one manually using the <runTest> elements. Of course once test class names changed or new classes added, you would have to update the list manually. Apex Test Getter does just that for you.

Apex Test Getter is written in C# and will work on Windows OS with .NET framework 4 or greater. It was not tested on Linux, but can probably be compiled in Mono with minimal or no changes.

Instructions for Jenkins:
- Extract executable and dll into the directory where Jenkins can access it
- Add a build step "Execute Windows Batch command" in Jenkins job BEFORE you invoke ANT
Example: "F:\Program Files\ApexTestGetter\KONE.SFDC.ApexTestGetter.exe" "F:\Jenkins\projects\devint\build.xml" "runTests"
First parameter is location of the build file second parameter is ANT target. You can check the build and properties files example in "Samples" directory
- Login credentials for SalesForce will be taken from build propeties file which is linked from the build file
- Optional parameter is /f which will filter out tests from the excludetests.txt file which you should create in the same directory as your build and properties file
