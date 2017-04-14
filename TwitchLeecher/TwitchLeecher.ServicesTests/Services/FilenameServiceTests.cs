using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitchLeecher.Core.Models;
using System;
using System.Collections.Generic;

namespace TwitchLeecher.Services.Services.Tests
{
    [TestClass()]
    public class FilenameServiceTests
    {
        [TestMethod()]
        public void SubstituteWildcardsTest()
        {
            List<TwitchVideoQuality> resolutions = new List<TwitchVideoQuality>();
            resolutions.Add(new TwitchVideoQuality(TwitchVideoQuality.QUALITY_SOURCE, "1280x720", "60"));

            TwitchVideo video = new TwitchVideo("test_channel", "test_title", "test_id", "Test Game", 0, new TimeSpan(1, 10, 45), resolutions, new DateTime(2017, 12, 31, 23, 50, 49), new Uri("https://example.com/"), new Uri("https://example.com"));
            FilenameService service = new FilenameService();
            
            Assert.AreEqual("test_channel.mp4", service.SubstituteWildcards("{channel}.mp4", video));
            Assert.AreEqual("test_title.mp4", service.SubstituteWildcards("{title}.mp4", video));
            Assert.AreEqual("est_id.mp4", service.SubstituteWildcards("{id}.mp4", video)); // The given ID has the first character stripped off
            Assert.AreEqual("Test Game.mp4", service.SubstituteWildcards("{game}.mp4", video));
            Assert.AreEqual("1280x720.mp4", service.SubstituteWildcards("{res}.mp4", video));
            Assert.AreEqual("60.mp4", service.SubstituteWildcards("{fps}.mp4", video));
            Assert.AreEqual("20171231.mp4", service.SubstituteWildcards("{date}.mp4", video));
            Assert.AreEqual("115049PM.mp4", service.SubstituteWildcards("{time}.mp4", video));
            Assert.AreEqual("235049.mp4", service.SubstituteWildcards("{time24}.mp4", video));

            // Common formats to test multiple values
            Assert.AreEqual("test_channel - test_title (1280x720, 60fps).mp4", service.SubstituteWildcards("{channel} - {title} ({res}, {fps}fps).mp4", video));
            Assert.AreEqual("[20171231-235049] [est_id] - test_title (1280x720, 60fps).mp4", service.SubstituteWildcards("[{date}-{time24}] [{id}] - {title} ({res}, {fps}fps).mp4", video));

            // Ensure invalid chars are stripped after all replacements
            TwitchVideo invalidVideo = new TwitchVideo(@"channel: invalid\chars", "test_title", "test_id", "Test Game", 0, new TimeSpan(1, 10, 45), resolutions, new DateTime(2017, 12, 31, 23, 50, 49), new Uri("https://example.com/"), new Uri("https://example.com"));
            Assert.AreEqual("channel_ invalid_chars.mp4", service.SubstituteWildcards("{channel}.mp4", invalidVideo));
        }
    }
}