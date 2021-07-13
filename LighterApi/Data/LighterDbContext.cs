using Lighter.Domain.Project;
using Microsoft.EntityFrameworkCore;

namespace LighterApi.Data
{
    public class LighterDbContext: DbContext
    {
        /*
         //无法识别 add-migration 命令
         Import-Module C:\Users\jiewi\.nuget\packages\microsoft.entityframeworkcore.tools\3.1.0\tools\EntityFrameworkCore.psd1
         
         add-migration initCreateLighter
         update-database 
         */
        public LighterDbContext(DbContextOptions<LighterDbContext> options) : base(options)
        {
            
        }
        public DbSet<Member> Members { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectGroup> ProjectGroups { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectProject> SubjectProjects { get; set; }

        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Project>()
            //    .Property(p => p.Id).ValueGeneratedOnAdd();//新增Project时 ID自动生成

            //配置1对1 和1对多 理解不是很透彻 仔细看官方文档 to do //这里要预先加载 一定要配置fluent么？
            modelBuilder.Entity<ProjectGroup>()
                .HasOne(s => s.Project)
                .WithMany(p => p.Groups);

            //通过中间表 配置多对多
            modelBuilder.Entity<SubjectProject>()
                .HasOne(s => s.Project)
                .WithMany(p => p.SubjectProjects)
                .HasForeignKey(s => s.ProjectId);

            modelBuilder.Entity<SubjectProject>()
                .HasOne(s => s.Subject)
                .WithMany(p => p.SubjectProjects)
                .HasForeignKey(s=>s.SubjectId);


            base.OnModelCreating(modelBuilder);
        }
    }
}
