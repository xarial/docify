---
layout: $
caption: Files Promoter
title: Files promoter plugin for Docify
description: Promoting levels of files and folders in Docify
repo-link: Plugins/Plugins.FilesPromoter
---
This plugin allows to promote the files in the specified folders to upper level.

This can be useful when files are grouped by folders, but it is not required to generate an url for this folder. For example it might be useful to group all the content markdowns into the *content* folder while all the configuration and styles into the *system* folder.

When this plugin is used, all the files matching the folder will be promoted to upper level.

For example given the following folders structure

~~~
folder1
  file1.txt
  file2.txt
folder2
  file3.txt
  file4.txt
~~~

And promoting files in *folder1* and *folder2* would be equivalent to

~~~
file1.txt
file2.txt
file3.txt
file4.txt
~~~

## Parameters

* folders - array of folders to promote

## Usage

Specify the list of folders to promote in the [configuration](/configuration/) file.

~~~
^files-promoter:
  folders:
    - folder1
    - folder2
~~~