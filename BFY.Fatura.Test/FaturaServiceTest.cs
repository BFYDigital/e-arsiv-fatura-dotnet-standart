using BFY.Fatura.Configuration;
using Xunit;

namespace BFY.Fatura.Test
{
    public class FaturaServiceTest
    {
        [Fact]
        public void FaturaService_Should_Get_Token_On_Initialisation()
        {
            var configuration = FaturaServiceConfigurationFactory.Create();
            configuration.Username = "33333320";
            configuration.Password = "1";

            FaturaService faturaService = new FaturaService(configuration);
            faturaService.GetToken();

            Assert.NotNull(configuration.Token);
        }
    }
}
