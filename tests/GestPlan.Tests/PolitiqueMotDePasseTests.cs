using GestPlan.Core.Securite;

namespace GestPlan.Tests;

public class PolitiqueMotDePasseTests
{
    private readonly PolitiqueMotDePasse _politique = new();

    [Theory]
    [InlineData("court1A!")]
    [InlineData("toutminuscule1!")]
    [InlineData("TOUTMAJUSCULE1!")]
    [InlineData("SansChiffreDuTout!")]
    [InlineData("SansCaractereSpecial1")]
    public void Valider_RejetteLesMotsDePasseNonConformes(string motDePasse)
    {
        Assert.False(_politique.EstValide(motDePasse));
    }

    [Fact]
    public void Valider_AccepteUnMotDePasseConforme()
    {
        Assert.True(_politique.EstValide("MotDePasseValide1!"));
    }

    [Fact]
    public void EstExpire_RetourneFauxSiAucuneDureeDeValiditeConfiguree()
    {
        _politique.DureeValidite = null;

        Assert.False(_politique.EstExpire(DateTime.UtcNow.AddYears(-10), DateTime.UtcNow));
    }

    [Fact]
    public void EstExpire_RetourneVraiApresLaDureeDeValiditeConfiguree()
    {
        _politique.DureeValidite = TimeSpan.FromDays(90);

        Assert.True(_politique.EstExpire(DateTime.UtcNow.AddDays(-91), DateTime.UtcNow));
        Assert.False(_politique.EstExpire(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow));
    }
}
