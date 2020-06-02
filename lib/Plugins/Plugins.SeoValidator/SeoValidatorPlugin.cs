//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Lib.Plugins.Common.Exceptions;
using Xarial.Docify.Lib.Plugins.Common.Helpers;

namespace Xarial.Docify.Lib.Plugins.SeoValidator
{
    [Plugin("seo-validator")]
    public class SeoValidatorPlugin : IPlugin<SeoValidatorPluginSettings>
    {
        private IDocifyApplication m_App;
        private SeoValidatorPluginSettings m_Setts;

        private readonly IValidator[] m_Validators;

        public SeoValidatorPlugin()
        {
            m_Validators = new IValidator[]
            {
                new TitleValidator(),
                new DescriptionValidator(),
                new ContentValidator()
            };
        }

        public void Init(IDocifyApplication app, SeoValidatorPluginSettings setts)
        {
            m_App = app;
            m_Setts = setts;

            m_App.Compiler.WritePageContent += OnWritePageContent;
        }

        private Task<string> OnWritePageContent(string content, IMetadata data, string url)
        {
            if (m_Setts.Scope?.Any() != true
                || m_Setts.Scope.Any(s => PathHelper.Matches(url, s))
                && (!data.ContainsKey("sitemap") || data.GetParameterOrDefault<bool>("sitemap")))
            {
                var html = new HtmlDocument();
                html.LoadHtml(content);

                foreach (var validator in m_Validators)
                {
                    Dictionary<string, object> validatorSetts = null;

                    m_Setts.Validators?.TryGetValue(validator.Name, out validatorSetts);

                    try
                    {
                        validator.Validate(html, validatorSetts);
                    }
                    catch (SeoValidationFailedException ex)
                    {
                        var error = $"SEO validation rule '{validator.Name}' failed for '{url}' : {ex.Message}";

                        if (m_Setts.TreatErrorAsWarning)
                        {
                            m_App.Logger.LogWarning(error);
                        }
                        else
                        {
                            throw new PluginUserMessageException(error, ex);
                        }
                    }
                }
            }

            return Task.FromResult(content);
        }
    }

    public interface IValidator
    {
        string Name { get; }
        void Validate(HtmlDocument doc, Dictionary<string, object> settings);
    }

    public abstract class ValidatorBase<TSetts> : IValidator
        where TSetts : class, new()
    {
        public abstract string Name { get; }

        public void Validate(HtmlDocument doc, Dictionary<string, object> settings)
        {
            TSetts specSetts = null;

            if (settings != null)
            {
                specSetts = MetadataExtension.ToObject<TSetts>(settings);
            }

            if (specSetts == null)
            {
                specSetts = new TSetts();
            }

            Validate(doc, specSetts);
        }

        protected abstract void Validate(HtmlDocument doc, TSetts settings);
    }

    public class SeoValidationFailedException : Exception
    {
        public SeoValidationFailedException(string err, Exception inner)
            : base(err, inner)
        {
        }

        public SeoValidationFailedException(string err)
            : base(err)
        {
        }
    }

    public static class TextLengthValidator
    {
        public static void Validate(HtmlDocument doc,
            string selector, int min, int max, string name,
            Func<HtmlNode, int> getLength)
        {
            var nodes = doc.DocumentNode.SelectNodes(selector);

            if (nodes == null || nodes.Count == 0)
            {
                throw new SeoValidationFailedException($"{name} node is missing in the the page head");
            }
            if (nodes.Count > 1)
            {
                throw new SeoValidationFailedException($"Multiple {name} nodes found in the the page head");
            }

            var length = getLength.Invoke(nodes[0]);

            if ((min != -1 && length < min)
                || (max != -1 && length > max))
            {
                throw new SeoValidationFailedException(
                    $"{name} length is {length} which is not within the recommended range ({min}...{max})");
            }
        }
    }

    public class TitleValidator : ValidatorBase<TitleValidator.TitleValidatorSettings>
    {
        public class TitleValidatorSettings
        {
            public int MinLength { get; set; } = 50;
            public int MaxLength { get; set; } = 60;
        }

        public override string Name => "Title";

        protected override void Validate(HtmlDocument doc, TitleValidatorSettings settings)
        {
            TextLengthValidator.Validate(doc, "//head/title",
                settings.MinLength, settings.MaxLength,
                "<title>", n => n.InnerText.Length);
        }
    }

    public class DescriptionValidator : ValidatorBase<DescriptionValidator.DescriptionValidatorSettings>
    {
        public class DescriptionValidatorSettings
        {
            public int MinLength { get; set; } = 50;
            public int MaxLength { get; set; } = 160;
        }

        public override string Name => "Description";

        protected override void Validate(HtmlDocument doc, DescriptionValidatorSettings settings)
        {
            TextLengthValidator.Validate(doc, "//head/meta[@name='description']",
                settings.MinLength, settings.MaxLength,
                "<meta description>", n =>
                {
                    var desc = n.Attributes["content"]?.Value;

                    if (string.IsNullOrEmpty(desc))
                    {
                        return 0;
                    }
                    else
                    {
                        return desc.Length;
                    }
                });
        }
    }

    public class ContentValidator : ValidatorBase<ContentValidator.ContentValidatorSettings>
    {
        public class ContentValidatorSettings
        {
            public int MinWords { get; set; } = 500;
            public int MaxWords { get; set; } = -1;
            public string ContentNodeSelector { get; set; } = "//body";
        }

        public override string Name => "Content";

        protected override void Validate(HtmlDocument doc, ContentValidatorSettings settings)
        {
            TextLengthValidator.Validate(doc, settings.ContentNodeSelector,
                settings.MinWords, settings.MaxWords,
                "{page content}", n => TextHelper.GetWords(HtmlHelper.HtmlToPlainText(n)).Count());
        }
    }
}
