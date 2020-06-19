---
layout: $
caption: RSS Feed
title: RSS feed XML file generation component
description: Component to automatically generate RSS feed xml file from the pages
image: 
---
This component will add the *feed.xml* containing the [RSS feed](https://en.wikipedia.org/wiki/RSS).

This component is included into the [base theme](/standard-library/themes/base/)

## Parameters

* image - default site image if image attribute is not specified for the main page of the site
* title-attribute - name of the page attributes which should be used for title in this feed. Default value is title
* sitemap (page metadata) - false to exclude this page from the feed
* date (page metadata) - optional parameter for the date attribute of the page
* description (page metadata) - description of the page in the feed
* categories (page metadata) - array of categories this page belongs to

## Usage

When this component is added *feed.xml* will be generated at the root of the site.

Data will be extracted from each page attributes

~~~
title: Page Title
description: page description
categories:
  - Category1
  - Category2
~~~