using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Gigya.Extensions.Abstractions.Analytics.Models
{
    //public class MappingField
    //{
    //    public string GigyaFieldName { get; set; }
    //    public string CmsFieldName { get; set; }
    //    public List<MappingField> Children { get; set; }
    //}

    public class MappingFieldGroup
    {
        public ContactPersonalInfoMapping PersonalInfoMapping { get; set; }
        public ContactPhoneNumbersMapping PhoneNumbersMapping { get; set; }
        public ContactEmailAddressesMapping EmailAddressesMapping { get; set; }
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

    public class ContactPhoneNumbersMapping : MappingBase
    {
        public string Preferred { get; set; }
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
        public string Preferred { get; set; }
        public List<ContactEmailAddressMapping> Entries { get; set; }
    }

    public class ContactEmailAddressMapping : MappingBase
    {
        public string SmtpAddress { get; set; }
        public string BounceCount { get; set; }
    }
}
