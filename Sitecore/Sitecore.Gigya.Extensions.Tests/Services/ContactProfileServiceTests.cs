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
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using Sitecore.Gigya.Extensions.Services;
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
    public class ContactProfileServiceTests
    {
        private ExpandoObject GetGigyaModel()
        {
            dynamic gigyaModel = new ExpandoObject();
            gigyaModel.profile = new ExpandoObject();
            gigyaModel.profile.firstName = "MyFirstName";
            gigyaModel.profile.lastName = "MyLastName";
            gigyaModel.profile.dob = DateTime.Now;
            gigyaModel.profile.gender = "Male";
            gigyaModel.profile.middleName = "Middle";
            gigyaModel.profile.nickName = "Nick";
            gigyaModel.profile.phoneNumber = "0123456789";
            return gigyaModel;
        }

        [Theory]
        [AutoDbData]
        public void ShouldUpdatePersonalFacet([NoAutoProperties] ContactProfileService contactProfileService, ITracker tracker, [Substitute] Session session)
        {
            // arrange
            tracker.IsActive.Returns(true);
            tracker.Session.Returns(session);

            var mapping = new MappingFieldGroup
            {
                PersonalInfoMapping = new ContactPersonalInfoMapping
                {
                    BirthDate = "profile.dob",
                    FirstName = "profile.firstName",
                    Gender = "profile.gender",
                    MiddleName = "profile.middleName",
                    Nickname = "profile.nickName",
                    Surname = "profile.lastName",
                    Title = "profile.title"
                }
            };

            dynamic gigyaModel = GetGigyaModel();

            using (new TrackerSwitcher(tracker))
            {
                // act
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                contactProfileService.UpdateFacets(gigyaModel, mapping);

                //var elapsed = "Took " + stopwatch.ElapsedMilliseconds;
                //elapsed.ShouldBeEquivalentTo("");

                // assert
                var facet = contactProfileService.ContactProfileProvider.PersonalInfo;

                facet.FirstName.ShouldBeEquivalentTo((string)gigyaModel.profile.firstName);
                facet.Surname.ShouldBeEquivalentTo((string)gigyaModel.profile.lastName);
                facet.BirthDate.ShouldBeEquivalentTo((DateTime)gigyaModel.profile.dob);
                facet.Gender.ShouldBeEquivalentTo((string)gigyaModel.profile.gender);
                facet.MiddleName.ShouldBeEquivalentTo((string)gigyaModel.profile.middleName);
                facet.Nickname.ShouldBeEquivalentTo((string)gigyaModel.profile.nickName);
            }
        }

        [Theory]
        [AutoDbData]
        public void ShouldUpdateExistingPhoneNumberEntryFacet([NoAutoProperties] ContactProfileService contactProfileService, ITracker tracker, [Substitute] Session session)
        {
            // arrange
            tracker.IsActive.Returns(true);
            tracker.Session.Returns(session);

            var mapping = new MappingFieldGroup
            {
                PhoneNumbersMapping = new ContactPhoneNumbersMapping
                {
                    Entries = new List<ContactPhoneNumberMapping>
                    {
                        new ContactPhoneNumberMapping
                        {
                            Key = "Primary",
                            Number = "profile.phoneNumber"
                        }
                    }
                }
            };

            dynamic gigyaModel = GetGigyaModel();

            using (new TrackerSwitcher(tracker))
            {
                // act
                contactProfileService.UpdateFacets(gigyaModel, mapping);

                // assert
                var facet = contactProfileService.ContactProfileProvider.PhoneNumbers;

                facet.Entries[mapping.PhoneNumbersMapping.Entries[0].Key].Number.ShouldBeEquivalentTo((string)gigyaModel.profile.phoneNumber);
            }
        }

        //[Theory]
        //[AutoDbData]
        //public void ShouldSetContactProfile([Frozen] IContactProfileProvider contactProfileProvider, string key, IPhoneNumber phoneNumber, string firstName, string lastName, IContactPersonalInfo personalInfo, Sitecore.Analytics.Tracking.Contact contact)
        //{
        //    contactProfileProvider.Contact.Returns(contact);
        //    var contactProfileService = new ContactProfileService(contactProfileProvider);
        //    contactProfileProvider.PersonalInfo.Returns(personalInfo);
        //    var profile = new EditProfile();
        //    profile.FirstName = firstName;
        //    profile.LastName = lastName;
        //    profile.PhoneNumber = phoneNumber.Number;
        //    contactProfileService.SetProfile(profile);
        //    contactProfileProvider.PersonalInfo.FirstName.ShouldBeEquivalentTo(profile.FirstName);
        //    contactProfileProvider.PersonalInfo.Surname.ShouldBeEquivalentTo(profile.LastName);
        //    contactProfileProvider.PhoneNumbers.Entries[contactProfileProvider.PhoneNumbers.Preferred].Number.ShouldBeEquivalentTo(profile.PhoneNumber);
        //}

        //[Theory]
        //[AutoDbData]
        //public void ShouldSetPreferredEmail([Frozen] IContactProfileProvider contactProfileProvider, [Greedy] ContactProfileService contactProfileService, string email, IEmailAddress emailAddress)
        //{
        //    contactProfileService.SetPreferredEmail(emailAddress.SmtpAddress);
        //    contactProfileProvider.Emails.Entries[contactProfileProvider.Emails.Preferred].SmtpAddress.ShouldBeEquivalentTo(emailAddress.SmtpAddress);
        //}

        //[Theory]
        //[AutoDbData]
        //public void SetTag_NotEmptyStringParameters_ShouldCallSetOnTagsPropertyOfContactWithSameParameters([Frozen] IContactProfileProvider contactProfileProvider, [Greedy] ContactProfileService contactProfileService, string tagName, string tagValue)
        //{
        //    contactProfileService.SetTag(tagName, tagValue);
        //    contactProfileProvider.Contact.Tags.Received().Set(tagName, tagValue);
        //}

        //[Theory]
        //[AutoDbData]
        //public void SetTag_TagValueParameterIsEmpty_ShouldNotCallSetOnTagsPropertyOfContact([Frozen] IContactProfileProvider contactProfileProvider, [Greedy] ContactProfileService contactProfileService, string tagName)
        //{
        //    contactProfileService.SetTag(tagName, null);
        //    contactProfileProvider.Contact.Tags.DidNotReceive().Set(tagName, null);
        //}

        //[Theory]
        //[AutoDbData]
        //public void SetTag_TagValueParameterExists_ShouldNotCallSetOnTagsPropertyOfContact([Frozen] IContactProfileProvider contactProfileProvider, [Greedy] ContactProfileService contactProfileService, string tagName, string tagValue)
        //{
        //    contactProfileProvider.Contact.Tags.GetAll(tagName).Returns(new List<string>() { tagValue });
        //    contactProfileService.SetTag(tagName, tagValue);
        //    contactProfileProvider.Contact.Tags.DidNotReceive().Set(tagName, tagValue);
        //}

        //[Theory]
        //[AutoDbData]
        //public void SetTag_TagValueParameterDoesNotExist_ShouldNotCallSetOnTagsPropertyOfContact([Frozen] IContactProfileProvider contactProfileProvider, [Greedy] ContactProfileService contactProfileService, string tagName, string tagValue, string anotherTagValue)
        //{
        //    contactProfileProvider.Contact.Tags.GetAll(tagName).Returns(new List<string>() { anotherTagValue });
        //    contactProfileService.SetTag(tagName, tagValue);
        //    contactProfileProvider.Contact.Tags.Received().Set(tagName, tagValue);
        //}
    }
}
