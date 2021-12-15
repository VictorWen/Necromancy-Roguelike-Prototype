public abstract class BaseItem
{
    public abstract string ID { get; }

    public abstract void OnAdd(PlayerController player);

    public abstract void OnRemove(PlayerController player);
}
