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
                }

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

                TempData["SuccessMessage"] = "Staff registration successful!";
                return RedirectToAction("Index");
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


        //all view
        public async Task<IActionResult> Index()
        {
            var staffList = await _context.StaffRegs
                .Include(s => s.Branch)
                .Include(s => s.StaffDepartments).ThenInclude(sd => sd.Department)
                .Include(s => s.StaffDesignations).ThenInclude(sd => sd.Designation)
                .Include(s => s.Addresses)
                .Include(s => s.Contacts)
                .Where(s => s.Active == "Y")
                .ToListAsync();

            return View(staffList);
        }

        //view by id
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var staff = await _context.StaffRegs
                .Include(s => s.Branch)
                .Include(s => s.StaffDepartments).ThenInclude(sd => sd.Department)
                .Include(s => s.StaffDesignations).ThenInclude(sd => sd.Designation)
                .Include(s => s.Addresses)
                .Include(s => s.Contacts)
                .FirstOrDefaultAsync(s => s.StaffId == id);

            if (staff == null)
                return NotFound();

            return View(staff);
        }



        //get details to display in edit 
        //[HttpGet]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //        return NotFound();

        //    var staff = await _context.StaffRegs
        //        .Include(s => s.StaffDepartments)
        //        .Include(s => s.StaffDesignations)
        //        .Include(s => s.Addresses)
        //        .Include(s => s.Contacts)
        //        .FirstOrDefaultAsync(s => s.StaffId == id);

        //    if (staff == null)
        //        return NotFound();

        //    var model = new StaffRegViewModel
        //    {
        //        Staff = staff,
        //        SelectedDeptIds = staff.StaffDepartments.Select(sd => sd.DeptId).ToList(),
        //        SelectedDesignationIds = staff.StaffDesignations.Select(sd => sd.DesignationId).ToList()
        //    };

        //    await ReloadDropdowns(model); // Load dropdowns like Departments, Designations, Branches etc.

        //    return View(model);
        //}


        ////post edit details to DB
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, StaffRegViewModel model)
        //{
        //    if (id != model.Staff.StaffId)
        //        return NotFound();

        //    try
        //    {
        //        var staff = await _context.StaffRegs
        //            .Include(s => s.StaffDepartments)
        //            .Include(s => s.StaffDesignations)
        //            .Include(s => s.Addresses)
        //            .Include(s => s.Contacts)
        //            .FirstOrDefaultAsync(s => s.StaffId == id);

        //        if (staff == null)
        //            return NotFound();

        //        // Basic Info
        //        staff.FirstName = model.Staff.FirstName;
        //        staff.LastName = model.Staff.LastName;
        //        staff.DOB = model.Staff.DOB?.ToUniversalTime();
        //        staff.DOJ = model.Staff.DOJ?.ToUniversalTime();
        //        staff.Remarks = model.Staff.Remarks;
        //        staff.BranchId = model.Staff.BranchId;
        //        staff.CategoryId = model.Staff.CategoryId;

        //        // Address
        //        var newAddr = model.Staff.Addresses?.FirstOrDefault();
        //        var existingAddr = staff.Addresses?.FirstOrDefault();
        //        if (existingAddr != null && newAddr != null)
        //        {
        //            existingAddr.Address1 = newAddr.Address1;
        //            existingAddr.Address2 = newAddr.Address2;
        //        }

        //        // Contact
        //        var newContact = model.Staff.Contacts?.FirstOrDefault();
        //        var existingContact = staff.Contacts?.FirstOrDefault();
        //        if (existingContact != null && newContact != null)
        //        {
        //            existingContact.Phone1 = newContact.Phone1;
        //            existingContact.Phone2 = newContact.Phone2;
        //            existingContact.Whatsapp = newContact.Whatsapp;
        //            existingContact.Email = newContact.Email;
        //            existingContact.PhoneTypeId = newContact.PhoneTypeId;
        //        }

        //        // Departments (many-to-many)
        //        _context.StaffDepartments.RemoveRange(staff.StaffDepartments);
        //        if (model.SelectedDeptIds.Any())
        //        {
        //            staff.StaffDepartments = model.SelectedDeptIds.Select(id => new StaffDepartment
        //            {
        //                StaffId = staff.StaffId,
        //                DeptId = id
        //            }).ToList();
        //        }

        //        // Designations (many-to-many)
        //        _context.StaffDesignations.RemoveRange(staff.StaffDesignations);
        //        if (model.SelectedDesignationIds.Any())
        //        {
        //            staff.StaffDesignations = model.SelectedDesignationIds.Select(id => new StaffDesignation
        //            {
        //                StaffId = staff.StaffId,
        //                DesignationId = id
        //            }).ToList();
        //        }

        //        await _context.SaveChangesAsync();
        //        TempData["SuccessMessage"] = "Staff updated successfully!";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Error: " + ex.Message;
        //        await ReloadDropdowns(model);
        //        return View(model);
        //    }
        //}


        //// GET: Confirm Deletion (optional, if you want a separate page – not used here)
        //[HttpGet]
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null) return NotFound();

        //    var staff = await _context.StaffRegs
        //        .Include(s => s.StaffDepartments)
        //        .Include(s => s.StaffDesignations)
        //        .Include(s => s.Addresses)
        //        .Include(s => s.Contacts)
        //        .FirstOrDefaultAsync(s => s.StaffId == id);

        //    if (staff == null) return NotFound();

        //    // Optional: return View(staff); // if you want a delete confirmation view
        //    return View(staff);
        //}

        //// POST: Delete confirmed
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    try
        //    {
        //        var staff = await _context.StaffRegs
        //            .Include(s => s.StaffDepartments)
        //            .Include(s => s.StaffDesignations)
        //            .Include(s => s.Addresses)
        //            .Include(s => s.Contacts)
        //            .Include(s => s.Credentials)
        //            .FirstOrDefaultAsync(s => s.StaffId == id);

        //        if (staff == null)
        //        {
        //            TempData["ErrorMessage"] = "Staff not found.";
        //            return RedirectToAction(nameof(Index));
        //        }

        //        // Remove related entries
        //        _context.StaffDepartments.RemoveRange(staff.StaffDepartments);
        //        _context.StaffDesignations.RemoveRange(staff.StaffDesignations);
        //        _context.StaffAddresses.RemoveRange(staff.Addresses);
        //        _context.StaffContacts.RemoveRange(staff.Contacts);
        //        _context.StaffCredentials.RemoveRange(staff.Credentials);

        //        // Remove the main Staff
        //        _context.StaffRegs.Remove(staff);

        //        await _context.SaveChangesAsync();

        //        TempData["SuccessMessage"] = "Staff deleted successfully!";
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Error deleting staff: " + ex.Message;
        //    }

        //    return RedirectToAction(nameof(Index));
        //}

    }
}
