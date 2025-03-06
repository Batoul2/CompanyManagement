using AutoMapper;
using CompanyManagement.Data;
using CompanyManagement.DTOs;
using CompanyManagement.InputModels;
using CompanyManagement.Models;
using CompanyManagement.QueryParameters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagement.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly CompanyDbContext _context;
        private readonly IMapper _mapper;

        public CompanyService(CompanyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(CompanyQueryParameters parameters, CancellationToken cancellationToken)
        {
            var query = _context.Companies.AsQueryable();

            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                query = query.Where(c => c.Name.Contains(parameters.SearchTerm));
            }

            query = parameters.SortBy.ToLower() switch
            {
                "name" => parameters.SortDir.ToLower() == "desc" ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                "id" => parameters.SortDir.ToLower() == "desc" ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                _ => query.OrderBy(c => c.Name)
            };

            var companies = await query
                .Skip(parameters.GetSkipAmount())
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }


        public async Task<CompanyDto?> GetCompanyByIdAsync(int id, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .Include(c => c.CompanyEmployee)
                .ThenInclude(ce => ce.Employee)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
            {
                return null;
            }

            return _mapper.Map<CompanyDto>(company);
        }

        public async Task<CompanyDto> AddCompanyAsync(CompanyInputModel inputModel, CancellationToken cancellationToken)
        {
            var company = _mapper.Map<Company>(inputModel);
            _context.Companies.Add(company);
            await _context.SaveChangesAsync(cancellationToken);
            var companyDto = _mapper.Map<CompanyDto>(company);
            return companyDto;
        }

        public async Task<CompanyDto?> UpdateCompanyAsync(int id, CompanyInputModel inputModel, CancellationToken cancellationToken)
        {
            var company = await _context.Companies.FindAsync(id, cancellationToken);
            if (company == null)
            {
                return null;
            }

            _mapper.Map(inputModel, company);
            await _context.SaveChangesAsync(cancellationToken);
            var companyDto = _mapper.Map<CompanyDto>(company);
            return companyDto;
        }

        public async Task<bool> DeleteCompanyAsync(int id, CancellationToken cancellationToken)
        {
            var company = await _context.Companies.FindAsync(id, cancellationToken);
            if (company == null)
            {
                return false;
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> CompanyNameExistsAsync(string name, int excludeId, CancellationToken cancellationToken)
        {
            return await _context.Companies.AnyAsync(c => c.Name == name && c.Id != excludeId, cancellationToken);
        }

        

    }
}
