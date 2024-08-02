using KYC360.Helpers;

namespace KYC360.Tests;

public class RetryHelperTests
{
        [Test]
        public void ExecuteWithRetry_SucceedsOnFirstAttempt()
        {
            // Arrange
            var attempt = 0;
            void Action() => attempt++;

            // Act
            RetryHelper.ExecuteWithRetry(Action, 3, 1000);

            // Assert
            Assert.That(1, Is.EqualTo(attempt));
        }

        [Test]
        public void ExecuteWithRetry_RetriesAndSucceeds()
        {
            // Arrange
            var attempt = 0;
            void Action()
            {
                attempt++;
                if (attempt < 3)
                {
                    throw new Exception("Transient failure");
                }
            }

            // Act
            RetryHelper.ExecuteWithRetry(Action, 3, 1000);

            // Assert
            Assert.That(3, Is.EqualTo(attempt));
        }

        [Test]
        public void ExecuteWithRetry_FailsAfterMaxRetries()
        {
            // Arrange
            var attempt = 0;
            void Action()
            {
                attempt++;
                throw new Exception("Persistent failure");
            }

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => RetryHelper.ExecuteWithRetry(Action, 3, 1000));
            Assert.That(3, Is.EqualTo(attempt));
            Assert.That("Persistent failure", Is.EqualTo(ex?.Message));
        }

        [Test]
        public void ExecuteWithRetry_WaitsBetweenRetries()
        {
            // Arrange
            var attempt = 0;
            var stopwatch = new System.Diagnostics.Stopwatch();
            void Action()
            {
                attempt++;
                if (attempt < 3)
                {
                    throw new Exception("Transient failure");
                }
            }

            // Act
            stopwatch.Start();
            RetryHelper.ExecuteWithRetry(Action, 3, 1000);
            stopwatch.Stop();

            // Assert
            Assert.That(3, Is.EqualTo(attempt));
            Assert.IsTrue(stopwatch.ElapsedMilliseconds >= 3000);
        }
    }
