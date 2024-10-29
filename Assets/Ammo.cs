using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ammo : MonoBehaviour
{
    public TextMeshProUGUI ammoCount;

    public int currentAmmo;
    public int maxAmmo = 60;
    public int currentClipSize;
    public int maxClipSize = 12;

    private void Start()
    {
        currentAmmo = maxAmmo;
        currentClipSize = maxClipSize;
    }

    private void Update()
    {
        ammoCount.text = ($"{currentClipSize}/{currentAmmo}");
    }
}