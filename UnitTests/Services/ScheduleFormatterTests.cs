using Moq;
using ReaSchedule.Models;
using TelegramBotService.Services;
using Xunit;

namespace UnitTests.Services
{
    public class ScheduleFormatterTests
    {
        private MockRepository mockRepository;



        public ScheduleFormatterTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        private ScheduleFormatter CreateScheduleFormatter()
        {
            return new ScheduleFormatter();
        }

        [Fact]
        public void Format_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var scheduleFormatter = this.CreateScheduleFormatter();
            ReaGroup reaGroup = null;

            // Act
            var result = scheduleFormatter.Format(
                reaGroup);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void Format_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange
            var scheduleFormatter = this.CreateScheduleFormatter();
            List<ReaClass> reaClasses = new();

            // Act
            var result = scheduleFormatter.Format(
                reaClasses);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void Format_StateUnderTest_ExpectedBehavior2()
        {
            // Arrange
            var scheduleFormatter = this.CreateScheduleFormatter();
            ScheduleWeek scheduleWeek = null;

            // Act
            var result = scheduleFormatter.Format(
                scheduleWeek);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void Format_StateUnderTest_ExpectedBehavior3()
        {
            // Arrange
            var scheduleFormatter = this.CreateScheduleFormatter();
            ScheduleDay scheduleDay = null;

            // Act
            var result = scheduleFormatter.Format(
                scheduleDay);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }
    }
}
