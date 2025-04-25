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
	public class FilesDTOValidator: AbstractValidator<FilesDTO>
	{
		public FilesDTOValidator()
		{
			RuleFor(f => f.Files).NotNull().WithMessage("Отправляемые файлы не должны быть null").
				Must(files => files.Count <= MiscellaneousConstants.MaxFilesCount).WithMessage("Количество файлов не должно превышать 30").
				ForEach(f =>
			{
				f.Must(f => f.Length <= MiscellaneousConstants.FileMaxSize).WithMessage("Размер файла не должен превышать 16 МБ");
				f.Must(f => MiscellaneousConstants.SupportedFileExtensions.Contains(Path.GetExtension(f.FileName)))
				.WithMessage("Файл должен быть графического формата");
			});
		}
	}
}
