---
layout: $
caption: RSS Feed
title: RSS feed XML file generation component
description: Component to automatically generate RSS feed xml file from the pages
image: 
---
This component will add the *feed.xml* containing the [RSS feed](https://en.wikipedia.org/wiki/RSS).

~~~ xml
<?xml version="1.0" encoding="UTF-8" ?>
<rss version="2.0" xmlns:atom="http://www.w3.org/2005/Atom">
    <channel>
        <title>Site Title</title>
        <description>Site Description</description>
        <link>https://www.example.com</link>
        <image>
            <url>/logo.png</url>
            <title>Site Title</title>
            <link>https://www.example.com</link>
        </image>
        <lastBuildDate>Sat, 20 Jun 2020 08:22:31 GMT</lastBuildDate>
        <atom:link href="/feed.xml" rel="self" type="application/rss+xml" />
      <item>
          <title>Page 1</title>
          <description>Description of page1</description>
          <link>/page1/</link>
          <guid isPermaLink="true">/page1/</guid>
      </item>
      <item>
          <title>Page 2</title>
          <description>Description of page2</description>
          <link>/page2/</link>
          <guid isPermaLink="true">/page2/</guid>
      </item>
    </channel>
</rss>
~~~

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
description: Page description
categories:
  - Category1
  - Category2
~~~