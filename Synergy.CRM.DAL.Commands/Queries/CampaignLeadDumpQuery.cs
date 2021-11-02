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
    public class CampaignLeadDumpQuery : CollectionQuery<Guid, IDictionary<string, object>>
    {
        private readonly ISynergyContext _context;

        public CampaignLeadDumpQuery(ISynergyContext context)
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
                                  where campaign.Id == id
                                  from leadState in this._context.State.Where(ls => ls.Id == lead.MailingStateId).DefaultIfEmpty()
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
                                      AmountDue = lead.TotalAmountDueProperties,
                                  })
                .Distinct()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var contactList = await (from campaignLead in this._context.CampaignLead
                                     join contact in this._context.Contact on campaignLead.LeadId equals contact.LeadId
                                     join t in this._context.ContactType on contact.ContactTypeId equals t.Id into ct
                                     from contactType in ct.DefaultIfEmpty()
                                     join s in this._context.State on contact.MailingStateId equals s.Id into cs
                                     from contactState in cs.DefaultIfEmpty()
                                     where campaignLead.CampaignId == id
                                     select new
                                     {
                                         campaignLead.LeadId,
                                         Type = contactType != null ? contactType.Name : string.Empty,
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
                                         State = contactState != null ? contactState.Name : string.Empty,
                                         ZipCode = contact.MailingZipCode,
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
                                r.AmountDue,
                                CampaignSubType = subTypeName,
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
