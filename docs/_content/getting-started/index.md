---
caption: Getting Started
title: Getting started with Docify for generating static sites
description: Overview of Docify engine to generate static sites for technical documentation, blogs and user guide
image: 
order: 1
---
Docify is available as [.NET Core Global Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) and published on [Nuget.org](https://www.nuget.org/packages/Docify/).

## Installing And Updating Docify

Docify can be installed by running the following command:

~~~
> dotnet install -g docify
~~~

To update to newer version run

~~~
> dotnet update -g docify
~~~

Once installed, explore the options by calling the *--help* of *docify*

~~~
> docify --help
~~~

## Building Static Sites

To build a simple site use the following command

~~~
> docify build --src C:\MySite --out C:\MyOutput
~~~