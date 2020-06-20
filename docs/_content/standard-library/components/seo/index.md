---
layout: $
caption: SEO
title: Search Engine Optimization (SEO) component for Docify
description: Component to automatically generate SEO attributes based on the page metadata
---
This component allows to automatically add required metadata into the head of HTML page to improve Search Engine Optimization (SEO). Component will automatically generate *title* and *meta description* nodes based on the page [metadata](/metadata/).

This component will also optionally generate [Open Graph](https://ogp.me/), LinkedIn metadata and [Twitter cards](https://developer.twitter.com/en/docs/tweets/optimize-with-cards/guides/getting-started).

## Parameters

* og - true (default value) to generate [Open Graph](https://ogp.me/)
* twitter - true (default value) to enable [Twitter cards](https://developer.twitter.com/en/docs/tweets/optimize-with-cards/guides/getting-started)
* linkedin - true (default value) to enable LinkedIn metadata

## Usage

Add include into the HTML head

~~~ html jagged
<head>
    \{% seo %}
</head>
~~~

This component is included into the [base theme](/standard-library/themes/base/)