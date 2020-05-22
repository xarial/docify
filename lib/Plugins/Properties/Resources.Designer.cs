﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Xarial.Docify.Lib.Plugins.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Xarial.Docify.Lib.Plugins.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to pre.code-snippet {
        ///    border: none;
        ///    overflow: auto;
        ///    background: #eff0f1;
        ///}
        ///
        ///pre.jagged-top {
        ///    background-color: #eff0f1;
        ///    background-image: linear-gradient(135deg, rgba(255,255,255,1) 0%, rgba(255,255,255,1) 50%, rgba(255,255,255,0) 50%, rgba(255,255,255,0) 100%), linear-gradient(-135deg, rgba(255,255,255,1) 0%, rgba(255,255,255,1) 50%, rgba(255,255,255,0) 50%, rgba(255,255,255,0) 100%), linear-gradient(to top, #eff0f1 0%, #eff0f1 100%);
        ///    background-position: top center;
        ///    back [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string code_snippet_css {
            get {
                return ResourceManager.GetString("code_snippet_css", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function openTab(evt, tabContainerName, tabName) {
        ///	var i, tabcontainer, tabcontent, tablinks;
        ///	tabcontainer = document.getElementById(tabContainerName);
        ///	tabcontent = tabcontainer.getElementsByClassName(&quot;tabcontent&quot;);
        ///	for (i = 0; i &lt; tabcontent.length; i++) {
        ///		tabcontent[i].style.display = &quot;none&quot;;
        ///	}
        ///	tablinks = tabcontainer.getElementsByClassName(&quot;tablinks&quot;);
        ///	for (i = 0; i &lt; tablinks.length; i++) {
        ///		tablinks[i].className = tablinks[i].className.replace(&quot; active&quot;, &quot;&quot;);
        ///	}
        ///	document.getElemen [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string code_snippet_js {
            get {
                return ResourceManager.GetString("code_snippet_js", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div id=&quot;{0}&quot;&gt;
        ///
        ///&lt;div class=&quot;code-tab&quot;&gt;
        ///   {1}
        /// &lt;/div&gt;
        /// {2}
        ///&lt;/div&gt;.
        /// </summary>
        internal static string code_snippet_tab_container {
            get {
                return ResourceManager.GetString("code_snippet_tab_container", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;figure&gt;
        ///   &lt;a href=&quot;{1}&quot; imageanchor=&quot;1&quot;&gt;
        ///   {0}
        ///   &lt;figcaption&gt;{2}&lt;/figcaption&gt;
        ///   &lt;/a&gt;
        ///&lt;/figure&gt;.
        /// </summary>
        internal static string img_figure {
            get {
                return ResourceManager.GetString("img_figure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///&lt;html&gt;
        ///&lt;head&gt;
        ///    &lt;title&gt;Redirect&lt;/title&gt;
        ///    &lt;meta http-equiv=&quot;refresh&quot; content=&quot;{1}; url={0}&quot;&gt;
        ///    &lt;link rel=&quot;canonical&quot; href=&quot;{0}&quot; /&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///&lt;/body&gt;
        ///&lt;/html&gt;.
        /// </summary>
        internal static string redirect {
            get {
                return ResourceManager.GetString("redirect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .responsive {
        ///    height: auto;
        ///    max-height: 100%;
        ///    max-width: 100%;
        ///    display: block;
        ///    margin-left: auto;
        ///    margin-right: auto;
        ///}
        ///
        ///figure {
        ///    text-align: center;
        ///}.
        /// </summary>
        internal static string responsive_image {
            get {
                return ResourceManager.GetString("responsive_image", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;link href=&quot;/assets/tipuesearch/normalize.min.css&quot;&gt;
        ///&lt;script src=&quot;/assets/tipuesearch/jquery.min.js&quot;&gt;&lt;/script&gt;
        ///
        ///&lt;script src=&quot;search-content.js&quot;&gt;&lt;/script&gt;
        ///&lt;link rel=&quot;stylesheet&quot; href=&quot;/assets/styles/tipue-search.css&quot;&gt;
        ///&lt;script src=&quot;/assets/tipuesearch/tipuesearch_set.js&quot;&gt;&lt;/script&gt;
        ///&lt;script src=&quot;/assets/tipuesearch/tipuesearch.min.js&quot;&gt;&lt;/script&gt;
        ///
        ///&lt;div id=&quot;tipue_search_content&quot;&gt;&lt;/div&gt;
        ///&lt;script&gt;
        ///    $(document).ready(function () {
        ///        $(&apos;#tipue_search_input&apos;).tipuesearch();
        ///    });
        ///&lt;/script&gt;.
        /// </summary>
        internal static string search {
            get {
                return ResourceManager.GetString("search", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] tipue_search {
            get {
                object obj = ResourceManager.GetObject("tipue_search", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;form action=&quot;/search/&quot;&gt;
        ///    &lt;div class=&quot;tipue_search_group&quot;&gt;
        ///        &lt;input type=&quot;image&quot; src=&quot;/assets/images/search.svg&quot; class=&quot;tipue_search_button&quot; width=&quot;25px&quot; border=&quot;0&quot; alt=&quot;Submit&quot; /&gt;
        ///        &lt;div style=&quot;overflow: hidden;padding-right: .5em;&quot;&gt;
        ///            &lt;input type=&quot;text&quot; name=&quot;q&quot; id=&quot;tipue_search_input&quot; placeholder=&quot;Search&quot; pattern=&quot;.{3,}&quot; title=&quot;At least 3 characters&quot; required&gt;
        ///        &lt;/div&gt;
        ///    &lt;/div&gt;
        ///&lt;/form&gt;.
        /// </summary>
        internal static string tipue_search_box {
            get {
                return ResourceManager.GetString("tipue_search_box", resourceCulture);
            }
        }
    }
}
