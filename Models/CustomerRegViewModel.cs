
using ZoomColorLab.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ZoomColorLab.Models
{

    public class CustomerRegViewModel
    {
        public IEnumerable<CustomerReg> CustomerRegs { get; set; } = new List<CustomerReg>();
        public IEnumerable<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();
        public IEnumerable<StateMaster> StateMasterRegs { get; set; } = new List<StateMaster>();
        public IEnumerable<RegionMaster> RegionMasterRegs { get; set; } = new List<RegionMaster>();
        public IEnumerable<CustomerContact> CustomerContacts { get; set; } = new List<CustomerContact>();
        public IEnumerable<PhoneType> PhoneTypes { get; set; } = new List<PhoneType>();
        public IEnumerable<CustomerCategory> CustomerCategories { get; set; } = new List<CustomerCategory>();


    }

}

