---
caption: Configuration
title: Configuration file in Docify
description: How to use configuration file to configure the site options in Docify
order: 3
---
Configuration file is the special YAML file named *_config.yml* in the root folder of the site, inherited from [metadata](/metadata/). Configuration file is optional.

Configuration holds additional information and parameters which can be accessed in [dynamic content](/content/dynamic/) and also utilized by [plugins](/custom-library/plugins/) and [components](/custom-library/components/)

## Special Parameters

Configuration can store any parameters, but there are several special parameters which serve a specific purpose

### Theme

*theme* parameters allow defining a theme of the site

~~~
theme: base
~~~

### Components

*components* array parameters define what components need to be included in the site from the library

~~~
components:
  - comp1
  - comp2
  - comp3
~~~

### Plugins

*plugins* array parameters define what plugins need to be included in the site from the library

~~~
plugins:
  - plugin1
  - plugin2
  - plugin3
~~~

> Components, themes and plugins are loaded from the specified [library](/getting-started#using-libraries)

### Ignore Files

*ignore* array parameters allows to specify the list of files to be ignored and not loaded

~~~
ignore:
  - *.xml
  - \bin\*
~~~

### Default Layout

*default-layout* parameter specifies the default layout to be used for pages, if the layout is not explicitly specified:

~~~
default-layout: default
~~~

## Environment Specific Configuration

[Environment](/getting-started#using-environment) specific configuration file can be specified by adding the environment as the suffix:

~~~
_config.{Environment Name}.yml
~~~

For example, _config.test.yml will be a *test* environment-specific configuration file and will only be loaded if *test* is specified as the environment name when building or serving the site using *--env* switch.

Environment specific configuration will override the default configuration for existing parameters (it will not replace the original configuration, but merge it).