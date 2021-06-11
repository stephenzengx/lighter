using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Lighter.Domain.Project;
using LighterApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LighterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly LighterDbContext _lighterDbContext;

        public ProjectController(LighterDbContext lighterDbContext)
        {
            _lighterDbContext = lighterDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetListAsync(CancellationToken cancellationToken)
        {
            return await _lighterDbContext.Projects.ToListAsync();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Project>> GetAsync(string id, CancellationToken cancellationToken)
        {
            return await _lighterDbContext.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        [HttpGet]
        [Route("{id}/profile")]
        public async Task<ActionResult<Project>> GetDetailAsync(string id, CancellationToken cancellationToken)
        {
            //加载相关group信息 三种加载方式 预先加载 显示加载 懒加载(延迟加载)
            //预先加载 
            return await _lighterDbContext.Projects.Include(proj => proj.Groups).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            //显示加载
            //var project2 = await _lighterDbContext.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            //await _lighterDbContext.Entry(project2).Collection(p => p.Groups).LoadAsync();
            //延迟加载 https://docs.microsoft.com/zh-cn/ef/core/querying/related-da ta/lazy
          

            //可通过链式调用 ThenInclude，进一步包含更深级别的关联数据
            /*
                var blogs = context.Blogs
                    .Include(blog => blog.Posts)
                        .ThenInclude(post => post.Author)
                            .ThenInclude(author => author.Photo)
                    .ToList();
             */
        }

        [HttpPost]
        public async Task<ActionResult<Project>> PostAsync([FromBody]Project project, CancellationToken cancellationToken)
        {
            //project.Id = System.Guid.NewGuid().ToString();
            _lighterDbContext.Projects.Add(project);
            await _lighterDbContext.SaveChangesAsync(cancellationToken);

            return StatusCode((int)HttpStatusCode.Created, project);
        }

        [HttpPut]
        public async Task<ActionResult<Project>> PutAsync([FromBody] Project project, CancellationToken cancellationToken)
        {
            /*entity state              Property State 拿请求参数动态改
                Added 添加                 IsModified
                Unchanged 未改变           CurrentValue
                Modified 已修改            OriginValue
                Deleted 已删除 
                Detached 未跟踪  unchanged和detached的区别
            */
            //问题：1-比如一个列表信息，前端难道需要每次检测哪些值修改了 然后选择性地传哪些字段？ 
            //所以这里修改时 通过frombody是不是还是得一个字段一个字段赋值
            //2- 拿请求参数 动态改n个字段的运用场景是什么？能举个例子么？目前自己碰到的修改 前端都是直接上传所有字段
            var originProject = await _lighterDbContext.Projects.FindAsync(project.Id, cancellationToken);
            if (project == null)
            {
                return null;
            }
            originProject.Supervisor = project.Supervisor;
            originProject.Title = project.Title;
            originProject.StartDate = project.StartDate;
            originProject.EndDate = project.EndDate;

            await _lighterDbContext.SaveChangesAsync();

            return originProject;
        }

        [HttpPatch]
        [Route("{id}/title")]
        public async Task<ActionResult<Project>> PatchTitleAsync(string id, [FromQuery]string title, CancellationToken cancellationToken)
        {
            //1- 查询更新
            //var originProject = await _lighterDbContext.Projects.FirstOrDefaultAsync(p=>p.Id == id, cancellationToken);
            //if (originProject == null)
            //{
            //    return NotFound();
            //}
            //originProject.Title = title;
            //await _lighterDbContext.SaveChangesAsync(); //entity state : Unchanged
            //return originProject;

            //2- 不查询更新
            var attachProject = new Project { Id = id }; 
            _lighterDbContext.Projects.Attach(attachProject);//entity state :Detached
            attachProject.Title = title; //entity state :Modified
            await _lighterDbContext.SaveChangesAsync(cancellationToken); //entity state : Unchanged
            return attachProject;
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<ActionResult<Project>> PatchAsync(string id, CancellationToken cancellationToken)
        {
            //3- 动态更新字段 利用反射
            var originProject = await _lighterDbContext.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (originProject == null)
            {
                return NotFound();
            }
            var properties = _lighterDbContext.Entry(originProject).Properties.ToList();
            foreach (var query in HttpContext.Request.Query)
            {
                var property = properties.FirstOrDefault(p => p.Metadata.Name == query.Key);
                if (property == null)
                    continue;

                var currentValue = Convert.ChangeType(query.Value.First(), property.Metadata.ClrType);
                _lighterDbContext.Entry(originProject).Property(query.Key).CurrentValue = currentValue;
                _lighterDbContext.Entry(originProject).Property(query.Key).IsModified = true;
            }

            await _lighterDbContext.SaveChangesAsync();

            return originProject;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<Project>> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            //查询删除
            //var project =  _lighterDbContext.Projects.FirstOrDefaultAsync(p=>p.Id == id, cancellationToken).Result;
            //if (project == null)
            //{
            //    return null;
            //}
            //_lighterDbContext.Projects.Remove(project);

            //不查询删除
            var project = new Project { Id = id };
            _lighterDbContext.Projects.Attach(project);
            //remove返回了一个EntityEntry<Project> 通过属性反射？
            var retMove = _lighterDbContext.Projects.Remove(project);//这如何能获得数据库里面的那个实体??难道还要查一次？

            await _lighterDbContext.SaveChangesAsync(cancellationToken);

            return project;
        }
    }
}
