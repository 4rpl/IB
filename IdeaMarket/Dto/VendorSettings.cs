using IdeaMarket.Dto.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace IdeaMarket.Dto
{
    public class VendorSettings
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirmation { get; set; }
        public VendorStatus Status { get; set; }
        public VendorVisibility Visibility { get; set; }
        public string PeopleWhoCanSee { get; set; }
        public List<IdeasCategory> Categories { get; set; }
        public string Description { get; set; }
    }
}