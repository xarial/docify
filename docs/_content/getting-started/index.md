---
caption: Getting Started
title: Getting started with Docify for generating static sites
description: Getting started with Docify. Installing and updating Docify tool. Building your first static site
image: vscode-build-site.png
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

## Development Environment

Docify files are plain text file and any compatible text editor can be used. The recommended text editors are [Visual Studio Code](https://code.visualstudio.com/), [Notepad++](https://notepad-plus-plus.org/), [Atom](https://atom.io/), [Visual Studio](https://visualstudio.microsoft.com/)

![Building site using Docify in Visual Studio Code](vscode-build-site.png)

## Building Static Sites

To build a simple site, use the following command

~~~
> docify build --src C:\MySite --out C:\MyOutput --host https://example.com
~~~

All files from the *C:\MySite* folder will be published to the *C:\MyOutput*.

It is possible to specify multiple folders for compilation

~~~
> docify build --src C:\MySiteDir1 C:\MySiteDir2 --out C:\MyOutput --host https://example.com
~~~

This can be useful to separate the content and the frames and layouts.

If *--src* is not specified, current working folder is used as the source for the site.

*--host* parameter is optional, but some components might need to use full url including the host, e.g. SEO components or canonical url generator.

If site is hosted in sub-folder (e.g. https://example.com/root/{site}), use *--base* parameter to specify base url. Docify will automatically update the references for scripts, styles, images and links which are using absolute url (e.g. start with /) to use base url.

~~~
> docify build --src C:\MySite --out C:\MyOutput --host https://example.com --base /root/
~~~

This will be required if using [GitHub Pages](https://pages.github.com/) for hosting the site with the default url as url will be generated in the following format: *https://{user-name}.github.io/{repo-name}/*. Use *--base /{repo-name}/*

### Using Libraries

In order to use [custom library](/custom-library/) use *--l* switch and specify the path to the library directory

~~~
> docify build --src C:\MySite C:\MySiteDir2 --out C:\MyOutput --host https://example.com --l D:\MyLibrary
~~~

To use the [secure library](/custom-library#secure-library/), specify the path to manifest and public key XML separated by |

~~~
> docify build --src C:\MySite C:\MySiteDir2 --out C:\MyOutput --host https://example.com --l D:\MySecureLibrary.manifest|D:\SecureLibraryPublicKey.xml
~~~

To use standard library specify * as a library path

~~~
> docify build --src C:\MySite C:\MySiteDir2 --out C:\MyOutput --host https://example.com --l *
~~~

On Linux systems (such as ubuntu) or MacOS it is required to protect the * library symbol, by enclosing it into quotes '*'

### Using Environment

Environment is an optional *--env* parameter allowing to define the current environment of the site, e.g. test, staging, production. Some [includes](/includes/) or [plugins](/plugins/) may refer the environment to enable or disable certain functionality

~~~
> docify build --env test
~~~

## Serving The Site

For design purpose, site can be served, in this case site will be built to temp location (unless explicitly specified) and served in the *localhost* so it can be accessed in any browser without the need to configure the host.

~~~
> docify serve
~~~

The url of the site as well as temp directory will be output to the console. Press any key to stop the host and delete all temp files.

Explore more options of *serve* command by using the *--help* switch

~~~
> docify serve --help
~~~

## Troubleshooting

Docify will output the common errors into the console allowing to investigate the cause of an issue. It is additionally possible to log additional information, such as exceptions stack trace and detailed steps report by using *--verbose* option.

~~~
> docify build --src C:\MySite --out C:\MyOutput --host https://example.com --l * --verbose
~~~

## Example

* Create a folder at D:\MySite
* Add new text file *index.md*
* Place the following content into the file

~~~ md
# My Site

Hello World from [Docify](https://docify.net)!
~~~

* If not already installed, run the following command to install docify (note, do not copy > symbol, it is used to indicate the command line):

~~~
> dotnet install -g docify
~~~

* Run the following command to build the site

~~~
> docify build --src D:\MySite --out D:\MyOutput --host https://example.com
~~~

* As the result static markdown is converted into index.html page:

![Example html page opened in internet browser](example-site-html.png)

~~~ html
<h1 id="my-site">My Site</h1>
<p>Hello World from <a href="https://docify.net">Docify</a>!</p>
~~~

Follow this user guide to learn more features of Docify which allows you to create websites for your blog or technical documentation. This help documentation itself is generated by Docify. You can explore the source code for documentation [here](https://github.com/xarial/docify/tree/master/docs)
