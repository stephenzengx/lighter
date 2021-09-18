using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lighter.Application.Contracts;
using Lighter.Application.Contracts.Dto;
using Lighter.Domain.Question;
using Lighter.Domain.Share;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Lighter.Application
{
    public class AnswerService : IAnswerService
    {
        private readonly IMongoCollection<Answer> _answerCollection;
        private readonly IMongoCollection<Vote> _voteCollection;
        public AnswerService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("lighter");

            _answerCollection = database.GetCollection<Answer>("answers");
            _voteCollection = database.GetCollection<Vote>("votes");
        }
        public async Task<IEnumerable<Answer>> GetListAsync(string questionId, CancellationToken cancellationToken)
        {
            //linq
            var result = await _answerCollection.AsQueryable().Where(m => m.QuestionId == questionId).ToListAsync(cancellationToken);

            return result;
        }
        public async Task UpdateAsync(string id, string content, string summary, CancellationToken cancellationToken)
        {
            var filter = Builders<Answer>.Filter.Eq(q => q.Id, id);

            var updateDefinition = Builders<Answer>.Update.Set(m => m.Content, content);
            await _answerCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);
        }
        public async Task CommentAsync(string id, CommentInput request, CancellationToken cancellationToken)
        {
            Comment comment = new Comment
            {
                CreateAt = DateTime.Now,
                Content = request.Content
            };

            var filter = Builders<Answer>.Filter.Eq(q => q.Id, id);
            var updateDefinition = Builders<Answer>.Update.Push(m => m.Comments, comment);
            await _answerCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);

        }
        public async Task DownAsync(string id, CancellationToken cancellationToken)
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
            updateFilterList.Add(Builders<Answer>.Update.Push(m => m.VoteUps, vote.Id));

            var updateDefinition = Builders<Answer>.Update.Combine(updateFilterList);
            //事务 to do
            await _answerCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);
            await _voteCollection.InsertOneAsync(vote);
        }      
        public async Task UpAsync(string id, CancellationToken cancellationToken)
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
            updateFilterList.Add(Builders<Answer>.Update.Push(m => m.VoteDowns, vote.Id));

            var updateDefinition = Builders<Answer>.Update.Combine(updateFilterList);
            //事务 to do
            await _answerCollection.UpdateOneAsync(filter, updateDefinition, null, cancellationToken);
            await _voteCollection.InsertOneAsync(vote);
        }
    }
}
