---
layout: $
caption: Embed
title: Embed external file plugin
description: Embeds the content from the external file into the page
---
This plugin allows to embed the content from external text file directly into the page. Content is embedded as is.

## Parameters

* file-name - Name of the file to embed (either relative or absolute path). File must reside in the site this will not work for external files

## Usage

~~~
\{% embed file-name: content.txt %}
~~~