namespace ArenaSync.Web.Dtos
{
    public class AssignmentResult
    {
        private AssignmentResult(bool succeeded, string message)
        {
            Succeeded = succeeded;
            Message = message;
        }

        public bool Succeeded { get; }
        public string Message { get; }

        public static AssignmentResult Success(string message = "Assignment saved.")
            => new(true, message);

        public static AssignmentResult Failure(string message)
            => new(false, message);
    }
}
