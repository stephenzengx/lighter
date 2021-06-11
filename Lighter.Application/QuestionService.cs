using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lighter.Application.Contracts;
using Lighter.Application.Contracts.Dto;
using Lighter.Domain.Question;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using MongoDB.Driver.Linq;
using System.Linq;
using Lighter.Domain.Share;

namespace Lighter.Application
{
    public class QuestionService : IQuestionService
    {
        private readonly IMongoCollection<Question> _questionCollection;
        private readonly IMongoCollection<Vote> _voteCollection;
        private readonly IMongoCollection<Answer> _answerCollection;

        public QuestionService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("lighter");

            _questionCollection = database.GetCollection<Question>("questions");
            _voteCollection = database.GetCollection<Vote>("votes");
            _answerCollection = database.GetCollection<Answer>("answers");
        }
        public async Task<List<Question>> GetListAsync(List<string> tags, string sortFiled = "createdAt", int page = 1, int pageSize = 10, CancellationToken cancellationToken=default)
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

            return result;
        }
        public async Task<Question> GetAsync(string id, CancellationToken cancellationToken)
        {
            //linq
            //var question = await _questionCollection.AsQueryable().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

            //mongo 查询表达式
            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);
            var question = await _questionCollection.Find(filter).FirstOrDefaultAsync();

            return question;
        }
        public async Task<QuestionAnswerReponse> GetWithAnswerAsync(string id, CancellationToken cancellationToken)
        {
            //级联多表
            var query = from question in _questionCollection.AsQueryable()
                        where question.Id == id
                        join a in _answerCollection.AsQueryable() on question.Id equals a.QuestionId into answers
                        select new { question, answers };

            var result = await query.FirstOrDefaultAsync(cancellationToken);
            
            return new QuestionAnswerReponse { Question = result.question,AnswerList= result.answers};
        }
        public async Task<Question> CreateAsync(Question question, CancellationToken cancellationToken)
        {
            question.Id = Guid.NewGuid().ToString();
            //question.UserId = ""; //fetch from token

            await _questionCollection.InsertOneAsync(question, new InsertOneOptions { BypassDocumentValidation = false }, cancellationToken);

            return question;
        }
        public async Task UpdateAsync(string id, QuestionUpdateRequest request, CancellationToken cancellationToken)
        {
            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);

            var updateFilterList = new List<UpdateDefinition<Question>>();
            if (!string.IsNullOrEmpty(request.Title))
                updateFilterList.Add(Builders<Question>.Update.Set(m => m.Title, request.Title));
            if (!string.IsNullOrEmpty(request.Content))
                updateFilterList.Add(Builders<Question>.Update.Set(m => m.Content, request.Content));
            if (request.Tags != null && request.Tags.Any())
                updateFilterList.Add(Builders<Question>.Update.Set(m => m.Tags, request.Tags));

            if (!string.IsNullOrEmpty(request.Summary))
                updateFilterList.Add(Builders<Question>.Update.Push(m => m.Comments, new Comment { Content = request.Summary, CreateAt = DateTime.Now }));

            UpdateDefinition<Question> updateDefinition = Builders<Question>.Update.Combine(updateFilterList);

            await _questionCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);
        }
        public async Task<Answer> AnswerAsync(string id, AnswerRequest request, CancellationToken cancellationToken)
        {
            Answer answer = new Answer();
            //answer.UserId = "";//通过token拿userid
            answer.CreateAt = DateTime.Now;
            answer.Id = Guid.NewGuid().ToString();
            answer.QuestionId = id;
            answer.Content = request.Content;

            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);
            var updateDefinition = Builders<Question>.Update.Push(m => m.Answers, answer.Id);

            //事务 to do
            await _questionCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);
            await _answerCollection.InsertOneAsync(answer);

            return answer;
        }
        public async Task<Comment> CommentAsync(string id, CommentRequest request, CancellationToken cancellationToken)
        {
            Comment comment = new Comment
            {
                Content = request.Content,
                CreateAt = DateTime.Now,
            };        

            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);
            var updateDefinition = Builders<Question>.Update.Push(m => m.Comments, comment);
            await _questionCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);

            return comment;
        }
        public async Task UpAsync(string id, CancellationToken cancellationToken)
        {
            // to do
            //未判断重复投票的问题，以及向上投票时，从向下投票里面清除数组元素，
            //vote先判断有没有投过票 有的话更新，没有直接插入

            var vote = new Vote
            {
                Id = Guid.NewGuid().ToString(),
                SourceId = id,
                SourceType = ConstVoteSourceType.Question,
                Direction = EnumVoteDirection.Up,
                //UserId = "",//通过token拿userid
            };

            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);

            var updateFilterList = new List<UpdateDefinition<Question>>();
            updateFilterList.Add(Builders<Question>.Update.Inc(m => m.VoteCount, 1));
            updateFilterList.Add(Builders<Question>.Update.Push(m => m.VoteUps, vote.Id));

            var updateDefinition = Builders<Question>.Update.Combine(updateFilterList);
            //事务 to do
            await _questionCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);
            await _voteCollection.InsertOneAsync(vote);
        }
        public async Task DownAsync(string id, CancellationToken cancellationToken)
        {
            var vote = new Vote
            {
                Id = Guid.NewGuid().ToString(),
                SourceId = id,
                SourceType = ConstVoteSourceType.Question,
                Direction = EnumVoteDirection.Down,
                //UserId = "",//通过token拿userid
            };

            var filter = Builders<Question>.Filter.Eq(q => q.Id, id);

            var updateFilterList = new List<UpdateDefinition<Question>>();
            updateFilterList.Add(Builders<Question>.Update.Inc(m => m.VoteCount, -1));
            updateFilterList.Add(Builders<Question>.Update.Push(m => m.VoteDowns, vote.Id));

            var updateDefinition = Builders<Question>.Update.Combine(updateFilterList);
            //事务 to do
            await _questionCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);
            await _voteCollection.InsertOneAsync(vote);
        }
    }
}
