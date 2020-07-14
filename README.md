[![User Guide](https://img.shields.io/badge/-Documentation-green.svg)](https://docify.xarial.com)

<img src="data/icon.png" width="100" />

[Docify](https://www.docify.net) - content oriented static site generator developed in .NET Core. Best suited for developing blogs, technical user documentation, help files.

## Features

* [Transforming the markdown into the html](https://docify.xarial.com/content/static/)
* [Layouts support](https://docify.xarial.com/layouts/)
* [Includes support (pseudo-dynamic) in C# Razor Pages syntax](https://docify.xarial.com/content/dynamic/)
* [Plugins in .NET Core](https://docify.xarial.com/plugins/)
* [Themes support](https://docify.xarial.com/custom-library/themes/)
* [Standard library of components, plugins and themes](https://docify.xarial.com/standard-library/)

## CLI
[![NuGet version (Docify)](https://img.shields.io/nuget/v/Docify.svg?style=flat-square)](https://www.nuget.org/packages/Docify/)
[![Build status](https://dev.azure.com/xarial/docify/_apis/build/status/cli)](https://dev.azure.com/xarial/docify/_build/latest?definitionId=21)

CLI can be accessed from the command line and published as global .NET Core Tool

To install

> dotnet tool install -g docify

Build the site using the **build** command

> docify build --src "{Source Folder}" --out "{Output Folder}"

## Library

[![Build status](https://dev.azure.com/xarial/docify/_apis/build/status/lib)](https://dev.azure.com/xarial/docify/_build/latest?definitionId=22)

Standard library can be installed using the following command

> docify library --install

To use the standard library specify the **--lib** argument and pass the * as parameter

> docify build --src "{Source Folder}" --out "{Output Folder}" --lib *