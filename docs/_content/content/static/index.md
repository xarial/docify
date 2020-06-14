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

~~~ md
First Line

Second Line
~~~

### Result

First Line

Second Line

## Headings

Use # symbol to specify the headings 

~~~ md
# Headings1
## Headings2
### Headings3
#### Headings4
~~~

### Result

# Headings1
## Headings2
### Headings3
#### Headings4

## Url

~~~ md
[Docify Site](https://docify.net)

[Pages Absolute Url](/pages/)

[Relative Url Of Image](sample-image.png)
~~~

### Result

[Docify Site](https://docify.net)

[Pages Absolute Url](/pages/)

[Relative Url Of Image](sample-image.png)

## Image

Image uses a similar format to url, but with a ! symbol at the beginning. Additional attributes, such as width and height can be added by adding the \{\} after the image as shown below

~~~ md
![Image Alt Text](sample-image.png)
![Image with width 100](sample-image.png){ width=100 }
![Image with height 200](sample-image.png){ height=200 }
![Image with height 50 and width 150](sample-image.png){ height=50, width=150 }
~~~

### Result

![Image Alt Text](sample-image.png)
![Image with width 100](sample-image.png){ width=100 }
![Image with height 200](sample-image.png){ height=200 }
![Image with height 50 and width 150](sample-image.png){ height=50 width=150 }

## Text Decorations

~~~ md
*Italic Text*

**Bold Text**
~~~

### Result

*Italic Text*

**Bold Text**

## HTML Code

html code can be injected into the markdown:

~~~ md
<u>underlined text</u>
~~~

### Result

<u>underlined text</u>

## Code Snippets

Use ~~~ to fence the code snippet. Optionally specify the language

``` md
~~~ cs
var myVar = "Hello World";
Console.WriteLine(myVar);
~~~
```

### Result

~~~ cs
var myVar = "Hello World";
Console.WriteLine(myVar);
~~~

## Unordered Lists

~~~ md
* Item 1
  * Item 1-1
  * Item 1-2
* Item 2
  * Item 2-1
* Item 3
~~~

### Result

* Item 1
  * Item 1-1
  * Item 1-2
* Item 2
  * Item 2-1
* Item 3

## Ordered List

~~~ md
1. Item 1
1. Item 2
1. Item 3
~~~

### Result

1. Item 1
1. Item 2
1. Item 3

## Tables

~~~ md
|Header1|Header2|
|--|--|
|cell1|cell2|
|cell3|cell4|
|cell5|cell6|
~~~

### Result

|Header1|Header2|
|--|--|
|cell1|cell2|
|cell3|cell4|
|cell5|cell6|

## Special Symbols

Use \\ symbol for escaping special symbols

~~~ md
\\
\{\}
\[not url](not url)
~~~

### Result

\\
\{\}
\[not url](not url)