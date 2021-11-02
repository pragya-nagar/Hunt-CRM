using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Synergy.Common.Domain.Models.Extensions;
using Synergy.DataAccess.Abstractions.Commands.Interfaces;
using Synergy.DataAccess.Context;

namespace Synergy.CRM.DAL.Commands.Queries
{
    public class CampaignPropertyDumpQuery : CollectionQuery<Guid, IDictionary<string, object>>
    {
        private readonly ISynergyContext _context;

        public CampaignPropertyDumpQuery(ISynergyContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override async Task<IEnumerable<IDictionary<string, object>>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var rootList = await (from campaignLead in this._context.CampaignLead
                                  join campaign in this._context.Campaign on campaignLead.CampaignId equals campaign.Id
                                  join ct in this._context.CampaignType on campaign.CampaignTypeId equals ct.Id
                                  join user in this._context.User on campaign.AssignedUserId equals user.Id
                                  join lead in this._context.Lead on campaignLead.LeadId equals lead.Id
                                  join property in this._context.Property on lead.Id equals property.LeadId
                                  join propertyState in this._context.State on property.StateId equals propertyState.Id
                                  join generalLandUseCode in this._context.GeneralLandUseCode on property.GeneralLandUseCodeId equals generalLandUseCode.Id
                                  from leadState in this._context.State.Where(ls => ls.Id == lead.MailingStateId).DefaultIfEmpty()
                                  where campaign.Id == id
                                  select new
                                  {
                                      campaignLead.LeadId,
                                      campaign.CampaignName,
                                      CampaignType = ct.Name,
                                      campaign.CreateDate,
                                      campaign.Description,
                                      campaign.TargetDate,
                                      campaign.Note,
                                      AssignedUser = user.FirstName + " " + user.LastName,
                                      lead.AccountName,
                                      lead.MailingAddress1,
                                      lead.MailingAddress2,
                                      lead.MailingAddress3,
                                      lead.MailingCity,
                                      MailingStateModel = leadState,
                                      lead.MailingZipCode,
                                      lead.DoNotContact,
                                      property.ParcelId,
                                      PropertyAddress = property.Address,
                                      PropertyCity = property.City,
                                      PropertyZipCode = property.ZipCode,
                                      PropertyState = propertyState.Name,
                                      PropertyCounty = property.County.Name,
                                      PropertyCADId = property.CADId,
                                      PropertyTAXId = property.TAXId,
                                      PropertyFolioId = property.FolioId,
                                      GeneralLandUseCode = generalLandUseCode.Name,
                                      AmountDue = property.TotalAmountDue,
                                      PropertyId = property.Id,
                                  }).ToListAsync(cancellationToken).ConfigureAwait(false);

            var contactList = await (from campaignLead in this._context.CampaignLead
                                     join contact in this._context.Contact on campaignLead.LeadId equals contact.LeadId
                                     join contactType in this._context.ContactType on contact.ContactTypeId equals contactType.Id
                                     join s in this._context.State on contact.MailingStateId equals s.Id into cs
                                     from contactState in cs.DefaultIfEmpty()
                                     where campaignLead.CampaignId == id
                                     select new
                                     {
                                         campaignLead.LeadId,
                                         Type = contactType.Name,
                                         contact.FirstName,
                                         contact.LastName,
                                         contact.MiddleName,
                                         contact.Title,
                                         contact.CellPhone,
                                         contact.OfficePhone,
                                         contact.Email,
                                         Address1 = contact.MailingAddress1,
                                         Address2 = contact.MailingAddress2,
                                         Address3 = contact.MailingAddress3,
                                         City = contact.MailingCity,
                                         State = contactState.Name,
                                         ZipCode = contact.MailingZipCode,
                                     }).ToListAsync(cancellationToken).ConfigureAwait(false);

            var valuations = await (from campaignLead in this._context.CampaignLead
                                    join property in this._context.Property on campaignLead.LeadId equals property.LeadId
                                    join propertyValuation in this._context.PropertyValuation on property.Id equals propertyValuation.PropertyId
                                    where campaignLead.CampaignId == id && propertyValuation.IsActive == true
                                    select new
                                    {
                                        property.Id,
                                        Year = propertyValuation.AppraisedYear,
                                        Value = propertyValuation.AppraisedValue,
                                    }).ToListAsync(cancellationToken).ConfigureAwait(false);

            var subTypeName = await this._context.Campaign.Where(x => x.Id == id).Select(x => x.CampaignSubType.Name).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

            // in-memory aggregation
            var aggregate = from r in rootList
                            select new
                            {
                                r.CampaignName,
                                r.CampaignType,
                                r.CreateDate,
                                r.Description,
                                r.TargetDate,
                                r.Note,
                                r.AssignedUser,
                                r.AccountName,
                                r.MailingAddress1,
                                r.MailingAddress2,
                                r.MailingAddress3,
                                r.MailingCity,
                                MailingState = r.MailingStateModel == null ? string.Empty : r.MailingStateModel.Name,
                                r.MailingZipCode,
                                r.DoNotContact,
                                r.ParcelId,
                                InternalPropertyId = r.PropertyId.ToString(),
                                r.PropertyAddress,
                                r.PropertyCity,
                                r.PropertyZipCode,
                                r.PropertyState,
                                r.PropertyCounty,
                                r.PropertyCADId,
                                r.PropertyTAXId,
                                r.PropertyFolioId,
                                r.GeneralLandUseCode,
                                r.AmountDue,
                                CampaignSubType = string.IsNullOrWhiteSpace(subTypeName) ? string.Empty : subTypeName,
                                AppraisedValue = valuations.Where(x => x.Id == r.PropertyId).OrderByDescending(x => x.Year).Select(x => x.Value ?? 0).FirstOrDefault(),
                                Contact = contactList.Where(x => x.LeadId == r.LeadId).Select(x => new
                                {
                                    x.Type,
                                    x.FirstName,
                                    x.LastName,
                                    x.MiddleName,
                                    x.Title,
                                    x.CellPhone,
                                    x.OfficePhone,
                                    x.Email,
                                    x.Address1,
                                    x.Address2,
                                    x.Address3,
                                    x.City,
                                    x.State,
                                    x.ZipCode,
                                }),
                            };

            return aggregate.Select(x => x.ToDataDump()).ToList();
        }
    }
}