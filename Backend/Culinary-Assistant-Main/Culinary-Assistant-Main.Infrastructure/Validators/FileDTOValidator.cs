using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Validators
{
	public class FileDTOValidator : AbstractValidator<FileDTO>
	{
		public FileDTOValidator()
		{
			RuleFor(f => f.File).NotNull().NotEmpty().WithMessage("Файл не может быть null или пустым")
				.Must(f => f.Length <= MiscellaneousConstants.FileMaxSize).WithMessage("Превышен максимальный размер файла")
				.Must(f => MiscellaneousConstants.SupportedFileExtensions.Contains(Path.GetExtension(f.FileName)))
				.WithMessage("Недопустимый формат файла");
		}
	}
}
