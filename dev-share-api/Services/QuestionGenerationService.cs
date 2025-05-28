using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

namespace dev_share_api.Services
{
    public class QuestionGenerationService
    {
        private readonly IChatClient _chatClient;

        public QuestionGenerationService(IChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async Task<ActionResult<string>>GenerateQuestionAsync(string summary)
        {
            var prompt = $$"""
                You are a curious learner who has just encountered a piece of summarised technical content from a blog and tutorial.
                Based only on this summary, think of a thoughtful precise question you might ask to better understand the topic, clarify a detail, or explore a related concept.

                Here is the content summary:

                {{summary}}
                """;

            var response = await _chatClient.GetResponseAsync(prompt);
            return response.Text;
        }
    }
}
