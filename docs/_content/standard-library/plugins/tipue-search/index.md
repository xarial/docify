---
layout: $
caption: Tipue Search
title: Adding the search functionality into the static site using Docify
description: How to enable the search functionality based on the Tipue Search in static site generated by Docify
image: search-results.png
repo-link: Plugins/Plugins.TipueSearch
---
This plugin is based on [Tipue Search](https://tipue.com/search/) and enables the search capabilities for the static site.

Search will generate an index file containing the information about keywords and corresponding pages. This file will be loaded and parsed to provide search results based on the query.

![Search results displayed on the page](search-results.png)

## Parameters

* page-content-node - XPath HTML node to index content from (default *//body*)
* search-page-layout - name of the layout to display search results in (default empty)
* search-page-title - title of the search results page (default *Search Results*)

## Usage

Insert the include where the search box should be displayed

![Search box to input search query](search-box.png)

~~~ html
<div>
\{% tipue-search %}
</div>
~~~

> Note the search result page set in the *search-page-layout* setting must also contain this include, otherwise results will not be displayed
