---
caption: Docify
title: Docify Static Site Generator | User Guide
description: Explanation of concepts and usage scenario of Docify static site generator for generating blogs, user guides, technical documentations, etc.
image: docify-logo.svg
---
![Docify Logo](docify-logo.svg){ width=400 }

Docify is an open-source cross-platform content oriented static site generator hosted on [GitHub](https://github.com/xarial/docify) developed in [C# .NET Core](https://docs.microsoft.com/en-us/dotnet/core/).

Structure of layouts and includes inspired by an awesome [Jekyll Static Site Generator](https://jekyllrb.com/), however using different engines and technologies so the sites are not compatible.

Docify enables users to use simple plain text files for source code of your site and compile to self-contained static html site which can be hosted on various hosting solutions which are mainly free or low cost. This includes but not limited to [GitHub Pages](https://pages.github.com/), [Netlify](https://www.netlify.com/), [Azure Storage](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-static-website).

Page content supports markdown syntax, while [multi-level layouts](/layouts/), [pseudo-dynamic includes](/includes/) support [C# Razor Pages](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-3.1&tabs=visual-studio) syntax.

Functionality of Docify can be extended by implementing [plugins](/custom-library/plugins/) in .NET Core.

Docify comes with a [standard library](/standard-library/) of components, themes, and plugins which provides additional features such as Search Engine Optimization, image management, code snippets management, assets bundling, etc.

Docify is ideal for blogs, user guides, technical documentation, etc.