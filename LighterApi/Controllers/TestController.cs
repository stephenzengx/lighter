using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;
using System.Security.Principal;

namespace LighterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public void Test1()
        {
            /*
            ClaimTypes                  //证件单元  
            ClaimsIdentity:IIdentity    //身份证                         
            public interface IIdentity  // 定义证件对象的基本功能。
            {
                //1、名字。2、类型。3、证件是否合法。              
                string Name { get; } //证件名称             
                string AuthenticationType { get; } // 用于标识证件的载体类型。               
                bool IsAuthenticated { get; } //是否是合法的证件。
            }
             
            System.Security.Principal.ClaimsPrincipal : IPrincipal  //证件当事人
            {
                //把拥有的证件都给当事人
                public ClaimsPrincipal(IEnumerable<ClaimsIdentity> identities){}    
                //当事人的主身份呢
                public virtual IIdentity Identity { get; }   
                public virtual IEnumerable<ClaimsIdentity> Identities { get; }    
                public virtual void AddIdentity(ClaimsIdentity identity);    
                //为什么没有RemoveIdentity ， 留给大家思考吧？
             }

             System.Security.Principal.IPrincipal
             {              
                IIdentity Identity { get; }     //身份                
                bool IsInRole(string role);     //在否属于某个角色     
             }
             */
        }
    }
}
