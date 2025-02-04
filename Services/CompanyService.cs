using AutoMapper;
using CompanyManagement.Data;
using CompanyManagement.DTOs;
using CompanyManagement.InputModels;
using CompanyManagement.Models;
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

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(CancellationToken cancellationToken, string sortBy = "Name", string sortDirection = "asc")
        {
            // Start with a queryable of Companies
            var query = _context.Companies
                .Include(c => c.CompanyEmployee)
                .ThenInclude(ce => ce.Employee)
                .AsQueryable();

            // Apply sorting based on the sortBy and sortDirection parameters
            query = sortBy.ToLower() switch
            {
                "name" => sortDirection.ToLower() == "desc" ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                "id" => sortDirection.ToLower() == "desc" ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                _ => query.OrderBy(c => c.Name) // Default to sorting by Name in ascending order
            };

            // Execute the query and map the results to DTOs
            var companies = await query.ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }


        public async Task<CompanyDto> GetCompanyByIdAsync(int id, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .Include(c => c.CompanyEmployee)
                .ThenInclude(ce => ce.Employee)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null) throw new KeyNotFoundException("Company not found");

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

        public async Task<CompanyDto> UpdateCompanyAsync(int id, CompanyInputModel inputModel, CancellationToken cancellationToken)
        {
            var company = await _context.Companies.FindAsync(id, cancellationToken);
            if (company == null) throw new KeyNotFoundException("Company not found");

            _mapper.Map(inputModel, company);
            await _context.SaveChangesAsync(cancellationToken);
            var companyDto = _mapper.Map<CompanyDto>(company);
            return companyDto;
        }

        public async Task<bool> DeleteCompanyAsync(int id, CancellationToken cancellationToken)
        {
            var company = await _context.Companies.FindAsync(id, cancellationToken);
            if (company == null) throw new KeyNotFoundException("Company not found");

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
