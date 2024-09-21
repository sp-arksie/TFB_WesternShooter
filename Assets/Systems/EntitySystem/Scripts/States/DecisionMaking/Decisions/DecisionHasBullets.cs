

public class DecisionHasBullets : DecisionNode
{
    protected override bool CheckCondition()
    {
        ProjectileWeaponForEntity weapon = entityController.HotBarManager.currentSelectedItem.GetComponent<ProjectileWeaponForEntity>();

        return weapon.ammoBehaviourForEntity.GetHasBulletInMagazine();
    }
}
