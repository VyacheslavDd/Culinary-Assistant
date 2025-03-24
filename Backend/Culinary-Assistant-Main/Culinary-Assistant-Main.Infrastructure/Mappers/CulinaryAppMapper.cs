using AutoMapper;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Mappers
{
	public class CulinaryAppMapper : Profile
	{
		public CulinaryAppMapper()
		{
			MapUsers();
		}

		private void MapUsers()
		{
			CreateMap<User, FullUserOutDTO>()
				.ForMember(dto => dto.Login, o => o.MapFrom(u => u.Login.Value))
				.ForMember(dto => dto.Phone, opt => opt.MapFrom(u => u.Phone == null ? null : u.Phone.Value))
				.ForMember(dto => dto.Email, o => o.MapFrom(u => u.Email == null ? "" : u.Email.Value))
				.ForMember(dto => dto.PictureUrl, o => o.MapFrom(u => u.ProfilePictureUrl));

			CreateMap<User, ShortUserOutDTO>()
				.ForMember(dto => dto.Login, o => o.MapFrom(u => u.Login.Value))
				.ForMember(dto => dto.PictureUrl, o => o.MapFrom(u => u.ProfilePictureUrl));
		}

	}
}
