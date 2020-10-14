using System;

namespace TCAdminSubdomain.Exceptions
{
    public class SubdomainException : Exception
    {
        public SubdomainException(string message) : base(message)
        {
        }
    }
}