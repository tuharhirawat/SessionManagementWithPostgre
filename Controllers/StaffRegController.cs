using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZoomColorLab.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace InternalProj.Controllers
{
    public class StaffRegController : Controller
    {
        private readonly ApplicationDbContext _context;



        public StaffRegController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StaffReg/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new StaffRegViewModel
            {
                Departments = await _context.DeptMasters.Where(d => d.Active == "Y").ToListAsync(),
                Designations = await _context.DesignationMasters.Where(d => d.Active == "Y").ToListAsync(),
                Branches = await _context.Branches.Where(b => b.Active == "Y").ToListAsync(),
                PhoneTypes = await _context.PhoneTypes.Where(p => p.Active == "Y").ToListAsync(),
                CustomerCategories = await _context.CustomerCategories.Where(p => p.Active == "Y").ToListAsync()
            };

            return View(model);
        }

        // POST: StaffReg/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            StaffRegViewModel model,
            string FirstName, string LastName, string Address1, string Address2,
            int DeptId, int DesignationId, int BranchId,
            string Phone1, string Phone2, string Whatsapp, string Email,
            int PhoneTypeId, int CategoryId,
            DateTimeOffset? DOB, DateTimeOffset? DOJ,
            string Remarks,
            string UserName, string Password)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. StaffReg
                    //var newStaff = new StaffReg
                    //{
                    //    FirstName = FirstName,
                    //    LastName = LastName,
                    //    DOB = DOB,
                    //    DOJ = DOJ ?? DateTimeOffset.UtcNow,
                    //    CreatedDate = DateTimeOffset.UtcNow,
                    //    Active = "Y",
                    //    DeptId = DeptId,
                    //    DesignationId = DesignationId,
                    //    BranchId = BranchId,
                    //    CategoryId = CategoryId,
                    //    Remarks = Remarks
                    //};

                    var newStaff = new StaffReg
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        DOB = DOB?.ToUniversalTime(),                      // Convert DOB to UTC
                        DOJ = (DOJ ?? DateTimeOffset.UtcNow).ToUniversalTime(), // Ensure DOJ is UTC
                        CreatedDate = DateTimeOffset.UtcNow,               // Already UTC
                        Active = "Y",
                        DeptId = DeptId,
                        DesignationId = DesignationId,
                        BranchId = BranchId,
                        CategoryId = CategoryId,
                        Remarks = Remarks
                    };

                    await _context.StaffRegs.AddAsync(newStaff);
                    await _context.SaveChangesAsync();

                    // 2. StaffAddress
                    var staffAddress = new StaffAddress
                    {
                        StaffId = newStaff.StaffId,
                        Address1 = Address1,
                        Address2 = Address2,
                        Active = "Y"
                    };
                    await _context.StaffAddresses.AddAsync(staffAddress);

                    // 3. StaffContact
                    var staffContact = new StaffContact
                    {
                        StaffId = newStaff.StaffId,
                        Phone1 = Phone1,
                        Phone2 = Phone2,
                        Whatsapp = Whatsapp,
                        Email = Email,
                        PhoneTypeId = PhoneTypeId,
                        Active = "Y"
                    };
                    await _context.StaffContacts.AddAsync(staffContact);

                    // 4. StaffCredentials
                    var staffCredentials = new StaffCredentials
                    {
                        StaffId = newStaff.StaffId,
                        UserName = UserName,
                        Status = 1,
                        Active = "Y"
                    };

                    var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<StaffCredentials>();
                    staffCredentials.Password = passwordHasher.HashPassword(staffCredentials, Password);
                    await _context.StaffCredentials.AddAsync(staffCredentials);

                    // 5. Save All
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Staff registration successful!";
                    return RedirectToAction("Create");
                }
                catch (Exception ex)
                {
                    var fullMessage = GetFullExceptionMessage(ex);
                    ModelState.AddModelError("", "An error occurred while saving the data: " + fullMessage);
                }
            }

            // Reload dropdowns on validation failure
            model.Departments = await _context.DeptMasters.Where(d => d.Active == "Y").ToListAsync();
            model.Designations = await _context.DesignationMasters.Where(d => d.Active == "Y").ToListAsync();
            model.Branches = await _context.Branches.Where(b => b.Active == "Y").ToListAsync();
            model.PhoneTypes = await _context.PhoneTypes.Where(p => p.Active == "Y").ToListAsync();
            model.CustomerCategories = await _context.CustomerCategories.Where(p => p.Active == "Y").ToListAsync();

            return View(model);
        }

        // AJAX: Get Designations by Department
        [HttpGet]
        public async Task<IActionResult> GetDesignationsByDepartment(int deptId)
        {
            var designations = await _context.DesignationMasters
                .Where(d => d.DesignationId == deptId && d.Active == "Y")
                .Select(d => new { id = d.DesignationId, name = d.Name })
                .ToListAsync();

            return Json(designations);
        }

        // Helper to get full nested exception message
        private string GetFullExceptionMessage(Exception ex)
        {
            var messages = new List<string>();
            while (ex != null)
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }
            return string.Join(" --> ", messages);
        }
    }
}
