﻿using JsonLD.Core;
using JsonLD.GenericJson;
using JsonLD.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace JsonLD.Test
{
    public class ExtendedFunctionalityTests
    {
        private const string ManifestRoot = "ExtendedFunctionality";

        [Theory, MemberData(nameof(ExtendedFunctionalityCases))]
        public void ExtendedFunctionalityTestPasses(string id, ExtendedFunctionalityTestCase testCase)
        {
            GenericJsonToken result = testCase.run();
            if (testCase.error != null)
            {
                Assert.True(((string)result["error"]).StartsWith((string)testCase.error), "Resulting error doesn't match expectations.");
            }
            else
            {
                if (!JsonLdUtils.DeepCompare(result, testCase.output, true))
                {
#if DEBUG
                    Console.WriteLine(id);
                    Console.WriteLine("Actual:");
                    Console.Write(JSONUtils.ToPrettyString(result));
                    Console.WriteLine("--------------------------");
                    Console.WriteLine("Expected:");
                    Console.Write(JSONUtils.ToPrettyString(testCase.output));
                    Console.WriteLine("--------------------------");
#endif

                    Assert.True(false, "Returned JSON doesn't match expectations.");
                }
            }
        }

        public class ExtendedFunctionalityTestCase
        {
            public GenericJsonToken  input { get; set; }
            public GenericJsonToken output { get; set; }
            public GenericJsonToken context { get; set; }
            public GenericJsonToken frame { get; set; }
            public GenericJsonToken error { get; set; }
            public Func<GenericJsonToken> run { get; set; }
        }

        public static IEnumerable<object[]> ExtendedFunctionalityCases()
        {
            foreach (var testCase in SortingTestCases())
            {
                yield return testCase;
            }
        }

        private static string[] SortingManifests =
        {
            "fromRdf-manifest.jsonld"
        };

        private static IEnumerable<object[]> SortingTestCases()
        {
            var jsonFetcher = new JsonFetcher();
            var rootDirectory = Path.Combine(ManifestRoot, "Sorting");
            
            foreach (string manifest in SortingManifests)
            {
                GenericJsonToken manifestJson = jsonFetcher.GetJson(manifest, rootDirectory);

                foreach (GenericJsonObject testcase in manifestJson["sequence"])
                {
                    Func<GenericJsonToken> run = null;
                    ExtendedFunctionalityTestCase newCase = new ExtendedFunctionalityTestCase();

                    newCase.input = jsonFetcher.GetJson(manifestJson["input"], rootDirectory);
                    newCase.output = jsonFetcher.GetJson(testcase["expect"], rootDirectory);

                    var options = new JsonLdOptions();
                    
                    var sortType = (string)testcase["sort-type"];

                    if (sortType == "jld:GraphsAndNodes")
                    {
                        options.SetSortGraphsFromRdf(true);
                        options.SetSortGraphNodesFromRdf(true);
                    }
                    else if (sortType == "jld:Graphs")
                    {
                        options.SetSortGraphsFromRdf(true);
                        options.SetSortGraphNodesFromRdf(false);
                    }
                    else if (sortType == "jld:Nodes")
                    {
                        options.SetSortGraphsFromRdf(false);
                        options.SetSortGraphNodesFromRdf(true);
                    }
                    else if (sortType == "jld:None")
                    {
                        options.SetSortGraphsFromRdf(false);
                        options.SetSortGraphNodesFromRdf(false);
                    }

                    JsonLdApi jsonLdApi = new JsonLdApi(options);

                    var testType = (string)testcase["test-type"];

                    if (testType == "jld:FromRDF")
                    {
                        GenericJsonToken quads = newCase.input["quads"];
                        RDFDataset rdf = new RDFDataset();

                        foreach (GenericJsonToken quad in quads)
                        {
                            string subject = (string)quad["subject"];
                            string predicate = (string)quad["predicate"];
                            string value = (string)quad["value"];
                            string graph = (string)quad["graph"];

                            rdf.AddQuad(subject, predicate, value, graph);
                        }

                        options.format = "application/nquads";

                        run = () => jsonLdApi.FromRDF(rdf);
                    }
                    else
                    {
                        run = () => { throw new Exception("Couldn't find a test type, apparently."); };
                    }

                    newCase.run = run;

                    yield return new object[] { manifest + (string)testcase["@id"], newCase };
                }
            }
        }
    }
}
