using System.Collections.Generic;
using System.Linq;
using Core.Generals;
using UnityEngine;

namespace Core.Graphics
{
    public static class Atlas
    {
        public static int s_Scale = 512;

        private static bool _isPrepared = false;

        private static Texture2D _atlas;
        private static Texture2D _blurAtlas;

        private struct AtlasProperty
        {
            public Rect[] Rects;
            public Texture2D Texture;

            public AtlasProperty(Rect[] rects, Texture2D texture)
            {
                Rects = rects;
                Texture = texture;
            }
        }

        private static AtlasProperty PrepareAtlas(Texture2D[] textures, bool blurTexture = false)
        {
            var atlas = new Texture2D(s_Scale, s_Scale, TextureFormat.RGBA32, false, false);
            
            var rects = atlas.PackTextures(
                textures, 32, s_Scale);

            uPaddingBleed.BleedEdges(atlas,32, rects, true);

            if (blurTexture == true)
            {
                BlurTexture blur = new BlurTexture();
                atlas = blur.FastBlur(atlas, 2, 2);
                atlas.filterMode = FilterMode.Trilinear;
            }
            else
            {
                atlas.filterMode = FilterMode.Point;
            }

            
            return new AtlasProperty(rects, atlas);
        }

        public static void Prepare()
        {
            if (_isPrepared == false)
            {
                List<Texture2D> texture2Ds = new List<Texture2D>();

                foreach (var block in BlockRegister.GetRegisteredBlocks())
                {
                    foreach (var texture in block.GetTextures())
                    {
                        texture2Ds.Add(texture);
                    }
                }

                var textureArray = texture2Ds.ToArray();

                for (int i = 0; i < textureArray.Length; i++)
                {
                    textureArray[i] = FlipTexture(textureArray[i]);
                    textureArray[i].filterMode = FilterMode.Point;
                }


                
                var property = PrepareAtlas(textureArray);
                var rects = property.Rects;

                _atlas = property.Texture;
                _blurAtlas = PrepareAtlas(textureArray, true).Texture;
                
                
                int index = 0;

                foreach (var block in BlockRegister.GetRegisteredBlocks())
                {
                    List<Rect> lRects = new List<Rect>();
                    
                    foreach (var texture in block.GetTextures())
                    {
                        lRects.Add(rects[index]);
                        index += 1;
                    }
                    
                    block.Init(lRects.ToArray());
                }
                
                _isPrepared = true;
            }
        }

        public static Texture2D GetAtlas()
        {
            Prepare();
            return _atlas;
        }
        
        public static Texture2D GetBlurAtlas()
        {
            Prepare();
            return _blurAtlas;
        }
        
        
        private static Texture2D Resize(Texture2D texture2D,int targetX,int targetY)
        {
            RenderTexture rt=new RenderTexture(targetX, targetY,24);
            RenderTexture.active = rt; 
            UnityEngine.Graphics.Blit(texture2D,rt);
            Texture2D result=new Texture2D(targetX,targetY);
            result.ReadPixels(new Rect(0,0,targetX,targetY),0,0);
            result.Apply();
            return result;
        }
        
        private static Texture2D FlipTexture(Texture2D original, bool upSideDown = true)
        {

            Texture2D flipped = new Texture2D(original.width, original.height);

            int xN = original.width;
            int yN = original.height;


            for (int i = 0; i < xN; i++)
            {
                for (int j = 0; j < yN; j++)
                {
                    if (upSideDown)
                    {
                        flipped.SetPixel(j, xN - i - 1, original.GetPixel(j, i));
                    }
                    else
                    {
                        flipped.SetPixel(xN - i - 1, j, original.GetPixel(i, j));
                    }
                }
            }
            flipped.Apply();

            return flipped;
        }
    }
}