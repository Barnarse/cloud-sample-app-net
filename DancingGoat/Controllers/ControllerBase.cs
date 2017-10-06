﻿using System;
using System.Globalization;
using System.Web.Mvc;

using KenticoCloud.Delivery;

using DancingGoat.Areas.Admin;
using DancingGoat.Infrastructure;
using DancingGoat.Models;
using DancingGoat.Localization;
using KenticoCloud.Personalization;

namespace DancingGoat.Controllers
{
    [SelfConfigActionFilter]
    public class ControllerBase : AsyncController
    {
        protected static readonly DeliveryClient baseClient = CreateDeliveryClient();
        protected static readonly PersonalizationClient basePersonalizationClient = CreatePersonalizationClient();
        public readonly IDeliveryClient client;
        public readonly PersonalizationClient personalizationClient;

        public ControllerBase()
        {
            var currentCulture = CultureInfo.CurrentUICulture.Name;
            if (currentCulture.Equals(LanguageClient.DEFAULT_LANGUAGE, StringComparison.InvariantCultureIgnoreCase))
            {
                client = baseClient;
            }
            else
            {
                client = new LanguageClient(baseClient, currentCulture);
            }

            personalizationClient = basePersonalizationClient;
        }
       

        public static DeliveryClient CreateDeliveryClient()
        {
            var previewToken = AppSettingProvider.PreviewToken;
            var projectId = AppSettingProvider.ProjectId ?? AppSettingProvider.DefaultProjectId;

            var clientInstance = 
                !string.IsNullOrEmpty(previewToken) ? 
                    new DeliveryClient(projectId.Value.ToString(), previewToken) : 
                    new DeliveryClient(projectId.Value.ToString());

            clientInstance.CodeFirstModelProvider.TypeProvider = new CustomTypeProvider();
            clientInstance.ContentLinkUrlResolver = new CustomContentLinkUrlResolver();
            return clientInstance;
        }

        public static PersonalizationClient CreatePersonalizationClient()
        {
            var personalizationToken = AppSettingProvider.PersonalizationToken;

            if (!string.IsNullOrWhiteSpace(personalizationToken))
            {
                return new PersonalizationClient(personalizationToken);
            }

            return null;
        }
    }
}

