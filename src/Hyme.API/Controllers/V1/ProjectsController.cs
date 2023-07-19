using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hyme.API.Controllers.V1
{

    /// <summary>
    /// 
    /// </summary>
    [Route("api/projects")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {

        /// <summary>
        /// 
        /// </summary>
        public ProjectsController()
        {
            
        }


        /// <summary>
        /// Get Projects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            return await Task.FromResult(Ok());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetProjectById")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            return await Task.FromResult(Ok());
        }

    }
}
