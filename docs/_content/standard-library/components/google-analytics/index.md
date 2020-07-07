---
layout: $
caption: Google Analytics
title: Tracking site statistics using Google Analytics
description: Enabling collection the site traffic data using Google Analytics
repo-link: Components/Lib/google-analytics
---
This components enables tracking of the site statistics using [Google Analytics](https://analytics.google.com/analytics/web/).

This component might need to be used together with [cookie-consent](/standard-library/components/cookie-consent).

## Parameters

* traking-code - google analytics tracking code
* environment - [environment](/getting-started#using-environment) where analytics is enabled (default *production*). Use *-* to enable analytics in all environments.

## Usage

Place the include into the head

~~~ html
<head>
    \{% google-analytics %}
</head>
~~~

This component is included into the [base theme](/standard-library/themes/base/)