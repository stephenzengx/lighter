using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LighterApi.Data;
using LighterApi.Share;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LighterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly IMongoCollection<Answer> _answerCollection;
        private readonly IMongoCollection<Vote> _voteCollection;

        public AnswerController(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("lighter");
            _answerCollection = database.GetCollection<Answer>("answer");
            _voteCollection = database.GetCollection<Vote>("vote");
        }

        /// <summary>
        /// 获取某个问题答案
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetListAsync([FromQuery] string questionId, CancellationToken cancellationToken)
        {
            //linq
            var result = await _answerCollection.AsQueryable().Where(m => m.QuestionId == questionId).ToListAsync(cancellationToken);

            return Ok(result);
        }

        //修改答案
        [HttpPatch]
        [Route("{id}")]
        public async Task<ActionResult> UpdateAsync(string id, [FromBody]Answer answer, CancellationToken cancellationToken)
        {
            var filter = Builders<Answer>.Filter.Eq(q => q.Id, id);

            var updateDefinition = Builders<Answer>.Update.Set(m => m.Content, answer.Content);
            await _answerCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);

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
        public async Task<ActionResult> CommentAsync(string id, [FromBody] Comment comment, CancellationToken cancellationToken)
        {
            comment.CreateAt = DateTime.Now;

            var filter = Builders<Answer>.Filter.Eq(q => q.Id, id);
            var updateDefinition = Builders<Answer>.Update.Push(m => m.Comments, comment);
            await _answerCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);

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
            var vote = new Vote
            {
                Id = Guid.NewGuid().ToString(),
                SourceId = id,
                SourceType = ConstVoteSourceType.Anwser,
                Direction = EnumVoteDirection.Up,
                UserId = "111",//通过token拿userid
            };

            var filter = Builders<Answer>.Filter.Eq(q => q.Id, id);

            var updateFilterList = new List<UpdateDefinition<Answer>>();
            updateFilterList.Add(Builders<Answer>.Update.Inc(m => m.VoteCount, 1));
            updateFilterList.Add(Builders<Answer>.Update.Push(m => m.VoteUpRecIds, vote.Id));

            var updateDefinition = Builders<Answer>.Update.Combine(updateFilterList);
            //事务 to do
            await _answerCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);
            await _voteCollection.InsertOneAsync(vote);

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
        public async Task<ActionResult<Project>> PostDownAsync(string id, CancellationToken cancellationToken)
        {
            var vote = new Vote
            {
                Id = Guid.NewGuid().ToString(),
                SourceId = id,
                SourceType = ConstVoteSourceType.Anwser,
                Direction = EnumVoteDirection.Down,
                UserId = "111",//通过token拿userid
            };

            var filter = Builders<Answer>.Filter.Eq(q => q.Id, id);

            var updateFilterList = new List<UpdateDefinition<Answer>>();
            updateFilterList.Add(Builders<Answer>.Update.Inc(m => m.VoteCount, -1));
            updateFilterList.Add(Builders<Answer>.Update.Push(m => m.VoteDownRecIds, vote.Id));

            var updateDefinition = Builders<Answer>.Update.Combine(updateFilterList);
            //事务 to do
            await _answerCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);
            await _voteCollection.InsertOneAsync(vote);

            return Ok();
        }
    }
}
