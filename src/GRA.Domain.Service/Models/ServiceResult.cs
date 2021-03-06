﻿namespace GRA.Domain.Service.Models
{
    public class ServiceResult
    {
        public ServiceResultStatus Status { get; set; }
        public string Message { get; set; }
        public string[] Arguments { get; set; }

        public ServiceResult() { }

        public ServiceResult(ServiceResultStatus status)
        {
            Status = status;
        }
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T Data;
    }

    public enum ServiceResultStatus
    {
        Success = 0,
        Warning = 1,
        Error = 2
    }
}
