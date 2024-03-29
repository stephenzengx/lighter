﻿
using Lighter.Application.Contracts.Dto;
using Lighter.Domain.Question;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Application.Contracts
{
    public interface IAnswerService
    {
        Task<IEnumerable<Answer>> GetListAsync(string questionId, CancellationToken cancellationToken);
        Task UpdateAsync(string id, string content, string summary, CancellationToken cancellationToken);
        Task CommentAsync(string id, CommentInput request, CancellationToken cancellationToken);

        Task UpAsync(string id, CancellationToken cancellationToken);
        Task DownAsync(string id, CancellationToken cancellationToken);
    }
}
