using Lighter.Application.Contracts.Dto;
using Lighter.Domain.Question;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Application.Contracts
{
    public interface IQuestionService
    {
        Task<Question> GetAsync(string id, CancellationToken cancellationToken);
        Task<QuestionAnswerReponse> GetWithAnswerAsync(string id, CancellationToken cancellationToken);
        Task<List<Question>> GetListAsync(List<string> tags, string sort = "createdAt", int page=1, int pageSize = 10, CancellationToken cancellationToken=default );
        Task<Question> CreateAsync(Question question, CancellationToken cancellationToken);
        Task UpdateAsync(string id, QuestionUpdateRequest request, CancellationToken cancellationToken);
        Task<Answer> AnswerAsync(string id, AnswerRequest request, CancellationToken cancellationToken);
        Task<Comment> CommentAsync(string id, CommentRequest request, CancellationToken cancellationToken);
        Task UpAsync(string id, CancellationToken cancellationToken);
        Task DownAsync(string id, CancellationToken cancellationToken);
    }
}
