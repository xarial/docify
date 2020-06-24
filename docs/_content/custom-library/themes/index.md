---
caption: Themes
title: Defining themes for reusing site layouts and branding in Docify
description: Creating and managing themes in Docify Engine
order: 1
---
Similar to [components](/custom-library/components/) themes are designed to store the reusable files. But unlike components themes are used to pre-configure site layouts and branding, e.g. blog, technical documentation, user guides, API reference etc.

Theme files are copied to the root directory of the site.

For example the following theme

~~~
_themes
    theme1
        _assets
            images
                logo.png
            scripts
                main.js
            styles
                main.css
        _layouts
            default.html
            article.html
        _config.yml
~~~

Applied to the site below

~~~
_assets
    images
        logo.png
        site.png
_layouts
    default.html
index.md
_config.yml
~~~

Would result into the following structure

~~~
_assets
    images
        logo.png
        site.png
    scripts
        main.js
    styles
        main.css
_layouts
    default.html
    article.html
index.md
_config.yml
~~~

Unlike [components](/custom-library/components/), different themes will not generate a conflict error in case the same file names are used. Instead, Docify engine will follow the merging rule to merge and override files.

In the example above *_assets\images\logo.png* and *_layouts\default.html* files from the theme will be overriden with the corresponding files from the site, while *_config.yml* files will be merged.

Themes are encouraged to have overridable files so the consumer can tailor the theme to their needs.

The theme itself can have a parent theme. In this case, it should have a *_config.yml* file with the *theme* attribute. For example, if the *_config.yml* of the theme in the example above will have the following attribute, the files from the *base* theme will be also merged to a site following the same merging and overriding rules.

~~~ yaml jagged-bottom
theme: base
~~~

Refer the [standard library](/standard-library/) themes section for an example of themes.