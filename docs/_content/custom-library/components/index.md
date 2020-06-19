---
caption: Components
title: Defining and referring reusable components in Docify
description: Creating and managing components in Docify Engine
order: 2
---
Components can be used to define reusable files. Components can contain any files.

When the component is referenced into the site its content is copied to the site content into the root folder and its files behave correspondingly. For example, if component contains the file in the *_include* folder, this file will be considered as [include](/includes/).

Defining the component structure below

~~~
_components
    comp1
        _includes
            my-include.cshtml
        _assets
            logo.png
            styles
                style.png
~~~

And referring it in the site below

~~~
_include
    site-include.cshtml
_assets
    main.png
    styles
        main.css
    scripts
        main.js
_config.yml
~~~

Would result in the following content to be compiled

~~~
_include
    site-include.cshtml
    my-include.cshtml
_assets
    main.png
    logo.png
    styles
        main.css
        style.png
    scripts
        main.js
_config.yml
~~~

Components are loaded from the library when

* Library is referenced with *--lib* argument of *build* or *serve* command
* Component is referenced in the *_config.yml* file

~~~
components:
  - comp1
~~~

Different components must have unique file names compared to different components or files in the site, otherwise, the conflict error will be thrown. This is a main difference between components and [themes](/custom-library/themes/) as themes are designed to support files overriding.

For example, referring following two components in the site will generate an error as *_assets\logo.png* file is used in both *comp1* and *comp2*:

~~~
_components
    comp1
        _includes
            my-include.cshtml
        _assets
            logo.png
    comp2
        _includes
            my-include2.cshtml
        _assets
            logo.png
~~~

Components are usually used to create reusable includes with supporting files (such as images, styles and scripts which include needs to consume).

Refer the [standard library](/standard-library/) components section for an example of components.