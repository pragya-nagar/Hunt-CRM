using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Synergy.Common.Exceptions;

namespace Synergy.CRM.Models
{
    public class FileId
    {
        protected FileId()
        {
        }

        public string EntityType { get; protected set; }

        public string Id { get; protected set; }

        public Guid CampaignId { get; protected set; }

        public string FileName { get; protected set; }

        public static FileId Generate(Guid campaignId, string entityType, string friendlyName = null)
        {
            var fileName = $"{entityType}/campaign_{campaignId}/{DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}/{Guid.NewGuid()}";

            if (string.IsNullOrWhiteSpace(friendlyName) == false)
            {
                fileName += "/" + friendlyName;
            }

            var id = fileName.Replace('/', ':');
            return new FileId()
            {
                CampaignId = campaignId,
                FileName = fileName,
                Id = id,
            };
        }

        public static FileId Parse(string id)
        {
            var idMatch = Regex.Match(id, @"^(?<entityType>\w+):campaign_(?<campaignId>.{36}):\d\d\d\d-\d\d-\d\d:.{36}(?<friendlyName>:.+)?$");
            if (idMatch.Success == false)
            {
                throw new ModelStateException("id", "Invalid id format");
            }

            var campaignIdString = idMatch.Groups["campaignId"].Value;

            if (Guid.TryParse(campaignIdString, out var campaignId) == false)
            {
                throw new ModelStateException("id", "Invalid id format");
            }

            var entityType = idMatch.Groups["entityType"].Value;

            return new FileId
            {
                EntityType = entityType,
                Id = id,
                CampaignId = campaignId,
                FileName = id.Replace(':', '/'),
            };
        }
    }
}
