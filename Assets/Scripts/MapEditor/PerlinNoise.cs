using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class PerlinNoise
{
    public int[,] areaGrid;

    float offsetX = 0;
    float offsetY = 0;

    public void RandomOffsets()
    {
        offsetX = Random.Range(0.0f, 1000.0f);
        offsetY = Random.Range(0.0f, 1000.0f);
    }

    public void Create(int width, int height, int xOffset, int yOffset, float scale, float minValue, float maxValue)
    {
        areaGrid = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = scale * (float)(offsetX + xOffset + x) / (float)width;
                float yCoord = scale * (float)(offsetY + yOffset + y) / (float)height;

                float val = Mathf.PerlinNoise(xCoord, yCoord);

                if (val >= minValue && val <= maxValue)
                {
                    areaGrid[x, y] = 1;
                }
                else
                {
                    areaGrid[x, y] = 0;
                }
            }
        }
    }

}