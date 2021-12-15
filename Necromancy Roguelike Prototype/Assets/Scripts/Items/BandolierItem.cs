using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BandolierItem : BaseItem
{
    public override string ID { get { return "bandolier"; } }

    public override void OnAdd(PlayerController player)
    {
        player.ReloadTimeModifier -= 0.5f;
    }

    public override void OnRemove(PlayerController player)
    {
        player.ReloadTimeModifier += 0.5f;
    }
}
