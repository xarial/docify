---
layout: $
caption: User Guide
title: User guide theme in docify
description: Theme to create user guides using Docify
image: user-guide-article-layout.png
---
{% youtube id: kWjYHH32w7A  %}

This theme is designed for the creation of user guides and technical documentation. Theme is based on [base theme](/standard-library/themes/base/).

![Article layout in user guide theme](user-guide-article-layout.png){ width=600 }

## Layouts

* article - layout for the article page.

## Standard Library Components

### Components

* [disqus](/standard-library/components/disqus/)
* [toc](/standard-library/components/toc/)

### Plugins:

* [code-syntax-highlighter](/standard-library/plugins/code-syntax-highlighter/)
* [code-snippet](/standard-library/plugins/code-snippet/)

## Theme Specific Components

### GitHub Issues

Loads the list of GitHub issues for the public repository with an option to raise new issue.

~~~ html
<div>
\{% github-issues { project: public-project-name, repo: public-repo-name } %}
</div>
~~~

![GitHub issues](github-issues.png)