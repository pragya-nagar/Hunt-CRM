using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.CRM.Models.Commands;

namespace Synergy.CRM.Services.Mappings
{
    public class CommentCommandMappingProfile : Profile
    {
        public CommentCommandMappingProfile()
        {
            this.CreateMap<LeadCommentCreateCommand, CreateLeadCommentModel>();

            this.CreateMap<CampaignCommentCreateCommand, CreateCampaignCommentModel>();
        }
    }
}
