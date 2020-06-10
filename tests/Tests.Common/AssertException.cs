using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Common
{
    public static class AssertException
    {
        public static async Task ThrowsInnerAsync<TInnerException>(Func<Task> func)
            where TInnerException: Exception
        {
            Exception thrownEx = null;
            Exception innerEx = null;

            try
            {
                await func.Invoke();
            }
            catch (Exception ex)
            {
                thrownEx = ex;
                innerEx = ex.InnerException;
            }

            if (thrownEx == null)
            {
                throw new Exception("No exception is thrown");
            }
            else if (innerEx == null)
            {
                throw new Exception("Thrown exception doesn't have inner exception");
            }
            else if (!(innerEx is TInnerException)) 
            {
                throw new Exception($"{innerEx.GetType().FullName} inner exception is thrown instead of expected {typeof(TInnerException).FullName}");
            }
        }

        public static async Task ThrowsOfTypeAsync<TException>(Func<Task> func)
            where TException : Exception
        {
            Exception thrownEx = null;
            
            try
            {
                await func.Invoke();
            }
            catch (Exception ex)
            {
                thrownEx = ex;
            }

            if (thrownEx == null)
            {
                throw new Exception("No exception is thrown");
            }
            else if (!(thrownEx is TException))
            {
                throw new Exception($"{thrownEx.GetType().FullName} exception is thrown instead of expected {typeof(TException).FullName}");
            }
        }
    }
}
