using AutoMapper;
using CompanyManagement.Data.Data;
using CompanyManagement.API.DTOs;
using CompanyManagement.API.InputModels;
using CompanyManagement.Data.Models;
using CompanyManagement.API.QueryParameters;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagement.API.Services
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

        public async Task<IEnumerable<ProjectDto>> GetFilteredProjectsAsync(ProjectQueryParameters parameters, CancellationToken cancellationToken)
        {
            var query = _context.Projects.AsQueryable();

            //Searching
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                query = query.Where(p => p.Title.Contains(parameters.SearchTerm));
            }

            //Sorting
            query = parameters.SortBy.ToLower() switch
            {
                "title" => parameters.SortDir.ToLower() == "desc" ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
                "id" => parameters.SortDir.ToLower() == "desc" ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
                _ => query.OrderBy(p => p.Title) 
            };

            //Pagination
            var projects = await query
                .Skip(parameters.GetSkipAmount())
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }


        public async Task<ProjectDto?> GetProjectByIdAsync(int id)
        {
            var project = await _context.Projects
                .Include(p => p.EmployeeProject)
                .ThenInclude(ep => ep.Employee)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return null;
            }

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

        public async Task<ProjectDto?> UpdateProjectAsync(int id, ProjectInputModel inputModel,CancellationToken cancellationToken)
        {
            var project = await _context.Projects.FindAsync(id,cancellationToken);
            if (project == null)
            {
                return null;
            }

            _mapper.Map(inputModel, project);
            await _context.SaveChangesAsync(cancellationToken);
            var projectDto = _mapper.Map<ProjectDto>(project);
            return projectDto;
        }

        public async Task<bool> DeleteProjectAsync(int id,CancellationToken cancellationToken)
        {
            var project = await _context.Projects.FindAsync(id,cancellationToken);
            if (project == null) 
            {
                return false;
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ProjectTitleExistsAsync(string title, int excludeId, CancellationToken cancellationToken)
        {
            return await _context.Projects.AnyAsync(p => p.Title == title && p.Id != excludeId, cancellationToken);
        }
    }
}
