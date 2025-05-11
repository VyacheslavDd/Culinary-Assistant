using Core.Base.Interfaces;
using Culinary_Assistant.Core.DTO.Feedback;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.ServicesResponses;
using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Feedbacks
{
	public interface IFeedbacksService : IService<Feedback, FeedbackInDTO, FeedbackUpdateDTO>
	{
		Task<EntitiesResponseWithCountAndPages<Feedback>> GetAllAsync(Guid receiptId, FeedbacksFilter feedbacksFilter, CancellationToken cancellationToken = default);
	}
}
