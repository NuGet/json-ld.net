﻿using Newtonsoft.Json.Linq;
using System;

namespace JsonLD.Infrastructure.Text
{
    public class JsonLdOptions
    {
        public JsonLdOptions()
        {
            this.SetBase(string.Empty);
        }

        public JsonLdOptions(string @base)
        {
            this.SetBase(@base);
        }

        public virtual JsonLdOptions Clone()
        {
            return new JsonLdOptions(GetBase());            
        }

        private string @base = null;

        private bool compactArrays = true;

        private string expandContext = null;

        internal Core.JsonLdOptions AsCore()
        {
            var options = new Core.JsonLdOptions(GetBase());
            options.SetCompactArrays(GetCompactArrays());
            var expandContextAsString = GetExpandContext();
            if (!string.IsNullOrWhiteSpace(expandContextAsString))
                options.SetExpandContext(JObject.Parse(expandContextAsString));
            options.SetProcessingMode(GetProcessingMode());
            options.SetEmbed(GetEmbed());
            options.SetExplicit(GetExplicit());
            options.SetOmitDefault(GetOmitDefault());
            options.SetUseRdfType(GetUseRdfType());
            options.SetUseNativeTypes(GetUseNativeTypes());
            options.SetProduceGeneralizedRdf(GetProduceGeneralizedRdf());
            options.SetSortGraphsFromRdf(GetSortGraphsFromRdf());
            options.SetSortGraphNodesFromRdf(GetSortGraphNodesFromRdf());
            options.format = format;
            options.useNamespaces = useNamespaces;
            options.outputForm = outputForm;
            options.documentLoader = documentLoader.AsCore();
            return options;
        }

        private string processingMode = "json-ld-1.0";

        private bool? embed = null;

        private bool? @explicit = null;

        private bool? omitDefault = null;

        internal bool useRdfType = false;

        internal bool useNativeTypes = false;

        private bool produceGeneralizedRdf = false;

        private bool sortGraphsFromRdf = true;

        private bool sortGraphNodesFromRdf = true;
        // base options
        // frame options
        // rdf conversion options
        public virtual bool? GetEmbed()
        {
            return embed;
        }

        public virtual void SetEmbed(bool? embed)
        {
            this.embed = embed;
        }

        public virtual bool? GetExplicit()
        {
            return @explicit;
        }

        public virtual void SetExplicit(bool? @explicit)
        {
            this.@explicit = @explicit;
        }

        public virtual bool? GetOmitDefault()
        {
            return omitDefault;
        }

        public virtual void SetOmitDefault(bool? omitDefault)
        {
            this.omitDefault = omitDefault;
        }

        public virtual bool GetCompactArrays()
        {
            return compactArrays;
        }

        public virtual void SetCompactArrays(bool compactArrays)
        {
            this.compactArrays = compactArrays;
        }

        public virtual string GetExpandContext()
        {
            return expandContext;
        }

        public virtual void SetExpandContext(string expandContext)
        {
            this.expandContext = expandContext;
        }

        public virtual string GetProcessingMode()
        {
            return processingMode;
        }

        public virtual void SetProcessingMode(string processingMode)
        {
            this.processingMode = processingMode;
        }

        public virtual string GetBase()
        {
            return @base;
        }

        public virtual void SetBase(string @base)
        {
            this.@base = @base;
        }

        public virtual bool GetUseRdfType()
        {
            return useRdfType;
        }

        public virtual void SetUseRdfType(bool useRdfType)
        {
            this.useRdfType = useRdfType;
        }

        public virtual bool GetUseNativeTypes()
        {
            return useNativeTypes;
        }

        public virtual void SetUseNativeTypes(bool useNativeTypes)
        {
            this.useNativeTypes = useNativeTypes;
        }

        public virtual bool GetProduceGeneralizedRdf()
        {
            // TODO Auto-generated method stub
            return this.produceGeneralizedRdf;
        }

        public virtual void SetProduceGeneralizedRdf(bool produceGeneralizedRdf)
        {
            this.produceGeneralizedRdf = produceGeneralizedRdf;
        }

        public virtual bool GetSortGraphsFromRdf()
        {
            return sortGraphsFromRdf;
        }

        public virtual void SetSortGraphsFromRdf(bool sortGraphs)
        {
            this.sortGraphsFromRdf = sortGraphs;
        }

        public virtual bool GetSortGraphNodesFromRdf()
        {
            return sortGraphNodesFromRdf;
        }

        public virtual void SetSortGraphNodesFromRdf(bool sortGraphNodes)
        {
            this.sortGraphNodesFromRdf = sortGraphNodes;
        }
        public string format = null;

        public bool useNamespaces = false;

        public string outputForm = null;

        public DocumentLoader documentLoader = new DocumentLoader();
    }
}
