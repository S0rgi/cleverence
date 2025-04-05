using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
namespace Cleverence
{
    public static class Server
    {
        private static int count;
        private static readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public static int GetCount()
        {
            rwLock.EnterReadLock();
            try
            {
                return count;
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public static void AddToCount(int value)
        {
            if ( value < 0 )
                throw new ArgumentOutOfRangeException(nameof(value)); // Исключение ДО захвата блокировки

            rwLock.EnterWriteLock();
            try
            {
                // чтобы протестировать закрытие доступа во время записи , усыпляем поток на value секунд
                Thread.Sleep(value * 1000);
                count += value;
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }
    }
}
