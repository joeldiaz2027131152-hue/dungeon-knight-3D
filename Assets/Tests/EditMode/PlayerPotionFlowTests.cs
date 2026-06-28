using NUnit.Framework;
using DungeonKnight.Level;
using DungeonKnight.Player;
using UnityEngine;

[TestFixture]
public class PlayerPotionFlowTests
{
    [Test]
    public void TryUsePotionHealsAndConsumesOnePotion()
    {
        GameObject playerObject = new GameObject("Player");
        PlayerController3D player = playerObject.AddComponent<PlayerController3D>();

        player.TakeDamage(60);

        Assert.That(player.Health, Is.EqualTo(60));
        Assert.That(player.Potions, Is.EqualTo(2));

        Assert.That(player.TryUsePotion(), Is.True);
        Assert.That(player.Health, Is.EqualTo(105));
        Assert.That(player.Potions, Is.EqualTo(1));

        Object.DestroyImmediate(playerObject);
    }

    [Test]
    public void PotionPickupRemainsWhenPotionBagIsFull()
    {
        GameObject playerObject = new GameObject("Player");
        playerObject.AddComponent<CharacterController>();
        PlayerController3D player = playerObject.AddComponent<PlayerController3D>();
        for (int i = 0; i < 3; i++)
        {
            player.AddPotion();
        }

        GameObject pickupObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        DungeonPickup3D pickup = pickupObject.AddComponent<DungeonPickup3D>();
        pickup.ConfigurePotion();

        Assert.That(player.Potions, Is.EqualTo(5));
        Assert.That(pickup.TryCollect(player), Is.False);
        Assert.That(pickupObject, Is.Not.Null);

        Object.DestroyImmediate(pickupObject);
        Object.DestroyImmediate(playerObject);
    }
}
