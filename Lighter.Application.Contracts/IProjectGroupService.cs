using Lighter.Domain.Project;
using System.Threading;
using System.Threading.Tasks;

namespace Lighter.Application.Contracts
{
    public interface IProjectGroupService
    {
        Task<ProjectGroup> CreateAsync(ProjectGroup group, CancellationToken cancellationToken);
    }
}
