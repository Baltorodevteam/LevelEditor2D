using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SaveScreenOverlay : BaseScreenOverlay
{
    [SerializeField]
    SaveScreen saveScreen;

    void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        base.OnEnable();
        PrepareKbrd();
    }

    void PrepareKbrd()
    {
        for (int i = 0; i < 10; i++)
        {
            list[i].rightNeighbour = list[i + 1];
            list[i + 1].leftNeighbour = list[i];
        }

        for (int i = 0; i < 10; i++)
        {
            list[i].downNeighbour = list[i + 11];
        }

        list[11].downNeighbour = list[30];

        ///////////////////////////////////////////////////////////

        for (int i = 11; i < 20; i++)
        {
            list[i].rightNeighbour = list[i + 1];
            list[i + 1].leftNeighbour = list[i];
        }

        for (int i = 11; i <= 20; i++)
        {
            list[i].upNeighbour = list[i - 11];
            list[i].downNeighbour = list[i + 10];
        }

        ///////////////////////////////////////////////////////////

        for (int i = 21; i < 30; i++)
        {
            list[i].rightNeighbour = list[i + 1];
            list[i + 1].leftNeighbour = list[i];
        }

        for (int i = 21; i <= 30; i++)
        {
            list[i].upNeighbour = list[i - 10];
            list[i].downNeighbour = list[i + 10];
        }

        /////////////////////////////////////////////////////////////

        for (int i = 31; i < 40; i++)
        {
            list[i].rightNeighbour = list[i + 1];
            list[i + 1].leftNeighbour = list[i];
        }

        for (int i = 31; i <= 40; i++)
        {
            list[i].downNeighbour = list[41];
            list[i].upNeighbour = list[i - 10];
        }

        ///////////////////////////////////////////////////////////

        list[41].upNeighbour = list[35];
    }
} 