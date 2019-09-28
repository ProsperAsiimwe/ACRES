using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ACRES.Domain.Entities;
using ACRES.Domain.Models;

namespace ACRES.WebUI.Models.Users
{
    public class ProfileViewModel : CreatePersonModel
    {
        public ProfileViewModel()
        {
           
        }

        public ProfileViewModel(ApplicationUser user)
        {
            this.SetFromEntity(user);
        }

        public string EditUserId { get; set; }

        public string UserId { get; set; }

        [UIHint("_DatePicker")]
        [Display(Name = "Date of Birth")]
        public DateTime? DOB { get; set; }

        [UIHint("_Checkbox")]
        [Display(Name = "Activate user?")]
        public bool? Activate { get; set; }

        [Display(Name ="Branch")]
        public int? BranchId { get; set; }

       

        public ApplicationUser User { get; set; }

        public ApplicationUser ParseAsEntity(ApplicationUser user)
        {
            user.Title = TitleId;
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.Email = Email;
            user.PhoneNumber = Phone;
            user.UserName = Email;
           

            if (Activate.HasValue) {
                user.EmailConfirmed = Activate.Value;
            }

            return user;
        }

        public void SetFromEntity(ApplicationUser user)
        {
            this.User = user;
            this.UserId = user.Id;
            this.TitleId = user.Title;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Email = user.Email;
            this.Phone = user.PhoneNumber;          
            this.Activate = user.EmailConfirmed;
          
        }

        public void SetLists(string editUserId)
        {
            base.SetLists();
            this.EditUserId = editUserId;
        }
    }
}