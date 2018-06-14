using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Gigya.Extensions.Abstractions.Analytics.Models
{
    public class MappingFieldGroup
    {
        public ContactPersonalInfoMapping PersonalInfoMapping { get; set; }
        public ContactPhoneNumbersMapping PhoneNumbersMapping { get; set; }
        public ContactEmailAddressesMapping EmailAddressesMapping { get; set; }
        public ContactAddressesMapping AddressesMapping { get; set; }
        public CommunicationProfileMapping CommunicationProfileMapping { get; set; }
        public PreferencesMapping CommunicationPreferencesMapping { get; set; }
        public GigyaFieldsMapping GigyaFieldsMapping { get; set; }
    }

    public abstract class MappingBase
    {
        public string Key { get; set; }
    }

    public class ContactPersonalInfoMapping : MappingBase
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string Title { get; set; }
        public string Suffix { get; set; }
        public string Nickname { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string JobTitle { get; set; }
    }

    public class CommunicationProfileMapping : MappingBase
    {
        public string CommunicationRevoked { get; set; }
        public string ConsentRevoked { get; set; }
    }

    public class PreferencesMapping : MappingBase
    {
        public string Language { get; set; }
    }

    public class ContactPhoneNumbersMapping : MappingBase
    {
        public List<ContactPhoneNumberMapping> Entries { get; set; }
    }

    public class ContactPhoneNumberMapping : MappingBase
    {
        public string CountryCode { get; set; }
        public string Number { get; set; }
        public string Extension { get; set; }
    }

    public class ContactEmailAddressesMapping : MappingBase
    {
        public List<ContactEmailAddressMapping> Entries { get; set; }
    }

    public class ContactEmailAddressMapping : MappingBase
    {
        public string SmtpAddress { get; set; }
        public string BounceCount { get; set; }
    }

    public class ContactAddressesMapping : MappingBase
    {
        public List<ContactAddressMapping> Entries { get; set; }
    }

    public class ContactAddressMapping : MappingBase
    {
        public string Country { get; set; }
        public string StateProvince { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string StreetLine1 { get; set; }
        public string StreetLine2 { get; set; }
        public string StreetLine3 { get; set; }
        public string StreetLine4 { get; set; }
        public string Latitude { get; }
        public string Longitude { get; }
    }

    public class GigyaFieldsMapping : MappingBase
    {
        public List<GigyaMapping> Entries { get; set; }
    }

    public class GigyaMapping : MappingBase
    {
        public string GigyaProperty { get; set; }
    }
}
