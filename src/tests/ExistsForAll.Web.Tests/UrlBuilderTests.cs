using System;
using System.Linq;
using ExistsForAll.Web.Url;
using FluentAssertions;
using FluentAssertions.Common;
using Xunit;

namespace ExistsForAll.Web.Tests
{
    public class UrlBuilderTests
    {
        [Fact]
        public void UrlBuilder_WhenCasualUrl_PartsAreCorrect()
        {
            const string url = "http://www.quali.com";

            var builder = new UrlBuilder(url);

            Assert.Equal("http", builder.Protocol);
            Assert.Equal("www.quali.com", builder.Host);
            Assert.Null(builder.Path);
            Assert.Equal(url, builder.ToString());
        }

        [Fact]
        public void UrlBuilder_WhenIPAddressUrl_PartsAreCorrect()
        {
            const string url = "http://10.10.10.10";

            var builder = new UrlBuilder(url);

            Assert.Equal("http", builder.Protocol);
            Assert.Equal("10.10.10.10", builder.Host);
            Assert.Null(builder.Path);
            Assert.Equal(url, builder.ToString());
        }

        [Fact]
        public void UrlBuilder_WhenUrlWithQuery_QueryIsGood()
        {
            const string url = "https://youtube/page.php?color=red&width=200&height=300";
            var builder = new UrlBuilder(url);

            builder.Query.GetValue("color").Should().BeEquivalentTo("red");
            builder.Query.GetValue("width").Should().BeEquivalentTo("200");
            builder.Query.GetValue("height").Should().BeEquivalentTo("300");
            builder.Query.Values.Should().HaveCount(3);
        }

        [Theory]
        [InlineData("http://www.biatch.com", "www.biatch.com")]
        [InlineData("http://www.biatch.com:1010", "www.biatch.com")]
        [InlineData("http://kewl:1787010/hhuhu", "kewl")]
        [InlineData("kewl:1787010/hhuhu", "kewl")]
        [InlineData("baaa/hhuhu", "baaa")]
        [InlineData("/hhuhu", null)]
        public void UrlBuilder_WhenVariousHosts_ParsedCorrectly(string url, string result)
        {
            new UrlBuilder(url).Host.Should().BeEquivalentTo(result);
        }

        [Fact]
        public void UrlBuilder_RelativeUrl_PathExists()
        {
            const string url = "/whatsup/allgoodya?jey=sa&do=ba";
            var builder = new UrlBuilder(url);
            builder.Protocol.Should().BeNull();
            builder.Host.Should().BeNull();
            builder.Port.HasValue.Should().BeFalse();
            builder.Path.Should().BeEquivalentTo("/whatsup/allgoodya");
            builder.Query.Values.Should().HaveCount(2);
            builder.Query.GetValue("jey").Should().BeEquivalentTo("sa");
            builder.Query.GetValue("do").Should().BeEquivalentTo("ba");
        }

        [Theory]
        [InlineData("hello")]
        [InlineData("hello man")]
        [InlineData("hello man, how you doing")]
        [InlineData("hello man, how you doing?")]
        [InlineData("hello man, how you doin'?")]
        [InlineData("&hello &=?man=, how!@ you doin'?")]
        public void UrlBuilder_QueryParameters_ShouldBeEncodedAndDecodedCorrectly(string param)
        {
            var builder = new UrlBuilder("www.jinx.com");
            builder.Query.AddOrUpdate("k", param);
            var secondBuilder = new UrlBuilder(builder.ToString());
            secondBuilder.Query.GetValue("k").Should().BeEquivalentTo(param);
        }

        [Fact]
        public void UrlBuilder_QueryParameters_ShouldFormatArraysIntoMultipleKeys()
        {
            var builder = new UrlBuilder();
            builder.Query.AddOrUpdateCollection("ids", new[] {"22", "#33", "444"});
            var url = builder.ToString();
            var key = Uri.EscapeDataString("ids[]");
            var secondValue = Uri.EscapeDataString("#33");
            url.Should().BeEquivalentTo($"?{key}=22&{key}={secondValue}&{key}=444");
        }

        [Fact]
        public void UrlBuilder_QueryParameters_ShouldParseArrays()
        {
            var key = Uri.EscapeDataString("ids[]");
            var url = $"http://www.dragons.com/?{key}=1&{key}=3&{key}=hello";
            var builder = new UrlBuilder(url);
            var values = builder.Query.GetValues("ids").ToArray();
            values.Length.Should().IsSameOrEqualTo(3);
            values[0].Should().BeEquivalentTo("1");
            values[1].Should().BeEquivalentTo("3");
            values[2].Should().BeEquivalentTo("hello");
            builder.ToString().Should().BeEquivalentTo(url);
        }

        [Fact]
        public void UrlBuilder_QueryParameters_EmptyParam()
        {
            var url = "/index.html?a=1&b";
            var builder = new UrlBuilder(url);
            builder.Query.GetValue("b").Should().BeEquivalentTo(null);
            builder.ToString().Should().BeEquivalentTo(url);
        }
    }
}
