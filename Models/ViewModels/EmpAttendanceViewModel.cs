

namespace EMS.Models.ViewModels
{
    public class EmployeePresentyRecord
    {
        public EmployeePresentyRecord(Employee emp1, DailyPresenty presenty)
        {
            this.emp1 = emp1;
            this.presenty = presenty;
            //this.leave = leave;
        }

        public Employee emp1 { get; set; }
         public DailyPresenty presenty { get; set; }
        //public LeaveRequest leave { get; set; }


    }

    public class RequestRecord
    {
        public RequestRecord(Employee emp2, LeaveRequest l)
        {
            this.emp2 = emp2;
            //this.presenty = presenty;
            this.leave = l;
        }

        public Employee emp2 { get; set; }
        //public DailyPresenty presenty { get; set; }
        public LeaveRequest leave { get; set; }


    }
    public class TaskReq
    {
        public TaskReq(ProjTask task)
        {
           
            //this.presenty = presenty;
            this.t = task;
        }

       
        public ProjTask t { get; set; }

    }
    public class Projects
    {
        public Projects(Project pro)
        {
            this.pro = pro;
           
        }

        public Project pro { get; set; }
      


    }

    public class EmpAttendanceViewModel
    {
        public List<Employee> employees { get; set; } = new List<Employee>();

        public List<DailyPresenty> dailyPresenties { get; set; } = new List<DailyPresenty>();
        public List<Project> projects { get; set; } = new List<Project>();
        public List<LeaveRequest> leaveRequests { get; set; } = new List<LeaveRequest>();
        public List<ProjTask> TaskReqests { get; set; } = new List<ProjTask>();

        public Employee employee { get; set; } = new Employee();

        public DailyPresenty dailyPresenty { get; set; } = new DailyPresenty();  
    
        public LeaveRequest requests { get; set; } = new LeaveRequest();  
        


        public int dailyPresentyCount { get; set; }
        // Dashboard counters
        public int TotalEmployees { get; set; }

        public int PresentToday { get; set; }

        public int PendingRequests { get; set; }

        public int ApprovedRequests { get; set; }


        public int LeaveRequest { get; set; }
        public int LeaveApproved { get; set; }
        public int absent { get; set; }
        public int present { get; set; }

        public List<EmployeePresentyRecord> list { get; set; } = new List<EmployeePresentyRecord>();
        public List<RequestRecord> Leavelist { get; set; } = new List<RequestRecord>();
        public List<Projects> Prolist { get; set; } = new List<Projects>();
        public List<TaskReq> Tasklist { get; set; } = new List<TaskReq>();
        // public List<ProjTask> tasks { get; set; } = new List<ProjTask>();



        public void present_list()
        {
            list = new List<EmployeePresentyRecord>();

            if (employees == null || dailyPresenties == null)
                return;

            foreach (var p in dailyPresenties.Where(x => x.Status == false))
            {
                var emp = employees.FirstOrDefault(e => e.EmployeeId == p.Empid && p.Status == false);

                if (emp != null)
                {
                    list.Add(new EmployeePresentyRecord(emp, p));
                }
            }
        }

        public void leaveRequest()
        {
            Leavelist = new List<RequestRecord>();

            if (employees == null || leaveRequests == null)
                return;

            foreach (var l in leaveRequests.Where(x => x.Status == false))
            {
                var emp = employees.FirstOrDefault(e => e.EmployeeId == l.Empid);

                if (emp != null)
                {
                    Leavelist.Add(new RequestRecord(emp, l));
                }
            }
        }

       

        public void TasksRequest()
        {
            Tasklist = new List<TaskReq>();
            foreach (var l in TaskReqests.Where(x => x.ApvlStatus == 1))
            {
            
                    Tasklist.Add(new TaskReq(l));
              
            }
        }
        public void allproject()
        {
            Prolist = new List<Projects>();

           
                var pro = projects.ToList();
           
        }
    }
}