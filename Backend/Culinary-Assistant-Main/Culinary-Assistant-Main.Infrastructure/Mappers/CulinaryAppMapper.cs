using AutoMapper;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.DTO.ReceiptCollection;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.Utils;
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
		private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions();

		public CulinaryAppMapper()
		{
			MapUsers();
			MapReceipts();
			MapReceiptCollections();
		}

		private void MapUsers()
		{
			CreateMap<User, FullUserOutDTO>()
				.ForMember(dto => dto.Login, o => o.MapFrom(u => u.Login.Value))
				.ForMember(dto => dto.Phone, opt => opt.MapFrom(u => u.Phone == null ? null : u.Phone.Value))
				.ForMember(dto => dto.Email, o => o.MapFrom(u => u.Email == null ? "" : u.Email.Value))
				.ForMember(dto => dto.PictureUrl, o => o.MapFrom(u => u.ProfilePictureUrl));

			CreateMap<User, AuthUserOutDTO>()
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
				.ForMember(fr => fr.Title, opt => opt.MapFrom(r => r.Title.Value))
				.ForMember(fr => fr.Description, opt => opt.MapFrom(r => r.Description.Value))
				.ForMember(fr => fr.Calories, opt => opt.MapFrom(r => r.Nutrients.Calories))
				.ForMember(fr => fr.Proteins, opt => opt.MapFrom(r => r.Nutrients.Proteins))
				.ForMember(fr => fr.Fats, opt => opt.MapFrom(r => r.Nutrients.Fats))
				.ForMember(fr => fr.Carbohydrates, opt => opt.MapFrom(r => r.Nutrients.Carbohydrates))
				.ForMember(fr => fr.Tags, opt => opt.MapFrom(r => Miscellaneous.GetTagsFromString(r.Tags)))
				.ForMember(fr => fr.Ingredients, opt => opt.MapFrom(r => JsonSerializer.Deserialize<List<Ingredient>>(r.Ingredients, _jsonSerializerOptions)))
				.ForMember(fr => fr.CookingSteps, opt => opt.MapFrom(r => JsonSerializer.Deserialize<List<CookingStep>>(r.CookingSteps, _jsonSerializerOptions)))
				.ForMember(fr => fr.PicturesUrls, opt => opt.MapFrom(r => JsonSerializer.Deserialize<List<FilePath>>(r.PicturesUrls, _jsonSerializerOptions)))
				.ForPath(fr => fr.User.Id, opt => opt.MapFrom(r => r.User == null ? Guid.Empty : r.User.Id))
				.ForPath(fr => fr.User.Login, opt => opt.MapFrom(r => r.User == null ? "DELETED" : r.User.Login.Value))
				.ForPath(fr => fr.User.PictureUrl, opt => opt.MapFrom(r => r.User == null ? "" : r.User.ProfilePictureUrl));

			CreateMap<Receipt, ShortReceiptOutDTO>()
				.ForMember(sr => sr.Title, opt => opt.MapFrom(r => r.Title.Value))
				.ForMember(sr => sr.Calories, opt => opt.MapFrom(r => r.Nutrients.Calories))
				.ForMember(sr => sr.Tags, opt => opt.MapFrom(r => Miscellaneous.GetTagsFromString(r.Tags)));
		}

		public void MapReceiptCollections()
		{
			CreateMap<ReceiptCollection, ReceiptCollectionShortOutDTO>()
				.ForMember(dto => dto.Title, opt => opt.MapFrom(rc => rc.Title.Value))
				.ForMember(dto => dto.Covers,
				opt => opt.MapFrom(rc => rc.ReceiptCovers == "" ? new List<FilePath>() : JsonSerializer.Deserialize<List<FilePath>>(rc.ReceiptCovers, _jsonSerializerOptions)));

			CreateMap<ReceiptCollection, ReceiptCollectionFullOutDTO>()
				.ForMember(dto => dto.Title, opt => opt.MapFrom(rc => rc.Title.Value))
				.ForMember(dto => dto.Covers,
				opt => opt.MapFrom(rc => rc.ReceiptCovers == "" ? new List<FilePath>() : JsonSerializer.Deserialize<List<FilePath>>(rc.ReceiptCovers, _jsonSerializerOptions)));
		}
	}
}
