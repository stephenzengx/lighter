using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Lighter.Application.Contracts;
using Lighter.Application.Contracts.Dto;
using Lighter.Domain.Question;

namespace LighterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerService _answerService;
        public AnswerController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        /// <summary>
        /// 获取某个问题答案 (答案列表)
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetListAsync([FromQuery] string questionId, CancellationToken cancellationToken)
        {
            //linq
            var result = await _answerService.GetListAsync(questionId, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// 修改答案
        /// </summary>
        /// <param name="id"></param>
        /// <param name="answer"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{id}")]
        public async Task<ActionResult> UpdateAsync(string id, [FromBody]Answer answer, CancellationToken cancellationToken)
        {
            await _answerService.UpdateAsync(id, answer.Content, "", cancellationToken);
            return Ok();
        }

        /// <summary>
        /// 评论答案
        /// </summary>
        /// <param name="id"></param>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/comment")]
        public async Task<ActionResult> CommentAsync(string id, [FromBody] CommentInput comment, CancellationToken cancellationToken)
        {
            await _answerService.CommentAsync(id, comment, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// 向上投票答案
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/voteup")]
        public async Task<ActionResult> VoteUpAsync(string id, CancellationToken cancellationToken)
        {
            await _answerService.UpAsync(id, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// 向下投票答案
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/down")]
        public async Task<ActionResult> PostDownAsync(string id, CancellationToken cancellationToken)
        {
            await _answerService.DownAsync(id, cancellationToken);

            return Ok();
        }
    }
}
