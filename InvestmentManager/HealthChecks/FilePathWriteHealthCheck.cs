using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace InvestmentManager.HealthChecks
{
    public class FilePathWriteHealthCheck : IHealthCheck
    {
        private readonly string _filePath;
        private readonly IReadOnlyDictionary<string, object> _HealthCheckData;

        public FilePathWriteHealthCheck(string filePath)
        {
            _filePath = filePath;
            _HealthCheckData = new Dictionary<string, object>
            {
                {"filePath", _filePath}
            };
        }
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = new CancellationToken()
            )
        {
            try
            {
                var testFile = $"{_filePath}\\test.text";
                var fileStream = File.Create(testFile);
                fileStream.Close();
                
                File.Delete(testFile);
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception e)
            {
                switch (context.Registration.FailureStatus)
                {
                    case HealthStatus.Degraded:
                        return Task.FromResult(
                            HealthCheckResult.Degraded(
                                "Issues writing o file path", e, _HealthCheckData));
                    case HealthStatus.Healthy:
                        return Task.FromResult(
                            HealthCheckResult.Healthy(
                                "Issues writing o file path", _HealthCheckData));
                    default:
                        return Task.FromResult(HealthCheckResult.Unhealthy("Something wrong", e, _HealthCheckData));
                }
            }
        }
    }

    public static class FilePathHealthCheckBuilderExtensions
    {
        private const string NAME = "Filepath write";

        public static IHealthChecksBuilder AddFilePathWrite(this IHealthChecksBuilder builder, string filePath, HealthStatus failureStatus, IEnumerable<string> tags = default)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            return builder.Add(new HealthCheckRegistration(
                NAME,
                new FilePathWriteHealthCheck(filePath),
                failureStatus,
                tags));
        }
    }
}
