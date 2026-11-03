using CharityPlatformAPI.DTOs;
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
        // СТВОРИТИ ПРОЄКТ
        [HttpPost]
        public async Task<ActionResult> CreateProject(ProjectCreateDTO dto)
        {
            var newProject = new Project
            {
                RequestId = dto.HelpRequestId, 
                EmployeeId = dto.EmployeeId,
                Title = dto.Title,
                Description = dto.Description,
                ApprovedBudget = dto.ApprovedBudget,
                Status = "активний", 
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };
            _context.Projects.Add(newProject);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Проєкт успішно створено!" });
        }

        // ЗНАЙТИ ВСІ ПРОЄКТИ
        [HttpGet]
        public async Task<ActionResult> GetAllProjects()
        {
            var list = await _context.Projects
                .Select(x => new ProjectResponseDTO
                {
                    ProjectId = x.ProjectId,
                    HelpRequestId = x.RequestId,
                    EmployeeId = x.EmployeeId,
                    Title = x.Title,
                    Description = x.Description,
                    ApprovedBudget = x.ApprovedBudget,
                    Status = x.Status,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate
                }).ToListAsync();

            return Ok(list);
        }
        // ЗНАЙТИ ПРОЄКТИ за ід
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProjectById(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if(project == null)
            {
                return NotFound(new { message = "Проєкт з таким ID не знайдено." });
            }

            var responseDto = new ProjectResponseDTO
            {
                ProjectId = project.ProjectId,
                HelpRequestId = project.RequestId,
                EmployeeId = project.EmployeeId,
                Title = project.Title,
                Description = project.Description,
                ApprovedBudget = project.ApprovedBudget,
                Status = project.Status,
                StartDate = project.StartDate,
                EndDate = project.EndDate

            };

            return Ok(responseDto);
        }
        //ЗНАЙТИ ПРОЄКТИ ЗА АЙДІ ПРАЦІВНИКА
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult> GetProjectsByEmployeeId(int employeeId)
        {
            var projects = await _context.Projects
            .Where(p => p.EmployeeId == employeeId) 
            .Select(project => new ProjectResponseDTO
            {
                ProjectId = project.ProjectId,
                HelpRequestId = project.RequestId,
                EmployeeId = project.EmployeeId,
                Title = project.Title,
                Description = project.Description,
                ApprovedBudget = project.ApprovedBudget,
                Status = project.Status,
                StartDate = project.StartDate,
                EndDate = project.EndDate
            }).ToListAsync(); 

            if (projects.Count == 0)
            {
                return NotFound(new { message = "Проєктів для цього працівника не знайдено." });
            }

            return Ok(projects);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> CompleteProject(int id, ProjectUpdateDTO dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound(new { message = "Проєкт з таким ID не знайдено." });
            }

            project.EndDate = dto.EndDate;
            project.Status = "завершено";

            await _context.SaveChangesAsync();
            return Ok(new { message = "Проєкт успішно закрито!" });
        }

    }
}
