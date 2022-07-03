using Core.World;
using UnityEngine;

namespace Core.Generals
{
    public class BirchBlock : BlockStructure
    {
        public override void PushStructure(IWorld world, int x, int y, int z)
        {
            world.BreakBlock(x, y, z);
            int random = ((y + x + z) % 10);

            int height = 6;
            if (random == 0)
            {
                height += 3;
            }


            for (int i = 0; i < height - 1; i++)
            {
                world.BreakBlock(x, y, z);
                world.PlaceBlock(CoreRegister.BIRCHLOG, x, y + i, z);
            }

            int r = height / 2;

            for (int _x = -r; _x < r; _x++)
            {
                for (int _y = -r; _y < r; _y++)
                {
                    for (int _z = -r; _z < r; _z++)
                    {
                        float distance = Vector3.Distance(new Vector3(_x, _y, _z), Vector3.zero);
                        if (distance < r)
                        {
                            world.PlaceBlock(CoreRegister.BIRCHLEAVES, x + _x, y + _y + (height - 2), z + _z);
                        }
                    }
                }
            }

            world.PlaceBlock(CoreRegister.BIRCHLOG, x, y, z);
        }
    }
}