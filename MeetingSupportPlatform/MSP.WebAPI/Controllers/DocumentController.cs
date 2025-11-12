using Microsoft.AspNetCore.Mvc;
using MSP.Application.Models.Requests.Document;
using MSP.Application.Services.Interfaces.Document;
using MSP.Shared.Common;

namespace MSP.WebAPI.Controllers
{
    [Route("api/v1/documents")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentRequest request)
        {
            var response = await _documentService.CreateDocumentAsync(request);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDocument([FromBody] UpdateDocumentRequest request)
        {
            var response = await _documentService.UpdateDocumentAsync(request);
            return Ok(response);
        }

        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteDocument([FromRoute] Guid documentId)
        {
            var response = await _documentService.DeleteDocumentAsync(documentId);
            return Ok(response);
        }

        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetDocumentById([FromRoute] Guid documentId)
        {
            var response = await _documentService.GetDocumentByIdAsync(documentId);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDocuments([FromQuery] PagingRequest request)
        {
            var response = await _documentService.GetAllDocumentsAsync(request);
            return Ok(response);
        }

        [HttpGet("by-project/{projectId}")]
        public async Task<IActionResult> GetDocumentsByProjectId([FromQuery] PagingRequest request, [FromRoute] Guid projectId)
        {
            var response = await _documentService.GetDocumentsByProjectIdAsync(request, projectId);
            return Ok(response);
        }

        [HttpGet("by-owner/{ownerId}")]
        public async Task<IActionResult> GetDocumentsByOwnerId([FromQuery] PagingRequest request, [FromRoute] Guid ownerId)
        {
            var response = await _documentService.GetDocumentsByOwnerIdAsync(request, ownerId);
            return Ok(response);
        }
    }
}
