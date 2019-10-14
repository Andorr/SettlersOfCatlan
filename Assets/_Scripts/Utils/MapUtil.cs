using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using State;
using System;

public static class MapUtil
{
    public static Vector3[] HexagonalLattice(Vector2 origin, int size = 5, float radius = 1, float offset = 0)
    {
        if(size%2 == 0) {
            throw new ArgumentException("Size needs to be an odd number.");
        }

        // Calculating the distances
        float angle30 = Mathf.Deg2Rad*30;
        double xStep = Mathf.Cos(angle30)*(radius + offset);
        double yStep = Mathf.Sin(angle30)*(radius + offset);
  
        int half = size/2; // number of hexagons on the first row
        Vector3[] tiles = new Vector3[3*(half + 1)*(half) + 1];
        int i = 0;
        for(int row = 0; row < size; row++)
        {
            int numberOfCols = size - Mathf.Abs(row - half);
            float xOffset = (float)(numberOfCols % 2 == 0 ? xStep : 0);
            for(int col = 0; col < numberOfCols; col++)
            {
                float xPos = (float)(origin.x + xStep*2*(-numberOfCols/2 + col) + xOffset);
                float yPos = (float)(origin.y + (yStep + (radius + offset))*(-size/2 + row));
                tiles[i] =  new Vector3(xPos, 0, yPos);
                i++;
            }
        }

        return tiles;
    }

    public static Vector3[] HexagonFromPoint(Vector2 origin, float radius)
    {
        Vector3[] points = new Vector3[6];
        float angle30 = Mathf.Deg2Rad*30;
        float angle60 = Mathf.Deg2Rad*60;
        for(int i = 0; i < 6; i++) {
            points[i] = new Vector3(origin.x + Mathf.Cos(angle30 + (angle60*i))*radius, 0, origin.y + Mathf.Sin(angle30 + (angle60*i))*radius);
        }
        return points;
    }
}
