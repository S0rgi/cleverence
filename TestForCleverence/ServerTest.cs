using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cleverence;
namespace TestForCleverence
{
    public class ServerTests
    {
        private void ResetServer()
        {
            typeof(Server)
                .GetField("count", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, 0);
        }

        [Fact]
        public void AddToCount_UpdatesValueCorrectly()
        {
            ResetServer();
            Server.AddToCount(3);
            Assert.Equal(3, Server.GetCount());
        }

        [Fact]
        public void WriteLock_BlocksParallelWrites()
        {
            ResetServer();
            var writeSignal = new ManualResetEventSlim(false);
            bool secondWriteCompleted = false;

            // Первый поток с долгой операцией записи (3 секунды)
            var firstWriter = Task.Run(() =>
            {
                Server.AddToCount(3);
                writeSignal.Set();
            });

            // Второй поток пытается записать сразу после старта первого
            var secondWriter = Task.Run(() =>
            {
                writeSignal.Wait();
                Server.AddToCount(2);
                secondWriteCompleted = true;
            });

            // Даем второму потоку 500мс на попытку выполнения
            Thread.Sleep(500);
            Assert.False(secondWriteCompleted, "Вторая запись не должна завершиться");

            // Дожидаемся завершения и проверяем результат
            Task.WaitAll(firstWriter, secondWriter);
            Assert.Equal(5, Server.GetCount());
        }

        [Fact]
        public void ReadOperation_WaitsForWriteLock()
        {
            ResetServer();
            var writeStarted = new ManualResetEventSlim(false);
            int readResult = 0;

            // Поток записи с задержкой
            var writer = Task.Run(() =>
            {
                Server.AddToCount(2); // Задержка 2 секунды
                writeStarted.Set();
            });

            // Поток чтения запускается после старта записи
            var reader = Task.Run(() =>
            {
                writeStarted.Wait();
                readResult = Server.GetCount();
            });

            // Проверяем состояние через 1 секунду (в середине операции записи)
            Thread.Sleep(1000);
            Assert.Equal(0, readResult); // Чтение должно ждать

            Task.WaitAll(writer, reader);
            Assert.Equal(2, Server.GetCount());
        }

        [Fact]
        public void ParallelReads_AllowedDuringNoWrites()
        {
            ResetServer();
            int readCount = 0;
            var readBarrier = new Barrier(2);

            Parallel.Invoke(
                () =>
                {
                    Server.GetCount();
                    readBarrier.SignalAndWait();
                    Interlocked.Increment(ref readCount);
                },
                () =>
                {
                    Server.GetCount();
                    readBarrier.SignalAndWait();
                    Interlocked.Increment(ref readCount);
                }
            );

            Assert.Equal(2, readCount);
        }

        [Fact]
        public void ExceptionInWrite_ReleasesLock()
        {
            ResetServer();

            var exception = Assert.Throws<AggregateException>(() =>
            {
                Parallel.For(0, 2, i =>
                {
                    if ( i == 0 )
                    {
                        // Эта итерация должна выбросить ArgumentOutOfRangeException
                        Server.AddToCount(-1);
                    }
                    else
                    {
                        // Эта итерация должна успешно добавить 3
                        Server.AddToCount(3);
                    }
                });
            });

            // Проверяем, что среди внутренних исключений есть ArgumentOutOfRangeException
            Assert.IsType<ArgumentOutOfRangeException>(exception.InnerExceptions[0]);

            // Проверяем, что вторая запись (AddToCount(3)) всё равно выполнилась
            Assert.Equal(3, Server.GetCount());
        }
    }
}
