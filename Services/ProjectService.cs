using AutoMapper;
using CompanyManagement.Data;
using CompanyManagement.DTOs;
using CompanyManagement.InputModels;
using CompanyManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagement.Services
{
    public class ProjectService : IProjectService
    {
        private readonly CompanyDbContext _context;
        private readonly IMapper _mapper;

        public ProjectService(CompanyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProjectDto>> GetFilteredProjectsAsync(string? title, CancellationToken cancellationToken)
        {
            var query = _context.Projects.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(p => p.Title.Contains(title));
            }
            var projects = await query.ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }


        public async Task<ProjectDto> GetProjectByIdAsync(int id)
        {
            var project = await _context.Projects
                .Include(p => p.EmployeeProject)
                .ThenInclude(ep => ep.Employee)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) throw new KeyNotFoundException("Project not found");

            return _mapper.Map<ProjectDto>(project);
        }

        public async Task<ProjectDto> AddProjectAsync(ProjectInputModel inputModel,CancellationToken cancellationToken)
        {
            var project = _mapper.Map<Project>(inputModel);
            _context.Projects.Add(project);
            await _context.SaveChangesAsync(cancellationToken);
            var projectDto = _mapper.Map<ProjectDto>(project);
            return projectDto;
        }

        public async Task<ProjectDto> UpdateProjectAsync(int id, ProjectInputModel inputModel,CancellationToken cancellationToken)
        {
            var project = await _context.Projects.FindAsync(id,cancellationToken);
            if (project == null) throw new KeyNotFoundException("Project not found");

            _mapper.Map(inputModel, project);
            await _context.SaveChangesAsync(cancellationToken);
            var projectDto = _mapper.Map<ProjectDto>(project);
            return projectDto;
        }

        public async Task<bool> DeleteProjectAsync(int id,CancellationToken cancellationToken)
        {
            var project = await _context.Projects.FindAsync(id,cancellationToken);
            if (project == null) throw new KeyNotFoundException("Project not found");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
