using AutoMapper;
using Culinary_Assistant_Main.Services.ReceiptCollections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio;

namespace Culinary_Assistant_Main.Controllers
{
	[Route("api/receipt-collections")]
	[ApiController]
	public class ReceiptCollectionsController(IReceiptCollectionsService receiptCollectionsService, IMinioClientFactory minioClientFactory, IMapper mapper) : ControllerBase
	{
		private readonly IReceiptCollectionsService _receiptCollectionsService = receiptCollectionsService;
		private readonly IMinioClientFactory _minioClientFactory = minioClientFactory;
		private readonly IMapper _mapper = mapper;


	}
}
