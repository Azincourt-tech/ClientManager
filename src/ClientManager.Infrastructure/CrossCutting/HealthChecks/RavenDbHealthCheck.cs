using Microsoft.Extensions.Diagnostics.HealthChecks;
using Raven.Client.ServerWide.Operations;

namespace ClientManager.Infrastructure.CrossCutting.HealthChecks
{
    public class RavenDbHealthCheck : IHealthCheck
    {
        private readonly IDocumentStore _documentStore;

        public RavenDbHealthCheck(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            try
            {
                var operation = new GetDatabaseNamesOperation(0, 1);
                await _documentStore.Maintenance.Server.SendAsync(operation, cts.Token);

                return HealthCheckResult.Healthy("RavenDB connection is healthy.");
            }
            catch (OperationCanceledException)
            {
                return HealthCheckResult.Degraded("RavenDB health check timed out. This may indicate a cold start or temporary unavailability.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("RavenDB connection is unhealthy.", ex);
            }
        }
    }
}
