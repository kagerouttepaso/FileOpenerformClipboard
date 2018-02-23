using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileOpenerformClipboard.Helper.Tests
{
    [TestClass()]
    public class ExtentionsTests
    {
        [TestMethod()]
        public void JointTailStringsTest()
        {
            var strings = Enumerable.Range(0, 5)
               .Select(x => x.ToString())
               .ToList();
            var query = Extentions.JointTailStrings(strings, new string[] { "", ":" });
            // 31通り
            Assert.AreEqual(31, query.Count());

            var results = new List<string>
            {
                "0",
                "01",
                "0:1",
                "012",
                "01:2",
                "0:12",
                "0:1:2",
                "0123",
                "012:3",
                "01:23",
                "01:2:3",
                "0:123",
                "0:12:3",
                "0:1:23",
                "0:1:2:3",
                "01234",
                "0123:4",
                "012:34",
                "012:3:4",
                "01:234",
                "01:23:4",
                "01:2:34",
                "01:2:3:4",
                "0:1234",
                "0:123:4",
                "0:12:34",
                "0:12:3:4",
                "0:1:234",
                "0:1:23:4",
                "0:1:2:34",
                "0:1:2:3:4",
            };
            foreach (var result in results)
            {
                Assert.IsTrue(query.Contains(result));
            }
        }

        [TestMethod()]
        public void FilePathBuilderTest()
        {
            var filePathQuery = Enumerable.Range(0, 3)
                .Select(x => x.ToString())
                .FilePathBuilder()
                .ToArray();
            Assert.AreEqual(13, filePathQuery.Length);
            var results = new List<string>
            {
                @"0",

                @"01",
                @"0\1",
                @"0 1",

                @"012",
                @"01\2",
                @"01 2",
                @"0\12",
                @"0\1\2",
                @"0\1 2",
                @"0 12",
                @"0 1\2",
                @"0 1 2",
            };

            foreach (var result in results)
            {
                Assert.IsTrue(filePathQuery.Contains(result));
            }
        }
    }
}