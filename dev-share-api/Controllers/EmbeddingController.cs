using Azure.Core;
using dev_share_api.Models;
using dev_share_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace dev_share_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmbeddingController : ControllerBase
    {
        private readonly EmbeddingService _embeddingService;
        public EmbeddingController(EmbeddingService embeddingService)
        {
            _embeddingService = embeddingService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateEmbedding([FromBody] EmbeddingRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Summary))
            {
                return BadRequest("string cannot be empty.");
            }

            var response = await _embeddingService.GetEmbeddingAsync(request);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }
    }
}
