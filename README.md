# AppxInstaller

Appx installation utility.

# ! BROKEN !

Use .NET Core 3.1 version instead: https://github.com/Noemata/AppxInstaller.Core

WPF + .NET 5 + WinRT does not yet appear to be ready for prime time :sob:
Multiple issues exist in terms of WinRT API access and generating a valid self contained executable.
Please let me know when these are fixed so I can remove this disclaimer.

Description: 

A utility application that installs the content of an embedded Appx and its Certificate file.

## Building
* In MainWindow.xaml.cs adjust string constants PackageName, ProductName, ProductVersion, HelpMessage, BundleName and CertificateName to your requirements.
* Replace the Resources content with your Appx files and folders.
* Replace the Certificate content with your Appx Certificate file (be sure to set the content property to "Embedded resource")
* Publish with "Produce single file" option to create self contained executable with your embedded resource Appx and Certificate.

## Usage
* Select Help ? for usage details.
* You will be prompted to install .Net 5 if it is not present.
* Unblock this executable if it is downloaded from a trusted source.

Note: This is a WIP POC that was created to reduce Appx sideloaded installation to one button click.  The Appx assets Resources folder sits next to the executable.  The certificate file remains as an embedded resource within the exectuable binary.  You could opt to encrypt the certificate file.

## Credits
* UI ideas: https://github.com/oleg-shilo/wixsharp
* IInitializeWithWindow lib: https://github.com/mveril
* Package handling ideas: https://github.com/colinkiama/UWP-Package-Installer , https://github.com/UWPX/UWPX-Installer

## Screenshot
![Screenshot](https://github.com/Noemata/AppxInstaller/raw/master/Screenshot.png)
