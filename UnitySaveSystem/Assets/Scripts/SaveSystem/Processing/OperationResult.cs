using System;


namespace SaveSystem.Processing
{
    public class OperationResult
    {
        public bool Success => Error == null;
        public Exception Error { get; }

            
        public OperationResult(Exception e = null) =>
            Error = e;
    }
}