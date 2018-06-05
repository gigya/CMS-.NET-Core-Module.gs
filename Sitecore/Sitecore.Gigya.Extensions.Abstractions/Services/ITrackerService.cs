﻿using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Gigya.Extensions.Abstractions.Services
{
    public interface ITrackerService
    {
        bool IsActive { get; }
        void IdentifyContact(string identifier);
        void UpdateFacets(dynamic gigyaModel, List<MappingFieldGroup> mappingFields);
    }
}