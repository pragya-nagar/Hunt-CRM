using AutoMapper;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.MapProfiles
{
    public class CreateCampaignCommentMapProfile : Profile
    {
        public CreateCampaignCommentMapProfile()
        {
            this.CreateMap<CreateCampaignCommentModel, CampaignComment>()
                .IgnoreAuditMembers()
                .ForMember(c => c.Author, src => src.Ignore())
                .ForMember(c => c.CommentDate, src => src.Ignore())
                .ForMember(c => c.Campaign, src => src.Ignore())
                .ForMember(c => c.AuthorId, src => src.Ignore())
                ;
        }
    }
}
