---
caption: Static
title: Creating static markdown content in Docify
description: Managing static markdown content in Docify for the content of the pages
image: sample-image.png
order: 1
---
Docify is a static site generator which means that the content of the page will not be changed after the compilation (the exception are Java Scripts which can dynamically change the DOM).

The content of the page files will be resolved during the compilation time and converted into the corresponding html files. Docify is using [markdig](https://github.com/lunet-io/markdig) engine to convert markdown into html. Below are the most commonly used syntax examples:

## Plain Text

Plain text will be resolved to corresponding paragraphs in the html

{% markdown-snippet name: plain-text.md_ %}

## Headings

Use # symbol to specify the headings 

{% markdown-snippet name: headings.md_ %}

## Url

{% markdown-snippet name: url.md_ %}

## Image

Image uses a similar format to url, but with a ! symbol at the beginning. Additional attributes, such as width and height can be added by adding the \{\} after the image as shown below

{% markdown-snippet name: image.md_ %}


## Text Decorations

{% markdown-snippet name: text-decorations.md_ %}

## HTML Code

html code can be injected into the markdown:

{% markdown-snippet name: html-code.md_ %}

## Code Snippets

Use ~~~ to fence the code snippet. Optionally specify the language

{% markdown-snippet name: code-snippet.md_ %}

## Unordered Lists

{% markdown-snippet name: unordered-list.md_ %}

## Ordered List

{% markdown-snippet name: ordered-list.md_ %}

## Tables

{% markdown-snippet name: tables.md_ %}

## Special Symbols

Use \\ symbol for escaping special symbols

{% markdown-snippet name: special-symbols.md_ %}