namespace ReportService.Data.Exceptions
{
    public class UnexpectedMultipleItemException : Exception
    {
        public UnexpectedMultipleItemException(string msg) : base(msg)
        { }
    }
}
