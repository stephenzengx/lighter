using LighterApi.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using LighterApi.Models;
using System.Linq;
using LighterApi.Share;

namespace LighterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IMongoCollection<Question> _questionCollection;
        private readonly IMongoCollection<Answer> _answerCollection;
        private readonly IMongoCollection<Vote> _voteCollection;

        public QuestionController(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("lighter");
            _questionCollection = database.GetCollection<Question>("question");
            _answerCollection = database.GetCollection<Answer>("answer");
            _voteCollection = database.GetCollection<Vote>("vote");
        }

        /// <summary>
        /// 获取问题列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetListAsync([FromQuery]List<string> tags, [FromQuery] string sortFiled, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            //linq
            //var questionList = await _questionCollection.AsQueryable()
            //    .Where(m => m.Tags.Except(tags).Count()<=m.Tags.Count).OrderBy(m => m.ViewCount)//这个写法不知道能不能翻译
            //    .Skip((page - 1) * pageSize)
            //    .Take(pageSize)
            //    .ToListAsync();

            //mongo表达式
            var filter = Builders<Question>.Filter.Empty;
            if (tags != null && tags.Any())
            {
                filter = Builders<Question>.Filter.AnyIn(q => q.Tags, tags);
            }

            var sortDefinition = Builders<Question>.Sort.Descending(new StringFieldDefinition<Question>(sortFiled));
            var result = await _questionCollection
                        .Find(filter)
                        .Sort(sortDefinition)
                        .Skip((page - 1) * pageSize)
                        .Limit(pageSize)
                        .ToListAsync();

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
            //linq
            //var question = await _questionCollection.AsQueryable().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

            //mongo 查询表达式
            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);
            var question = await _questionCollection.Find(filter).FirstOrDefaultAsync();
            
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
            //级联多表
            var query = from question in _questionCollection.AsQueryable()
                         where question.Id == id
                         join a in _answerCollection.AsQueryable() on question.Id equals a.QuestionId into answers
                         select new { question, answers };

            var result = await query.FirstOrDefaultAsync(cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// 创建问题
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Question question, CancellationToken cancellationToken)
        {
            question.UserId = "111";//通过token拿userid
            question.Id = Guid.NewGuid().ToString();
            await _questionCollection.InsertOneAsync(question, new InsertOneOptions { BypassDocumentValidation = false }, cancellationToken);
            
            return StatusCode((int)HttpStatusCode.Created, question);
        }

        /// <summary>
        /// 修改问题
        /// </summary>
        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] QuestionUpdateRequest request, CancellationToken cancellationToken)
        {
            var filter = Builders<Question>.Filter.Eq(q=>q.Id,id);

            var updateFilterList = new List<UpdateDefinition<Question>>();
            if (!string.IsNullOrEmpty(request.Title))
                updateFilterList.Add(Builders<Question>.Update.Set(m => m.Title, request.Title));
            if (!string.IsNullOrEmpty(request.Content))
                updateFilterList.Add(Builders<Question>.Update.Set(m => m.Content, request.Content));
            if (request.Tags != null && request.Tags.Any())
                updateFilterList.Add(Builders<Question>.Update.Set(m => m.Tags, request.Tags));

            if (!string.IsNullOrEmpty(request.Summary))
                updateFilterList.Add(Builders<Question>.Update.Push(m=>m.Comments,new Comment {Content=request.Summary,CreateAt= DateTime.Now }));

            UpdateDefinition<Question> updateDefinition = Builders<Question>.Update.Combine(updateFilterList);
            await _questionCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);

            return Ok();
        }

        /// <summary> 
        /// 删除问题 未测试
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);
            await _questionCollection.DeleteOneAsync(filter);

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
        public async Task<IActionResult> AnswerAsync(string id,[FromBody] Answer answer, CancellationToken cancellationToken)
        {
            answer.UserId = "111";//通过token拿userid
            answer.CreateAt = DateTime.Now;
            answer.Id = Guid.NewGuid().ToString();
            answer.QuestionId = id;

            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);
            var updateDefinition = Builders<Question>.Update.Push(m=>m.AnswerRecIds, answer.Id);

            //事务 to do
            await _questionCollection.UpdateOneAsync(filter,updateDefinition,null, cancellationToken);
            await _answerCollection.InsertOneAsync(answer);

            return Ok();
        }

        /// <summary>
        /// 评论问题
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/comment")]
        public async Task<ActionResult<Project>> CommentAsync(string id,[FromBody]Comment comment, CancellationToken cancellationToken)
        {
            comment.CreateAt = DateTime.Now;
            
            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);
            var updateDefinition = Builders<Question>.Update.Push(m => m.Comments,  comment);
            await _questionCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// 向上投票问题
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/up")]
        public async Task<ActionResult<Project>> PostUpAsync(string id, CancellationToken cancellationToken)
        {
            // to do
            //未判断重复投票的问题，以及向上投票时，从向下投票里面清除数组元素，
            //vote先判断有没有投过票 有的话更新，没有直接插入

            var vote = new Vote { 
                Id = Guid.NewGuid().ToString(),
                SourceId = id,
                SourceType = ConstVoteSourceType.Question,
                Direction = EnumVoteDirection.Up,
                UserId = "111",//通过token拿userid
            };

            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);
            
            var updateFilterList = new List<UpdateDefinition<Question>>();
            updateFilterList.Add(Builders<Question>.Update.Inc(m => m.VoteCount, 1));
            updateFilterList.Add(Builders<Question>.Update.Push(m => m.VoteUpRecIds, vote.Id));

            var updateDefinition = Builders<Question>.Update.Combine(updateFilterList);
            //事务 to do
            await _questionCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);
            await _voteCollection.InsertOneAsync(vote);

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
        public async Task<ActionResult<Project>> PostDownAsync(string id, CancellationToken cancellationToken)
        {
            var vote = new Vote
            {
                Id = Guid.NewGuid().ToString(),
                SourceId = id,
                SourceType = ConstVoteSourceType.Question,
                Direction = EnumVoteDirection.Down,
                UserId = "111",//通过token拿userid
            };

            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);

            var updateFilterList = new List<UpdateDefinition<Question>>();
            updateFilterList.Add(Builders<Question>.Update.Inc(m => m.VoteCount, -1));
            updateFilterList.Add(Builders<Question>.Update.Push(m => m.VoteDownRecIds, vote.Id));

            var updateDefinition = Builders<Question>.Update.Combine(updateFilterList);
            //事务 to do
            await _questionCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);
            await _voteCollection.InsertOneAsync(vote);

            return Ok();
        }
    }
}