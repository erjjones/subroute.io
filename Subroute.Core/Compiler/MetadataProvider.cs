﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Spatial;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;
using System.Web.Hosting;

namespace Subroute.Core.Compiler
{
    public interface IMetadataProvider
    {
        MetadataReference[] GetCompilationMetadata();
    }

    public class MetadataProvider : IMetadataProvider
    {
        static MetadataReference[] MetadataCache = null;

        private IDictionary<string, string> GetDocumentationLookup()
        {
            // Locate all xml files in app path and cache them in a concurrent dictionary.
            // These file will be the assembly documentation files we need to provide xml
            // documentation to the intellisense front-end.
            var appPath = HostingEnvironment.ApplicationPhysicalPath;
            var searchDirectories = new List<string>(Directory.EnumerateDirectories(appPath, "*", SearchOption.AllDirectories));
            var result = new Dictionary<string, string>();

            // Add additional search locations for the framework assemblies.
            var programFiles = Environment.Is64BitOperatingSystem ? Environment.SpecialFolder.ProgramFilesX86 : Environment.SpecialFolder.ProgramFiles;
            var frameworkPath = Path.Combine(Environment.GetFolderPath(programFiles), Settings.DocumentationPath);

            searchDirectories.Add(frameworkPath);

            foreach (var path in searchDirectories)
            {
                foreach (var file in Directory.GetFiles(path, "*.xml"))
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    var filename = Path.GetFileName(file);

                    if (!result.ContainsKey(name))
                        result.Add(name, string.Concat(path, "\\", filename));
                }
            }

            return result;
        }

        public MetadataReference[] GetCompilationMetadata()
        {
            if (MetadataCache != null)
                return MetadataCache;

            var assemblies = new[]
            {
                typeof (Uri).Assembly,
                typeof (object).Assembly,
                typeof (Enumerable).Assembly,
                typeof (ConfigurationManager).Assembly,
                typeof (JObject).Assembly,
                typeof (HttpClient).Assembly,
                typeof (HttpContentExtensions).Assembly,
                typeof (Common.RouteRequest).Assembly,
                typeof (XmlElement).Assembly,
                typeof (XObject).Assembly,
                typeof (DataContractAttribute).Assembly,
                typeof (BasicHttpBinding).Assembly,
                typeof (DataSet).Assembly,
                typeof (DataRowExtensions).Assembly,
                typeof (Geography).Assembly
            };

            var fileLookup = GetDocumentationLookup();

            return MetadataCache = assemblies
                .Select(a =>
                {
                    // Locate the name of the assembly to determine what the documentation file name is.
                    var assemblyName = a.GetName()?.Name;

                    // Attempt to locate an xml documentation file in the cache.
                    if (assemblyName != null && fileLookup.TryGetValue(assemblyName, out string docPath))
                        return MetadataReference.CreateFromFile(a.Location, documentation: XmlDocumentationProvider.CreateFromFile(docPath));

                    // Otherwise we have no documentation file and we'll return it without a doc provider.
                    return MetadataReference.CreateFromFile(a.Location);
                })
                .ToArray();
        }
    }
}
