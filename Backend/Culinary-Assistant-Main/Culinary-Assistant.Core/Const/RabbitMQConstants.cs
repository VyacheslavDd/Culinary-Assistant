using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Const
{
	public static class RabbitMQConstants
	{
		public const string ImagesExchangeName = "ImagesExchange";
		public const string UploadImagesQueue = "UploadsQueue";
		public const string RemoveImagesQueue = "RemoveUploadsQueue";
		public const string UploadsRoutingKey = "#.upload.#";
		public const string RemoveUploadsRoutingKey = "#.remove.#";
	}
}
