---
caption: Custom Library
title: Managing custom library in Docify
description: Creating reusable library components, plugins and themes in Docify
order: 10
---
Library is a way to reuse components, define plugins and themes. In addition to [standard library](/standard-library/) it is possible to create custom library stored on the local drive.

There are currently 2 types of custom library supported. Those are the types related to loading the library by Docify engine, while syntax of components, plugins, and themes and their usage in the site will be the same regardless of the type of the library.

Library elements need to follow specific rules regarding the folder structure. All components must be placed into the *_components* special folders, while plugins and themes need to be placed in *_plugins* and themes *_themes* folders respectively.

Sub-folders of the above elements would be equal to each individual name and sub-folder name equals to an item name. All of the files and folders of this sub-folder considered to be related to this library item.

For example, creating library components *my-component* which will have [include](/includes/) named *my-include* and image file *images/logo.png* for the library located at *D:\lib* would result into the following folders structure

~~~
D:\lib\_components\my-component\_includes\my-include.cshtml
D:\lib\_components\my-component\images\logo.png
~~~

Refer [components](components/), [plugins](plugins/) and [themes](themes/) sections for more information about managing specific library items.

## Folder Based Library

This is a simple option to create a custom library. In this case, it is required to store the library items in the folder structure defined above.

In order to load this component into the site, it is required to specify the *--lib* parameter with the path to the folder

~~~
> docify serve --lib D:\lib
~~~

Or 

~~~
> docify build --lib D:\lib --out D:\out
~~~

## Secure Library

In order to secure the library from modifying, it is possible to protect it with [code signing certificate](https://en.wikipedia.org/wiki/Code_signing). In this case Docify will not load the library from a folder rather use a library manifest which contains information about the components and digital signature for each file. In case the file is missing or digital signature does not match, Docify will generate an error and will not load the component.

In order to generate secure manifest for an existing library folder, it is required to run the **genman** command

> docify genman --lib D:\secure-lib --cert D:\my-certificate.pfx --version 0.1.0 --pwd CertPassword --pkey D:\pkey.xml

**genman** utility requires the path to *.pfx* certificate and optional password and will extract the digital signatures of all files in the library folder and generate the *library.manifest* file. It is possible to optionally specify the path to a public key (note, this file will not changes unless certificate changes).

In order to load the secure library, specify the path to the manifest (instead of the folder) and public key XML file, separated by **|**

~~~
> docify serve --lib D:\secure-lib\library.manifest|D:\pkey.xml
~~~

The manifest file must be in the root directory of the library.

Any changes in the files will require the regeneration of manifest, otherwise, Docify will refuse loading modified files with a mismatched signature.