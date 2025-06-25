
using ZoomColorLab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternalProj.Controllers
{
    public class CustomerRegController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerRegController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CustomerRegViewModel
            {
                StateMasterRegs = _context.StateMasters.Where(s => s.Active == "Y").ToList(),
                RegionMasterRegs = _context.RegionMasters.Where(r => r.Active == "Y").ToList(),
                PhoneTypes = _context.PhoneTypes.Where(p => p.Active == "Y").ToList(),
                CustomerCategories = _context.CustomerCategories.Where(p => p.Active == "Y").ToList(),

            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CustomerRegViewModel model, string FirstName, string LastName, string Address1, string Address2, int StateId, int RegionId, string Phone1,
            string Phone2, string Whatsapp, string Email, int PhoneTypeId, int CategoryId)
        {
            if (ModelState.IsValid)
            {
                var newCustomer = new CustomerReg
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    CreatedAt = DateTime.UtcNow,
                    Active = "Y",
                    CategoryId = CategoryId
                };

                _context.CustomerRegs.Add(newCustomer);
                _context.SaveChanges();

                _context.CustomerAddresses.Add(new CustomerAddress
                {
                    CustomerId = newCustomer.Id,
                    Address1 = Address1,
                    Address2 = Address2,
                    StateId = StateId,
                    RegionId = RegionId,
                    Active = "Y"
                });

                _context.CustomerContacts.Add(new CustomerContact
                {
                    CustomerId = newCustomer.Id,
                    Phone1 = Phone1,
                    Phone2 = Phone2,
                    Whatsapp = Whatsapp,
                    Email = Email,
                    PhoneTypeId = PhoneTypeId,
                    Active = "Y"
                });

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Customer registration successful!";
                return RedirectToAction("Create");
            }

            // Repopulate dropdowns on post failure
            model.StateMasterRegs = _context.StateMasters.Where(s => s.Active == "Y").ToList();
            model.RegionMasterRegs = _context.RegionMasters.Where(r => r.Active == "Y").ToList();
            model.PhoneTypes = _context.PhoneTypes.Where(p => p.Active == "Y").ToList();
            model.CustomerCategories = _context.CustomerCategories.Where(p => p.Active == "Y").ToList();


            return View(model);
        }

        [HttpGet]
        public IActionResult GetRegionsByState(int stateId)
        {
            var regions = _context.RegionMasters
                .Where(r => r.StateId == stateId && r.Active == "Y")
                .Select(r => new { id = r.Id, name = r.Name })
                .ToList();

            return Json(regions);
        }
    }
}
