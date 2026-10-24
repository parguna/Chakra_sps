using System;

namespace DI.Chakra.Model
{
    public class TaskReportData
    {
        public long TaskId { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public string Organisation { get; set; }
        public byte Level { get; set; }
        public string ActionUser { get; set; }
        public string AttestationName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<DateTime> SubmittedOn { get; set; }
        public int ReponseTimeInDays { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }

        public DateTime SubmitBefore
        {
            get
            {
                return this.CreatedDate.AddDays(this.ReponseTimeInDays);
            }
        }

        public string Status
        {
            get
            {
                if (this.IsActive && !this.IsCompleted)
                {
                    return "Pending";
                }
                else if (this.IsCompleted)
                {
                    return "Completed";
                }
                else
                {
                    return "Auto Completed";
                }
            }
        }

    }
}
