﻿using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlPerformanceKit.Test
{
    [TestClass]
    public class RcDataTest
    {
        private readonly List<HtmlParseErrorEventArgs> parseErrors = new List<HtmlParseErrorEventArgs>();
        private HtmlReader reader;

        [TestMethod]
        public void RcDataEmpty()
        {
            reader = HtmlReaderFactory.FromString("<title></title>", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.EndTag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void RcDataWithStartTag()
        {
            reader = HtmlReaderFactory.FromString("<title><p></title>", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("<p>", reader.Text);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.EndTag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void RcDataWithStartAndEndTag()
        {
            reader = HtmlReaderFactory.FromString("<title><p></p></title>", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("<p></p>", reader.Text);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.EndTag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void RcDataWithSelfClosingTag()
        {
            reader = HtmlReaderFactory.FromString("<title><br /></title>", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("<br />", reader.Text);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.EndTag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void RcDataWithText()
        {
            reader = HtmlReaderFactory.FromString("<title>hello</title>", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("hello", reader.Text);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.EndTag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void RcDataWithTag()
        {
            reader = HtmlReaderFactory.FromString("<title></title><p>", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.EndTag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
            Assert.AreEqual("p", reader.Name);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void RcDataWithCharacterReference()
        {
            reader = HtmlReaderFactory.FromString("<title>&amp;</title><p>", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("&", reader.Text);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.EndTag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
            Assert.AreEqual("p", reader.Name);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void RcDataWithCharacterReferenceWhenDecodingSkipped()
        {
            var options = new HtmlReaderOptions
            {
                DecodeHtmlCharacters = false
            };

            reader = HtmlReaderFactory.FromString("<title>&amp;</title><p>", parseErrors, options);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("&amp;", reader.Text);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.EndTag, reader.TokenKind);
            Assert.AreEqual("title", reader.Name);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Tag, reader.TokenKind);
            Assert.AreEqual("p", reader.Name);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }
    }
}
