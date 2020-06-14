---
caption: Assets
title: Concept of assets contents in Docify engine
description: Explanation of assets concepts used in Docify, relationship with pages and asset folders
order: 6
---
All of the files in the input site which are not [pages](/pages/) are considered to be assets.

Usually main page contains images, styles, scripts as assets, while children pages may contain images etc.

For example the following folder structure

~~~
index.md
_assets
    styles
        main.css
    scripts
        main.js
page1
    index.md
    logo.png
~~~

Will result into *_assets\styles\main.css* and *_assets\scripts\main.js* assets be associated with the main (index.html) page while *logo.png* will be associated with *page1*

This will generate the following urls in the resulting site

~~~
/
/_assets/styles/main.css
/_assets/scripts/main.js
/page1/
/page1/logo.png
~~~

> Although *_assets* is not a special name in Docify and assets can be placed in any folder, it is recommended for the root assets to use the *_assets* folder. This name is used in all items from the [standard library](/standard-library/)

By default assets are not compilable and will be output as is into the published site. However in some cases it can be beneficial for the assets to use [dynamic content](/content/dynamic/). This can be achieved by referring the [includes](/includes/) and explicitly setting the files pattern in *compilable-assets* array, to resolve includes in the [configuration](/configuration/).

The following setting in the configuration would resolve the includes for all xml files

~~~
compilable-assets:
    - '*.xml'
~~~

~~~ xml
<?xml version="1.0" encoding="UTF-8"?>
<myxml>
{\% my-include %}
</myxml>
~~~

