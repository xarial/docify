---
caption: Plugins
title: Creating plugins for Docify engine to automate the site building process
description: Creating and managing plugins in Docify Engine
order: 9
---
{% youtube id: Z98CKsQt4p8 %}

Plugins are powerful mechanisms to tailor Docify engine to specific needs.

Docify engine exposes various APIs which can intercept and modify data on the various steps of the compilation and publishing process.

Plugins need to be implemented in .NET Core.

Plugins have access to interfaces defined in *Base.dll*

To create plugin:

* Create new class library in .NET Core or .NET Standard
* Install the [Xarial.Docify.Base](https://www.nuget.org/packages/Xarial.Docify.Base/) nuget package. Optionally install [Xarial.Docify.Base.Extensions](https://www.nuget.org/packages/Xarial.Docify.Base.Extensions/) package for some extension methods for common interfaces.
* Create new public class and implement *Xarial.Docify.Base.Plugins.IPlugin* or *Xarial.Docify.Base.Plugins.IPlugin<T>* interface. Later interface allows to specify the plugin settings which can be defined by the user in the [configuration](/configuration/) file

~~~ cs
public abstract class MyPlugin1 : IPlugin
{
    public IDocifyApplication App { get; private set; }

    public void Init(IDocifyApplication app)
    {
        App = app;
    }
}
~~~

or 

~~~ cs
public class MyPlugin2Settings
{
    public string TextSetting { get; set; }
    public int NumberSetting { get; set; }
}

public class MyPlugin2 : IPlugin<MyPlugin2Settings>
{
    private MyPlugin2Settings m_Settings;

    private IDocifyApplication m_App;

    public void Init(IDocifyApplication app, MyPlugin2Settings setts)
    {
        m_App = app;
        m_Settings = setts;
    }
~~~

Explore the *Xarial.Docify.Base.Plugins.IDocifyApplication* interface for available APIs.

> Detailed API references for the plugins is coming soon...

All plugin output files must be placed into the folder which will be used as the plugin name and placed into the *_plugins* folder in the root folder of the site.

~~~
_plugins
    MyPlugin1
        MyPlugin1.dll
        ...
    MyPlugin2
        MyPlugin2.dll
        ...
~~~

It must be only one class that implements the *Xarial.Docify.Base.Plugins.IPlugin* or *Xarial.Docify.Base.Plugins.IPlugin<T>* interface within the single plugin.

Refer the [standard library](/standard-library/) plugins section for an example of plugins.

Users can define plugins settings in the *_config.yml* file using the *^* followed by the plugin name. For example for the *MyPlugin2* above, its *MyPlugin2Settings* can be specified by the user as follows

~~~
^MyPlugin2
  text-settings: Text Value
  number-settings: 10
~~~

Note the [naming convention](metadata#naming-convention) of settings and the class properties.

These values then will be automatically assigned and passed to IPlugin<>.Init method.