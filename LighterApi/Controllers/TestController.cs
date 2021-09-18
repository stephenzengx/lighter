using Microsoft.AspNetCore.Mvc;

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

            //Microsoft.AspNetCore.Http.HttpContext

            //Microsoft.AspNetCore.Server.HttpSys.AuthenticationManager  / System.Net.AuthenticationManager
            //AuthenticationManager  AuthenticationSchemes（验证方案名称）

            //Microsoft.AspNetCore.Authentication.IAuthenticationHandler
            //CookieAuthentication 中间件 HandleSignInAsync

            //var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "奥巴马") }, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme));
            //HttpContext.Authentication.SignInAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, user);

            // Microsoft.IdentityModel 是属于 WIF(Windows Identity Foundation) 的一部分
            // System.IdentityModel.Tokens.Jwt

            //HttpContext.User.Identity
            /*
              Microsoft.AspNetCore.Authentication.IAuthenticationService
              AuthenticateAsync,ChallengeAsync,ForbidAsync,SignInAsync,SignOutAsync
             */

        }
    }
}
