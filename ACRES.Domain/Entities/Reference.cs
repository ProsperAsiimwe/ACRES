using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ACRES.Domain.Enums;
using ACRES.Domain.Models;

namespace ACRES.Domain.Entities
{
    public class Reference : Referee
    {
        const int maxReminder = 4;

        public Reference()
        {
            Init();
        }

        private void Init()
        {
            this.AuthCode = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 12);
            this.Performance = new CandidatePerformance();
            this.Employment = new EmploymentReference();
            this.Education = new EducationReference();
            Update();
        }

        public void Update()
        {
            this.Updated = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReferenceId { get; set; }

        [ForeignKey("User")]
        [StringLength(128)]
        public string UserId { get; set; }

        public ReferenceType Type { get; set; }

        [StringLength(80)]
        public string Capacity { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        [UIHint("_PerformanceScale")]
        [Display(Name = "Honest, trustworthy, highest integrity?")]
        public PerformanceScale HonestIntegrity { get; set; }

        //[StringLength(512)]
        //[Display(Name = "Honest, trustworthy, highest integrity comment")]
        //public string HonestIntegrityComment { get; set; }

        public CandidatePerformance Performance { get; set; }

        public EmploymentReference Employment { get; set; }

        public EducationReference Education { get; set; }

        [StringLength(1024)]
        [Display(Name = "Any other information")]
        public string Comments { get; set; }

        [StringLength(60)]
        public string Sections { get; set; }

        [Display(Name = "Latest reminder sent")]
        public DateTime? Notified { get; set; }

        [Display(Name = "Next reminder")]
        public DateTime? RemindDate { get; set; }

        [Display(Name = "System reminders sent")]
        public int RemindCount { get; set; }

        public DateTime Updated { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Submitted { get; set; }

        [StringLength(128)]
        public string Doc { get; set; }

        [StringLength(12)]
        public string AuthCode { get; set; }

        // Virtuals
        public virtual ApplicationUser User { get; set; }

        [NotMapped]
        public List<int> CompletedSections
        {
            get
            {
                if (!String.IsNullOrEmpty(Sections)) {
                    string[] s_array = Sections.Split(',');
                    return s_array.Select(Int32.Parse).ToList();
                }
                else {
                    return new List<int>();
                }
            }
            set
            {
                Sections = String.Join(",", value.ToArray());
            }
        }

        [NotMapped]
        public Referee Referee
        {
            get
            {
                return new Referee {
                    Title = this.Title,
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    JobTitle = this.JobTitle,
                    Organisation = this.Organisation,
                    PhoneNumber = this.PhoneNumber,
                    Email = this.Email,
                    MarketingDate = this.MarketingDate,
                    OptedOutDate = this.OptedOutDate
                };
            }
            set
            {
                Title = value.Title;
                FirstName = value.FirstName;
                LastName = value.LastName;
                JobTitle = value.JobTitle;
                Organisation = value.Organisation;
                PhoneNumber = value.PhoneNumber;
                Email = value.Email;
                MarketingDate = value.MarketingDate;
                OptedOutDate = value.OptedOutDate;
            }
        }

        // Functions
        public string GetReferenceId()
        {
            return string.Format("REF{0}", ReferenceId);
        }

        public string GetStatus()
        {
            if (Submitted.HasValue) {
                return "Reference Supplied";
            }
            else if (!Submitted.HasValue && RemindCount >= maxReminder) {
                return "Referee Outstanding";
            }
            else if (OptedOutDate.HasValue) {
                return "Referee Opted Out";
            }
            else if (Started.HasValue) {
                return "Referee Consented";
            }
            else {
                return "Pending";
            }
        }

        public string GetDoc()
        {
            if (string.IsNullOrEmpty(Doc)) {
                return string.Empty;
            }

            string absolutePath = string.Format(@"{0}\{1}", User.DocFolder(), Doc);

            if (!System.IO.File.Exists(absolutePath)) {
                return string.Empty;
            }

            return absolutePath;
        }

        public bool RequiresAction()
        {
            string status = GetStatus();
            return (status == "Reference Supplied" || status == "Referee Outstanding" || status == "Referee Opted Out");
        }

        public string GetStatusCssClass()
        {
            string status = GetStatus();

            switch (status) {
                case "Reference Supplied":
                    return "success";
                case "Referee Opted Out":
                case "Referee Outstanding":
                    return "warning";
                case "Referee Consented":
                    return "info";
                default:
                    return "danger";
            }
        }

        public bool HasCompletedSection(ReferenceSection section)
        {
            int baseId = (int)section;

            if (this.CompletedSections.Contains(baseId)) {
                return true;
            }
            else {
                return false;
            }
        }

        // Routines
        public void Reset()
        {
            RemindCount = 0;
            RemindDate = DateTime.Today;
            HonestIntegrity = 0;
            Sections = null;
            Capacity = null;
            DateFrom = null;
            DateTo = null;
            Started = null;
            Submitted = null;
            OptedOutDate = null;
            Comments = null;
            Doc = null;
            Init();
        }

        public void AddCompletedSection(ReferenceSection section)
        {
            List<int> sections = this.CompletedSections;

            int baseId = (int)section;

            // Add the section if not in list
            if (!sections.Contains(baseId)) {
                sections.Add(baseId);
            }

            // Re-sort the list
            sections.Sort();

            // Set sections
            this.CompletedSections = sections;
        }
    }

    public class EmploymentReference
    {
        [Display(Name = "Employment terminated")]
        public EmploymentTerminated Terminated { get; set; }

        [Display(Name = "Would re-employ the applicant?")]
        public bool? WouldRemploy { get; set; }
    }

    public class EducationReference
    {
        [Display(Name = "Complete their course?")]
        public bool? CompleteCourse { get; set; }
    }

    public class CandidatePerformance
    {
        [Display(Name = "Co-operative and helpful")]
        public virtual PerformanceScale Cooperative { get; set; }

        [Display(Name = "Able to work under pressure")]
        public virtual PerformanceScale UnderPressure { get; set; }

        [Display(Name = "Flexible?")]
        public virtual PerformanceScale Flexible { get; set; }

        [Display(Name = "Punctual and reliable")]
        public virtual PerformanceScale Punctual { get; set; }

        [Display(Name = "Competent at setting and meeting priorities")]
        public virtual PerformanceScale Competent { get; set; }

        [Display(Name = "Able to produce work of a satisfactory standard")]
        public virtual PerformanceScale WorkSatisfactory { get; set; }
    }
}