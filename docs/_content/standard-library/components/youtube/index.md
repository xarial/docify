---
layout: $
caption: YouTube
title: Embed YouTube to site
description: Component to embed YouTube video to the site
image: embedded-youtube.png
---
This components provides a shortcut of embedding video from YouTube from the site.

![Embedded YouTube video](embedded-youtube.png)

Embedded video frame is responsive and will change when the page resizes.

## Usage

Add the reference to *_assets/styles/youtube.css* style into the HTML head

~~~ html jagged
<head>
    <link rel="stylesheet" type="text/css" href="/_assets/styles/youtube.css" />
</head>
~~~

Refer include and specify the video id.

~~~
\{% youtube id: VIDEO_ID  %}
~~~

Video ID can be found in the YouTube url

![ID of YouTube video](youtube-video-id.png)

This component is included into the [user guide theme](/standard-library/themes/user-guide/) and [blog theme](/standard-library/themes/blog/)