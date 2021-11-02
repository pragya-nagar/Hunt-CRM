using System;
using AutoMapper;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions.Models;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Queries.Original.MapProfiles
{
    public class CampaignCommentMapProfile : Profile
    {
        public CampaignCommentMapProfile()
        {
            this.CreateMap<CampaignComment, CampaignCommentModel>()
                .ForMember(e => e.Author, t => t.MapFrom(src => new FastEntityModel<Guid> { Id = src.AuthorId, Name = $"{src.Author.FirstName} {src.Author.LastName}" }))
                ;
        }
    }
}
