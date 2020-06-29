---
layout: $
caption: Redirect
title: Redirect from and to pages plugin in Docify
description: Plugin to create redirect pages from or to page in Docify
---
This plugin allows to automatically generate redirect pages either to the current page or from the page.

Plugin will generate the required pages as follows:

~~~ html
<!DOCTYPE html>
<html>
<head>
    <title>Redirect</title>
    <meta http-equiv="refresh" content="TIMEOUT; url=URL">
    <link rel="canonical" href="URL" />
</head>
<body>
</body>
</html>
~~~

## Parameters

* wait-seconds - number of seconds to wait before redirecting (default 0)
* redirect-from (page) - array of urls to redirect from to this page
* redirect-to (page) - url to redirect from this page

## Usage

~~~
title: MyPage
redirect-from:
  - /old/my-page1/
  - /old/my-page2/
~~~

~~~
title: MyPage
redirect-to: /new/page1/
~~~