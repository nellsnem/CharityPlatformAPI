using CharityPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CharityPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly CharityContext _context;

        public ProjectController(CharityContext context)
        {
            _context = context;
        }


        // GET: api/Project
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            var projects = await _context.Projects.ToListAsync();
            return Ok(projects);
        }
    }
}