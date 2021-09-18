using Lighter.Domain.Project;
using Microsoft.EntityFrameworkCore;

namespace LighterApi.Data
{
    public class LighterDbContext: DbContext
    {
        /*
         //无法识别 add-migration 命令
         Import-Module C:\Users\jiewi\.nuget\packages\microsoft.entityframeworkcore.tools\3.1.0\tools\EntityFrameworkCore.psd1
         
        add-migration v1.1.5 -Context AccountContext
        update-database -Context AccountContext
        Remove-Migration -Context AccountContext
        script-migration -from v1.1.2 -to v1.1.3 -Context AccountContext //查看两个版本之间的差异sql
         */
        public LighterDbContext(DbContextOptions<LighterDbContext> options) : base(options)
        {
            
        }
        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Project>()
            //    .Property(p => p.Id).ValueGeneratedOnAdd();//新增Project时 ID自动生成

            //base.OnModelCreating(modelBuilder);
        }
    }
}
