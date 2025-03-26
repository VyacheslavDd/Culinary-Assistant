using AutoMapper;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Mappers
{
	public class CulinaryAppMapper : Profile
	{
		private readonly Func<Receipt, IEnumerable<Tag>> _tagsMap = (Receipt receipt) => receipt.Tags.Split(MiscellaneousConstants.ValuesSeparator, StringSplitOptions.RemoveEmptyEntries)
				.Select(t => (Tag)int.Parse(t));
		private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions();

		public CulinaryAppMapper()
		{
			MapUsers();
			MapReceipts();
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

		private void MapReceipts()
		{
			CreateMap<Receipt, FullReceiptOutDTO>()
				.ForMember(fr => fr.Tags, opt => opt.MapFrom(r => _tagsMap(r)))
				.ForMember(fr => fr.Ingredients, opt => opt.MapFrom(r => JsonSerializer.Deserialize<List<Ingredient>>(r.Ingredients, _jsonSerializerOptions)))
				.ForMember(fr => fr.CookingSteps, opt => opt.MapFrom(r => JsonSerializer.Deserialize<List<CookingStep>>(r.CookingSteps, _jsonSerializerOptions)))
				.ForMember(fr => fr.PicturesUrls, opt => opt.MapFrom(r => JsonSerializer.Deserialize<List<PictureUrl>>(r.PicturesUrls, _jsonSerializerOptions)));

			CreateMap<Receipt, ShortReceiptOutDTO>()
				.ForMember(sr => sr.Tags, opt => opt.MapFrom(r => _tagsMap(r)));
		}
	}
}
