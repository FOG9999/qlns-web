using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VIETTEL.Mvc
{
    public class FileContentResultWithContentDisposition : FileContentResult
    {
        private const string ContentDispositionHeaderName = "Content-Disposition";

        public FileContentResultWithContentDisposition(byte[] fileContents, string contentType, ContentDisposition contentDisposition)
            : base(fileContents, contentType)
        {
            // check for null or invalid ctor arguments
            ContentDisposition = contentDisposition;
        }

        public ContentDisposition ContentDisposition { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            // check for null or invalid method argument
            ContentDisposition.FileName = ContentDisposition.FileName ?? FileDownloadName;
            ContentDisposition.Inline = true;

            var response = context.HttpContext.Response;
            response.ContentType = ContentType;
            response.AddHeader(ContentDispositionHeaderName, ContentDisposition.ToString());
            WriteFile(response);
        }
    }
}
