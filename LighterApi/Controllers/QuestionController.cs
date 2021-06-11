using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Lighter.Application.Contracts;
using Lighter.Application.Contracts.Dto;
using Lighter.Domain.Question;

namespace LighterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _quesitonService;
        public QuestionController(IQuestionService quesitonService)
        {
            _quesitonService = quesitonService;
        }

        /// <summary>
        /// 获取问题列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetListAsync([FromQuery] List<string> tags, [FromQuery] string sortFiled, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var result = await _quesitonService.GetListAsync(tags, sortFiled, page, pageSize, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// 查看单个问题 (不带答案)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAsync(string id, CancellationToken cancellationToken)
        {
            var question = await _quesitonService.GetAsync(id, cancellationToken);

            return Ok(question);
        }

        /// <summary>
        /// 查看单个问题带答案
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/answer")]
        public async Task<IActionResult> GetWithAnswerAsync(string id, CancellationToken cancellationToken)
        {
            var result = await _quesitonService.GetWithAnswerAsync(id, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// 创建问题
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Question request, CancellationToken cancellationToken)
        {
            var question = await _quesitonService.CreateAsync(request, cancellationToken);

            return StatusCode((int)HttpStatusCode.Created, question);
        }

        /// <summary>
        /// 修改问题
        /// </summary>
        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] QuestionUpdateRequest request, CancellationToken cancellationToken)
        {
            await _quesitonService.UpdateAsync(id, request, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// 回答问题(带答案)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/answer")]
        public async Task<IActionResult> AnswerAsync(string id, [FromBody] AnswerRequest request, CancellationToken cancellationToken)
        {
            var question = await _quesitonService.AnswerAsync(id, request, cancellationToken);

            return Ok(question);
        }

        /// <summary>
        /// 评论问题
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/comment")]
        public async Task<ActionResult> CommentAsync(string id, [FromBody] CommentRequest request, CancellationToken cancellationToken)
        {
            var comment = await  _quesitonService.CommentAsync(id, request, cancellationToken);

            return Ok(comment);
        }

        /// <summary>
        /// 向上投票问题
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/up")]
        public async Task<ActionResult> PostUpAsync(string id, CancellationToken cancellationToken)
        {
            await _quesitonService.UpAsync(id, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// 向下投票问题
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/down")]
        public async Task<ActionResult> PostDownAsync(string id, CancellationToken cancellationToken)
        {
            await _quesitonService.DownAsync(id, cancellationToken);

            return Ok();
        }
    }
}