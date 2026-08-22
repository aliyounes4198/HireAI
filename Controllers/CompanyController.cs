using HireAI.API.Data;
using HireAI.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HireAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompanyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create a company
        [HttpPost]
        public async Task<IActionResult> CreateCompany(Company company)
        {
            _context.Companies.Add(company);

            await _context.SaveChangesAsync();

            return Ok(company);
        }

        // Get all companies
        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _context.Companies
                .ToListAsync();

            return Ok(companies);
        }

        // Get company by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompany(int id)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
            {
                return NotFound("Company not found.");
            }

            return Ok(company);
        }
    }
}