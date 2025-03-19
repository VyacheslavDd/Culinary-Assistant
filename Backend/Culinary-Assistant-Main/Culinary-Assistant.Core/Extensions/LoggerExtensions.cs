using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
	public static class LoggerExtensions
	{
		public static void LogEntityCreation<T>(this ILogger logger, string entityName, T entity)
		{
			logger.Information("{@entityName}: создана новая сущность: {@entity}", entityName, entity);
		}

		public static void LogEntityUpdate(this ILogger logger, string entityName, Guid updatedEntityId)
		{
			logger.Information("{@entityName}: обновлена сущность с Id {@id}", entityName, updatedEntityId);
		}

		public static void LogEntityDeletion(this ILogger logger, string entityName, Guid deletedEntityId)
		{
			logger.Information("{@entityName}: удалена сущность с Id {@id}", entityName, deletedEntityId);
		}
	}
}
