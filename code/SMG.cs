using EasyWeapons.Bullets.Spawners;
using EasyWeapons.Weapons.Modules.Attack.ShootingModes;
using EasyWeapons.Weapons.Modules.Attack;
using EasyWeapons.Weapons;
using Sandbox;
using EasyWeapons.Sounds;
using EasyWeapons.Inventories;
using EasyWeapons.Weapons.Modules.Reload;
using EasyWeapons.Recoiles.Modules;

namespace EasyWeapons.Demo.Weapons;

[Spawnable]
[Library("ew_smg")]
public partial class SMG : Weapon
{
    public const int DefaultMaxAmmoInClip = 20;
    public const float Force = 150f;
    public const float Spread = 0.05f;
    public const float Damage = 7f;
    public const float Distance = 5000f;
    public const float BulletSize = 3f;
    public const float ReloadTime = 4.5f;

    [Net, Local]
    protected BulletSpawner BulletSpawner { get; private set; }

    [Net, Local]
    protected OneTypeAmmoInventory Clip { get; private set; }


    public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";

    public SMG()
    {
        if(Game.IsServer)
        {
            HoldType = CitizenAnimationHelper.HoldTypes.Rifle;
            DeployAnimation = "deploy";
            UseOwnerAimRay = true;
            DefaultLocalAimRay = new Ray(new(1.65f, -17.5f, 3.8f), Vector3.Right);
            DeployTime = 0.5f;
            BulletSpawner = new TraceBulletSpawner(Spread, Force, Damage, Distance, BulletSize, this);
            Clip = OneTypeAmmoInventory.Full("smg", DefaultMaxAmmoInClip);

            var attackModule = new SimpleAttackModule(Clip, BulletSpawner, new AutoShootingMode())
            {
                AttackSound = new DelayedSound("rust_smg.shoot"),
                DryfireSound = new DelayedSound("rust_smg.dryfire.fixed"),
                FireRate = 15f,
                Recoil = new RandomRecoil() { XRecoil = new RangedFloat(-1, 1), YRecoil = 3 },
                NoOwnerRecoilForce = 100000
            };

            var reloadModule = new SimpleReloadModule(Clip)
            {
                ReloadTime = ReloadTime,
                ReloadFailSound = new DelayedSound("no_ammo"),

            };

            Components.Add(attackModule);
            Components.Add(reloadModule);
        }
        else
        {

            BulletSpawner = null!;
            Clip = null!;
        }
    }

    public override void Spawn()
    {
        base.Spawn();
        SetModel("weapons/rust_smg/rust_smg.vmdl");
    }
}
