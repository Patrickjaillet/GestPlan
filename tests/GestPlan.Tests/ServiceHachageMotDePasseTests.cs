using GestPlan.Core.Securite;

namespace GestPlan.Tests;

public class ServiceHachageMotDePasseTests
{
    private readonly ServiceHachageMotDePasse _service = new();

    [Fact]
    public void Hacher_NeStockeJamaisLeMotDePasseEnClair()
    {
        var hash = _service.Hacher("MotDePasseSecret1!");

        Assert.DoesNotContain("MotDePasseSecret1!", hash);
    }

    [Fact]
    public void Verifier_ReussitAvecLeBonMotDePasse()
    {
        var hash = _service.Hacher("MotDePasseSecret1!");

        Assert.True(_service.Verifier("MotDePasseSecret1!", hash));
    }

    [Fact]
    public void Verifier_EchoueAvecUnMauvaisMotDePasse()
    {
        var hash = _service.Hacher("MotDePasseSecret1!");

        Assert.False(_service.Verifier("MauvaisMotDePasse", hash));
    }

    [Fact]
    public void Hacher_ProduitUnHashDifferentPourLeMemeMotDePasse()
    {
        var hash1 = _service.Hacher("MotDePasseSecret1!");
        var hash2 = _service.Hacher("MotDePasseSecret1!");

        Assert.NotEqual(hash1, hash2);
    }
}
