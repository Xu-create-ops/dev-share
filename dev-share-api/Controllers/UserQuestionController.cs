using dev_share_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace dev_share_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserQuestionController : ControllerBase
    {
        private readonly QuestionGenerationService _questionService;

        public UserQuestionController(QuestionGenerationService questionService)
        {
            _questionService = questionService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateUserQuestion([FromBody] string summary)
        {
            if (string.IsNullOrWhiteSpace(summary))
                return BadRequest("Content summary must not be empty.");

            var question = await _questionService.GenerateQuestionAsync(summary);
            return Ok(question);
        }
    }
}
