using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ACRES.Domain.Enums;

namespace ACRES.Domain.Models
{
    public class Referee : Person
    {
        public Referee()
        {
        }

        public DateTime? OptedOutDate { get; set; }

        public DateTime? MarketingDate { get; set; }

        public static string GetRefereeType(ReferenceType type)
        {
            return type.ToString();
        }

        public void Set(Person person)
        {
            Title = person.Title;
            FirstName = person.FirstName;
            LastName = person.LastName;
            JobTitle = person.JobTitle;
            Organisation = person.Organisation;
            PhoneNumber = person.PhoneNumber;
            Email = person.Email;
        }

        public Person AsPerson()
        {
            return new Person {
                Title = Title,
                FirstName = FirstName,
                LastName = LastName,
                JobTitle = JobTitle,
                Organisation = Organisation,
                PhoneNumber = PhoneNumber,
                Email = Email
            };
        }
    }
}