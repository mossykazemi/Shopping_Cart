using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping_Cart.Jobs
{
    public class JobSchedule
    {
        public Type JobType { get; set; }
        public string CronExpression { get; set; }

        public JobSchedule(Type jobType, string cronExpression)
        {
            JobType = jobType;
            CronExpression = cronExpression;
        }

    }
}
