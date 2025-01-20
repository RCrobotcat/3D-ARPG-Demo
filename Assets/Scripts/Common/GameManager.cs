using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    public List<ItemData_SO> allWeapon; // 所有武器
    public List<ItemData_SO> allArmor; // 所有防具

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public ItemData_SO GetWeaponByName(string weaponName)
    {
        foreach (var weapon in allWeapon)
        {
            if (weapon.itemName == weaponName)
            {
                return weapon;
            }
        }
        return null;
    }
    public ItemData_SO GetArmorByName(string armorName)
    {
        foreach (var armor in allArmor)
        {
            if (armor.itemName == armorName)
            {
                return armor;
            }
        }
        return null;
    }
}
