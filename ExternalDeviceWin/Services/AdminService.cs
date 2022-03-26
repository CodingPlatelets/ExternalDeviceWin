namespace ExternalDeviceWin.Services
{
    public class AdminService : AdminController.AdminControllerBase
    {
        private readonly ILogger<AdminService> _logger;
        public AdminService(ILogger<AdminService> logger)
        {
            _logger = logger;
        }
    }
}
