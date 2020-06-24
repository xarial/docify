---
layout: $
caption: Scripts And Styles Optimizer
title: Optimized scripts and styles in the site using Docify
description: 'Plugin to optimize scripts and styles: bundle reusable scripts and styles, remove unused files and minify'
---
This plugin allows to optimize scripts and styles in the site in 3 ways

* Bundle scripts and files into a single file
* Remove unused scripts and styles (usually as the result of bundling)
* Minify styles and scripts. This functionality is implemented using the [YUICompressor.NET](https://www.nuget.org/packages/YUICompressor.NET/)

## Parameters

* minify-css - true to minify all style (css) files (default is false)
* minify-js - true to minify all JavaScript (js) files (default is false)
* assets-scope-paths - array of paths for the css and styles to process (use this if certain styles or scripts need to be excluded from the optimization). Default is all files
* delete-unused-css - true to delete all unused css files (default is false)
* delete-unused-js - true to delete all unused js files (default is false)
* bundles - dictionary of bundles, where key is the target bundle file path and value is an array of files to merge to a bundle

## Usage

Set the settings in the [configuration](/configuration/) file.

~~~
^script-style-optimizer:
  minify-css: true
  minify-js: true
  assets-scope-paths:
    - /_assets/styles/*
    - /_assets/scripts/*
  delete-unused-css: true
  delete-unused-js: true
  bundles:
    /_assets/styles/main.min.css:
      - /_assets/styles/style1.css
      - /_assets/styles/style2.css
    /_assets/scripts/main.min.js:
      - /_assets/scripts/script1.js
      - /_assets/scripts/script2.js
~~~

The above configuration will result into all css and js files in the *_assets/styles* and *_assets/scripts* folders respectively to be minified, while all other css and js outside of these directories will not be processed. 

*_assets/styles/style1.css* and *_assets/styles/style2.css* files will be merged into *_assets/styles/main.min.css*. *_assets/scripts/script1.js* and *_assets/scripts/script2.js* will be merged into *_assets/scripts/main.min.js*. Plugin will also replace the references to all bundles in all pages where applicable.

> Note, the plugin will only replace the bundle if all references in the page are being used.

For example this HTML page

~~~ html jagged
<head>
<link rel="stylesheet" type="text/css" href="/_assets/styles/style1.css" />
<link rel="stylesheet" type="text/css" href="/_assets/styles/style2.css" />
</head>
~~~

will be optimized to

~~~ html jagged
<head>
<link rel="stylesheet" type="text/css" href="/_assets/styles/main.min.css" />
</head>
~~~

while this HTML page will remain as is as its bundle is incomplete

~~~ html jagged
<head>
<link rel="stylesheet" type="text/css" href="/_assets/styles/style1.css" />
</head>
~~~

Plugin will only replace bundles in the HTML *\<head>* node.

Finally, plugin will remove all unused css and js files that are not referenced in any of the pages.