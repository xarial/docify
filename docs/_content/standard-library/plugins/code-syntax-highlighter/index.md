---
layout: $
caption: Code Syntax Highlighter
title: Highlighting code syntax in Docify static site generator
description: Enabling the automatic code syntax highlighting for the most popular programming languages
---
This plugin converts the markdown code snippets into the code with highlighted syntax.

Plugin is based on [Color Code](https://github.com/windows-toolkit/ColorCode-Universal)

## Supported Languages

* PowerShell
* Haskell
* Koka
* F#
* TypeScript
* C++
* Css
* Php
* Xml
* VB.NET
* Sql
* Markdown
* Fortran
* Java
* Html
* C#
* ASPX VB
* ASPX C#
* ASPX
* Asax
* Ashx
* JavaScript

## Parameters

* embed-style - true to embed the style within the snippet itself (default option). Otherwise the style will be added as separate assets css file. This is useful as style can be reused across pages

## Usage

Snippet needs to specify the short name of the language to highlight

```
~~~ cs
var msg = "Hello World";
Console.WriteLine(msg);
~~~
```

~~~ cs
var msg = "Hello World";
Console.WriteLine(msg);
~~~