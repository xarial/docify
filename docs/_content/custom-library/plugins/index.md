---
caption: Plugins
title: Accessing plugins from Docify library
description: Adding plugins to Docify library
order: 3
---
[Plugins](/plugins/) which are not specific to the site are usually added to the library so they can be reused in multiple sites.

Plugins must be placed into a special folder *_plugins* in the library and must be referenced in the [configuration](/configuration/) file as follows

~~~
plugins:
  - plugin1
  - plugin2
~~~

Plugin names must not conflict with the plugins already in the site or the plugins from different libraries loaded to site.

Refer the [standard library](/standard-library/) plugins section for an example of plugins.