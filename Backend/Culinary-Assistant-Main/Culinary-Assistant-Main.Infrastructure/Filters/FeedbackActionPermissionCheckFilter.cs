using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Infrastructure.Filters.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Filters
{
	public class FeedbackActionPermissionCheckFilter(IUsersRepository usersRepository, IFeedbacksRepository feedbacksRepository) : ActionPermissionCheckFilter<Feedback>(usersRepository)
	{
		private readonly IFeedbacksRepository _feedbacksRepository = feedbacksRepository;

		protected override async Task<Feedback?> GetEntityAsync(Guid entityId)
		{
			return await _feedbacksRepository.GetBySelectorAsync(f => f.Id == entityId);
		}
	}
}
