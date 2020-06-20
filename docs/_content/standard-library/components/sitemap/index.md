---
layout: $
caption: Sitemap
title: Adding sitemap for the site
description: Automatically generating sitemap XML for the site
---
Automatically generates [sitemap](https://en.wikipedia.org/wiki/Site_map) for all pages of the site where *sitemap* attribute is not set to *false*.

This component will automatically add the *sitemap.xml* file into the root folder of the site.

~~~ xml
<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
    <url>
        <loc>/</loc>
    </url>
    <url>
        <loc>/page1/</loc>
    </url>
    <url>
        <loc>/page2/</loc>
    </url>
</urlset>
~~~

This component is included into the [base theme](/standard-library/themes/base/)