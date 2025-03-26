using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.RabbitMQ.Images
{
    public interface IFileMessagesProducerService
    {
        Task SendUploadImagesMessagesAsync(List<IFormFile> files, List<string> correspondingUniqueFileNames, string bucketName, string entityInfo);
        Task SendRemoveImagesMessageAsync(List<string> imagesUrls, string bucketName, string entityInfo);
    }
}
