using FluentAssertions;
using NSubstitute;
using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.Xunit2;
using Sitecore.Analytics;
using Sitecore.Analytics.Model.Entities;
using Sitecore.Analytics.Tracking;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.FakeDb.AutoFixture;
using Sitecore.Gigya.Connector.Services;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using Sitecore.Gigya.Testing.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sitecore.Gigya.Extensions.Tests.Services
{
    public class TrackerServiceTests
    {
        //[Theory, AutoDbData]
        //public void TrackLogin_Call_ShouldTrackLoginGoal(string identifier, Db db, [Frozen]ITrackerService trackerService)
        //{
        //    // Arrange

        //    db.Add(new DbItem("Item", AccountTrackerService.LoginGoalId));

        //    // Act
        //    accountTrackerService.TrackLoginAndIdentifyContact(identifier);

        //    // Assert
        //    trackerService.Received().TrackPageEvent(Arg.Is<ID>(AccountTrackerService.LoginGoalId));
        //}

        //[Theory, AutoDbData]
        //public void TrackRegister_Call_ShouldTrackRegistrationGoal(Db db, ID outcomeID, ITracker tracker, [Frozen]ITrackerService trackerService)
        //{
        //    // Arrange

        //    accountsSettingsService.GetRegistrationOutcome(Arg.Any<Item>()).Returns(outcomeID);

        //    db.Add(new DbItem("Item", AccountTrackerService.RegistrationGoalId));
        //    db.Add(new DbItem("Item", AccountTrackerService.LoginGoalId));

        //    //Act
        //    trackerService.IdentifyContact();

        //    //Assert
        //    trackerService.Received().TrackPageEvent(AccountTrackerService.RegistrationGoalId);
        //    trackerService.Received().TrackOutcome(outcomeID);
        //}

        [Theory]
        [AutoDbData]
        public void IdentifyContact_ValidIdentifier_ShouldIdentifyContact([NoAutoProperties] TrackerService trackerService, string contactIdentifier, ITracker tracker, [Substitute] Session session)
        {
            tracker.IsActive.Returns(true);
            tracker.Session.Returns(session);
            using (new TrackerSwitcher(tracker))
            {
                trackerService.IdentifyContact(contactIdentifier);
                tracker.Session.Received().Identify(contactIdentifier);
            }
        }
    }
}
