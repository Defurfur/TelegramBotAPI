using ScheduleUpdateService.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.ExtensionsTests
{
    public class DateTimeExtensionTests
    {
        [Fact]
        public void GetWeekNumber_Before_NewYear_Returns_Correct_Value()
        {
            //ARRANGE
            var date = new DateOnly(2022, 10, 24);
            //ACT
            var weekNumber = date.GetWeekNumber();
            //ASSERT
            Assert.True(weekNumber == 9);
        }
        [Fact]
        public void GetWeekNumber_After_NewYear_Returns_Correct_Value()
        {
            //ARRANGE
            var date = new DateOnly(2023, 1, 21);
            //ACT
            var weekNumber = date.GetWeekNumber();
            //ASSERT
            Assert.True(weekNumber == 21);
        } 

        [Fact]
        public void GetWeekStart_On_Sunday_Returns_Correct_Value()
        {
            //ARRANGE
            var date = new DateOnly(2023, 1, 15);
            //ACT
            var weekStart = date.GetWeekStart();
            //ASSERT
            Assert.True(weekStart.CompareTo(new DateOnly(2023, 1, 9)) == 0);
        }

        [Fact]
        public void GetWeekStart_Not_On_Sunday_Returns_Correct_Value()
        {
            //ARRANGE
            var date = new DateOnly(2023, 1, 18);
            //ACT
            var weekStart = date.GetWeekStart();
            //ASSERT
            Assert.True(weekStart.CompareTo(new DateOnly(2023, 1, 16)) == 0);
        }
        [Fact]
        public void GetWeekEnd_On_Sunday_Returns_Correct_Value()
        {
            //ARRANGE
            var date = new DateOnly(2023, 1, 15);
            //ACT
            var weekStart = date.GetWeekEnd();
            //ASSERT
            Assert.True(weekStart.CompareTo(new DateOnly(2023, 1, 15)) == 0);
        }

        [Fact]
        public void GetWeekEnd_Not_On_Sunday_Returns_Correct_Value()
        {
            //ARRANGE
            var date = new DateOnly(2023, 1, 18);
            //ACT
            var weekStart = date.GetWeekEnd();
            //ASSERT
            Assert.True(weekStart.CompareTo(new DateOnly(2023, 1, 22)) == 0);
        }

        [Fact]
        public void GetDayByDayOfWeek_If_Passed_Same_Day_Returns_Correct_Value()
        {
            //ARRANGE
            var date = new DateOnly(2023, 1, 18);
            //ACT
            var targetDate = date.GetDateByDayOfWeek("среда");
            //ASSERT
            Assert.True(targetDate.CompareTo(new DateOnly(2023, 1, 18)) == 0);
        }
        [Fact]
        public void GetDayByDayOfWeek_If_TargetDate_Is_Earlier_Returns_Correct_Value()
        {
            //ARRANGE
            var date = new DateOnly(2023, 1, 18);
            //ACT
            var targetDate = date.GetDateByDayOfWeek("вторник");
            //ASSERT
            Assert.True(targetDate.CompareTo(new DateOnly(2023, 1, 17)) == 0);
        }
        [Fact]
        public void GetDayByDayOfWeek_If_TargetDate_Is_Later_Returns_Correct_Value()
        {
            //ARRANGE
            var date = new DateOnly(2023, 1, 18);
            //ACT
            var targetDate = date.GetDateByDayOfWeek("суббота");
            //ASSERT
            Assert.True(targetDate.CompareTo(new DateOnly(2023, 1, 21)) == 0);
        }

        [Fact]
        public void GetDayByDayOfWeek_Throws_Exception_When_Invalid_Input()
        {
            //ARRANGE
            var date = new DateOnly(2023, 1, 18);
            //ACT + ASSERT
            Assert.Throws<ArgumentException>(() => date.GetDateByDayOfWeek("курятница"));
        }

        [Theory]
        [ClassData(typeof(GetDateTestData))]
        public void GetDate_WithDifferent_Params_Returns_Correct_Date(DateOnly expected, string date)
        {
            //ARRANGE + ACT
            var result = DateTimeExtension.GetDate(date);
            //ASSERT
            Assert.Equal(expected, result);
        }

        public class GetDateTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] {new DateOnly(2023, 1, 26), "26 января 2023"};
                yield return new object[] {new DateOnly(2024, 2, 13), "13 февраля 2024"};
                yield return new object[] {new DateOnly(2019, 3, 15), "15 марта 2019"};
                yield return new object[] {new DateOnly(2023, 4, 1), "1 апреля 2023"};
                yield return new object[] {new DateOnly(2023, 5, 26), "26 мая 2023"};
                yield return new object[] {new DateOnly(2023, 6, 13), "13 июня 2023"};
                yield return new object[] {new DateOnly(2023, 7, 17), "17 июля 2023"};
                yield return new object[] {new DateOnly(2023, 8, 30), "30 августа 2023"};
                yield return new object[] {new DateOnly(2023, 9, 28), "28 сентября 2023"};
                yield return new object[] {new DateOnly(2023, 10, 2), "2 октября 2023"};
                yield return new object[] {new DateOnly(2023, 11, 3), "3 ноября 2023"};
                yield return new object[] {new DateOnly(2023, 12, 9), "9 декабря 2023"};
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Theory]
        [ClassData(typeof(GetMondayByWeekNumberTestData))]
        public void GetMondayByWeekNumber_Returns_Correct_Date(DateOnly expected, int weekNumber, int year)
        {
            //ACT
            var result = DateTimeExtension.GetMondayByWeekNumber(weekNumber, year);
            //ACT + ASSERT
            Assert.Equal(expected, result);
        }
        public class GetMondayByWeekNumberTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] {new DateOnly(2023, 5, 1), 36, 2022};
                yield return new object[] {new DateOnly(2022, 11, 14), 12, 2022};
                yield return new object[] {new DateOnly(2023, 1, 9), 20, 2022};
                yield return new object[] {new DateOnly(2022, 12, 26), 18, 2022};
                yield return new object[] {new DateOnly(2022, 9, 26), 5, 2022 };
          
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
