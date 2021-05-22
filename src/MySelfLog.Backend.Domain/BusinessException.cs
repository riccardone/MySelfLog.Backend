using System;

namespace MySelfLog.Backend.Domain
{
    public class BusinessException : Exception
    {
        public BusinessException(string err) : base(err)
        {
            
        }
    }
}
