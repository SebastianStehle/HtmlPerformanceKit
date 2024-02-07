﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

namespace HtmlPerformanceKit.Benchmark
{
    [Config(typeof(Config))]
    [MemoryDiagnoser]
    public class BenchmarkHtmlPerformanceKit
    {
#if DEBUG || RELEASE
        private static readonly HtmlReaderOptions KeepOpen = new HtmlReaderOptions
        {
            KeepOpen = true
        };
#endif
        private readonly Stream stream;
        private readonly StreamReader streamReader;

        public BenchmarkHtmlPerformanceKit()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            stream = executingAssembly.GetManifestResourceStream("HtmlPerformanceKit.Benchmark.en.wikipedia.org_wiki_List_of_Australian_treaties.html");
            streamReader = new StreamReader(stream);
        }

        public class Config : ManualConfig
        {
            public Config()
            {
                var baseJob = Job.ShortRun;

                AddJob(baseJob
                    .WithId("Dev").WithBaseline(true));

                AddJob(baseJob.WithCustomBuildConfiguration("V0_8_1")
                    .WithId("0.8.1"));
            }
        }

        [Benchmark]
        public List<string> ExtractLinks()
        {
            stream.Seek(0, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();

#if DEBUG || RELEASE
            using var htmlReader = new HtmlReader(streamReader, KeepOpen);
#else
            var htmlReader = new HtmlReader(streamReader);
#endif
            var links = new List<string>();

            while (htmlReader.Read())
            {
                if (htmlReader.TokenKind == HtmlTokenKind.Tag && htmlReader.Name == "a")
                {
                    var hrefAttributeValue = htmlReader.GetAttribute("href");
                    if (hrefAttributeValue != null)
                    {
                        links.Add(hrefAttributeValue);
                    }
                }
            }

            return links;
        }

#if DEBUG || RELEASE
        [Benchmark]
        public List<ReadOnlyMemory<char>> ExtractLinksAsMemory()
        {
            stream.Seek(0, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();

            using var htmlReader = new HtmlReader(streamReader, KeepOpen);
            var links = new List<ReadOnlyMemory<char>>();

            while (htmlReader.Read())
            {
                if (htmlReader.TokenKind == HtmlTokenKind.Tag && htmlReader.Name == "a")
                {
                    var hrefAttributeValue = htmlReader.GetAttributeAsMemory("href");
                    if (hrefAttributeValue.Length > 0)
                    {
                        links.Add(hrefAttributeValue);
                    }
                }
            }

            return links;
        }
#else
        [Benchmark]
        public List<ReadOnlyMemory<char>> ExtractLinksAsMemory()
        {
            return new List<ReadOnlyMemory<char>>();
        }
#endif

        [Benchmark]
        public List<string> ExtractTexts()
        {
            stream.Seek(0, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();

#if DEBUG || RELEASE
            using var htmlReader = new HtmlReader(streamReader, KeepOpen);
#else
            var htmlReader = new HtmlReader(streamReader);
#endif
            var texts = new List<string>();

            while (htmlReader.Read())
            {
                if (htmlReader.TokenKind == HtmlTokenKind.Text)
                {
                    texts.Add(htmlReader.Text);
                }
            }

            return texts;
        }

#if DEBUG || RELEASE
        [Benchmark]
        public List<ReadOnlyMemory<char>> ExtractTextsAsMemory()
        {
            stream.Seek(0, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();

            using var htmlReader = new HtmlReader(streamReader, KeepOpen);
            var texts = new List<ReadOnlyMemory<char>>();

            while (htmlReader.Read())
            {
                if (htmlReader.TokenKind == HtmlTokenKind.Text)
                {
                    texts.Add(htmlReader.TextAsMemory);
                }
            }

            return texts;
        }
#else
        [Benchmark]
        public List<ReadOnlyMemory<char>> ExtractTextsAsMemory()
        {
            return new List<ReadOnlyMemory<char>>();
        }
#endif
    }
}
