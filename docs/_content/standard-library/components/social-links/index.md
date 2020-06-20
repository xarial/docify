---
layout: $
caption: Social Links
title: Adding social links to the site
description: Adding social links (e.g. LinkedIn, YouTube, Facebook, etc.) to the site
image: social-links.png
---
This component allows to automatically generate social links for the most common social platforms.

![Social links on the site](social-links.png)

## Parameters

* linkedin - url to LinkedIn page
* facebook - url to Facebook page
* pinterest - url to Pinterest page
* reddit - url to Reddit page
* github - url to GitHub page
* youtube - url to YouTube page
* nuget - url to NuGet page
* twitter - url to Twitter page
* rss - url to RSS page
* email - contact e-mail
* color - custom color. If not specified, default color of the social platform is used

If url is not specified, social link will not be added.

## Usage

Add the reference to */_assets/styles/social-links.css* style into the head of the page.

Add the include into the HTML node (usually added to the footer)

~~~ html jagged
<head>
    <link rel="stylesheet" type="text/css" href="/_assets/styles/social-links.css" />
</head>
<div class="foot-social">
    \{% social-links %}
</div>
~~~

This component is included into the [base theme](/standard-library/themes/base/)