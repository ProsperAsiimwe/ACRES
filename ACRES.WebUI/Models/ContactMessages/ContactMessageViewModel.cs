using ACRES.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACRES.WebUI.Models.ContactMessages
{
    public class ContactMessageViewModel
    {

        public ContactMessageViewModel() { }

        public ContactMessageViewModel(ContactMessage ContactMessage)
        {

            SetFromContactMsg(ContactMessage);
        }

        [Key]
        public int ContactMessageId { get; set; }

        [Display(Name = "Name")]
        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [StringLength(50)]
        public string Email { get; set; }

        [Display(Name = "Subject")]
        [StringLength(50)]
        public string Subject { get; set; }

        [Display(Name = "Phone")]
        [StringLength(50)]
        public string Phone { get; set; }

        [Display(Name = "Message")]
        [StringLength(2000)]
        public string Message { get; set; }


        public ContactMessage ParseAsEntity(ContactMessage ContactMessage)
        {
            if (ContactMessage == null)
            {
                ContactMessage = new ContactMessage();
            }

            ContactMessage.Name = Name;
            ContactMessage.Email = Email;
            ContactMessage.Phone = Phone;
            ContactMessage.Subject = Subject;
            ContactMessage.Message = Message;

            return ContactMessage;
        }

        protected void SetFromContactMsg(ContactMessage ContactMessage)
        {
            this.ContactMessageId = ContactMessage.ContactMessageId;
            this.Name = ContactMessage.Name;           
            this.Email = ContactMessage.Email;
            this.Phone = ContactMessage.Phone;
            this.Subject = ContactMessage.Subject;
            this.Message = ContactMessage.Message;
        }

    }
}