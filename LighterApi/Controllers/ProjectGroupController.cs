using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Lighter.Domain.Project;
using LighterApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LighterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectGroupController : ControllerBase
    {
        private readonly LighterDbContext _lighterDbContext;

        public ProjectGroupController(LighterDbContext lighterDbContext)
        {
            _lighterDbContext = lighterDbContext;
        }


        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] ProjectGroup group, CancellationToken cancellationToken)
        {
            //if (!ModelState.IsValid)
            //    return ValidationProblem();

            _lighterDbContext.ProjectGroups.Add(group);
            await _lighterDbContext.SaveChangesAsync();

            return StatusCode((int)HttpStatusCode.Created,group);
        }
    }
}
