using UnityEngine;

public class ConsumableManager : MonoBehaviour
{
    public static ConsumableManager instance;

    private PlayerVitals player;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        player = FindObjectOfType<PlayerVitals>();
    }

    public void Consume(Slot slot)
    {
        if (slot.slotItem.itemType != ItemType.Consumable)
            return;

        Target[] targets = new Target[]
        {
            new Target(slot.slotItem.durability, TargetVital.Health),
            new Target(slot.slotItem.damage, TargetVital.Energy)
        };

        foreach (Target t in targets)
        {
            switch (t.target)
            {
                default:
                    player.AddHealth(t.amount);
                    break;

                case TargetVital.Energy:
                    player.AddEnergy(t.amount);
                    break;
            }
        }

        slot.RemoveOne();
    }

    private enum TargetVital
    {
        Health,
        Energy
    }

    private struct Target
    {
        public float amount;
        public TargetVital target;

        public Target(float amount, TargetVital target)
        {
            this.amount = amount;
            this.target = target;
        }
    }
}
