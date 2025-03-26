using AutoMapper;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Culinary_Assistant_Main.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestsController(CulinaryAppContext cp, IMapper mapper) : ControllerBase
	{
		private readonly IMapper _mapper = mapper;

		[HttpGet]
		[Route("")]
		public async Task<IActionResult> DoTest()
		{
			return Ok();
		}
	}
}
