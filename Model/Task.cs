namespace AwsDotnetCsharp.Model
{
    public class Task
    {
        public string TaskId { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }

        public Task()
        {

        }

        public Task(string taskId, string description, bool completed)
        {
            TaskId = taskId;
            Description = description;
            Completed = completed;
        }
    }
}
