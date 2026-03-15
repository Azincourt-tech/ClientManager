using Microsoft.Extensions.Diagnostics.HealthChecks;
using Raven.Client.Documents;

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
            try
            {
                // Tenta realizar uma consulta assíncrona simples para testar a conexão
                using (var session = _documentStore.OpenAsyncSession())
                {
                    // RavenDB Async API para Raw Query
                    await session.Advanced.AsyncRawQuery<dynamic>("from @all_docs limit 1")
                                 .ToListAsync(cancellationToken);
                }

                return HealthCheckResult.Healthy("RavenDB connection is healthy.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("RavenDB connection is unhealthy.", ex);
            }
        }
    }
}
