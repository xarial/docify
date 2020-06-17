---
caption: Metadata
title: Metadata in pages, assets, layouts and includes
description: How to configure the metadata for entities in Docify
order: 2
---
Metadata is a set of attributes which can be assigned to [pages](/pages/), [layouts](/layouts/) and [includes](/includes/) and also used in the [configuration](/configuration/).

Metadata is using [YAML](https://en.wikipedia.org/wiki/YAML) syntax and is specified in the front matter of a file. This is the data fenced with --- at the beginning of the file

~~~
---
title: My Page
---
# Page1
~~~

## Syntax

Below are the basic syntax options for the front matter attributes

### Variables

Variable can be specified by providing the key-value pair separated by **:**

~~~
---
title: My Page
order: 1
toc: false
---
~~~

### Arrays

Array elements can be specified by using **-**

~~~
---
pages:
  - page1
  - page2
  - page3
---
~~~

### Dictionary

Dictionary is a collection of key-value pairs and can be defined as follows:

~~~
---
setts:
  dict: 
    key1: val1
    key2: val2
    key3: val3
---
~~~

### Complex Structures

Complex structures can be defined by nesting simple variables and arrays

~~~
---
struct:
  property1: Value
  sub-property:
    - Val1:
      - SubVal1:
        - SubSubVal1
      - SubVal2:
        - SubSubVal2
---
~~~

## Inheritance

Docify supports inheritance when composing the metadata for [pages](/pages/), [layouts](/layouts/), [includes](/includes/) and [configuration](/configuration/).

For example, when the layout inherits another layout, the parent's layout metadata will be merged with child's layout metadata and new properties will be added. If the property already exists, in the lower-level layout it won't be overridden.

Arrays can inherit the values from the higher-level layout by using the **$** special value.

For example

### Layout L1

~~~
---
prp1: X
prp3: Y
arr1:
  - v1
  - v2
arr2:
  - x1
  - x2
---
~~~

### Layout L2

~~~
---
layout: l1
prp1: A
prp2: B
arr1:
  - z1
  - z2
arr2:
  - $
  - k1
  - k2
---
~~~

As the result metadata of layout L2 will look:

~~~
---
prp1: A
prp2: B
prp3: Y # copied from the parent's layout
arr1: # overrides parent's array
  - z1
  - z2
arr2: #inherits the values from the parent's array and adds new values
  - x1
  - x2
  - k1
  - k2
---
~~~

## Naming Convention

You can use any naming convention compatible with YAML, however, default naming convention adapted across all items in the [standard library](/standard-library/) is [lisp-case](https://en.wikipedia.org/wiki/Naming_convention_(programming)#Lisp), i.e. all words are lower case separated by a hyphen, e.g. *prp1*, *my-property*.

This convention is required when [Deserializing The Metadata To Structure](#deserializing-the-metadata-to-structure). In this case, the attributes namings in the metadata should follow [lisp-case](https://en.wikipedia.org/wiki/Naming_convention_(programming)#Lisp), while property names in the class should follow *PascalCase*.

## Accessing Data

When creating elements with the [dynamic content](/content/dynamic/), metadata is usually accessed to compose the dynamic nodes on the page. There are several APIs available to read this data from the model

### Getting The Attribute Value By Name

IContextMetadata implements [IReadOnlyDictionary<string, object>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2?view=netcore-3.1) which means all the methods to get data from dictionary are applicable.

Additionally IContextMetadata interface provides additional methods for safe extraction and casting of values:

~~~ cs
@using RazorLight
@using System
@using Xarial.Docify.Base.Context
@inherits TemplatePage<Xarial.Docify.Base.Context.IContextModel>
@{
    var val = Model.Data.GetOrDefault<bool>("bool-field");
}
~~~

*GetOrDefault* method returns the value of the specified field casting this to the specified type or default value if the field is not assigned.

### Deserializing The Metadata To Structure

For complex structures, it might be beneficial to deserialize the data into the class for easier access.

Use Xarial.Docify.Lib.Tools.DataDeserializer class and provide the type of the class to deserialize and the metadata. Note the [naming convention](#naming-convention) for naming the property names of the class.

~~~
---
text-value: Value1
keys:
  key1: a1
  key2: a2
  key3: a3
arr:
  - b1
  - b2
  - b3
---
~~~

~~~ cs
@using RazorLight
@using Xarial.Docify.Lib.Tools
@inherits TemplatePage<Xarial.Docify.Base.Context.IContextModel>
@functions
{
    public class MyData
    {
        public string TextValue { get; set; }
        public Dictionary<string, string> Keys { get; set; }
        public string[] Arr { get;set; }
    }
}
@{
    var data = DataDeserializer.Deserialize<MyData>(Model.Data);
}
~~~