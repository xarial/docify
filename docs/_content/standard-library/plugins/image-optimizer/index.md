---
layout: $
caption: Image Optimizer
title: Optimizing images plugin for Docify
description: Optimize images by applying the lossless compression and converting vector images to png
repo-link: Plugins/Plugins.ImageOptimizer
---
This plugin automatically optimizes images used in the site in 2 ways

* Applying lossless image compression to significantly reduce file size. This functionality is implemented using the [nQuant.Core.NETStandard](https://www.nuget.org/packages/nQuant.Core.NETStandard/) library
* Converting vector graphics images (SVG) into the PNG files. This functionality is implemented using the [Svg](https://www.nuget.org/packages/Svg/) library

SVG to PNG conversion is useful when SVG file is not supported (for example Open Graph image attribute *og:image*) while it is still beneficial to keep .svg file, as it is smaller in size and easier to maintain and it can be scaled to any size. This plugin will convert the file and inject *image-png* attribute to the page file which can be used by [includes](/includes/) or [layouts](/layouts/) as needed.

## Parameters

* match-pattern - array of patterns to image files to optimize (by default matches all common image formats: .png, .jpg, .jpeg, .bmp, .tif, .tiff)
* ignore-match-case - true (default) to ignore the case of pattern
* svg-png-width - width of the png image to generate (default 1200). Use 0 to maintain aspect ratio
* svg-png-height - width of the png image to generate (default 0). Use 0 to maintain aspect ratio

## Usage

Set the parameters in the [configuration](/configuration/) file.

~~~
^image-optimizer:
  match-pattern:
    - |*_do_not_optimize.png$
    - *.png$
    - *.jpg$
    - *.bmp$
  svg-png-width: 600
~~~

When using this plugin on Linux systems (such as Ubuntu) or MacOS, it is required to install the *libgdiplus* library.

Run the following command in the terminal to install the library

### Linux

~~~
> sudo apt install libgdiplus
~~~

### MacOS

~~~
> brew install mono-libgdiplus
~~~