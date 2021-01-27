# AppxInstaller

Appx installation utility.

Description: 

A utility application that installs the content of an embedded Appx and it certificate file resource.

## Building
* In MainWindow.xaml.cs adjust string constants ProductName, ProductVersion and HelpMessage to your requirements.
* Replace the Resources content with your Appx files and folders.
* Publish with "Produce single file" option to create self contained executable with your embedded resource Appx and certificate.

## Usage
* Select Help ? for usage details.
* You will be prompted to install .Net 5 if it is not present.
* Unblock this executable if it is downloaded from a trusted source.

Note: This is a WIP POC that was created to reduce Appx installation to one button click.

## Screenshot
![Screenshot](https://github.com/Noemata/AssetInstaller/raw/master/Screenshot.png)
