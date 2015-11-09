﻿using HaloSharp.Exception;
using HaloSharp.Extension;
using HaloSharp.Model;
using HaloSharp.Model.Stats.CarnageReport;
using HaloSharp.Query.Stats;
using HaloSharp.Query.Stats.CarnageReport;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HaloSharp.Test.Query.Stats.CarnageReport
{
    [TestFixture]
    public class GetCampaignMatchDetailsTests : TestSessionSetup
    {
        [Test]
        [TestCase("Greenskull")]
        [TestCase("Furiousn00b")]
        public async Task GetCampaignMatchDetails(string gamertag)
        {
            var getMatchesQuery = new GetMatches()
                .InGameMode(Enumeration.GameMode.Campaign)
                .ForPlayer(gamertag);

            var matches = await Session.Query(getMatchesQuery);

            Assert.IsTrue(matches.Results.Any());

            foreach (var match in matches.Results)
            {
                var getCampaignMatchDetailsQuery = new GetCampaignMatchDetails()
                    .ForMatchId(match.Id.MatchId);

                var result = await Session.Query(getCampaignMatchDetailsQuery);

                Assert.IsInstanceOf(typeof(CampaignMatch), result);
            }
        }

        [Test]
        [TestCase("00000000-0000-0000-0000-000000000000")]
        public async Task GetCampaignMatchDetails_InvalidGuid(string guid)
        {
            var query = new GetCampaignMatchDetails()
                .ForMatchId(new Guid(guid));

            try
            {
                await Session.Query(query);
                Assert.Fail("An exception should have been thrown");
            }
            catch (HaloApiException e)
            {
                Assert.AreEqual((int)Enumeration.StatusCode.NotFound, e.HaloApiError.StatusCode);
            }
            catch (System.Exception e)
            {
                Assert.Fail("Unexpected exception of type {0} caught: {1}", e.GetType(), e.Message);
            }
        }

        [Test]
        public async Task GetCampaignMatchDetails_MissingGuid()
        {
            var query = new GetCampaignMatchDetails();

            try
            {
                await Session.Query(query);
                Assert.Fail("An exception should have been thrown");
            }
            catch (HaloApiException e)
            {
                Assert.AreEqual((int)Enumeration.StatusCode.NotFound, e.HaloApiError.StatusCode);
            }
            catch (System.Exception e)
            {
                Assert.Fail("Unexpected exception of type {0} caught: {1}", e.GetType(), e.Message);
            }
        }
    }
}