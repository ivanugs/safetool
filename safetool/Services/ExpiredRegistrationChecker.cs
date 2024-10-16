
namespace safetool.Services
{
    public class ExpiredRegistrationChecker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ExpiredRegistrationChecker> _logger;

        public ExpiredRegistrationChecker(IServiceScopeFactory scopeFactory, ILogger<ExpiredRegistrationChecker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var formSubmissionService = scope.ServiceProvider.GetRequiredService<FormSubmissionService>();

                        // Ejecutar el chequeo de registros expirados
                        await formSubmissionService.CheckAndNotifyExpiredRegistrations();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error revisando registros vencidos: {ex.Message}");
                }

                // Ejecutar este chequeo cada 24 horas (puedes ajustar este valor)
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
