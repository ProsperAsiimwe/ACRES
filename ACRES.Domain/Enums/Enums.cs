using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRES.Domain.Enums
{
   public enum Status: int
    {
        Pending = 1,
        Dispatched =2,
        Delivered = 3,
        Returned = 4
    }

    public enum SponsType : int
    {
        Full = 1,
        Partial = 2
         
    }

    public enum SchoolLevel : int
    {
        [Description("Nursery")]
        Nursery = 1,
        [Description("Primary 1")]
        Primary1 = 2,
        [Description("Primary 2")]
        Primary2 = 3,
        [Description("Primary 3")]
        Primary3 = 4,
        [Description("Primary 4")]
        Primary4 = 5,
        [Description("Primary 5")]
        Primary5 = 6,
        [Description("Primary 6")]
        Primary6 = 7,
        [Description("Primary 7")]
        Primary7 = 8,
        [Description("Senior 1")]
        Secondary1 = 9,
        [Description("Senior 2")]
        Secondary2 = 10,
        [Description("Senior 3")]
        Secondary3 = 11,
        [Description("Senior 4")]
        Secondary4 = 12,
        [Description("Senior 5")]
        Secondary5 = 13,
        [Description("Senior 6")]
        Secondary6 = 14,
        [Description("University")]
        University = 15,
        [Description("Tertiary Institution")]
        Tertiary = 16


    }

    public enum Gender
    {
        Male,
        Female,
        Unknown
    }

    public enum StudyMode
    {
        Day,
        Boarding,
        Both
    }
}
