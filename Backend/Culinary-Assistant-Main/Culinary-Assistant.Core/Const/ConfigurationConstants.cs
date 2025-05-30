using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Constants
{
	public static class ConfigurationConstants
	{
		public const string PostgreSQL = "PostgreSQL";
		public const string Minio = "Minio";
		public const string RabbitMQ = "RabbitMQ";
		public const string MinioLocalHost = "MinioLocalHost";
		public const string FrontendHost = "FrontendHost";
		public const string FrontendVMHost = "FrontendVMHost";
		public const string ElasticSearchOptions = "ElasticSearchOptions";
		public const string FrontendPolicy = "FrontendPolicy";
		public const string JWTSecretKey = "JWTSecretKey";
		public const string Redis = "Redis";
		public const string NotificationsHttpClientName = "NotificationsHttpClientName";
		public const string NotificationsHttpClientBaseAddress = "NotificationsHttpClientBaseAddress";
		public const string EmailOptions = "EmailOptions";
		public const string SMTPPassword = "SMTP_PASSWORD";
	}
}
