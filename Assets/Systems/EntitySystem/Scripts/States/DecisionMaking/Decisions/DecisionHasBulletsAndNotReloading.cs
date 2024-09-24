

public class DecisionHasBulletsAndNotReloading : DecisionNode
{
    protected override bool CheckCondition()
    {
        ProjectileWeaponForEntity weapon = entityController.HotBarManager.currentSelectedItem.GetComponent<ProjectileWeaponForEntity>();

        return weapon.ammoBehaviourForEntity.GetHasBulletInMagazine() && !weapon.GetIsReloading();
    }
}
