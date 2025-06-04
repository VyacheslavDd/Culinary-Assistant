using Culinary_Assistant.Core.Base.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Culinary_Assistant_Main.Controllers
{
	[Route("api/ingredients")]
	[ApiController]
	public class IngredientsController : ControllerBase
	{

		/// <summary>
		/// Получить список всех ингредиентов
		/// </summary>
		/// <response code="200">Список ингредиентов</response>
		[HttpGet]
		[Route("")]
		public OkObjectResult GetAll()
		{
			var ingredientsList = IngredientsData.Data.Select(i => i.Key).ToList();
			return Ok(ingredientsList);
		}
	}
}
