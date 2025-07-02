using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZoomColorLab.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ZoomColorLab.Controllers
{
    public class StaffRegController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffRegController(ApplicationDbContext context)
        {
            _context = context;
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            StaffRegViewModel model,
            string FirstName, string LastName, string Address1, string Address2,
            int BranchId,
            string Phone1, string Phone2, string Whatsapp, string Email,
            int PhoneTypeId, int CategoryId,
            DateTimeOffset? DOB, DateTimeOffset? DOJ,
            string Remarks,
            string UserName, string Password)
        {
            if (!ModelState.IsValid)
            {
                await ReloadDropdowns(model);
                return View(model);
            }

            try
            {
                var newStaff = new StaffReg
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    DOB = DOB?.ToUniversalTime(),
                    DOJ = (DOJ ?? DateTimeOffset.UtcNow).ToUniversalTime(),
                    CreatedDate = DateTimeOffset.UtcNow,
                    Active = "Y",
                    BranchId = BranchId,
                    CategoryId = CategoryId,
                    Remarks = Remarks
                };

                await _context.StaffRegs.AddAsync(newStaff);
                int rows = await _context.SaveChangesAsync();

                var staffAddress = new StaffAddress
                {
                    StaffId = newStaff.StaffId,
                    Address1 = Address1,
                    Address2 = Address2,
                    Active = "Y"
                };
                await _context.StaffAddresses.AddAsync(staffAddress);
                rows = await _context.SaveChangesAsync();

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
                rows = await _context.SaveChangesAsync();

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
                rows = await _context.SaveChangesAsync();
                Console.WriteLine($"Saved StaffCredentials, rows affected: {rows}");

                // Save many-to-many: Departments
                if (model.SelectedDeptIds != null && model.SelectedDeptIds.Count > 0)
                {
                    foreach (var deptId in model.SelectedDeptIds)
                    {
                        var staffDept = new StaffDepartment
                        {
                            StaffId = newStaff.StaffId,
                            DeptId = deptId
                        };
                        await _context.StaffDepartments.AddAsync(staffDept);
                    }
                    rows = await _context.SaveChangesAsync();
                    Console.WriteLine($"Saved StaffDepartments, rows affected: {rows}");
                }
                else
                {
                    Console.WriteLine("No Departments selected.");
                }

                // Save many-to-many: Designations
                if (model.SelectedDesignationIds != null && model.SelectedDesignationIds.Count > 0)
                {
                    foreach (var desigId in model.SelectedDesignationIds)
                    {
                        var staffDesig = new StaffDesignation
                        {
                            StaffId = newStaff.StaffId,
                            DesignationId = desigId
                        };
                        await _context.StaffDesignations.AddAsync(staffDesig);
                    }
                    rows = await _context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("No Designations selected.");
                }

                TempData["SuccessMessage"] = "Staff registration successful!";
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                var fullMessage = GetFullExceptionMessage(ex);
                ModelState.AddModelError("", "An error occurred while saving the data: " + fullMessage);
            }

            await ReloadDropdowns(model);
            return View(model);
        }

        private async Task ReloadDropdowns(StaffRegViewModel model)
        {
            model.Departments = await _context.DeptMasters.Where(d => d.Active == "Y").ToListAsync();
            model.Designations = await _context.DesignationMasters.Where(d => d.Active == "Y").ToListAsync();
            model.Branches = await _context.Branches.Where(b => b.Active == "Y").ToListAsync();
            model.PhoneTypes = await _context.PhoneTypes.Where(p => p.Active == "Y").ToListAsync();
            model.CustomerCategories = await _context.CustomerCategories.Where(p => p.Active == "Y").ToListAsync();
        }

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
