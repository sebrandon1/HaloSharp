﻿using System;
using System.IO;
using System.Threading.Tasks;
using HaloSharp.Exception;
using HaloSharp.Extension;
using HaloSharp.Model;
using HaloSharp.Model.Stats.CarnageReport;
using HaloSharp.Query.Stats.CarnageReport;
using HaloSharp.Test.Utility;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NUnit.Framework;

namespace HaloSharp.Test.Query.Stats.CarnageReport
{
    [TestFixture]
    public class GetCampaignMatchDetailsTests
    {
        private IHaloSession _mockSession;
        private CampaignMatch _campaignMatch;

        [SetUp]
        public void Setup()
        {
            _campaignMatch = JsonConvert.DeserializeObject<CampaignMatch>(File.ReadAllText(Config.CampaignMatchJsonPath));

            var mock = new Mock<IHaloSession>();
            mock.Setup(m => m.Get<CampaignMatch>(It.IsAny<string>()))
                .ReturnsAsync(_campaignMatch);

            _mockSession = mock.Object;
        }

        [Test]
        public void GetConstructedUri_NoParamaters_MatchesExpected()
        {
            var query = new GetCampaignMatchDetails();

            var uri = query.GetConstructedUri();

            Assert.AreEqual("stats/h5/campaign/matches/", uri);
        }

        [Test]
        [TestCase("00000000-0000-0000-0000-000000000000")]
        public void GetConstructedUri_ForMatchId_MatchesExpected(string guid)
        {
            var query = new GetCampaignMatchDetails()
                .ForMatchId(new Guid(guid));

            var uri = query.GetConstructedUri();

            Assert.AreEqual($"stats/h5/campaign/matches/{guid}", uri);
        }

        [Test]
        public async Task Query_DoesNotThrow()
        {
            var query = new GetCampaignMatchDetails();

            var result = await _mockSession.Query(query);

            Assert.IsInstanceOf(typeof(CampaignMatch), result);
            Assert.AreEqual(_campaignMatch, result);
        }

        [Test]
        [TestCase("71fe5636-9db1-41fe-871d-515cd5e0ed87")]
        public async Task GetCampaignMatchDetails_DoesNotThrow(string guid)
        {
            var query = new GetCampaignMatchDetails()
                .ForMatchId(new Guid(guid));

            var result = await Global.Session.Query(query);

            Assert.IsInstanceOf(typeof(CampaignMatch), result);
        }

        [Test]
        [TestCase("71fe5636-9db1-41fe-871d-515cd5e0ed87")]
        public async Task GetCampaignMatchDetails_SchemaIsValid(string guid)
        {
            var weaponsSchema = JSchema.Parse(File.ReadAllText(Config.CampaignMatchJsonSchemaPath), new JSchemaReaderSettings
            {
                Resolver = new JSchemaUrlResolver(),
                BaseUri = new Uri(Path.GetFullPath(Config.CampaignMatchJsonSchemaPath))
            });

            var query = new GetCampaignMatchDetails()
                .ForMatchId(new Guid(guid));

            var jArray = await Global.Session.Get<JObject>(query.GetConstructedUri());

            SchemaUtility.AssertSchemaIsValid(weaponsSchema, jArray);
        }

        [Test]
        [TestCase("71fe5636-9db1-41fe-871d-515cd5e0ed87")]
        public async Task GetCampaignMatchDetails_ModelMatchesSchema(string guid)
        {
            var schema = JSchema.Parse(File.ReadAllText(Config.CampaignMatchJsonSchemaPath), new JSchemaReaderSettings
            {
                Resolver = new JSchemaUrlResolver(),
                BaseUri = new Uri(Path.GetFullPath(Config.CampaignMatchJsonSchemaPath))
            });

            var query = new GetCampaignMatchDetails()
                .ForMatchId(new Guid(guid));

            var result = await Global.Session.Query(query);

            var json = JsonConvert.SerializeObject(result);
            var jContainer = JsonConvert.DeserializeObject<JContainer>(json);

            SchemaUtility.AssertSchemaIsValid(schema, jContainer);
        }

        [Test]
        [TestCase("71fe5636-9db1-41fe-871d-515cd5e0ed87")]
        public async Task GetCampaignMatchDetails_IsSerializable(string guid)
        {
            var query = new GetCampaignMatchDetails()
                .ForMatchId(new Guid(guid));

            var result = await Global.Session.Query(query);

            SerializationUtility<CampaignMatch>.AssertRoundTripSerializationIsPossible(result);
        }

        [Test]
        [TestCase("00000000-0000-0000-0000-000000000000")]
        public async Task GetCampaignMatchDetails_InvalidGuid(string guid)
        {
            var query = new GetCampaignMatchDetails()
                .ForMatchId(new Guid(guid));

            try
            {
                await Global.Session.Query(query);
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
                await Global.Session.Query(query);
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