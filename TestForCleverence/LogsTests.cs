using cleverence;
using Cleverence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestForCleverence
{
    public class LogsTests
    {
        [Fact]
        public void TestLogParser_Format1()
        {
            // Тестовый лог в формате 1
            string logEntry = "10.03.2025 15:14:49.523 INFORMATION  Версия программы: '3.4.0.48729'";
            string expectedOutput = "10-03-2025\t15:14:49.523\tINFO\tDEFAULT\tВерсия программы: '3.4.0.48729'";

            string actualOutput = LogStandartization.Standartization(logEntry).Item1;

            Assert.Equal(expectedOutput, actualOutput);
        }

        [Fact]
        public void TestLogParser_Format1_WithMethod()
        {
            // Тестовый лог в формате 1 с методом
            string logEntry = "10.03.2025 15:14:49.523 INFORMATION MobileComputer.GetDeviceId Версия программы: '3.4.0.48729'";
            string expectedOutput = "10-03-2025\t15:14:49.523\tINFO\tMobileComputer.GetDeviceId\tВерсия программы: '3.4.0.48729'";

            string actualOutput = LogStandartization.Standartization(logEntry).Item1;

            Assert.Equal(expectedOutput, actualOutput);
        }

        [Fact]
        public void TestLogParser_Format2_WithMethod()
        {
            // Тестовый лог в формате 2
            string logEntry = "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'";
            string expectedOutput = "2025-03-10\t15:14:51.5882\tINFO\tMobileComputer.GetDeviceId\tКод устройства: '@MINDEO-M40-D-410244015546'";

            string actualOutput = LogStandartization.Standartization(logEntry).Item1;

            Assert.Equal(expectedOutput, actualOutput);
        }
        [Fact]
        public void TestLogParser_Format2()
        {
            // Тестовый лог в формате 2
            string logEntry = "2025-03-10 15:14:51.5882| INFO|11|| Код устройства: '@MINDEO-M40-D-410244015546'";
            string expectedOutput = "2025-03-10\t15:14:51.5882\tINFO\tDEFAULT\tКод устройства: '@MINDEO-M40-D-410244015546'";

            string actualOutput = LogStandartization.Standartization(logEntry).Item1;

            Assert.Equal(expectedOutput, actualOutput);
        }
        [Fact]
        public void TestLogParser_InvalidFormat()
        {
            // Тестовый лог с неверным форматом
            string logEntry = "Неверный формат лога";
            string expectedOutput = logEntry;

            string actualOutput = LogStandartization.Standartization(logEntry).Item1;

            Assert.Equal(expectedOutput, actualOutput);
        }
    }
}
