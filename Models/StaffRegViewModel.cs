using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZoomColorLab.Models;

namespace ZoomColorLab.Models
{
    public class StaffRegViewModel
    {
        //public StaffReg Staff { get; set; }
        public IEnumerable<DeptMaster> Departments { get; set; } = new List<DeptMaster>();
        public IEnumerable<DesignationMaster> Designations { get; set; } = new List<DesignationMaster>();
        public IEnumerable<Branch> Branches { get; set; } = new List<Branch>();
        public IEnumerable<PhoneType> PhoneTypes { get; set; } = new List<PhoneType>();
        public IEnumerable<CustomerCategory> CustomerCategories { get; set; } = new List<CustomerCategory>();

        public IEnumerable<StaffReg> StaffRegs { get; set; } = new List<StaffReg>();
        public IEnumerable<StaffAddress> StaffAddresses { get; set; } = new List<StaffAddress>();
        public IEnumerable<StaffContact> StaffContacts { get; set; } = new List<StaffContact>();
        public IEnumerable<StaffCredentials> StaffCredentials { get; set; } = new List<StaffCredentials>();

        [Display(Name = "Departments")]
        public List<int> SelectedDeptIds { get; set; } = new();

        [Display(Name = "Designations")]
        public List<int> SelectedDesignationIds { get; set; } = new();
    }
}
