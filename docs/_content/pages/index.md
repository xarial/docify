---
caption: Pages
title: Concept of pages (html and markdown) in Docify engine
description: Explanation of pages concepts used in Docify, understanding the main page and pages hierarchy
order: 4
---
Pages are user facing elements of site.

Docify will scan all the files in input folder and identify pages. Pages are html files or markdown (.md) files.

Pages will support [static content](/content/static/).

[Metadata](/metadata/) can be added to the page in the front matter

~~~
---
title: My Page
description: My Page Description
layout: default
---
~~~

*layout* attribute is a system attribute which allows to specify the [layout](/layout/) for the page.

All pages will be grouped by hierarchy, based on the folder structure and will be assigned with url correspondingly.

Source must contain one main page (index.md or index.html) in the root folder of the site.

Docify also supports default pages (e.g. index.md or index.html). In this case the folder name will be considered as the page name.

For example the following folder structure:

~~~
index.md
sub-folder
    index.md
    sub-sub-folder
        index.md
        page4.md
page2.md
~~~

Will generate 5 pages with the following urls

~~~
/
/sub-folder/
/sub-folder/sub-sub-folder/
/sub-folder/sub-sub-folder/page4/
/page2/
~~~

Docify supports phantom pages (i.e. sub folder which doesn't have a default page). For example the following folder structure

~~~
index.md
sub-folder
    sub-sub-folder
        index.md
page2.md
~~~

Will generate 3 pages with the following urls, where url for *sub-sub-folder* is not generated as there is no default page in the *sub-sub-folder* folder.

~~~
/
/sub-folder/
/sub-folder/sub-sub-folder/page4/
/page2/
~~~