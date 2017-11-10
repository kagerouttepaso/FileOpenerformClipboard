using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileOpenerformClipboard.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
            var query = Helper.Extentions.JointTailStrings(strings, new string[] { "", ":" });
            // 31通り
            Assert.AreEqual(31, query.Count());

            var results = new List<string>
            {
                "0",
                "01",
                "012",
                "0123",
                "01234"
            };
            foreach (var result in results)
            {
                Assert.IsTrue(query.Contains(result));
            }
        }

        [TestMethod()]
        public void FilePathBuilderTest()
        {
            var filePathQuery = Enumerable.Range(0, 6)
                .Select(x => x.ToString())
                .FilePathBuilder();
            var results = new List<string>
            {
                "0",
                "01",
                "012",
                "0123",
                "01234",
                "012345",
                "1",
                "12",
                "123",
                "1234",
                "12345",
                "2",
                "23",
                "234",
                "2345",
                "3",
                "34",
                "345",
                "4",
                "45",
                "5",
            };
            foreach (var result in results)
            {
                Assert.IsTrue(filePathQuery.Contains(result));
            }
        }

    }
}
