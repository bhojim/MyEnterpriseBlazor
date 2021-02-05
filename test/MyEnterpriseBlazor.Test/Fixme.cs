using MyEnterpriseBlazor.Infrastructure.Data;
using MyEnterpriseBlazor.Domain;
using MyEnterpriseBlazor.Test.Setup;

namespace MyEnterpriseBlazor.Test
{
    public static class Fixme
    {
        public static User ReloadUser<TEntryPoint>(AppWebApplicationFactory<TEntryPoint> factory, User user)
            where TEntryPoint : class
        {
            var applicationDatabaseContext = factory.GetRequiredService<ApplicationDatabaseContext>();
            applicationDatabaseContext.Entry(user).Reload();
            return user;
        }
    }
}
