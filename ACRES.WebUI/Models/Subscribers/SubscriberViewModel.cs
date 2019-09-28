using ACRES.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACRES.WebUI.Models.Subscribers
{
    public class SubscriberViewModel
    {
        public SubscriberViewModel() { }
        public SubscriberViewModel(Subscriber entity)
        {
            SubscriberId = entity.SubscriberId;
            Name = entity.Name;
            Email = entity.Email;
        }

        public int SubscriberId { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        public Subscriber ParseAsEntity(Subscriber entity)
        {
            if (entity == null)
            {
                entity = new Subscriber();
            }

            entity.Name = Name;
            entity.Email = Email;

            return entity;
        }
    }
}